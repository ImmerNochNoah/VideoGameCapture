# Building VGC-Ally from Source

## Prerequisites

| Tool | Version | Notes |
|---|---|---|
| Unity | 2022.3.50f1 LTS | Match upstream project version |
| Unity IL2CPP Linux module | Same | Install via Unity Hub |
| CMake | ≥ 3.16 | For native V4L2 plugin |
| GCC | ≥ 11 | Linux build machine |
| `v4l-utils` | Any | For testing |

---

## Step 1 — Fork Setup

```bash
# Fork Vet-TV/VideoGameCapture-Ally on GitHub, then:
git clone https://github.com/Vet-TV/VideoGameCapture-Ally.git
cd VideoGameCapture-Ally

# Set upstream remote to pull VGC updates
git remote add upstream https://github.com/ImmerNochNoah/VideoGameCapture.git
git fetch upstream

# Overlay branch workflow:
# - main         = upstream VGC + ally-overlay merged
# - ally-dev     = active development
# - ally-overlay = our additions only (rebase-friendly)
```

---

## Step 2 — Build Native V4L2 Plugin

This creates `libvgc_v4l2.so` which Unity P/Invokes for capture card access.

```bash
cd native/v4l2
cmake -B build -DCMAKE_BUILD_TYPE=Release
cmake --build build

# Output auto-copied to Assets/Plugins/Linux/x86_64/libvgc_v4l2.so
```

**Verify the .so:**
```bash
ldd native/v4l2/build/libvgc_v4l2.so
# Should show: libc.so only (no external deps)

nm -D native/v4l2/build/libvgc_v4l2.so | grep vgc_
# Should show: vgc_enumerate_devices, vgc_open_device, etc.
```

---

## Step 3 — Unity Project Setup

1. Open Unity Hub → Add project → select repo root
2. Unity version: **2022.3.50f1 LTS** (install if missing)
3. Build Settings → Switch Platform → **Linux x86_64**
4. Player Settings:
   - Scripting Backend: **IL2CPP**
   - Graphics APIs: **Vulkan** (first), **OpenGLCore** (fallback)
   - Allow unsafe code: **✅** (required for native plugin P/Invoke)
5. Apply overlay scripts:
   - Copy `ally-overlay/Scripts/` → `Assets/Scripts/`
   - These add V4L2, gamepad nav, and XDG settings on top of upstream

---

## Step 4 — Merge Upstream Changes

When upstream VGC releases a new version:

```bash
git fetch upstream
git checkout ally-dev
git merge upstream/main

# Resolve conflicts — our overlay scripts are in separate namespaces
# so conflicts should be minimal. The main merge points are:
#   Assets/Scripts/CaptureManager.cs   ← we added platform abstraction
#   Assets/Scripts/AudioManager.cs     ← we added Linux audio path
#   Assets/Scripts/Settings/Settings.cs ← we redirected to XDGSettingsManager

# After merge, test the native plugin still loads:
# Unity Console should show: "[VGC-Ally] Found N V4L2 capture device(s)."
```

---

## Step 5 — Build Linux Release

**Via Unity Editor:**
1. File → Build Settings → Build
2. Output to `build/linux/output/`
3. Copy `build/linux/launch_vgc_ally.sh` alongside the binary
4. `chmod +x launch_vgc_ally.sh`

**Via GitHub Actions (automated):**
- Push a tag: `git tag v0.1.0-ally && git push --tags`
- CI builds, packages, and creates a GitHub Release automatically

---

## Testing on ROG Ally / SteamOS

```bash
# 1. Verify V4L2 capture card is visible
v4l2-ctl --list-devices

# 2. Check formats your card supports
v4l2-ctl --device /dev/video0 --list-formats-ext

# 3. Quick capture preview sanity check (no Unity needed)
mpv --no-config av://v4l2:/dev/video0 --profile=low-latency

# 4. Run VGC-Ally
./launch_vgc_ally.sh

# 5. Check logs at:
~/.config/vgc-ally/Player.log
```

---

## Troubleshooting

**"No V4L2 video devices found"**
- Ensure USB capture card is plugged in
- `sudo usermod -aG video $USER` then log out/in
- Check `dmesg | grep -i video` for kernel messages

**"libvgc_v4l2.so: cannot open shared object file"**
- Confirm `Assets/Plugins/Linux/x86_64/libvgc_v4l2.so` exists in Unity build output
- Run `./launch_vgc_ally.sh` (sets `LD_LIBRARY_PATH` correctly)

**Audio not working on SteamOS**
- SteamOS uses PipeWire. Confirm with: `pactl info | grep Server`
- FMOD Linux backend should auto-detect PipeWire via PulseAudio compatibility layer
- Check `~/.config/vgc-ally/settings.json` → `audioVolume` should be `0.0–1.0`

**Gray/washed-out video**
- Use **Color Range → Expand** in settings (v0.0.14 feature, preserved from upstream)
- Common with retro consoles outputting limited-range (16–235) signal
