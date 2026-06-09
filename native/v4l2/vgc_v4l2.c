/*
 * vgc_v4l2.c — V4L2 capture backend native plugin for VGC-Ally
 *
 * Exposes a C API that Unity P/Invokes via LinuxV4L2CaptureProvider.cs
 * Handles: device enumeration, format negotiation, MMAP buffer capture,
 *          YUYV->RGBA conversion
 *
 * Build: cmake -B build -DCMAKE_BUILD_TYPE=Release && cmake --build build
 * Output: libvgc_v4l2.so -> Assets/Plugins/Linux/x86_64/
 *
 * License: GPLv3 — fork of ImmerNochNoah/VideoGameCapture
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <errno.h>
#include <fcntl.h>
#include <unistd.h>
#include <sys/ioctl.h>
#include <sys/mman.h>
#include <linux/videodev2.h>
#include <dirent.h>
#include <limits.h>

/* Mark all public API symbols as exported (required for Unity P/Invoke) */
#define VGC_EXPORT __attribute__((visibility("default")))

#define VGC_MAX_DEVICES   16
#define VGC_MAX_BUFFERS    4
#define VGC_DEVICE_NAME_LEN 256

/* ── Public API types ─────────────────────────────────────────────────────── */

typedef struct {
    char path[NAME_MAX + 6];
    char name[VGC_DEVICE_NAME_LEN];
} VGCDeviceInfo;

typedef struct {
    void   *start;
    size_t  length;
} VGCBuffer;

typedef struct {
    int          fd;
    int          width;
    int          height;
    int          fps;
    __u32        pixelformat;
    VGCBuffer    buffers[VGC_MAX_BUFFERS];
    int          n_buffers;
    int          streaming;
} VGCDevice;

/* ── Internal helpers ─────────────────────────────────────────────────────── */

static int xioctl(int fd, unsigned long req, void *arg)
{
    int r;
    do { r = ioctl(fd, req, arg); } while (r == -1 && errno == EINTR);
    return r;
}

/*
 * YUYV (YUY2) -> RGBA conversion (BT.601)
 * Two pixels per 4-byte macropixel: Y0 U Y1 V
 * Output: RGBA8888, 4 bytes per pixel
 */
static void yuyv_to_rgba(const unsigned char *yuyv,
                          unsigned char       *rgba,
                          int width, int height)
{
    int total = width * height / 2;
    for (int i = 0; i < total; i++) {
        int y0 = yuyv[0];
        int u  = yuyv[1] - 128;
        int y1 = yuyv[2];
        int v  = yuyv[3] - 128;
        yuyv += 4;

        int r0 = y0 + (int)(1.402f * v);
        int g0 = y0 - (int)(0.344f * u) - (int)(0.714f * v);
        int b0 = y0 + (int)(1.772f * u);
        int r1 = y1 + (int)(1.402f * v);
        int g1 = y1 - (int)(0.344f * u) - (int)(0.714f * v);
        int b1 = y1 + (int)(1.772f * u);

#define CLAMP(x) ((x) < 0 ? 0 : (x) > 255 ? 255 : (x))
        *rgba++ = CLAMP(r0); *rgba++ = CLAMP(g0); *rgba++ = CLAMP(b0); *rgba++ = 255;
        *rgba++ = CLAMP(r1); *rgba++ = CLAMP(g1); *rgba++ = CLAMP(b1); *rgba++ = 255;
#undef CLAMP
    }
}

/* ── Public API ───────────────────────────────────────────────────────────── */

VGC_EXPORT
int vgc_enumerate_devices(VGCDeviceInfo *out, int max)
{
    int count = 0;
    struct dirent *ent;
    DIR *dir = opendir("/dev");
    if (!dir) return 0;

    while ((ent = readdir(dir)) != NULL && count < max) {
        if (strncmp(ent->d_name, "video", 5) != 0) continue;

        char path[NAME_MAX + 6];
        snprintf(path, sizeof(path), "/dev/%s", ent->d_name);

        int fd = open(path, O_RDWR | O_NONBLOCK);
        if (fd < 0) continue;

        struct v4l2_capability cap;
        if (xioctl(fd, VIDIOC_QUERYCAP, &cap) == 0 &&
            (cap.capabilities & V4L2_CAP_VIDEO_CAPTURE) &&
            (cap.capabilities & V4L2_CAP_STREAMING)) {

            strncpy(out[count].path, path, sizeof(out[count].path) - 1);
            strncpy(out[count].name, (char*)cap.card, VGC_DEVICE_NAME_LEN - 1);
            count++;
        }
        close(fd);
    }
    closedir(dir);
    return count;
}

