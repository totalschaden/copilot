using System;

namespace CoPilot
{
    public static class Helper
    {
        internal static Random random = new Random();
        internal static float MoveTowards(float cur, float tar, float max)
        {
            if (Math.Abs(tar - cur) <= max)
                return tar;
            return cur + Math.Sign(tar - cur) * max;
        }
    }
}