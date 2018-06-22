#if UNITY_EDITOR

using UnityEditor;
using ParadoxNotion.Serialization;

namespace NodeCanvas.Editor{

	///NC framework preferences
	public static class NCPrefs {

		[System.Serializable]
		class SerializedData{
			public bool showNodeInfo               = true;
			public bool isLocked                   = false;
			public bool showIcons                  = true;
			public NodeHeaderStyle nodeHeaderStyle = NodeHeaderStyle.ColorizeHeader;
			public ConnectionStyle connectionStyle = ConnectionStyle.Curved;
			public bool doSnap                     = false;
			public bool showTaskSummary            = true;
			public bool showBlackboard             = true;
			public bool showNodePanel              = true;
			public bool showComments               = true;
			public bool showNodeIDs                = false;
			public bool hierarchicalMove           = false;
			public bool useExternalInspector       = false;
			public bool showWelcomeWindow          = true;
			public bool logEvents                  = true;
			public bool highlightOwnersInHierarchy = false;
			public bool useBrowser                 = true;
			public bool breakpointPauseEditor      = true;
			public float inspectorPanelWidth       = 330;
			public float blackboardPanelWidth      = 350;

			public bool consoleLogInfo             = true;
			public bool consoleLogWarning          = true;
			public bool consoleLogError            = true;
			public ConsoleLogOrder consoleLogOrder = ConsoleLogOrder.Ascending;

			public UnityEngine.Vector2 minimapSize = new UnityEngine.Vector2(170, 100);
		}

		private static SerializedData _data;
		private static SerializedData data{
			get
			{
				if (_data == null){
					var pref = EditorPrefs.GetString("NodeCanvas.EditorPreferences");
					if (!string.IsNullOrEmpty(pref)){
						_data = JSONSerializer.Deserialize<SerializedData>(pref);
					}
					if (_data == null){
						_data = new SerializedData();
					}
				}
				return _data;
			}
		}

		private static UnityEngine.Vector2 minimapMinSize = new UnityEngine.Vector2(50,30);
		private static UnityEngine.Vector2 minimapMaxSize = new UnityEngine.Vector2(500,300);

		public enum NodeHeaderStyle
		{
			ColorizeHeader,
			ColorizeTitle
		}

		public enum ConnectionStyle
		{
			Curved,
			Stepped,
			Linear
		}

		public enum ConsoleLogOrder
		{
			Ascending,
			Descending
		}

		public static bool showNodeInfo{
			get {return data.showNodeInfo;}
			set {if (data.showNodeInfo != value){ data.showNodeInfo = value; Save(); } }
		}

		public static bool isLocked{
			get {return data.isLocked;}
			set {if (data.isLocked != value){ data.isLocked = value; Save(); } }
		}

		public static bool showIcons{
			get {return data.showIcons;}
			set {if (data.showIcons != value){ data.showIcons = value; Save(); } }
		}

		public static NodeHeaderStyle nodeHeaderStyle{
			get {return data.nodeHeaderStyle;}
			set {if (data.nodeHeaderStyle != value){ data.nodeHeaderStyle = value; Save(); } }
		}

		public static ConnectionStyle connectionStyle{
			get {return data.connectionStyle;}
			set {if (data.connectionStyle != value){ data.connectionStyle = value; Save(); } }
		}

		public static bool doSnap{
			get {return data.doSnap;}
			set {if (data.doSnap != value){ data.doSnap = value; Save(); } }
		}

		public static bool showTaskSummary{
			get {return data.showTaskSummary;}
			set {if (data.showTaskSummary != value){ data.showTaskSummary = value; Save(); } }
		}

		public static bool showBlackboard{
			get {return data.showBlackboard;}
			set {if (data.showBlackboard != value){ data.showBlackboard = value; Save(); } }
		}

		public static bool showNodePanel{
			get {return data.showNodePanel;}
			set {if (data.showNodePanel != value){ data.showNodePanel = value; Save(); } }
		}

		public static bool showComments{
			get {return data.showComments;}
			set {if (data.showComments != value){ data.showComments = value; Save(); } }
		}

		public static bool showNodeIDs{
			get {return data.showNodeIDs;}
			set {if (data.showNodeIDs != value){ data.showNodeIDs = value; Save(); } }
		}

		public static bool hierarchicalMove{
			get {return data.hierarchicalMove;}
			set {if (data.hierarchicalMove != value){ data.hierarchicalMove = value; Save(); } }
		}

		public static bool useExternalInspector{
			get {return data.useExternalInspector;}
			set {if (data.useExternalInspector != value){ data.useExternalInspector = value; Save(); } }
		}

		public static bool showWelcomeWindow{
			get {return data.showWelcomeWindow;}
			set {if (data.showWelcomeWindow != value){ data.showWelcomeWindow = value; Save(); } }
		}

		public static bool logEvents{
			get {return data.logEvents;}
			set {if (data.logEvents != value){ data.logEvents = value; Save(); } }
		}

		public static bool highlightOwnersInHierarchy{
			get {return data.highlightOwnersInHierarchy;}
			set {if (data.highlightOwnersInHierarchy != value){ data.highlightOwnersInHierarchy = value; Save(); } }
		}

		public static bool useBrowser{
			get {return data.useBrowser;}
			set {if (data.useBrowser != value){ data.useBrowser = value; Save(); } }
		}

		public static bool breakpointPauseEditor{
			get {return data.breakpointPauseEditor;}
			set {if (data.breakpointPauseEditor != value){ data.breakpointPauseEditor = value; Save(); } }
		}

		public static float inspectorPanelWidth{
			get {return data.inspectorPanelWidth;}
			set {if (data.inspectorPanelWidth != value){ data.inspectorPanelWidth = UnityEngine.Mathf.Clamp(value, 300, 600); Save(); } }
		}

		public static float blackboardPanelWidth{
			get {return data.blackboardPanelWidth;}
			set {if (data.blackboardPanelWidth != value){ data.blackboardPanelWidth = UnityEngine.Mathf.Clamp(value, 300, 600); Save(); } }
		}

		public static bool consoleLogInfo{
			get {return data.consoleLogInfo;}
			set {if (data.consoleLogInfo != value){ data.consoleLogInfo = value; Save(); } }
		}

		public static bool consoleLogWarning{
			get {return data.consoleLogWarning;}
			set {if (data.consoleLogWarning != value){ data.consoleLogWarning = value; Save(); } }
		}

		public static bool consoleLogError{
			get {return data.consoleLogError;}
			set {if (data.consoleLogError != value){ data.consoleLogError = value; Save(); } }
		}

		public static ConsoleLogOrder consoleLogOrder{
			get {return data.consoleLogOrder;}
			set {if (data.consoleLogOrder != value){ data.consoleLogOrder = value; Save(); } }
		}

		public static UnityEngine.Vector2 minimapSize{
			get
			{
				var result = data.minimapSize;
				result = UnityEngine.Vector2.Max(result, minimapMinSize);
				result = UnityEngine.Vector2.Min(result, minimapMaxSize);
				return result;
			}
			set
			{
				if (data.minimapSize != value){
					data.minimapSize = UnityEngine.Vector2.Max(value, minimapMinSize);
					data.minimapSize = UnityEngine.Vector2.Min(value, minimapMaxSize);
					Save();
				}
			}
		}

		static void Save(){
			EditorPrefs.SetString("NodeCanvas.EditorPreferences", JSONSerializer.Serialize(typeof(SerializedData), data));
		}
	}
}

#endif