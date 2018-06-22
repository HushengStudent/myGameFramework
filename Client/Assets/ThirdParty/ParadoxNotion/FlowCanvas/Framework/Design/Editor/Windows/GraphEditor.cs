#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using CanvasGroup = NodeCanvas.Framework.CanvasGroup;
using Logger = ParadoxNotion.Services.Logger;

namespace NodeCanvas.Editor{

	public partial class GraphEditor : EditorWindow{

		//the root graph that was first opened in the editor
		[System.NonSerialized]
		private Graph _rootGraph;
		private int _rootGraphID;

		//the GrapOwner if any, that was used to open the editor and from which to read the rootGraph
		[System.NonSerialized]
		private GraphOwner _targetOwner;
		private int _targetOwnerID;

		///----------------------------------------------------------------------------------------------

		//the current instance of the opened editor
		public static GraphEditor current;
		//the current graph loaded for editing. Can be a nested graph of the root graph
		public static Graph currentGraph;

		///----------------------------------------------------------------------------------------------
		const float TOOLBAR_HEIGHT = 22;
		const float TOP_MARGIN     = 22;
		const float BOTTOM_MARGIN  = 5;
		const int GRID_SIZE        = 15;
		private static Rect canvasRect; //rect within which the graph is drawn (the window)
		private static Rect viewRect; //the panning rect that is drawn within canvasRect
		private static Rect minimapRect; //rect to show minimap within

		///----------------------------------------------------------------------------------------------

		private static Event e;
		private static bool isMultiSelecting;
		private static Vector2 selectionStartPos;
		private static System.Action OnDoPopup;
		private static bool isResizingMinimap;
		private static bool isDraggingMinimap;
		private static bool willRepaint = true;
		private static bool fullDrawPass = true;

		private static Node[] tempCanvasGroupNodes;
		private static CanvasGroup[] tempCanvasGroupGroups;

		///----------------------------------------------------------------------------------------------

		private static float lastUpdateTime    = -1;
		private static Vector2? smoothPan      = null;
		private static float? smoothZoomFactor = null;
		private static Vector2 _panVelocity    = Vector2.one;
		private static float _zoomVelocity     = 1;

		///----------------------------------------------------------------------------------------------

	    private static bool welcomeShown = false;
		private readonly static Vector2 virtualCenterOffset = new Vector2(-5000, -5000);
		private readonly static Vector2 zoomPoint = new Vector2(5, TOP_MARGIN);

		///----------------------------------------------------------------------------------------------

		//The graph from which we start editing
		private static Graph rootGraph{
			get
			{
				if (current._rootGraph == null){
					current._rootGraph = EditorUtility.InstanceIDToObject(current._rootGraphID) as Graph;
				}
				return current._rootGraph;
			}
			set
			{
				current._rootGraph = value;
				current._rootGraphID = value != null? value.GetInstanceID() : 0;
			}
		}

		//The owner of the root graph if any
		private static GraphOwner targetOwner{
			get
			{
				if (current._targetOwner == null){
					current._targetOwner = EditorUtility.InstanceIDToObject(current._targetOwnerID) as GraphOwner;
				}
				return current._targetOwner;
			}
			set
			{
				current._targetOwner = value;
				current._targetOwnerID = value != null? value.GetInstanceID() : 0;
			}
		}

		//The translation of the graph
		private static Vector2 pan{
			get {return currentGraph != null? Vector2.Min(currentGraph.translation, Vector2.zero) : virtualCenter;}
			set
			{
				if (currentGraph != null){
					var t = currentGraph.translation;
					t = Vector2.Min(value, Vector2.zero);
					if (smoothPan == null){
						t.x = Mathf.Round(t.x); //pixel perfect correction
						t.y = Mathf.Round(t.y); //pixel perfect correction
					}
					currentGraph.translation = t;
				}
			}
		}

		//The zoom factor of the graph
		private static float zoomFactor{
			get {return currentGraph != null? Mathf.Clamp(currentGraph.zoomFactor, 0.25f, 1f) : 1f; }
			set {if (currentGraph != null) currentGraph.zoomFactor = Mathf.Clamp(value, 0.25f, 1f); }
		}

		//The center of the canvas
		private static Vector2 virtualCenter{
			get {return -virtualCenterOffset + viewRect.size/2;}
		}

		//The mouse position in the canvas
		private static Vector2 mousePosInCanvas{
			get {return ViewToCanvas(Event.current.mousePosition); }
		}

        //window width
		private static float screenWidth{ //for retina
        	get {return EditorGUIUtility.currentViewWidth;}
        }

        //window height
		private static float screenHeight{ //for consistency
        	get {return Screen.height;}
        }

		///----------------------------------------------------------------------------------------------

		//...
		void OnEnable(){
			current = this;
	        var canvasIcon = (Texture)Resources.Load("CanvasIcon");
	        titleContent = new GUIContent("Canvas", canvasIcon);

			willRepaint = true;
			fullDrawPass = true;
			wantsMouseMove = true;
			minSize = new Vector2(700, 300);

			#if UNITY_2017_2_OR_NEWER
			EditorApplication.playModeStateChanged -= PlayModeChange;
			EditorApplication.playModeStateChanged += PlayModeChange;
			#else
			EditorApplication.playmodeStateChanged -= PlayModeChange;
			EditorApplication.playmodeStateChanged += PlayModeChange;
			#endif
			Selection.selectionChanged -= OnSelectionChange;
			Selection.selectionChanged += OnSelectionChange;
			Logger.RemoveListener(OnLogMessageReceived);
			Logger.AddListener(OnLogMessageReceived);
		}

