using UnityEngine;

namespace Helpers
{
    public static class ColorHelpers
    {
        public static Color WithA(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }
    }
}