using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion
{

    public static class ColorUtils
    {

        ///The color with alpha
        public static Color WithAlpha(this Color color, float alpha) {
            color.a = alpha;
            return color;
        }

        ///Convert Color to Hex.
        private static Dictionary<Color32, string> colorHexCache = new Dictionary<Color32, string>();
        public static string ColorToHex(Color32 color) {
#if UNITY_EDITOR
            {
                if ( !UnityEditor.EditorGUIUtility.isProSkin ) {
                    if ( color == Color.white ) {
                        return "#000000";
                    }
                }
            }
#endif

            string result;
            if ( colorHexCache.TryGetValue(color, out result) ) {
                return result;
            }
            result = ( "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") ).ToUpper();
            return colorHexCache[color] = result;
        }

        ///Convert Hex to Color.
        private static Dictionary<string, Color> hexColorCache = new Dictionary<string, Color>(System.StringComparer.OrdinalIgnoreCase);
        public static Color HexToColor(string hex) {
            Color result;
            if ( hexColorCache.TryGetValue(hex, out result) ) {
                return result;
            }
            if ( hex.Length != 6 ) {
                throw new System.Exception("Invalid length for hex color provided");
            }
            var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            result = new Color32(r, g, b, 255);
            return hexColorCache[hex] = result;
        }

    }
}