		//...
		void OnDisable(){
			current = null;
		    welcomeShown = false;
			GraphEditorUtility.activeElement = null;
			#if UNITY_2017_2_OR_NEWER
			EditorApplication.playModeStateChanged -= PlayModeChange;
			#else
			EditorApplication.playmodeStateChanged -= PlayModeChange;
			#endif
			Selection.selectionChanged -= OnSelectionChange;
			Logger.RemoveListener(OnLogMessageReceived);
		}

		//...
		void PlayModeChange
			(
#if UNITY_2017_2_OR_NEWER
			PlayModeStateChange state
#endif
			){
			GraphEditorUtility.activeElement = null;
			welcomeShown = true;
			willRepaint = true;
			fullDrawPass = true;
		}

		//Listen to Logs and return true if handled
		bool OnLogMessageReceived(Logger.Message msg){
			if (msg.tag == "Editor"){
				ShowNotification(new GUIContent(msg.text));
				return true;
			}
			return false;
		}

		//Whenever the graph we are viewing has changed and after the fact.
		void OnCurrentGraphChanged(){
			UpdateReferencesAndNodeIDs();
			GraphEditorUtility.activeElement = null;
			willRepaint = true;
			fullDrawPass = true;
			smoothPan = null;
			smoothZoomFactor = null;
		}

		//Update the references for editor convenience.
		void UpdateReferencesAndNodeIDs(){
			rootGraph = targetOwner != null? targetOwner.graph : rootGraph;
			if (rootGraph != null){
				if (targetOwner != null){
					rootGraph.agent = targetOwner;
					rootGraph.blackboard = targetOwner.blackboard;
				}
				rootGraph.UpdateNodeIDs(true);
				rootGraph.UpdateReferences();

				//update refs for the currenlty viewing nested graph as well
				var current = GetCurrentGraph(rootGraph);
				if (targetOwner != null){
					current.agent = targetOwner;
					current.blackboard = targetOwner.blackboard;
				}
				current.UpdateNodeIDs(true);
				current.UpdateReferences();
			}
		}

		[UnityEditor.Callbacks.OnOpenAsset(1)]
		public static bool OpenAsset(int instanceID, int line){
			var target = EditorUtility.InstanceIDToObject(instanceID) as Graph;
			if (target != null){
				GraphEditor.OpenWindow(target);
				return true;
			}
			return false;
		}

		///Open the window without any references
		public static GraphEditor OpenWindow(){ return OpenWindow(null, null, null); }
	    ///Opening the window for a graph owner
	    public static GraphEditor OpenWindow(GraphOwner owner){	return OpenWindow(owner.graph, owner, owner.blackboard); }
	    ///For opening the window from gui button in the nodegraph's Inspector.
	    public static GraphEditor OpenWindow(Graph newGraph){ return OpenWindow(newGraph, null, newGraph.blackboard); }
	    ///Open GraphEditor initializing target graph
	    public static GraphEditor OpenWindow(Graph newGraph, GraphOwner owner, IBlackboard blackboard) {
			var window = GetWindow<GraphEditor>();
			SetReferences(newGraph, owner, blackboard);
            if (NCPrefs.showWelcomeWindow && !Application.isPlaying && welcomeShown == false){
                welcomeShown = true;
				var graphType = newGraph != null? newGraph.GetType() : null;
                WelcomeWindow.ShowWindow(graphType);
            }
            return window;
	    }

		///Set GraphEditor inspected references
		public static void SetReferences(GraphOwner owner){ SetReferences(owner.graph, owner, owner.blackboard); }
		///Set GraphEditor inspected references
		public static void SetReferences(Graph newGraph){ SetReferences(newGraph, null, newGraph.blackboard); }
		///Set GraphEditor inspected references
		public static void SetReferences(Graph newGraph, GraphOwner owner, IBlackboard blackboard){
			if (current == null){
				Debug.Log("GraphEditor is not open.");
				return;
			}
			willRepaint = true;
			fullDrawPass = true;
	        rootGraph = newGraph;
	        targetOwner = owner;
		    if (rootGraph != null){
		        rootGraph.agent = owner;
		        rootGraph.blackboard = blackboard;	        	
		        rootGraph.currentChildGraph = null;
		        rootGraph.UpdateNodeIDs(true);
		        rootGraph.UpdateReferences();
		    }
	        GraphEditorUtility.activeElement = null;
			current.Repaint();
		}

		//Change viewing graph based on Graph or GraphOwner
		void OnSelectionChange(){
			
			if (NCPrefs.isLocked){
				return;
			}

			if (Selection.activeObject is GraphOwner){
				SetReferences((GraphOwner)Selection.activeObject);
				return;				
			}

			if (Selection.activeObject is Graph){
				SetReferences((Graph)Selection.activeObject);
				return;
			}

			if (Selection.activeGameObject != null){
				var foundOwner = Selection.activeGameObject.GetComponent<GraphOwner>();
				if (foundOwner != null){
					SetReferences(foundOwner);
				}
			}
		}

