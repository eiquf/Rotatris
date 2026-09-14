using System;
using UnityEngine;

namespace Rotatris.InputHandling
{
    /// <summary>
    /// Reads the legacy Input Manager, matching the reference site's
    /// keyboard mapping.
    /// </summary>
    public class UnityInputProvider : IInputProvider
    {
        private bool _errorLogged;

        public bool MoveLeftPressed() => SafeGetKeyDown(KeyCode.A) || SafeGetKeyDown(KeyCode.LeftArrow);
        public bool MoveRightPressed() => SafeGetKeyDown(KeyCode.D) || SafeGetKeyDown(KeyCode.RightArrow);
        public bool RotateCwPressed() => SafeGetKeyDown(KeyCode.W) || SafeGetKeyDown(KeyCode.X) || SafeGetKeyDown(KeyCode.UpArrow);
        public bool RotateCcwPressed() => SafeGetKeyDown(KeyCode.Z);
        public bool HardDropPressed() => SafeGetKeyDown(KeyCode.Space);
        public bool HoldPressed() => SafeGetKeyDown(KeyCode.C);
        public bool PausePressed() => SafeGetKeyDown(KeyCode.P) || SafeGetKeyDown(KeyCode.Escape);
        public bool RestartPressed() => SafeGetKeyDown(KeyCode.R);
        public bool SoftDropHeld() => SafeGetKey(KeyCode.S) || SafeGetKey(KeyCode.DownArrow);

        private bool SafeGetKeyDown(KeyCode key)
        {
            try { return Input.GetKeyDown(key); }
            catch (InvalidOperationException) { LogInputSettingError(); return false; }
        }

        private bool SafeGetKey(KeyCode key)
        {
            try { return Input.GetKey(key); }
            catch (InvalidOperationException) { LogInputSettingError(); return false; }
        }

        private void LogInputSettingError()
        {
            if (_errorLogged) return;
            _errorLogged = true;
            Debug.LogError(
                "Rotatris: keyboard input is disabled because this project's Active Input " +
                "Handling is set to 'Input System Package (New)' only. Go to Edit > Project " +
                "Settings > Player > Other Settings > Active Input Handling and set it to " +
                "'Both' (or 'Input Manager (Old)'), then restart Play mode.");
        }
    }
}