VGC_EXPORT
VGCDevice* vgc_open_device(const char *path, int width, int height, int fps)
{
    VGCDevice *dev = (VGCDevice*)calloc(1, sizeof(VGCDevice));
    if (!dev) return NULL;

    dev->fd = open(path, O_RDWR | O_NONBLOCK);
    if (dev->fd < 0) { free(dev); return NULL; }

    struct v4l2_format fmt = {0};
    fmt.type                = V4L2_BUF_TYPE_VIDEO_CAPTURE;
    fmt.fmt.pix.width       = width;
    fmt.fmt.pix.height      = height;
    fmt.fmt.pix.pixelformat = V4L2_PIX_FMT_YUYV;
    fmt.fmt.pix.field       = V4L2_FIELD_NONE;

    if (xioctl(dev->fd, VIDIOC_S_FMT, &fmt) < 0) {
        close(dev->fd); free(dev); return NULL;
    }

    dev->width       = fmt.fmt.pix.width;
    dev->height      = fmt.fmt.pix.height;
    dev->pixelformat = fmt.fmt.pix.pixelformat;

    struct v4l2_streamparm parm = {0};
    parm.type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
    parm.parm.capture.timeperframe.numerator   = 1;
    parm.parm.capture.timeperframe.denominator = fps;
    xioctl(dev->fd, VIDIOC_S_PARM, &parm);
    dev->fps = fps;

    struct v4l2_requestbuffers req = {0};
    req.count  = VGC_MAX_BUFFERS;
    req.type   = V4L2_BUF_TYPE_VIDEO_CAPTURE;
    req.memory = V4L2_MEMORY_MMAP;

    if (xioctl(dev->fd, VIDIOC_REQBUFS, &req) < 0 || req.count < 2) {
        close(dev->fd); free(dev); return NULL;
    }

    dev->n_buffers = req.count;

    for (int i = 0; i < dev->n_buffers; i++) {
        struct v4l2_buffer buf = {0};
        buf.type   = V4L2_BUF_TYPE_VIDEO_CAPTURE;
        buf.memory = V4L2_MEMORY_MMAP;
        buf.index  = i;

        if (xioctl(dev->fd, VIDIOC_QUERYBUF, &buf) < 0) {
            close(dev->fd); free(dev); return NULL;
        }

        dev->buffers[i].length = buf.length;
        dev->buffers[i].start  = mmap(NULL, buf.length,
                                       PROT_READ | PROT_WRITE,
                                       MAP_SHARED, dev->fd, buf.m.offset);

        if (dev->buffers[i].start == MAP_FAILED) {
            close(dev->fd); free(dev); return NULL;
        }
    }

    for (int i = 0; i < dev->n_buffers; i++) {
        struct v4l2_buffer buf = {0};
        buf.type   = V4L2_BUF_TYPE_VIDEO_CAPTURE;
        buf.memory = V4L2_MEMORY_MMAP;
        buf.index  = i;
        xioctl(dev->fd, VIDIOC_QBUF, &buf);
    }

    enum v4l2_buf_type type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
    if (xioctl(dev->fd, VIDIOC_STREAMON, &type) < 0) {
        close(dev->fd); free(dev); return NULL;
    }

    dev->streaming = 1;
    return dev;
}

VGC_EXPORT
int vgc_grab_frame(VGCDevice *dev, unsigned char *rgba_out)
{
    if (!dev || !dev->streaming) return -1;

    struct v4l2_buffer buf = {0};
    buf.type   = V4L2_BUF_TYPE_VIDEO_CAPTURE;
    buf.memory = V4L2_MEMORY_MMAP;

    if (xioctl(dev->fd, VIDIOC_DQBUF, &buf) < 0) {
        if (errno == EAGAIN) return 0;
        return -1;
    }

    yuyv_to_rgba((unsigned char*)dev->buffers[buf.index].start,
                 rgba_out, dev->width, dev->height);

    xioctl(dev->fd, VIDIOC_QBUF, &buf);
    return 1;
}

VGC_EXPORT int vgc_get_width(VGCDevice *dev)  { return dev ? dev->width  : 0; }
VGC_EXPORT int vgc_get_height(VGCDevice *dev) { return dev ? dev->height : 0; }

VGC_EXPORT
void vgc_close_device(VGCDevice *dev)
{
    if (!dev) return;

    if (dev->streaming) {
        enum v4l2_buf_type type = V4L2_BUF_TYPE_VIDEO_CAPTURE;
        xioctl(dev->fd, VIDIOC_STREAMOFF, &type);
    }

    for (int i = 0; i < dev->n_buffers; i++) {
        if (dev->buffers[i].start && dev->buffers[i].start != MAP_FAILED)
            munmap(dev->buffers[i].start, dev->buffers[i].length);
    }

    if (dev->fd >= 0) close(dev->fd);
    free(dev);
}