		///Editor update
		void Update(){
			var currentTime = Time.realtimeSinceStartup;
			var deltaTime = currentTime - lastUpdateTime;
			lastUpdateTime = currentTime;

			UpdateSmoothPan(deltaTime);
			UpdateSmoothZoom(deltaTime);
			if (smoothPan != null || smoothZoomFactor != null){
				Repaint();
			}
		}

		///Update smooth pan
		void UpdateSmoothPan(float deltaTime){

			if (smoothPan == null){
				return;
			}
			
			var targetPan = (Vector2)smoothPan;
			if ( (targetPan - pan).magnitude < 0.1f ){
				smoothPan = null;
				return;
			}

			targetPan = new Vector2(Mathf.FloorToInt(targetPan.x), Mathf.FloorToInt(targetPan.y));
			pan = Vector2.SmoothDamp(pan, targetPan, ref _panVelocity, 0.05f, Mathf.Infinity, deltaTime);
		}

		///Update smooth pan
		void UpdateSmoothZoom(float deltaTime){
			
			if (smoothZoomFactor == null){
				return;
			}

			var targetZoom = (float)smoothZoomFactor;
			if ( Mathf.Abs(targetZoom - zoomFactor) < 0.00001f ){
				smoothZoomFactor = null;
				return;
			}
				
			zoomFactor = Mathf.SmoothDamp(zoomFactor, targetZoom, ref _zoomVelocity, 0.05f, Mathf.Infinity, deltaTime);
			if (zoomFactor > 0.99999f){ zoomFactor = 1; }
		}

		//GUI space to canvas space
		static Vector2 ViewToCanvas(Vector2 viewPos){
			return (viewPos - pan)/zoomFactor;
		}

		//Canvas space to GUI space
		static Vector2 CanvasToView(Vector2 canvasPos){
			return (canvasPos * zoomFactor) + pan;
		}

		//Show modal quick popup
		static void DoPopup(System.Action Call){
			OnDoPopup = Call;
		}

		//Just so that there is some repainting going on
		void OnInspectorUpdate(){
			if (!willRepaint){
				Repaint();
			}
		}

