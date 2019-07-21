using UnityEngine;

namespace ParadoxNotion
{

    ///to create styles easier like 'new GUIStyle().Padding(2,2,2,2).Margin(1,1,1,1)...' etc
    public static class GUIStyleUtils
    {

        public static GUIStyle Margin(this GUIStyle style, int left, int right, int top, int bottom) {
            style.margin = new RectOffset(left, right, top, bottom);
            return style;
        }

        public static GUIStyle Padding(this GUIStyle style, int left, int right, int top, int bottom) {
            style.padding = new RectOffset(left, right, top, bottom);
            return style;
        }

        public static GUIStyle Border(this GUIStyle style, int left, int right, int top, int bottom) {
            style.border = new RectOffset(left, right, top, bottom);
            return style;
        }

        public static GUIStyle Overflow(this GUIStyle style, int left, int right, int top, int bottom) {
            style.overflow = new RectOffset(left, right, top, bottom);
            return style;
        }

        public static GUIStyle TextAlignment(this GUIStyle style, TextAnchor anchor) {
            style.alignment = anchor;
            return style;
        }

        public static GUIStyle RichText(this GUIStyle style, bool rich) {
            style.richText = rich;
            return style;
        }
    }
}