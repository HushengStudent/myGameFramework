#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace ParadoxNotion.Design
{

    [InitializeOnLoad]
    public static class Colors
    {

        static Colors() { Load(); }

        [InitializeOnLoadMethod]
        static void Load() {
            lightOrange = EditorGUIUtility.isProSkin ? new Color(1, 0.9f, 0.4f) : Color.white;
            lightBlue = EditorGUIUtility.isProSkin ? new Color(0.8f, 0.8f, 1) : Color.white;
            lightRed = EditorGUIUtility.isProSkin ? new Color(1, 0.5f, 0.5f, 0.8f) : Color.white;
        }

        public const string HEX_LIGHT = "eeeeee";
        public const string HEX_DARK = "333333";

        public static Color lightOrange { get; private set; }
        public static Color lightBlue { get; private set; }
        public static Color lightRed { get; private set; }

        ///----------------------------------------------------------------------------------------------

        ///A greyscale color
        public static Color Grey(float value) {
            return new Color(value, value, value);
        }

        ///----------------------------------------------------------------------------------------------

        ///Return a color for a type.
        public static Color GetTypeColor(System.Type type) {
            return TypePrefs.GetTypeColor(type);
        }

        ///Return a string hex color for a type.
        public static string GetTypeHexColor(System.Type type) {
            return TypePrefs.GetTypeHexColor(type);
        }

    }
}

#endif