#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace ParadoxNotion.Design
{

    ///Common Styles Database
    public static class Styles
    {

        private static GUIStyle _centerLabel;
        public static GUIStyle centerLabel {
            get
            {
                if ( _centerLabel == null ) {
                    _centerLabel = new GUIStyle(GUI.skin.GetStyle("label"));
                    _centerLabel.richText = true;
                    _centerLabel.alignment = TextAnchor.MiddleCenter;
                }
                return _centerLabel;
            }
        }


        private static GUIStyle _leftLabel;
        public static GUIStyle leftLabel {
            get
            {
                if ( _leftLabel == null ) {
                    _leftLabel = new GUIStyle(GUI.skin.GetStyle("label"));
                    _leftLabel.richText = true;
                    _leftLabel.alignment = TextAnchor.MiddleLeft;
                    _leftLabel.padding.right = 6;
                }
                return _leftLabel;
            }
        }

        private static GUIStyle _rightLabel;
        public static GUIStyle rightLabel {
            get
            {
                if ( _rightLabel == null ) {
                    _rightLabel = new GUIStyle(GUI.skin.GetStyle("label"));
                    _rightLabel.richText = true;
                    _rightLabel.alignment = TextAnchor.MiddleRight;
                    _rightLabel.padding.left = 6;
                }
                return _rightLabel;
            }
        }

        ///----------------------------------------------------------------------------------------------

        private static GUIStyle _roundedBox;
        public static GUIStyle roundedBox {
            get
            {
                if ( _roundedBox != null ) { return _roundedBox; }
                _roundedBox = new GUIStyle((GUIStyle)"ShurikenEffectBg");
                if ( !UnityEditor.EditorGUIUtility.isProSkin ) {
                    _roundedBox.normal.background = null;
                }
                return _roundedBox;
            }
        }

        private static GUIStyle _highlightBox;
        public static GUIStyle highlightBox {
            get { return _highlightBox ?? ( _highlightBox = new GUIStyle((GUIStyle)"LightmapEditorSelectedHighlight") ); }
        }

        private static GUIStyle _toolbarSearchField;
        public static GUIStyle toolbarSearchTextField {
            get { return _toolbarSearchField ?? ( _toolbarSearchField = new GUIStyle((GUIStyle)"ToolbarSeachTextField") ); }
        }

        private static GUIStyle _toolbarSearchButton;
        public static GUIStyle toolbarSearchCancelButton {
            get { return _toolbarSearchButton ?? ( _toolbarSearchButton = new GUIStyle((GUIStyle)"ToolbarSeachCancelButton") ); }
        }

        private static GUIStyle _shadowedBackground;
        public static GUIStyle shadowedBackground {
            get { return _shadowedBackground ?? ( _shadowedBackground = new GUIStyle((GUIStyle)"CurveEditorBackground") ); }
        }

        ///----------------------------------------------------------------------------------------------

    }
}

#endif