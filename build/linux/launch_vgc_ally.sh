#!/usr/bin/env bash
# launch_vgc_ally.sh — VGC-Ally Steam launch wrapper
#
# Handles:
#   - gamemoderun (AMD perf profile unlock on ROG Ally)
#   - V4L2 device permission check (user must be in 'video' group)
#   - PipeWire sanity check
#   - First-run XDG config directory creation
#   - Correct LD_LIBRARY_PATH for libvgc_v4l2.so
#
# Place this alongside the VGCAlly binary.
# Add to Steam as a Non-Steam game: "launch_vgc_ally.sh"

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BINARY="${SCRIPT_DIR}/VGCAlly.x86_64"
NATIVE_LIB_DIR="${SCRIPT_DIR}/VGCAlly_Data/Plugins/Linux/x86_64"
CONFIG_DIR="${XDG_CONFIG_HOME:-$HOME/.config}/vgc-ally"

# ── Preflight checks ──────────────────────────────────────────────────────────

# 1. Check binary exists
if [[ ! -x "${BINARY}" ]]; then
    echo "[VGC-Ally] ERROR: Binary not found at ${BINARY}"
    exit 1
fi

# 2. V4L2 device check — warn if no capture cards detected
if ! ls /dev/video* &>/dev/null; then
    echo "[VGC-Ally] WARNING: No V4L2 video devices found in /dev/video*."
    echo "           Plug in your USB capture card before starting."
    # Don't abort — user may plug it in after launch
fi

# 3. Video group check
if ! id -Gn | grep -qw "video"; then
    echo "[VGC-Ally] WARNING: Current user is not in the 'video' group."
    echo "           Capture cards may not be accessible."
    echo "           Fix: sudo usermod -aG video \$USER  (then log out/in)"
fi

# 4. PipeWire check (SteamOS ships PipeWire; ALSA fallback if missing)
if ! pactl info &>/dev/null; then
    echo "[VGC-Ally] WARNING: PipeWire/PulseAudio not responding. Audio may not work."
fi

# 5. Create XDG config dir if it doesn't exist
mkdir -p "${CONFIG_DIR}"

# ── Library path ──────────────────────────────────────────────────────────────

export LD_LIBRARY_PATH="${NATIVE_LIB_DIR}:${LD_LIBRARY_PATH:-}"

# ── GameMode ──────────────────────────────────────────────────────────────────
# GameMode requests performance CPU governor + disables power throttling.
# Critical on the ROG Ally Z1 Extreme for consistent low-latency capture.

if command -v gamemoderun &>/dev/null; then
    echo "[VGC-Ally] Launching with GameMode (performance profile)..."
    exec gamemoderun "${BINARY}" "$@"
else
    echo "[VGC-Ally] gamemoderun not found — launching without GameMode."
    echo "           Install: sudo pacman -S gamemode  (or via Discover)"
    exec "${BINARY}" "$@"
fi