		//...
		void OnGUI(){

			if (EditorApplication.isCompiling){
				ShowNotification(new GUIContent("...Compiling Please Wait..."));
				willRepaint = true;
				return;			
			}

			//Init
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
			GUI.skin.label.richText = true;
			e = Event.current;

			//get the graph from the GraphOwner if one is set
			if (targetOwner != null){
				rootGraph = targetOwner.graph;
			}

			if (rootGraph == null){
				ShowEmptyGraphGUI();
				return;
			}

			//set the currently viewing graph by getting the current child graph from the root graph recursively
			var curr = GetCurrentGraph(rootGraph);
			if (!ReferenceEquals(curr, currentGraph)){
				currentGraph = curr;
				OnCurrentGraphChanged();
			}

			if (currentGraph == null || ReferenceEquals(currentGraph, null)){
				return;
			}

			//handle undo/redo keyboard commands
			if (e.type == EventType.ValidateCommand && e.commandName == "UndoRedoPerformed"){
                GUIUtility.hotControl = 0;
                GUIUtility.keyboardControl = 0;
                GraphEditorUtility.activeElement = null;
                willRepaint = true;
                fullDrawPass = true;
                UpdateReferencesAndNodeIDs();
                currentGraph.Validate();
                e.Use();
				return;
			}

			if (e.type == EventType.MouseDown){
				RemoveNotification();
			}

			if (mouseOverWindow == current && (e.isMouse || e.isKey) ){
				willRepaint = true;
			}

			///should we set dirty? Put in practise at the end
			var willDirty = false;
			if (
				(e.rawType == EventType.MouseUp && e.button != 2) ||
				(e.type == EventType.DragPerform) ||
				(e.type == EventType.KeyUp && (e.keyCode == KeyCode.Return || GUIUtility.keyboardControl == 0 ) )
				)
			{
				willDirty = true;
			}

			//initialize rects
			canvasRect = Rect.MinMaxRect(5, TOP_MARGIN, position.width -5, position.height - BOTTOM_MARGIN);
			minimapRect = Rect.MinMaxRect(canvasRect.xMax - NCPrefs.minimapSize.x, canvasRect.yMax - NCPrefs.minimapSize.y, canvasRect.xMax - 2, canvasRect.yMax - 2);
			var originalCanvasRect = canvasRect;

			//canvas background
			GUI.Box(canvasRect, string.Empty, CanvasStyles.canvasBG);
			//background grid
			DrawGrid(canvasRect, pan, zoomFactor);
			//handle minimap
			HandleMinimapEvents(minimapRect);
			//PRE nodes events
			HandlePreNodesGraphEvents(currentGraph, mousePosInCanvas);

			var oldMatrix = default(Matrix4x4);
			if (zoomFactor != 1){
				canvasRect = StartZoomArea(canvasRect, zoomFactor, out oldMatrix);
			}

			//main group
			GUI.BeginGroup(canvasRect);
			{
				//pan the view rect
				var totalCanvas = canvasRect;
				totalCanvas.x = 0;
				totalCanvas.y = 0;
				totalCanvas.x += pan.x/zoomFactor;
				totalCanvas.y += pan.y/zoomFactor;
				totalCanvas.width -= pan.x/zoomFactor;
				totalCanvas.height -= pan.y/zoomFactor;

				//begin panning group
				GUI.BeginGroup(totalCanvas);
				{
					//inverse pan the view rect
					viewRect = totalCanvas;
					viewRect.x = 0;
					viewRect.y = 0;
					viewRect.x -= pan.x/zoomFactor;
					viewRect.y -= pan.y/zoomFactor;
					viewRect.width += pan.x/zoomFactor;
					viewRect.height += pan.y/zoomFactor;

					DoCanvasGroups();

					BeginWindows();
					ShowNodesGUI(currentGraph, viewRect, fullDrawPass, mousePosInCanvas, zoomFactor);
					EndWindows();

					DoCanvasRectSelection(viewRect);
				}
			
				GUI.EndGroup();
			}

			GUI.EndGroup();

			if (zoomFactor != 1 && oldMatrix != default(Matrix4x4)){
				EndZoomArea(oldMatrix);
			}

			//minimap
			DrawMinimap(minimapRect);

			//Breadcrumb navigation
			GUILayout.BeginArea(new Rect(20, TOP_MARGIN + 5, screenWidth, screenHeight));
			DoBreadCrumbNavigation(rootGraph);
			GUILayout.EndArea();

			//POST nodes events
			HandlePostNodesGraphEvents(currentGraph, mousePosInCanvas);
			//Graph controls (after windows so that panels (inspector, blackboard) show on top)
			ShowToolbar(currentGraph);
			ShowPanels(currentGraph, mousePosInCanvas);
			AcceptDrops(currentGraph, mousePosInCanvas);
			//


			//dirty?
			if (willDirty){
				willDirty = false;
				willRepaint = true;
				currentGraph.Serialize();
				EditorUtility.SetDirty(currentGraph);
			}

			//repaint?
			if (willRepaint || e.type == EventType.MouseMove || rootGraph.isRunning ){
				Repaint();
			}

			if (e.type == EventType.Repaint){
				fullDrawPass = false;
				willRepaint = false;
			}

			//playmode indicator
			if (Application.isPlaying){
				var r = new Rect(0, 0, 120, 10);
				r.center = new Vector2(screenWidth/2, screenHeight - BOTTOM_MARGIN - 50);
				GUI.color = Color.green;
				GUI.Box(r, "PlayMode Active", CanvasStyles.windowHighlight);
			}

			//hack for quick popups
			if (OnDoPopup != null){
				var temp = OnDoPopup;
				OnDoPopup = null;
				QuickPopup.Show(temp);
			}

			//PostGUI
			GraphEditorUtility.InvokePostGUI();


			//closure
			GUI.Box(originalCanvasRect, string.Empty, CanvasStyles.canvasBorders);
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
		}

		///----------------------------------------------------------------------------------------------

		//Recursively get the currenlty showing nested graph starting from the root
		static Graph GetCurrentGraph(Graph root){
			if (root.currentChildGraph == null){
				return root;
			}
			return GetCurrentGraph(root.currentChildGraph);
		}

		//Starts a zoom area, returns the scaled container rect
		static Rect StartZoomArea(Rect container, float zoomFactor, out Matrix4x4 oldMatrix){
			GUI.EndGroup();
			container.height += TOOLBAR_HEIGHT;
			container.width *= 1/zoomFactor;
			container.height *= 1/zoomFactor;
			oldMatrix = GUI.matrix;
			var matrix1 = Matrix4x4.TRS( zoomPoint, Quaternion.identity, Vector3.one );
			var matrix2 = Matrix4x4.Scale(new Vector3(zoomFactor, zoomFactor, 1f));
			GUI.matrix = matrix1 * matrix2 * matrix1.inverse * GUI.matrix;
			return container;
		}

		//Ends the zoom area
		static void EndZoomArea(Matrix4x4 oldMatrix){
			GUI.matrix = oldMatrix;
			var zoomRecoveryRect = new Rect(0, TOOLBAR_HEIGHT, screenWidth, screenHeight);
			#if !UNITY_2017_2_OR_NEWER
			zoomRecoveryRect.y -= 3; //i honestly dont know what that 3 is, but fixes a 3 px dislocation. Unity seems to have fixed it in 2017.
			#endif
			GUI.BeginGroup(zoomRecoveryRect); //Recover rect
		}

		//This is called while within Begin/End windows
		static void ShowNodesGUI(Graph graph, Rect drawCanvas, bool fullDrawPass, Vector2 canvasMousePos, float zoomFactor){
			for (var i = 0; i < graph.allNodes.Count; i++){
				//ensure IDs are updated
				if (graph.allNodes[i].ID != i){
					graph.UpdateNodeIDs(true);
					break;
				}

				Node.ShowNodeGUI(graph.allNodes[i], drawCanvas, fullDrawPass, canvasMousePos, zoomFactor);
			}

			if (graph.primeNode != null){
				GUI.Box(new Rect(graph.primeNode.rect.x, graph.primeNode.rect.y - 20, graph.primeNode.rect.width, 20), "<b>START</b>", CanvasStyles.box);
			}
		}
	
