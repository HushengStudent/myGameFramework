#if UNITY_EDITOR

using UnityEngine;
using ParadoxNotion.Design;

namespace NodeCanvas.Editor
{

    public partial class StyleSheet
    {

        private static GUIStyle _portContentImage;
        public static GUIStyle portContentImage {
            get
            {
                if ( _portContentImage == null ) {
                    _portContentImage = new GUIStyle(GUI.skin.GetStyle("label"));
                    _portContentImage.alignment = TextAnchor.MiddleCenter;
                    _portContentImage.padding = new RectOffset(0, 0, _portContentImage.padding.top, _portContentImage.padding.bottom);
                    _portContentImage.margin = new RectOffset(0, 0, _portContentImage.margin.top, _portContentImage.margin.bottom);
                }
                return _portContentImage;
            }
        }

        private static GUIStyle _rightPortLabel;
        public static GUIStyle rightPortLabel {
            get
            {
                if ( _rightPortLabel == null ) {
                    _rightPortLabel = new GUIStyle(ParadoxNotion.Design.Styles.rightLabel);
                    _rightPortLabel.margin = new RectOffset(0, 0, _rightPortLabel.margin.top, _rightPortLabel.margin.bottom);
                    _rightPortLabel.padding = new RectOffset(8, 0, _rightPortLabel.padding.top, _rightPortLabel.padding.bottom);
                }
                return _rightPortLabel;
            }
        }

        private static GUIStyle _leftPortlabel;
        public static GUIStyle leftPortLabel {
            get
            {
                if ( _leftPortlabel == null ) {
                    _leftPortlabel = new GUIStyle(ParadoxNotion.Design.Styles.leftLabel);
                    _leftPortlabel.margin = new RectOffset(0, 0, _leftPortlabel.margin.top, _leftPortlabel.margin.bottom);
                    _leftPortlabel.padding = new RectOffset(0, 8, _leftPortlabel.padding.top, _leftPortlabel.padding.bottom);
                }
                return _leftPortlabel;
            }
        }

    }

}

#endif