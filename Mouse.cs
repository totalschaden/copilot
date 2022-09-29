using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ExileCore.Shared;
using SharpDX;

namespace CoPilot
{
    [SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
    public static class Mouse
    {
        public const int MouseeventfMove = 0x0001;
        public const int MouseeventfLeftdown = 0x02;
        public const int MouseeventfLeftup = 0x04;
        public const int MouseeventfMiddown = 0x0020;
        public const int MouseeventfMidup = 0x0040;
        public const int MouseeventfRightdown = 0x0008;
        public const int MouseeventfRightup = 0x0010;
        public const int MouseEventWheel = 0x800;
        
        public static float speedMouse = 1;
        public static bool IsMouseLeftPressed()
        {
            return Control.MouseButtons == MouseButtons.Left;
        }
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        public static void LeftMouseDown()
        {
            mouse_event(MouseeventfLeftdown, 0, 0, 0, 0);
        }

        public static void LeftMouseUp()
        {
            mouse_event(MouseeventfLeftup, 0, 0, 0, 0);
        }

        public static void RightMouseDown()
        {
            mouse_event(MouseeventfRightdown, 0, 0, 0, 0);
        }

        public static void RightMouseUp()
        {
            mouse_event(MouseeventfRightup, 0, 0, 0, 0);
        }
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);
        public static void SetCursorPos(Vector2 vec)
        {
            SetCursorPos((int)vec.X, (int)vec.Y);
        }

        public static IEnumerator LeftClick()
        {
            LeftMouseDown();
            yield return new WaitTime(40);
            LeftMouseUp();
            yield return new WaitTime(100);
        }
        
        public static IEnumerator RightClick()
        {
            RightMouseDown();
            yield return new WaitTime(40);
            RightMouseUp();
            yield return new WaitTime(100);
        }

        public static IEnumerator SetCursorPosHuman(Vector2 targetPos, bool limited = true)
        {
            // Keep Curser Away from Screen Edges to prevent UI Interaction.
            var windowRect = CoPilot.Instance.GameController.Window.GetWindowRectangle();
            var edgeBoundsX = windowRect.Size.Width / 4;
            var edgeBoundsY = windowRect.Size.Height / 4;

            if (limited)
            {
                if (targetPos.X <= windowRect.Left + edgeBoundsX ) targetPos.X = windowRect.Left + edgeBoundsX;
                if (targetPos.Y <= windowRect.Top + edgeBoundsY) targetPos.Y = windowRect.Left + edgeBoundsY;
                if (targetPos.X >= windowRect.Right - edgeBoundsX) targetPos.X = windowRect.Right -edgeBoundsX;
                if (targetPos.Y >= windowRect.Bottom - edgeBoundsY) targetPos.Y = windowRect.Bottom - edgeBoundsY;
            }
            
            
            var step = (float)Math.Sqrt(Vector2.Distance(CoPilot.Instance.GetMousePosition(), targetPos)) * speedMouse / 20;

            if (step > 6)
                for (var i = 0; i < step; i++)
                {
                    var vector2 = Vector2.SmoothStep(CoPilot.Instance.GetMousePosition(), targetPos, i / step);
                    SetCursorPos((int)vector2.X, (int)vector2.Y);
                    yield return new WaitTime(5);
                }
            else
                SetCursorPos(targetPos);
        }
        public static IEnumerator SetCursorPosAndLeftClickHuman(Vector2 coords, int extraDelay)
        {
            SetCursorPos(coords);
            yield return new WaitTime(CoPilot.Instance.Settings.autoPilotInputFrequency + extraDelay);
            LeftMouseDown();
            yield return new WaitTime(CoPilot.Instance.Settings.autoPilotInputFrequency + extraDelay);
            LeftMouseUp();
            yield return new WaitTime(100);
        }
    }
}