		///Translate the graph to focus selection
		public static void FocusSelection(){
			if (GraphEditorUtility.activeElements != null && GraphEditorUtility.activeElements.Count > 0){
				FocusPosition(GetNodeBounds(GraphEditorUtility.activeElements.Cast<Node>().ToList()).center);
				return;
			}
			if (GraphEditorUtility.activeNode != null){
				FocusNode(GraphEditorUtility.activeNode);
				return;
			}
			if (GraphEditorUtility.activeConnection != null){
				FocusConnection(GraphEditorUtility.activeConnection);
				return;
			}
			if (currentGraph.allNodes.Count > 0){
				FocusPosition(GetNodeBounds(currentGraph.allNodes).center);
				return;
			}
			FocusPosition(virtualCenter);
		}

		///Translate the graph to the center of the target node
		public static void FocusNode(Node node){
			if (currentGraph == node.graph){
				FocusPosition(node.rect.center);
			}
		}

		///Translate the graph to the center of the target connection
		public static void FocusConnection(Connection connection){
			if (currentGraph == connection.sourceNode.graph){
				var bound = RectUtils.GetBoundRect(connection.sourceNode.rect, connection.targetNode.rect);
				FocusPosition(bound.center);
			}
		}

		///Translate the graph to to center of the target pos
		static void FocusPosition(Vector2 targetPos, bool smooth = true){
			if (smooth){
				smoothPan = -targetPos;
				smoothPan += new Vector2( viewRect.width/2, viewRect.height/2);
				smoothPan *= zoomFactor;
			} else {
				pan = -targetPos;
				pan += new Vector2( viewRect.width/2, viewRect.height/2);
				pan *= zoomFactor;
				smoothPan = null;
				smoothZoomFactor = null;				
			}
		}

		///Zoom with center position
		static void ZoomAt(Vector2 center, float delta){
			var pinPoint = (center - pan)/zoomFactor;
			var newZ = zoomFactor;
			newZ += delta;
			newZ = Mathf.Clamp(newZ, 0.25f, 1f);
			smoothZoomFactor = newZ;
			var a = (pinPoint * newZ) + pan;
			var b = center;
			var diff = b - a;
			smoothPan = pan + diff;
		}

 		//Handles Drag&Drop operations
		static void AcceptDrops(Graph graph, Vector2 canvasMousePos){
			if (GraphEditorUtility.allowClick){
				if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length == 1){
					if (e.type == EventType.DragUpdated){
						DragAndDrop.visualMode = DragAndDropVisualMode.Link;
					}
					if (e.type == EventType.DragPerform){
						var value = DragAndDrop.objectReferences[0];
						DragAndDrop.AcceptDrag();
						graph.CallbackOnDropAccepted(value, canvasMousePos);
					}
				}
			}
		}
	
		///Gets the bound rect for the nodes
		static Rect GetNodeBounds(List<Node> nodes){
			if (nodes == null || nodes.Count == 0){
				return default(Rect);
			}

			var arr = new Rect[nodes.Count];
			for (var i = 0; i < nodes.Count; i++){
				arr[i] = nodes[i].rect;
			}
			return RectUtils.GetBoundRect(arr);
		}


		//Do graphical multi selection box for nodes
		static void DoCanvasRectSelection(Rect container){
			
			if (GraphEditorUtility.allowClick && e.type == EventType.MouseDown && e.button == 0 && !e.alt && !e.shift && canvasRect.Contains(CanvasToView(e.mousePosition)) ){
				GraphEditorUtility.activeElement = null;
				selectionStartPos = e.mousePosition;
				isMultiSelecting = true;
				e.Use();
			}

			if (isMultiSelecting && e.rawType == EventType.MouseUp){
				var rect = RectUtils.GetBoundRect(selectionStartPos, e.mousePosition);
				var overlapedNodes = currentGraph.allNodes.Where(n => rect.Overlaps(n.rect) && !n.isHidden).ToList();
				isMultiSelecting = false;
				if (e.control && rect.width > 50 && rect.height > 50){
					Undo.RegisterCompleteObjectUndo(currentGraph, "Create Group");
					if (currentGraph.canvasGroups == null){
						currentGraph.canvasGroups = new List<CanvasGroup>();
					}
					currentGraph.canvasGroups.Add( new CanvasGroup(rect, "New Canvas Group") );
				} else {
					if (overlapedNodes.Count > 0){
						GraphEditorUtility.activeElements = overlapedNodes.Cast<object>().ToList();
						e.Use();
					}
				}
			}

			if (isMultiSelecting){
				var rect = RectUtils.GetBoundRect(selectionStartPos, e.mousePosition);
				if (rect.width > 5 && rect.height > 5){
					GUI.color = new Color(0.5f,0.5f,1,0.3f);
					GUI.Box(rect, string.Empty, CanvasStyles.box);
					foreach (var node in currentGraph.allNodes){
						if (rect.Overlaps(node.rect) && !node.isHidden){
							var highlightRect = node.rect;
							GUI.Box(highlightRect, string.Empty, CanvasStyles.windowHighlight);
						}
					}
					if (rect.width > 50 && rect.height > 50){
						GUI.color = new Color(1,1,1, e.control? 0.6f : 0.15f);
						GUI.Label(new Rect( e.mousePosition.x + 16, e.mousePosition.y, 120, 22 ), "<i>+ control for group</i>", CanvasStyles.label);
					}
				}
			}

			GUI.color = Color.white;
		}



