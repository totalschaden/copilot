using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ExileCore;
using ExileCore.Shared;

namespace CoPilot
{
    public static class Keyboard
    {
        private const string CoroutineKeyPress = "KeyPress";
        private static Coroutine _keyboardCoroutine;
        
        private const int KeyeventfExtendedkey = 0x0001;
        private const int KeyeventfKeyup = 0x0002;

        [DllImport("user32.dll")]
        private static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool BlockInput(bool fBlockIt);

        public static void KeyDown(Keys key)
        {
            if (key.HasFlag(Keys.Shift))
            {
                KeyDown(Keys.ShiftKey);
            }
            if (key.HasFlag(Keys.Control))
            {
                KeyDown(Keys.ControlKey);
            }
            if (key.HasFlag(Keys.Alt))
            {
                KeyDown(Keys.Menu);
            }

            keybd_event((byte) key, 0, KeyeventfExtendedkey | 0, 0);
        }

        public static void KeyUp(Keys key)
        {
            keybd_event((byte) key, 0, KeyeventfExtendedkey | KeyeventfKeyup, 0); //0x7F

            if (key.HasFlag(Keys.Shift))
            {
                KeyUp(Keys.ShiftKey);
            }
            if (key.HasFlag(Keys.Control))
            {
                KeyUp(Keys.ControlKey);
            }
            if (key.HasFlag(Keys.Alt))
            {
                KeyUp(Keys.Menu);
            }
        }

        [DllImport("USER32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        public static bool IsKeyDown(int nVirtKey)
        {
            return GetKeyState(nVirtKey) < 0;
        }
        
        public static void KeyPress(Keys key, bool anyDelay = true)
        {
            if (anyDelay)
                CoPilot.instance.lastTimeAny = DateTime.Now;
            _keyboardCoroutine = new Coroutine(KeyPressRoutine(key), CoPilot.instance, CoroutineKeyPress);
            Core.ParallelRunner.Run(_keyboardCoroutine);
        }

        private static IEnumerator KeyPressRoutine(Keys key)
        {
            if (key.HasFlag(Keys.Shift))
            {
                KeyDown(Keys.ShiftKey);
                yield return new WaitTime(20);
            }
            if (key.HasFlag(Keys.Control))
            {
                KeyDown(Keys.ControlKey);
                yield return new WaitTime(20);
            }
            if (key.HasFlag(Keys.Alt))
            {
                KeyDown(Keys.Menu);
                yield return new WaitTime(20);
            }

            KeyDown(key & Keys.KeyCode);
            yield return new WaitTime(20);
            KeyUp(key & Keys.KeyCode);

            if (key.HasFlag(Keys.Shift))
            {
                yield return new WaitTime(20);
                KeyUp(Keys.ShiftKey);
            }
            if (key.HasFlag(Keys.Control))
            {
                yield return new WaitTime(20);
                KeyUp(Keys.ControlKey);
            }
            if (key.HasFlag(Keys.Alt))
            {
                yield return new WaitTime(20);
                KeyUp(Keys.Menu);
            }
        }
    }
}