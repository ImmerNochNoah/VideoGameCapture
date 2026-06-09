# VGC-Ally 🎮
### VideoGameCapture — ASUS ROG Ally / SteamOS Edition

**Fork of:** [ImmerNochNoah/VideoGameCapture](https://github.com/ImmerNochNoah/VideoGameCapture) (GPLv3)  
**Maintained by:** [Vet-TV](https://github.com/Vet-TV)  
**License:** GPLv3 (attribution to upstream required per license terms)

---

## What Is This?

VGC-Ally takes the excellent VideoGameCapture (VGC) project and optimizes it specifically for:

- **ASUS ROG Ally Z1 Extreme** handheld hardware
- **SteamOS 3.x** (Arch-based, Valve's official OS for the Ally as of 2025)
- **Gamepad-first UI** — no keyboard or mouse required
- **USB capture cards** via V4L2 (Video4Linux2) — plug in your Elgato, AVerMedia, etc.
- **Low-latency preview** tuned for the Ally's 1080p 120Hz display
- **Steam GameMode compatible** for full performance unlocking

---

## Hardware Target

| Component | Spec |
|---|---|
| Device | ASUS ROG Ally Z1 Extreme |
| CPU/APU | AMD Ryzen Z1 Extreme (8c/16t, RDNA 3 iGPU) |
| Display | 7" 1080p 120Hz touchscreen |
| USB | USB-C (USB 3.2 Gen 2 + DisplayPort Alt) |
| OS | SteamOS 3.x (KDE Plasma desktop mode) |
| Capture API | V4L2 via USB capture card |
| Audio | PipeWire (SteamOS default) |

**Tested capture cards:** Elgato HD60 X, AVerMedia Live Gamer Portable 2 Plus  
*(Any V4L2-compatible USB capture card should work — open an issue if yours doesn't)*

---

## What's Changed vs Upstream

| Area | Upstream VGC | VGC-Ally |
|---|---|---|
| Platform | Windows only (.exe) | Linux / SteamOS native |
| Capture API | DirectShow (Windows) | V4L2 (`/dev/video*`) |
| Audio backend | FMOD Windows | FMOD Linux → PipeWire |
| Input | Mouse + keyboard | Gamepad (Steam Input) + touch |
| UI layout | Desktop (1920×1080+) | Handheld-optimized (1080p compact) |
| Settings path | App directory | XDG: `~/.config/vgc-ally/` |
| Launch | .exe | Steam shortcut + `gamemoderun` wrapper |
| Color range | ✅ (v0.0.14 feature) | ✅ Retained + shader-optimized for RDNA3 |
| Video adjustments | Brightness/Contrast/Sat sliders | ✅ Retained + AMD FidelityFX hint added |
| Audio volume bug | 0.01–0.30 workaround | Fixed: proper normalized [0.0–1.0] range |

---

## Installation

### Prerequisites
- SteamOS 3.x (desktop mode) or any Arch-based Linux distro
- USB capture card (V4L2-compatible)
- `v4l-utils` installed: `sudo pacman -S v4l-utils` (or via Discover on SteamOS)

### Quick Install (Flatpak — recommended)
```bash
flatpak install flathub com.vettv.VGCAlly
```
*(Flatpak submission in progress — use manual install below until published)*

### Manual Install
```bash
# Download the latest release
wget https://github.com/Vet-TV/VideoGameCapture-Ally/releases/latest/download/VGCAlly-linux-x64.tar.gz

# Extract
tar -xzf VGCAlly-linux-x64.tar.gz
cd VGCAlly-linux-x64

# Run (with GameMode for best performance)
./launch_vgc_ally.sh
```

### Add to Steam (Gaming Mode)
1. Steam → Library → Add a Game → Add a Non-Steam Game
2. Browse to `launch_vgc_ally.sh`
3. Set launch options: *(none needed, script handles gamemoderun)*
4. Add artwork from the `steam-assets/` folder

---

## Capture Card Setup

```bash
# Verify your capture card is detected
v4l2-ctl --list-devices

# Check supported formats
v4l2-ctl --device /dev/video0 --list-formats-ext

# Test raw preview (optional sanity check)
mpv --no-config av://v4l2:/dev/video0 --profile=low-latency
```

---

## Building from Source

See [docs/BUILDING.md](docs/BUILDING.md) for full Unity IL2CPP Linux build instructions.

**TL;DR:**
```bash
git clone https://github.com/Vet-TV/VideoGameCapture-Ally.git
cd VideoGameCapture-Ally
git remote add upstream https://github.com/ImmerNochNoah/VideoGameCapture.git
git fetch upstream
# Open in Unity 2022.3 LTS or later
# Build Target: Linux x86_64, IL2CPP scripting backend, Vulkan graphics API
```

---

## Credits

- **[ImmerNochNoah/VideoGameCapture](https://github.com/ImmerNochNoah/VideoGameCapture)** — Original VGC project (GPLv3)
- **[Vet-TV / Channel82 LABS](https://github.com/Vet-TV)** — Ally/SteamOS fork

---

## License

GPLv3. All source modifications are open. If you use this code, keep it open and credit upstream VGC.