		//Draw a simple grid
		static void DrawGrid(Rect container, Vector2 offset, float zoomFactor){
			
			if (Event.current.type != EventType.Repaint){
				return;
			}

			// GL.Begin(GL.LINES);
			// GL.Color( new Color(0,0,0, 0.1f) );
			Handles.color = new Color(0,0,0, 0.15f);

			var drawGridSize = zoomFactor > 0.5f? GRID_SIZE : GRID_SIZE * 5;
			var step = drawGridSize * zoomFactor;
			
			var xDiff = offset.x % step;
			var xStart = container.xMin + xDiff;
			var xEnd = container.xMax;
			for (var i = xStart; i < xEnd; i += step){
				Handles.DrawLine( new Vector3(i, container.yMin, 0), new Vector3(i, container.yMax, 0) );
				// GL.Vertex( new Vector3(i, container.yMin, 0) );
				// GL.Vertex( new Vector3(i, container.yMax, 0) );
			}

			var yDiff = offset.y % step;
			var yStart = container.yMin + yDiff;
			var yEnd = container.yMax;
			for (var i = yStart; i < yEnd; i += step){
				Handles.DrawLine( new Vector3(0, i, 0), new Vector3( container.xMax, i, 0) );
				// GL.Vertex( new Vector3(0, i, 0) );
				// GL.Vertex( new Vector3( container.xMax, i, 0) );
			}

			// GL.End();
		}


