// AllyInputBridge.cs
// VGC-Ally — Bridges gamepad input to existing VGC keyboard shortcuts
//
// Upstream VGC uses keyboard shortcuts:
//   Escape → open settings menu
//   F5     → toggle fullscreen
//   F9     → screenshot
//   Up/Down arrows → volume
//   M      → mute
//
// This bridge maps ROG Ally gamepad buttons to those same actions
// by calling the existing VideoGameCaptureController methods directly.
// No upstream code changes needed.
//
// Button mapping:
//   Menu (Start)    → Settings (was: Escape)
//   View (Select)   → Screenshot (was: F9)
//   L3 + R3         → Toggle fullscreen (was: F5)
//   R-Stick Up/Down → Volume +/- (was: Arrow keys)
//   R3 (click)      → Mute (was: M)
//
// Attach to the same GameObject as VideoGameCaptureController.
//
// License: GPLv3 — fork of ImmerNochNoah/VideoGameCapture

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(VideoGameCaptureController))]
public class AllyInputBridge : MonoBehaviour
{
    [SerializeField] private VideoGameCaptureController vgcc;

    [Header("Volume Adjustment")]
    [SerializeField] private float volumeStep   = 0.10f;
    [SerializeField] private float volumeRepeatDelay    = 0.5f;
    [SerializeField] private float volumeRepeatInterval = 0.15f;

    // Input actions
    private InputAction _settings;
    private InputAction _screenshot;
    private InputAction _fullscreen;
    private InputAction _volumeAxis;
    private InputAction _mute;

    private float _volRepeatTimer;
    private bool  _volHeld;

    private void Awake()
    {
        if (vgcc == null)
            vgcc = GetComponent<VideoGameCaptureController>();

        var map = new InputActionMap("AllyBridge");

        _settings   = map.AddAction("Settings",    binding: "<Gamepad>/start");
        _screenshot = map.AddAction("Screenshot",  binding: "<Gamepad>/select");
        _mute       = map.AddAction("Mute",        binding: "<Gamepad>/rightStickPress");
        _fullscreen = map.AddAction("Fullscreen",  InputActionType.Button);
        // L3 + R3 simultaneous — bind both sticks, check both in callback
        _fullscreen.AddBinding("<Gamepad>/leftStickPress");

        _volumeAxis = map.AddAction("VolumeAxis", InputActionType.Value);
        _volumeAxis.AddBinding("<Gamepad>/rightStick/y");

        map.Enable();

        _settings.performed   += _ => vgcc.openSettingsMenu();
        _screenshot.performed += _ => vgcc.screenshotManager.takeScreenshot();
        _mute.performed       += _ => vgcc.muteSound();

        // Fullscreen: require both stick presses to avoid accidents
        _fullscreen.performed += ctx =>
        {
            var gp = Gamepad.current;
            if (gp != null &&
                gp.leftStickButton.isPressed &&
                gp.rightStickButton.isPressed)
            {
                vgcc.changeWindowMode();
            }
        };
    }

    private void Update()
    {
        HandleVolumeRepeat();
    }

    private void HandleVolumeRepeat()
    {
        float axis = _volumeAxis.ReadValue<float>();
        if (Mathf.Abs(axis) < 0.5f)
        {
            _volHeld = false;
            _volRepeatTimer = 0f;
            return;
        }

        if (!_volHeld)
        {
            _volHeld = true;
            _volRepeatTimer = Time.unscaledTime + volumeRepeatDelay;
            ApplyVolume(axis);
            return;
        }

        if (Time.unscaledTime >= _volRepeatTimer)
        {
            _volRepeatTimer = Time.unscaledTime + volumeRepeatInterval;
            ApplyVolume(axis);
        }
    }

    private void ApplyVolume(float axis)
    {
        float delta = axis > 0 ? volumeStep : -volumeStep;
        vgcc.changeAudioVolume(vgcc.audioPureFMOD.volume + delta);
    }

    private void OnDestroy()
    {
        _settings?.Disable();
        _screenshot?.Disable();
        _fullscreen?.Disable();
        _volumeAxis?.Disable();
        _mute?.Disable();
    }
}
