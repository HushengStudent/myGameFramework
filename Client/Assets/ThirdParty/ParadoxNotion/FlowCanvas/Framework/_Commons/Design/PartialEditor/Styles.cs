#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace ParadoxNotion.Design {

	///Common Styles Database
	public static class Styles {

		private static GUIStyle _centerLabel;
		public static GUIStyle centerLabel{
			get
			{
				if (_centerLabel == null){
					_centerLabel = new GUIStyle(GUI.skin.GetStyle("label"));
					_centerLabel.richText = true;
					_centerLabel.alignment = TextAnchor.MiddleCenter;
				}
				return _centerLabel;
			}
		}


		private static GUIStyle _leftLabel;
		public static GUIStyle leftLabel{
			get
			{
				if (_leftLabel == null){
					_leftLabel = new GUIStyle(GUI.skin.GetStyle("label"));
					_leftLabel.richText = true;
					_leftLabel.alignment = TextAnchor.MiddleLeft;
				}
				return _leftLabel;
			}
		}

		private static GUIStyle _rightLabel;
		public static GUIStyle rightLabel{
			get
			{
				if (_rightLabel == null){
					_rightLabel = new GUIStyle(GUI.skin.GetStyle("label"));
					_rightLabel.richText = true;
					_rightLabel.alignment = TextAnchor.MiddleRight;
				}
				return _rightLabel;
			}
		}

		///----------------------------------------------------------------------------------------------

		private static GUIStyle _roundedBox;
		public static GUIStyle roundedBox{
			get {return _roundedBox?? (_roundedBox = (GUIStyle)"ShurikenEffectBg"); }
		}

		private static GUIStyle _highlightBox;
		public static GUIStyle highlightBox{
			get {return _highlightBox?? (_highlightBox = (GUIStyle)"LightmapEditorSelectedHighlight"); }
		}

		private static GUIStyle _toolbarSearchField;
		public static GUIStyle toolbarSearchTextField{
			get {return _toolbarSearchField?? (_toolbarSearchField = (GUIStyle)"ToolbarSeachTextField"); }
		}

		private static GUIStyle _toolbarSearchButton;
		public static GUIStyle toolbarSearchCancelButton{
			get {return _toolbarSearchButton?? (_toolbarSearchButton = (GUIStyle)"ToolbarSeachCancelButton"); }
		}

		private static GUIStyle _shadowedBackground;
		public static GUIStyle shadowedBackground{
			get {return _shadowedBackground?? (_shadowedBackground = (GUIStyle)"CurveEditorBackground"); }
		}		

		///----------------------------------------------------------------------------------------------

	}
}

#endif