		//This is the hierarchy shown at top left. Recusrsively show the nested path
		static void DoBreadCrumbNavigation(Graph root){

			if (root == null){
				return;
			}

			//if something selected the inspector panel shows on top of the breadcrub. If external inspector active it doesnt matter, so draw anyway.
			if (GraphEditorUtility.activeElement != null && !NCPrefs.useExternalInspector){
				return;
			}

			var boundInfo = "Bound";
			var prefabType = targetOwner != null? PrefabUtility.GetPrefabType(targetOwner) : PrefabType.None;
			if (prefabType == PrefabType.Prefab){ boundInfo += " Prefab Asset"; }
			if (prefabType == PrefabType.PrefabInstance){ boundInfo += " Prefab Instance"; }
			var assetInfo = EditorUtility.IsPersistent(root)? "Asset Reference" : "Instance";
			var graphInfo = string.Format("<color=#ff4d4d>({0})</color>", targetOwner != null && targetOwner.graph == root && targetOwner.graphIsBound? boundInfo : assetInfo );

			GUI.color = new Color(1f,1f,1f,0.5f);
			
			GUILayout.BeginVertical();
			if (root.currentChildGraph == null){

				if (root.agent == null && root.blackboard == null){
					GUILayout.Label(string.Format("<b><size=22>{0} {1}</size></b>", root.name, graphInfo), CanvasStyles.label);	
				} else {
					var agentInfo = root.agent != null? root.agent.gameObject.name : "No Agent";
					var bbInfo = root.blackboard != null? root.blackboard.name : "No Blackboard";
					GUILayout.Label(string.Format("<b><size=22>{0} {1}</size></b>\n<size=10>{2} | {3}</size>", root.name, graphInfo, agentInfo, bbInfo), CanvasStyles.label);
				}

			} else {

				GUILayout.BeginHorizontal();
				GUILayout.Label("⤴ " + root.name, CanvasStyles.button);
				if (e.type == EventType.MouseUp && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)){
					root.currentChildGraph = null;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				DoBreadCrumbNavigation(root.currentChildGraph);
			}

			GUILayout.EndVertical();
			GUI.color = Color.white;
		}

		
		///Canvas groups
		static void DoCanvasGroups(){

			if (currentGraph.canvasGroups == null){
				return;
			}

			for (var i = 0; i < currentGraph.canvasGroups.Count; i++){
				var group = currentGraph.canvasGroups[i];
				var handleRect = new Rect( group.rect.x, group.rect.y, group.rect.width, 25);
				var scaleRect = new Rect( group.rect.xMax - 20, group.rect.yMax -20, 20, 20);
				var style = CanvasStyles.editorPanel;
				
				GUI.color = EditorGUIUtility.isProSkin? new Color(1,1,1,0.4f) : new Color(0.5f,0.5f,0.5f,0.3f);
				GUI.Box(group.rect, string.Empty, style);

				if (group.color != default(Color)){
					GUI.color = group.color;
					GUI.DrawTexture(group.rect, EditorGUIUtility.whiteTexture);
				}

				GUI.color = Color.white;
				GUI.Box(new Rect(scaleRect.x+10, scaleRect.y+10, 6,6), string.Empty, CanvasStyles.scaleArrow );
				
				var size = 14 / zoomFactor;
				var name = string.Format("<size={0}>{1}</size>", size, group.name);
				GUI.Label(handleRect, name, style);

				EditorGUIUtility.AddCursorRect(handleRect, group.isRenaming? MouseCursor.Text : MouseCursor.Link);
				EditorGUIUtility.AddCursorRect(scaleRect, MouseCursor.ResizeUpLeft);

				if (group.isRenaming){
					GUI.SetNextControlName("GroupRename");
					group.name = EditorGUI.TextField(handleRect, group.name, style);
					GUI.FocusControl("GroupRename");
					if (e.keyCode == KeyCode.Return || (e.type == EventType.MouseDown && !handleRect.Contains(e.mousePosition)) ){
						group.isRenaming = false;
		                GUIUtility.hotControl = 0;
		                GUIUtility.keyboardControl = 0;
					}
				}

				if (e.type == EventType.MouseDown && GraphEditorUtility.allowClick){

					if (handleRect.Contains(e.mousePosition)){

						Undo.RegisterCompleteObjectUndo(currentGraph, "Move Canvas Group");

						//calc group nodes
						tempCanvasGroupNodes = currentGraph.allNodes.Where(n => group.rect.Encapsulates(n.rect) ).ToArray();
						tempCanvasGroupGroups = currentGraph.canvasGroups.Where(c => group.rect.Encapsulates(c.rect) ).ToArray();

						if (e.button == 1){
							var menu = new GenericMenu();
							menu.AddItem(new GUIContent("Rename"), false, ()=> { group.isRenaming = true; } );
							menu.AddItem(new GUIContent("Edit Color"), false, ()=> { DoPopup( ()=>
								{
									group.color = EditorGUILayout.ColorField(group.color);
								} ); } );
							menu.AddItem(new GUIContent("Select Nodes"), false, ()=> { GraphEditorUtility.activeElements = tempCanvasGroupNodes.Cast<object>().ToList(); } );
							menu.AddItem(new GUIContent("Delete Group"), false, ()=>
							{
								currentGraph.canvasGroups.Remove(group);
								if (EditorUtility.DisplayDialog("Delete Group", "Delete contained nodes as well?", "Yes", "No")){
									foreach(var node in tempCanvasGroupNodes){
										currentGraph.RemoveNode(node);
									}
								}
							});
							GraphEditorUtility.PostGUI += ()=> { menu.ShowAsContext(); };
						} else if (e.button == 0){
							group.isDragging = true;
						}

						e.Use();
					}

					if (e.button == 0 && scaleRect.Contains(e.mousePosition)){
						Undo.RegisterCompleteObjectUndo(currentGraph, "Scale Canvas Group");
						group.isRescaling = true;
						e.Use();
					}
				}

				if (e.rawType == EventType.MouseUp){
					group.isDragging = false;
					group.isRescaling = false;
				}

				if (e.type == EventType.MouseDrag){

					if (group.isDragging){

						group.rect.x += e.delta.x;
						group.rect.y += e.delta.y;						

						if (!e.shift){
							for (var j = 0; j < tempCanvasGroupNodes.Length; j++){
								tempCanvasGroupNodes[j].position += e.delta;
							}

							for (var j = 0; j < tempCanvasGroupGroups.Length; j++){
								tempCanvasGroupGroups[j].rect.x += e.delta.x;
								tempCanvasGroupGroups[j].rect.y += e.delta.y;
							}
						}
					}

					if (group.isRescaling){
						group.rect.xMax = Mathf.Max(e.mousePosition.x + 5, group.rect.x + 100);
						group.rect.yMax = Mathf.Max(e.mousePosition.y + 5, group.rect.y + 100);
					}
				}
			}
		}


		//Snap all nodes either to grid if option enabled or to pixel perfect integer position
		static void SnapNodesToGridAndPixel(Graph graph){
			for (var i = 0; i < graph.allNodes.Count; i++){
				var node = graph.allNodes[i];
				var pos = node.position;
				if (NCPrefs.doSnap){
					pos.x = Mathf.Round(pos.x / GRID_SIZE) * GRID_SIZE;
					pos.y = Mathf.Round(pos.y / GRID_SIZE) * GRID_SIZE;
				} else {
					pos.x = (int)pos.x;
					pos.y = (int)pos.y;
				}
				node.position = pos;
			}
		}


		//before nodes for handling events
		static void HandleMinimapEvents(Rect container){
			if (!GraphEditorUtility.allowClick){ return; }
			var resizeRect = new Rect(container.x, container.y, 6, 6);
			EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeUpLeft);
			if (e.type == EventType.MouseDown && e.button == 0 && resizeRect.Contains(e.mousePosition)){
				isResizingMinimap = true;
				e.Use();
			}
			if (e.rawType == EventType.MouseUp){
				isResizingMinimap = false;
			}
			if (isResizingMinimap && e.type == EventType.MouseDrag){
				NCPrefs.minimapSize -= e.delta;
				e.Use();
			}

