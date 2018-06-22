#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NodeCanvas.Editor{

	[InitializeOnLoad]
	public static class CanvasStyles {

		private static GUISkin styleSheet;

		static CanvasStyles(){ Load(); }
		
		[InitializeOnLoadMethod]
		static void Load(){
			styleSheet = Resources.Load<GUISkin>( EditorGUIUtility.isProSkin? "StyleSheet/StyleSheetDark" : "StyleSheet/StyleSheetLight" );
		}

		///----------------------------------------------------------------------------------------------

		public static GUIStyle window{
			get {return styleSheet.window;}
		}

		public static GUIStyle button{
			get {return styleSheet.button;}
		}

		public static GUIStyle box{
			get {return styleSheet.box;}
		}

		public static GUIStyle label{
			get {return styleSheet.label;}
		}

		public static GUIStyle textField{
			get {return styleSheet.textField;}
		}

		public static GUIStyle textArea{
			get {return styleSheet.textArea;}
		}

		///----------------------------------------------------------------------------------------------

		private static GUIStyle _nodePortConnected;
		public static GUIStyle nodePortConnected{
			get {return _nodePortConnected?? (_nodePortConnected = styleSheet.GetStyle("nodePortConnected"));}
		}

		private static GUIStyle _nodePortEmpty;
		public static GUIStyle nodePortEmpty{
			get {return _nodePortEmpty?? (_nodePortEmpty = styleSheet.GetStyle("nodePortEmpty"));}
		}

		private static GUIStyle _arrowRight;
		public static GUIStyle arrowRight{
			get {return _arrowRight?? (_arrowRight = styleSheet.GetStyle("arrowRight"));}
		}

		private static GUIStyle _arrowLeft;
		public static GUIStyle arrowLeft{
			get {return _arrowLeft?? (_arrowLeft = styleSheet.GetStyle("arrowLeft"));}
		}

		private static GUIStyle _arrowBottom;
		public static GUIStyle arrowBottom{
			get {return _arrowBottom?? (_arrowBottom = styleSheet.GetStyle("arrowBottom"));}
		}

		private static GUIStyle _arrowTop;
		public static GUIStyle arrowTop{
			get {return _arrowTop?? (_arrowTop = styleSheet.GetStyle("arrowTop"));}
		}

		private static GUIStyle _nodePortContainer;
		public static GUIStyle nodePortContainer{
			get {return _nodePortContainer?? (_nodePortContainer = styleSheet.GetStyle("nodePortContainer"));}
		}

		private static GUIStyle _scaleArrow;
		public static GUIStyle scaleArrow{
			get {return _scaleArrow?? (_scaleArrow =  styleSheet.GetStyle("scaleArrow"));}
		}

		private static GUIStyle _scaleArrowTL;
		public static GUIStyle scaleArrowTL{
			get {return _scaleArrowTL?? (_scaleArrowTL = styleSheet.GetStyle("scaleArrowTL"));}
		}

		private static GUIStyle _canvasBG;
		public static GUIStyle canvasBG{
			get {return _canvasBG?? (_canvasBG = styleSheet.GetStyle("canvasBG"));}
		}

		private static GUIStyle _canvasBorders;
		public static GUIStyle canvasBorders{
			get {return _canvasBorders?? (_canvasBorders = styleSheet.GetStyle("canvasBorders"));}
		}

		private static GUIStyle _windowShadow;
		public static GUIStyle windowShadow{
			get {return _windowShadow?? (_windowShadow = styleSheet.GetStyle("windowShadow"));}
		}

		private static GUIStyle _checkMark;
		public static GUIStyle checkMark{
			get {return _checkMark?? (_checkMark = styleSheet.GetStyle("checkMark"));}
		}

		private static GUIStyle _clockMark;
		public static GUIStyle clockMark{
			get {return _clockMark?? (_clockMark = styleSheet.GetStyle("clockMark"));}
		}

		private static GUIStyle _xMark;
		public static GUIStyle xMark{
			get {return _xMark?? (_xMark = styleSheet.GetStyle("xMark"));}
		}

		private static GUIStyle _windowHighlight;
		public static GUIStyle windowHighlight{
			get {return _windowHighlight?? (_windowHighlight = styleSheet.GetStyle("windowHighlight"));}
		}

		private static GUIStyle _editorPanel;
		public static GUIStyle editorPanel{
			get {return _editorPanel?? (_editorPanel = styleSheet.GetStyle("editorPanel"));}
		}

		private static GUIStyle _circle;
		public static GUIStyle circle{
			get {return _circle?? (_circle = styleSheet.GetStyle("circle"));}
		}		

		private static GUIStyle _windowHeader;
		public static GUIStyle windowHeader{
			get {return _windowHeader?? (_windowHeader = styleSheet.GetStyle("windowHeader"));}
		}


		private static GUIStyle _nodeTitle;
		public static GUIStyle nodeTitle{
			get
			{
				if (_nodeTitle == null){
					_nodeTitle = new GUIStyle();
					_nodeTitle.margin = new RectOffset(4,4,4,4);
					_nodeTitle.padding = new RectOffset(0,0,3,3);
					_nodeTitle.alignment = TextAnchor.MiddleCenter;
					_nodeTitle.richText = true;
				}
				return _nodeTitle;
			}			
		}

	}
}

#endif