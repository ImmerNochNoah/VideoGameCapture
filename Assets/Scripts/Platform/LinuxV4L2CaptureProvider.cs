// LinuxV4L2CaptureProvider.cs
// VGC-Ally — Linux/SteamOS capture backend
//
// P/Invokes into libvgc_v4l2.so (native/v4l2/vgc_v4l2.c)
// Replaces the Windows DirectShow capture path from upstream VGC.
//
// License: GPLv3 — fork of ImmerNochNoah/VideoGameCapture

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VGCAlly.Platform
{
    /// <summary>
    /// Device descriptor returned by V4L2 enumeration.
    /// </summary>
    [Serializable]
    public class V4L2DeviceInfo
    {
        public string Path;   // e.g. /dev/video0
        public string Name;   // e.g. "Elgato HD60 X"
    }

    /// <summary>
    /// Linux V4L2 capture backend.
    /// Enumerates /dev/video* devices, opens one, and pumps RGBA frames
    /// into a Unity Texture2D each Update().
    ///
    /// Usage:
    ///   var provider = new LinuxV4L2CaptureProvider();
    ///   var devices  = provider.EnumerateDevices();
    ///   provider.Open(devices[0].Path, 1920, 1080, 60);
    ///   // In Update(): provider.GrabFrame(targetTexture);
    ///   provider.Close();
    /// </summary>
    public class LinuxV4L2CaptureProvider : IDisposable
    {
        // ── Native interop ──────────────────────────────────────────────────

        private const string LibName = "vgc_v4l2";

        // Must match VGCDeviceInfo layout in vgc_v4l2.c exactly
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct NativeDeviceInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string Path;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Name;
        }

        [DllImport(LibName, EntryPoint = "vgc_enumerate_devices")]
        private static extern int Native_EnumerateDevices(
            [Out] NativeDeviceInfo[] devices, int max);

        [DllImport(LibName, EntryPoint = "vgc_open_device")]
        private static extern IntPtr Native_OpenDevice(
            string path, int width, int height, int fps);

        [DllImport(LibName, EntryPoint = "vgc_grab_frame")]
        private static extern int Native_GrabFrame(IntPtr dev, IntPtr rgbaOut);

        [DllImport(LibName, EntryPoint = "vgc_get_width")]
        private static extern int Native_GetWidth(IntPtr dev);

        [DllImport(LibName, EntryPoint = "vgc_get_height")]
        private static extern int Native_GetHeight(IntPtr dev);

        [DllImport(LibName, EntryPoint = "vgc_close_device")]
        private static extern void Native_CloseDevice(IntPtr dev);

        // ── Internal state ──────────────────────────────────────────────────

        private IntPtr  _device     = IntPtr.Zero;
        private byte[]  _frameBuffer;
        private GCHandle _frameHandle;
        private int     _width;
        private int     _height;
        private bool    _disposed;

        // ── Public API ──────────────────────────────────────────────────────

        /// <summary>Enumerate all V4L2 capture devices on the system.</summary>
        public List<V4L2DeviceInfo> EnumerateDevices()
        {
            var native = new NativeDeviceInfo[16];
            int count  = Native_EnumerateDevices(native, native.Length);

            var result = new List<V4L2DeviceInfo>(count);
            for (int i = 0; i < count; i++)
                result.Add(new V4L2DeviceInfo
                {
                    Path = native[i].Path,
                    Name = native[i].Name
                });

            Debug.Log($"[VGC-Ally] Found {count} V4L2 capture device(s).");
            return result;
        }

        /// <summary>
        /// Open a capture device. Negotiates the closest available resolution/fps.
        /// Call EnumerateDevices() first to get a valid path.
        /// </summary>
        public bool Open(string devicePath, int requestedWidth = 1920,
                         int requestedHeight = 1080, int requestedFps = 60)
        {
            if (_device != IntPtr.Zero)
            {
                Debug.LogWarning("[VGC-Ally] Device already open. Close first.");
                return false;
            }

            _device = Native_OpenDevice(devicePath, requestedWidth, requestedHeight, requestedFps);

            if (_device == IntPtr.Zero)
            {
                Debug.LogError($"[VGC-Ally] Failed to open {devicePath}. " +
                               "Check permissions (user in 'video' group?) and that the card is plugged in.");
                return false;
            }

            _width  = Native_GetWidth(_device);
            _height = Native_GetHeight(_device);

            Debug.Log($"[VGC-Ally] Opened {devicePath} @ {_width}x{_height} {requestedFps}fps");

            // Allocate frame buffer: RGBA8888
            _frameBuffer = new byte[_width * _height * 4];
            _frameHandle = GCHandle.Alloc(_frameBuffer, GCHandleType.Pinned);

            return true;
        }

        /// <summary>
        /// Grab the latest frame and upload it to the provided Texture2D.
        /// Texture must be RGBA32 format and match the negotiated resolution
        /// (use NegotiatedWidth / NegotiatedHeight).
        /// Call every Update() or LateUpdate().
        /// </summary>
        /// <returns>True if a new frame was available and uploaded.</returns>
        public bool GrabFrame(Texture2D target)
        {
            if (_device == IntPtr.Zero || target == null) return false;

            int result = Native_GrabFrame(_device, _frameHandle.AddrOfPinnedObject());

            if (result == 1)
            {
                // Upload raw RGBA bytes to GPU texture
                target.LoadRawTextureData(_frameBuffer);
                target.Apply(false);  // false = don't recalculate mipmaps (perf)
                return true;
            }

            if (result < 0)
                Debug.LogError("[VGC-Ally] Frame grab error — device disconnected?");

            return false;
        }

        /// <summary>Create a Texture2D sized to the negotiated capture resolution.</summary>
        public Texture2D CreateCaptureTexture()
        {
            if (_device == IntPtr.Zero)
                throw new InvalidOperationException("Open a device first.");

            return new Texture2D(_width, _height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode   = TextureWrapMode.Clamp,
                name       = "VGCAlly_CaptureTexture"
            };
        }

        public int  NegotiatedWidth  => _width;
        public int  NegotiatedHeight => _height;
        public bool IsOpen           => _device != IntPtr.Zero;

        /// <summary>Stop capture, release V4L2 resources.</summary>
        public void Close()
        {
            if (_device == IntPtr.Zero) return;

            Native_CloseDevice(_device);
            _device = IntPtr.Zero;

            if (_frameHandle.IsAllocated)
                _frameHandle.Free();

            _frameBuffer = null;
            _width = _height = 0;

            Debug.Log("[VGC-Ally] Capture device closed.");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Close();
                _disposed = true;
            }
        }
    }
}