			EditorGUIUtility.AddCursorRect(container, MouseCursor.MoveArrow);
			if (e.type == EventType.MouseDown && e.button == 0 && container.Contains(e.mousePosition)){
				var finalBound = ResolveMinimapBoundRect(currentGraph, viewRect);
				var norm = Rect.PointToNormalized(container, e.mousePosition);
				var pos = Rect.NormalizedToPoint(finalBound, norm);
				FocusPosition(pos);
				isDraggingMinimap = true;
				e.Use();
			}
			if (e.rawType == EventType.MouseUp){
				isDraggingMinimap = false;
			}
			if (isDraggingMinimap && e.type == EventType.MouseDrag){
				var finalBound = ResolveMinimapBoundRect(currentGraph, viewRect);
				var norm = Rect.PointToNormalized(container, e.mousePosition);
				var pos = Rect.NormalizedToPoint(finalBound, norm);
				FocusPosition(pos);
				e.Use();
			}
		}

		///after nodes, a cool minimap
		static void DrawMinimap(Rect container){
			GUI.color = new Color(0.5f,0.5f,0.5f,0.85f);
			GUI.Box(container, string.Empty, CanvasStyles.windowShadow);
			GUI.Box(container, currentGraph.allNodes.Count > 0? string.Empty : "Minimap", CanvasStyles.box);
			var finalBound = ResolveMinimapBoundRect(currentGraph, viewRect);
			var lensRect = viewRect.TransformSpace(finalBound, container);
			GUI.color = new Color(1,1,1,0.8f);
			GUI.Box(lensRect, string.Empty, CanvasStyles.box);
			GUI.color = Color.white;
			finalBound = finalBound.ExpandBy(25);
			if (currentGraph.canvasGroups != null){
				for (var i = 0; i < currentGraph.canvasGroups.Count; i++){
					var group = currentGraph.canvasGroups[i];
					var blipRect = group.rect.TransformSpace(finalBound, container);
					var blipHeaderRect = Rect.MinMaxRect(blipRect.xMin, blipRect.yMin, blipRect.xMax, blipRect.yMin + 2);
					var color = group.color != default(Color)? group.color : Color.gray;
					color.a = 0.5f;
					GUI.color = color;
					GUI.DrawTexture(blipRect, Texture2D.whiteTexture);
					GUI.DrawTexture(blipHeaderRect, Texture2D.whiteTexture);
					GUI.color = Color.white;					
				}
			}
			if (currentGraph.allNodes != null){
				for (var i = 0; i < currentGraph.allNodes.Count; i++){
					var node = currentGraph.allNodes[i];
					if (node.isHidden){ continue; }
					var blipRect = node.rect.TransformSpace(finalBound, container);
					var color = node.nodeColor != default(Color)? node.nodeColor : Color.grey;
					GUI.color = color;
					GUI.DrawTexture(blipRect, Texture2D.whiteTexture);
					GUI.color = Color.white;
				}
			}

			var resizeRect = new Rect(container.x, container.y, 6, 6);
			GUI.color = Color.white;
			GUI.Box(resizeRect, string.Empty, CanvasStyles.scaleArrowTL);
		}

		//resolves the bounds used in the minimap
		static Rect ResolveMinimapBoundRect(Graph graph, Rect container){
			var arr1 = new Rect[graph.allNodes.Count];
			for (var i = 0; i < graph.allNodes.Count; i++){
				arr1[i] = graph.allNodes[i].rect;
			}

			var nBounds = RectUtils.GetBoundRect(arr1);
			var finalBound = nBounds;

			if (graph.canvasGroups != null && graph.canvasGroups.Count > 0){
				var arr2 = new Rect[graph.canvasGroups.Count];
				for (var i = 0; i < graph.canvasGroups.Count; i++){
					arr2[i] = graph.canvasGroups[i].rect;
				}
				var gBounds = RectUtils.GetBoundRect(arr2);
				finalBound = RectUtils.GetBoundRect(nBounds, gBounds);
			}

			finalBound = RectUtils.GetBoundRect(finalBound, container);
			return finalBound;
		}

		//an idea but it's taking up space i dont like
		static void ShowConsoleLog(){
			var rect = Rect.MinMaxRect(canvasRect.xMin, canvasRect.yMax + 5, canvasRect.xMax, screenHeight - TOOLBAR_HEIGHT);
			var msg = GraphConsole.GetFirstMessageForGraph(currentGraph);
			if (msg.IsValid()){
				rect.xMin += 2;
				GUI.Label(rect, GraphConsole.GetFormatedGUIContentForMessage(msg), CanvasStyles.label);
				EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
				if (GUI.Button(rect, string.Empty, GUIStyle.none)){
					GraphConsole.ShowWindow();
				}
			}
		}

		//this is shown when root graph is null
		static void ShowEmptyGraphGUI(){
			if (targetOwner != null){
				var text = string.Format("The selected {0} does not have a {1} assigned.\n Please create or assign a new one in it's inspector.", targetOwner.GetType().Name, targetOwner.graphType.Name);
				current.ShowNotification(new GUIContent(text));
				return;
			}
			current.ShowNotification(new GUIContent("Please select a GraphOwner GameObject or a Graph Asset."));			
		}

	}
}

#endif