using System;
using System.Diagnostics.CodeAnalysis;
using SharpDX;
using ExileCore.PoEMemory.MemoryObjects;

namespace CoPilot
{
    [SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
    public static class Helper
    {
        internal static Random random = new Random();
        private static Camera Camera => CoPilot.Instance.GameController.Game.IngameState.Camera;
        
        internal static float MoveTowards(float cur, float tar, float max)
        {
            if (Math.Abs(tar - cur) <= max)
                return tar;
            return cur + Math.Sign(tar - cur) * max;
        }
        internal static Vector2 WorldToValidScreenPosition(Vector3 worldPos)
        {
            var windowRect = CoPilot.Instance.GameController.Window.GetWindowRectangle();
            var screenPos = Camera.WorldToScreen(worldPos);
            var result = screenPos + windowRect.Location;

            var edgeBounds = 50;
            if (!windowRect.Intersects(new RectangleF(result.X, result.Y, edgeBounds, edgeBounds)))
            {
                //Adjust for offscreen entity. Need to clamp the screen position using the game window info. 
                if (result.X < windowRect.TopLeft.X) result.X = windowRect.TopLeft.X + edgeBounds;
                if (result.Y < windowRect.TopLeft.Y) result.Y = windowRect.TopLeft.Y + edgeBounds;
                if (result.X > windowRect.BottomRight.X) result.X = windowRect.BottomRight.X - edgeBounds;
                if (result.Y > windowRect.BottomRight.Y) result.Y = windowRect.BottomRight.Y - edgeBounds;
            }
            return result;
        }
    }
}