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

namespace NodeCanvas.Editor
{

    public partial class GraphEditor : EditorWindow
    {

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
        const float TOP_MARGIN = 22;
        const float BOTTOM_MARGIN = 5;
        const int GRID_SIZE = 15;
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

        private static float lastUpdateTime = -1;
        private static Vector2? smoothPan = null;
        private static float? smoothZoomFactor = null;
        private static Vector2 _panVelocity = Vector2.one;
        private static float _zoomVelocity = 1;
        private static float pingValue;
        private static Rect pingRect;

        ///----------------------------------------------------------------------------------------------

        private static bool welcomeShown = false;

        ///----------------------------------------------------------------------------------------------

        public static event System.Action<Graph> onCurrentGraphChanged;

        //The graph from which we start editing
        public static Graph rootGraph {
            get
            {
                if ( current._rootGraph == null ) {
                    current._rootGraph = EditorUtility.InstanceIDToObject(current._rootGraphID) as Graph;
                }
                return current._rootGraph;
            }
            private set
            {
                current._rootGraph = value;
                current._rootGraphID = value != null ? value.GetInstanceID() : 0;
            }
        }

        //The owner of the root graph if any
        public static GraphOwner targetOwner {
            get
            {
                if ( current == null ) { //this fix the maximize/minimize window
                    current = OpenWindow();
                }

                if ( current._targetOwner == null ) {
                    current._targetOwner = EditorUtility.InstanceIDToObject(current._targetOwnerID) as GraphOwner;
                }
                return current._targetOwner;
            }
            private set
            {
                current._targetOwner = value;
                current._targetOwnerID = value != null ? value.GetInstanceID() : 0;
            }
        }

        //The translation of the graph
        private static Vector2 pan {
            get { return currentGraph != null ? currentGraph.translation : viewCanvasCenter; }
            set
            {
                if ( currentGraph != null ) {
                    var t = value;
                    if ( smoothPan == null ) {
                        t.x = Mathf.Round(t.x); //pixel perfect correction
                        t.y = Mathf.Round(t.y); //pixel perfect correction
                    }
                    currentGraph.translation = t;
                }
            }
        }

        //The zoom factor of the graph
        private static float zoomFactor {
            get { return currentGraph != null ? Mathf.Clamp(currentGraph.zoomFactor, 0.25f, 1f) : 1f; }
            set { if ( currentGraph != null ) currentGraph.zoomFactor = Mathf.Clamp(value, 0.25f, 1f); }
        }

        //The center of the canvas
        private static Vector2 viewCanvasCenter {
            get { return viewRect.size / 2; }
        }

        //The mouse position in the canvas
        private static Vector2 mousePosInCanvas {
            get { return ViewToCanvas(Event.current.mousePosition); }
        }

        //window width. Handles retina
        private static float screenWidth {
            get { return Screen.width / EditorGUIUtility.pixelsPerPoint; }
        }

        //window height. Handles retina
        private static float screenHeight {
            get { return Screen.height / EditorGUIUtility.pixelsPerPoint; }
        }

        ///----------------------------------------------------------------------------------------------

        //...
        void OnEnable() {
            current = this;
            titleContent = new GUIContent("Canvas", StyleSheet.canvasIcon);

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
        void OnDisable() {
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
            ) {
            GraphEditorUtility.activeElement = null;
            welcomeShown = true;
            willRepaint = true;
            fullDrawPass = true;
        }

        //Listen to Logs and return true if handled
        bool OnLogMessageReceived(Logger.Message msg) {
            if ( msg.tag == "Editor" ) {
                if ( !string.IsNullOrEmpty(msg.text) ) {
                    ShowNotification(new GUIContent(msg.text));
                }
                return true;
            }
            return false;
        }

        //Whenever the graph we are viewing has changed and after the fact.
        void OnCurrentGraphChanged() {
            UpdateReferencesAndNodeIDs();
            GraphEditorUtility.activeElement = null;
            willRepaint = true;
            fullDrawPass = true;
            smoothPan = null;
            smoothZoomFactor = null;
            if ( onCurrentGraphChanged != null ) {
                onCurrentGraphChanged(currentGraph);
            }
        }

        //Update the references for editor convenience.
        void UpdateReferencesAndNodeIDs() {
            rootGraph = targetOwner != null ? targetOwner.graph : rootGraph;
            if ( rootGraph != null ) {
                rootGraph.UpdateNodeIDs(true);
                rootGraph.UpdateReferencesFromOwner(targetOwner);

                //update refs for the currenlty viewing nested graph as well
                var deepGraph = GetCurrentGraph(rootGraph);
                deepGraph.UpdateNodeIDs(true);
                deepGraph.UpdateReferencesFromOwner(targetOwner);
            }
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OpenAsset(int instanceID, int line) {
            var target = EditorUtility.InstanceIDToObject(instanceID) as Graph;
            if ( target != null ) {
                GraphEditor.OpenWindow(target);
                return true;
            }
            return false;
        }

        ///Open the window without any references
        public static GraphEditor OpenWindow() { return OpenWindow(null, null, null); }
        ///Opening the window for a graph owner
        public static GraphEditor OpenWindow(GraphOwner owner) { return OpenWindow(owner.graph, owner, owner.blackboard); }
        ///For opening the window from gui button in the nodegraph's Inspector.
        public static GraphEditor OpenWindow(Graph newGraph) { return OpenWindow(newGraph, null, newGraph.blackboard); }
        ///Open GraphEditor initializing target graph
        public static GraphEditor OpenWindow(Graph newGraph, GraphOwner owner, IBlackboard blackboard) {
            var window = GetWindow<GraphEditor>();
            SetReferences(newGraph, owner, blackboard);
            if ( Prefs.showWelcomeWindow && !Application.isPlaying && welcomeShown == false ) {
                welcomeShown = true;
                var graphType = newGraph != null ? newGraph.GetType() : null;
                WelcomeWindow.ShowWindow(graphType);
            }
            return window;
        }

        ///Set GraphEditor inspected references
        public static void SetReferences(GraphOwner owner) { SetReferences(owner.graph, owner, owner.blackboard); }
        ///Set GraphEditor inspected references
        public static void SetReferences(Graph newGraph) { SetReferences(newGraph, null, newGraph.blackboard); }
        ///Set GraphEditor inspected references
        public static void SetReferences(Graph newGraph, GraphOwner owner, IBlackboard blackboard) {
            if ( current == null ) {
                Debug.Log("GraphEditor is not open.");
                return;
            }
            willRepaint = true;
            fullDrawPass = true;
            rootGraph = newGraph;
            targetOwner = owner;
            if ( rootGraph != null ) {
                rootGraph.currentChildGraph = null;
                rootGraph.UpdateNodeIDs(true);
                rootGraph.UpdateReferences(owner, blackboard);
            }
            GraphEditorUtility.activeElement = null;
            current.Repaint();
        }

        //Change viewing graph based on Graph or GraphOwner
        void OnSelectionChange() {

            if ( Prefs.isEditorLocked ) {
                return;
            }

            if ( Selection.activeObject is GraphOwner ) {
                SetReferences((GraphOwner)Selection.activeObject);
                return;
            }

            if ( Selection.activeObject is Graph ) {
                SetReferences((Graph)Selection.activeObject);
                return;
            }

            if ( Selection.activeGameObject != null ) {
                var foundOwner = Selection.activeGameObject.GetComponent<GraphOwner>();
                if ( foundOwner != null ) {
                    SetReferences(foundOwner);
                }
            }
        }

        ///Editor update
        void Update() {
            var currentTime = Time.realtimeSinceStartup;
            var deltaTime = currentTime - lastUpdateTime;
            lastUpdateTime = currentTime;

            var needsRepaint = false;
            needsRepaint |= UpdateSmoothPan(deltaTime);
            needsRepaint |= UpdateSmoothZoom(deltaTime);
            needsRepaint |= UpdatePing(deltaTime);
            if ( needsRepaint ) {
                Repaint();
            }
        }

        ///Update smooth pan
        bool UpdateSmoothPan(float deltaTime) {

            if ( smoothPan == null ) {
                return false;
            }

            var targetPan = (Vector2)smoothPan;
            if ( ( targetPan - pan ).magnitude < 0.1f ) {
                smoothPan = null;
                return false;
            }

            targetPan = new Vector2(Mathf.FloorToInt(targetPan.x), Mathf.FloorToInt(targetPan.y));
            pan = Vector2.SmoothDamp(pan, targetPan, ref _panVelocity, 0.08f, Mathf.Infinity, deltaTime);
            return true;
        }

        ///Update smooth pan
        bool UpdateSmoothZoom(float deltaTime) {

            if ( smoothZoomFactor == null ) {
                return false;
            }

            var targetZoom = (float)smoothZoomFactor;
            if ( Mathf.Abs(targetZoom - zoomFactor) < 0.00001f ) {
                smoothZoomFactor = null;
                return false;
            }

            zoomFactor = Mathf.SmoothDamp(zoomFactor, targetZoom, ref _zoomVelocity, 0.08f, Mathf.Infinity, deltaTime);
            if ( zoomFactor > 0.99999f ) { zoomFactor = 1; }
            return true;
        }

        ///Update ping value
        bool UpdatePing(float deltaTime) {
            if ( pingValue > 0 ) {
                pingValue -= deltaTime;
                return true;
            }
            return false;
        }

        ///----------------------------------------------------------------------------------------------

        //GUI space to canvas space
        static Vector2 ViewToCanvas(Vector2 viewPos) {
            return ( viewPos - pan ) / zoomFactor;
        }

        //Canvas space to GUI space
        static Vector2 CanvasToView(Vector2 canvasPos) {
            return ( canvasPos * zoomFactor ) + pan;
        }

        //Show modal quick popup
        static void DoPopup(System.Action Call) {
            OnDoPopup = Call;
        }

        //Just so that there is some repainting going on
        void OnInspectorUpdate() {
            if ( !willRepaint ) {
                Repaint();
            }
        }

        //...
        void OnGUI() {

            if ( EditorApplication.isCompiling ) {
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
            if ( targetOwner != null ) {
                rootGraph = targetOwner.graph;
            }

            if ( rootGraph == null ) {
                ShowEmptyGraphGUI();
                return;
            }

            //set the currently viewing graph by getting the current child graph from the root graph recursively
            var curr = GetCurrentGraph(rootGraph);
            if ( !ReferenceEquals(curr, currentGraph) ) {
                currentGraph = curr;
                OnCurrentGraphChanged();
            }

            if ( currentGraph == null || ReferenceEquals(currentGraph, null) ) {
                return;
            }

            //handle undo/redo keyboard commands
            if ( e.type == EventType.ValidateCommand && e.commandName == "UndoRedoPerformed" ) {
                GUIUtility.hotControl = 0;
                GUIUtility.keyboardControl = 0;

                if ( GraphEditorUtility.activeNode != null ) {
                    var id = GraphEditorUtility.activeNode.ID;
                    //re-select node by id to view new deserialized node.
                    GraphEditorUtility.activeElement = currentGraph.GetNodeWithID(id);
                } else {
                    //TODO: need to do this for connections as well.
                    GraphEditorUtility.activeElement = null;
                }

                willRepaint = true;
                fullDrawPass = true;
                UpdateReferencesAndNodeIDs();
                currentGraph.Validate();
                e.Use();
                return;
            }

            if ( e.type == EventType.MouseDown ) {
                RemoveNotification();
            }

            if ( mouseOverWindow == current && ( e.isMouse || e.isKey ) ) {
                willRepaint = true;
            }

            ///should we set dirty? Put in practise at the end
            var willDirty = false;
            if (
                ( e.rawType == EventType.MouseUp && e.button != 2 ) ||
                ( e.type == EventType.DragPerform ) ||
                ( e.type == EventType.KeyUp && ( e.keyCode == KeyCode.Return || GUIUtility.keyboardControl == 0 ) )
                ) {
                willDirty = true;
            }

            //initialize rects
            canvasRect = Rect.MinMaxRect(5, TOP_MARGIN, position.width - 5, position.height - BOTTOM_MARGIN);
            var aspect = canvasRect.width / canvasRect.height;
            minimapRect = Rect.MinMaxRect(canvasRect.xMax - ( Prefs.minimapSize.y * aspect ), canvasRect.yMax - Prefs.minimapSize.y, canvasRect.xMax - 2, canvasRect.yMax - 2);
            var originalCanvasRect = canvasRect;

            //canvas background
            GUI.Box(canvasRect, string.Empty, StyleSheet.canvasBG);
            //background grid
            DrawGrid(canvasRect, pan, zoomFactor);
            //handle minimap
            HandleMinimapEvents(minimapRect);
            //PRE nodes events
            HandlePreNodesGraphEvents(currentGraph, mousePosInCanvas);

            //begin zoom
            var oldMatrix = default(Matrix4x4);
            if ( zoomFactor != 1 ) {
                canvasRect = StartZoomArea(canvasRect, zoomFactor, out oldMatrix);
            }

            // calc viewRect
            {
                viewRect = canvasRect;
                viewRect.x = 0;
                viewRect.y = 0;
                viewRect.position -= pan / zoomFactor;
            }

            //main group
            GUI.BeginClip(canvasRect, pan / zoomFactor, default(Vector2), false);
            {
                DoCanvasGroups();

                BeginWindows();
                ShowNodesGUI(currentGraph, viewRect, fullDrawPass, mousePosInCanvas, zoomFactor);
                EndWindows();

                DrawPings();

                DoCanvasRectSelection(viewRect);
            }
            GUI.EndClip();

            //end zoom
            if ( zoomFactor != 1 && oldMatrix != default(Matrix4x4) ) {
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
            if ( willDirty ) {
                willDirty = false;
                willRepaint = true;
                currentGraph.Serialize();
                EditorUtility.SetDirty(currentGraph);
            }

            //repaint?
            if ( willRepaint || e.type == EventType.MouseMove || rootGraph.isRunning ) {
                Repaint();
            }

            if ( e.type == EventType.Repaint ) {
                fullDrawPass = false;
                willRepaint = false;
            }

            //playmode indicator
            if ( Application.isPlaying ) {
                var r = new Rect(0, 0, 120, 10);
                r.center = new Vector2(screenWidth / 2, screenHeight - BOTTOM_MARGIN - 50);
                GUI.color = Color.green;
                GUI.Box(r, "PlayMode Active", StyleSheet.windowHighlight);
            }

            //hack for quick popups
            if ( OnDoPopup != null ) {
                var temp = OnDoPopup;
                OnDoPopup = null;
                QuickPopup.Show(temp);
            }

            //PostGUI
            GraphEditorUtility.InvokePostGUI();

            //closure
            GUI.Box(originalCanvasRect, string.Empty, StyleSheet.canvasBorders);
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
        }

        ///----------------------------------------------------------------------------------------------

        //Recursively get the currenlty showing nested graph starting from the root
        static Graph GetCurrentGraph(Graph root) {
            if ( root.currentChildGraph == null ) {
                return root;
            }
            return GetCurrentGraph(root.currentChildGraph);
        }

        //Starts a zoom area, returns the scaled container rect
        static Rect StartZoomArea(Rect container, float zoomFactor, out Matrix4x4 oldMatrix) {
            GUI.EndGroup();
            container.y += TOOLBAR_HEIGHT;
            container.width *= 1 / zoomFactor;
            container.height *= 1 / zoomFactor;
            oldMatrix = GUI.matrix;
            var matrix1 = Matrix4x4.TRS(new Vector2(container.x, container.y), Quaternion.identity, Vector3.one);
            var matrix2 = Matrix4x4.Scale(new Vector3(zoomFactor, zoomFactor, 1f));
            GUI.matrix = matrix1 * matrix2 * matrix1.inverse * GUI.matrix;
            return container;
        }

        //Ends the zoom area
        static void EndZoomArea(Matrix4x4 oldMatrix) {
            GUI.matrix = oldMatrix;
            GUI.BeginGroup(new Rect(0, TOOLBAR_HEIGHT, screenWidth, screenHeight));
        }

        //This is called while within Begin/End windows
        static void ShowNodesGUI(Graph graph, Rect drawCanvas, bool fullDrawPass, Vector2 canvasMousePos, float zoomFactor) {
            //ensure IDs are updated. Must do on seperate iteration before gui
            //FIXME: move elsewhere
            for ( var i = 0; i < graph.allNodes.Count; i++ ) {
                if ( graph.allNodes[i].ID != i ) {
                    graph.UpdateNodeIDs(true);
                    break;
                }
            }

            for ( var i = 0; i < graph.allNodes.Count; i++ ) {
                Node.ShowNodeGUI(graph.allNodes[i], drawCanvas, fullDrawPass, canvasMousePos, zoomFactor);
            }

            if ( graph.primeNode != null ) {
                GUI.Box(new Rect(graph.primeNode.rect.x, graph.primeNode.rect.y - 20, graph.primeNode.rect.width, 20), "<b>START</b>", StyleSheet.box);
            }
        }

        ///Translate the graph to focus selection
        public static void FocusSelection() {
            if ( GraphEditorUtility.activeElements != null && GraphEditorUtility.activeElements.Count > 0 ) {
                FocusPosition(GetNodeBounds(GraphEditorUtility.activeElements.Cast<Node>().ToList()).center);
                return;
            }
            if ( GraphEditorUtility.activeElement != null ) {
                FocusElement(GraphEditorUtility.activeElement);
                return;
            }
            if ( currentGraph.allNodes.Count > 0 ) {
                FocusPosition(GetNodeBounds(currentGraph.allNodes).center);
                return;
            }
            FocusPosition(viewCanvasCenter);
        }

        ///Ping element
        public static void PingElement(IGraphElement element) {
            if ( element is Node ) { PingRect(( element as Node ).rect); }
            if ( element is Connection ) { PingRect(( element as Connection ).GetMidRect()); }
        }

        ///Translate the graph to the center of target element (node, connection)
        public static void FocusElement(IGraphElement element, bool alsoSelect = false) {
            if ( element is Node ) { FocusNode((Node)element, alsoSelect); }
            if ( element is Connection ) { FocusConnection((Connection)element, alsoSelect); }
        }

        ///Translate the graph to the center of the target node
        public static void FocusNode(Node node, bool alsoSelect = false) {
            if ( currentGraph == node.graph ) {
                FocusPosition(node.rect.center);
                PingRect(node.rect);
                if ( alsoSelect ) { GraphEditorUtility.activeElement = node; }
            }
        }

        ///Translate the graph to the center of the target connection
        public static void FocusConnection(Connection connection, bool alsoSelect = false) {
            if ( currentGraph == connection.sourceNode.graph ) {
                FocusPosition(connection.GetMidRect().center);
                PingRect(connection.GetMidRect());
                if ( alsoSelect ) { GraphEditorUtility.activeElement = connection; }
            }
        }

        ///Translate the graph to to center of the target pos
        public static void FocusPosition(Vector2 targetPos, bool smooth = true) {
            if ( smooth ) {
                smoothPan = -targetPos;
                smoothPan += new Vector2(viewRect.width / 2, viewRect.height / 2);
                smoothPan *= zoomFactor;
            } else {
                pan = -targetPos;
                pan += new Vector2(viewRect.width / 2, viewRect.height / 2);
                pan *= zoomFactor;
                smoothPan = null;
                smoothZoomFactor = null;
            }
        }

        ///Ping rect
        public static void PingRect(Rect rect) {
            pingValue = 1;
            pingRect = rect;
        }

        ///Zoom with center position
        static void ZoomAt(Vector2 center, float delta) {
            if ( zoomFactor == 1 && delta > 0 ) return;
            var pinPoint = ( center - pan ) / zoomFactor;
            var newZ = zoomFactor;
            newZ += delta;
            newZ = Mathf.Clamp(newZ, 0.25f, 1f);
            smoothZoomFactor = newZ;

            var a = ( pinPoint * newZ ) + pan;
            var b = center;
            var diff = b - a;
            smoothPan = pan + diff;
        }

        //Handles Drag&Drop operations
        static void AcceptDrops(Graph graph, Vector2 canvasMousePos) {
            if ( GraphEditorUtility.allowClick ) {
                if ( DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length == 1 ) {
                    if ( e.type == EventType.DragUpdated ) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    }
                    if ( e.type == EventType.DragPerform ) {
                        var value = DragAndDrop.objectReferences[0];
                        DragAndDrop.AcceptDrag();
                        graph.CallbackOnDropAccepted(value, canvasMousePos);
                    }
                }
            }
        }

        ///Gets the bound rect for the nodes
        static Rect GetNodeBounds(List<Node> nodes) {
            if ( nodes == null || nodes.Count == 0 ) {
                return default(Rect);
            }

            var arr = new Rect[nodes.Count];
            for ( var i = 0; i < nodes.Count; i++ ) {
                arr[i] = nodes[i].rect;
            }
            return RectUtils.GetBoundRect(arr);
        }

        //Do graphical multi selection box for nodes
        static void DoCanvasRectSelection(Rect container) {

            if ( GraphEditorUtility.allowClick && e.type == EventType.MouseDown && e.button == 0 && !e.alt && !e.shift && canvasRect.Contains(CanvasToView(e.mousePosition)) ) {
                GraphEditorUtility.activeElement = null;
                selectionStartPos = e.mousePosition;
                isMultiSelecting = true;
                e.Use();
            }

            if ( isMultiSelecting && e.rawType == EventType.MouseUp ) {
                var rect = RectUtils.GetBoundRect(selectionStartPos, e.mousePosition);
                var overlapedNodes = currentGraph.allNodes.Where(n => rect.Overlaps(n.rect) && !n.isHidden).ToList();
                isMultiSelecting = false;
                if ( e.control && rect.width > 50 && rect.height > 50 ) {
                    Undo.RegisterCompleteObjectUndo(currentGraph, "Create Group");
                    if ( currentGraph.canvasGroups == null ) {
                        currentGraph.canvasGroups = new List<CanvasGroup>();
                    }
                    currentGraph.canvasGroups.Add(new CanvasGroup(rect, "New Canvas Group"));
                } else {
                    if ( overlapedNodes.Count > 0 ) {
                        GraphEditorUtility.activeElements = overlapedNodes.Cast<IGraphElement>().ToList();
                        e.Use();
                    }
                }
            }

            if ( isMultiSelecting ) {
                var rect = RectUtils.GetBoundRect(selectionStartPos, e.mousePosition);
                if ( rect.width > 5 && rect.height > 5 ) {
                    GUI.color = new Color(0.5f, 0.5f, 1, 0.3f);
                    GUI.Box(rect, string.Empty, StyleSheet.box);
                    for ( var i = 0; i < currentGraph.allNodes.Count; i++ ) {
                        var node = currentGraph.allNodes[i];
                        if ( rect.Overlaps(node.rect) && !node.isHidden ) {
                            var highlightRect = node.rect;
                            GUI.Box(highlightRect, string.Empty, StyleSheet.windowHighlight);
                        }
                    }
                    if ( rect.width > 50 && rect.height > 50 ) {
                        GUI.color = new Color(1, 1, 1, e.control ? 0.6f : 0.15f);
                        GUI.Label(new Rect(e.mousePosition.x + 16, e.mousePosition.y, 120, 22), "<i>+ control for group</i>", StyleSheet.labelOnCanvas);
                    }
                }
            }

            GUI.color = Color.white;
        }



        //Draw a simple grid
        static void DrawGrid(Rect container, Vector2 offset, float zoomFactor) {

            if ( Event.current.type != EventType.Repaint ) {
                return;
            }

            Handles.color = new Color(0, 0, 0, 0.15f);

            var drawGridSize = zoomFactor > 0.5f ? GRID_SIZE : GRID_SIZE * 5;
            var step = drawGridSize * zoomFactor;

            var xDiff = offset.x % step;
            var xStart = container.xMin + xDiff;
            var xEnd = container.xMax;
            for ( var i = xStart; i < xEnd; i += step ) {
                Handles.DrawLine(new Vector3(i, container.yMin, 0), new Vector3(i, container.yMax, 0));
            }

            var yDiff = offset.y % step;
            var yStart = container.yMin + yDiff;
            var yEnd = container.yMax;
            for ( var i = yStart; i < yEnd; i += step ) {
                Handles.DrawLine(new Vector3(0, i, 0), new Vector3(container.xMax, i, 0));
            }

            Handles.color = Color.white;
        }


        //This is the hierarchy shown at top left. Recusrsively show the nested path
        static void DoBreadCrumbNavigation(Graph root) {

            if ( root == null ) {
                return;
            }

            //if something selected the inspector panel shows on top of the breadcrub. If external inspector active it doesnt matter, so draw anyway.
            if ( GraphEditorUtility.activeElement != null && !Prefs.useExternalInspector ) {
                return;
            }

            var resultInfo = targetOwner != null && targetOwner.graph == root && targetOwner.graphIsBound ? "Bound" : "Asset Reference";
            if ( targetOwner != null && EditorUtility.IsPersistent(targetOwner) ) { resultInfo += " | Prefab Asset"; }
            var graphInfo = string.Format("<color=#ff4d4d>({0})</color>", resultInfo);

            GUI.color = new Color(1f, 1f, 1f, 0.5f);

            GUILayout.BeginVertical();
            if ( root.currentChildGraph == null ) {

                if ( root.agent == null && root.blackboard == null ) {
                    GUILayout.Label(string.Format("<b><size=22>{0} {1}</size></b>", root.name, graphInfo), StyleSheet.labelOnCanvas);
                } else {
                    var agentInfo = root.agent != null ? root.agent.gameObject.name : "No Agent";
                    var bbInfo = root.blackboard != null ? root.blackboard.name : "No Blackboard";
                    GUILayout.Label(string.Format("<b><size=22>{0} {1}</size></b>\n<size=10>{2} | {3}</size>", root.name, graphInfo, agentInfo, bbInfo), StyleSheet.labelOnCanvas);
                }

            } else {

                GUILayout.BeginHorizontal();
                GUILayout.Label("⤴ " + root.name, StyleSheet.button);
                if ( e.type == EventType.MouseUp && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) ) {
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
        static void DoCanvasGroups() {

            if ( currentGraph.canvasGroups == null ) {
                return;
            }

            for ( var i = 0; i < currentGraph.canvasGroups.Count; i++ ) {
                var group = currentGraph.canvasGroups[i];
                var handleRect = new Rect(group.rect.x, group.rect.y, group.rect.width, 25);
                var scaleRectBR = new Rect(group.rect.xMax - 20, group.rect.yMax - 20, 20, 20);
                var style = StyleSheet.editorPanel;

                GUI.color = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.4f) : new Color(0.5f, 0.5f, 0.5f, 0.3f);
                GUI.Box(group.rect, string.Empty, style);

                if ( group.color != default(Color) ) {
                    GUI.color = group.color;
                    GUI.DrawTexture(group.rect, EditorGUIUtility.whiteTexture);
                }

                GUI.color = Color.white;
                GUI.Box(new Rect(scaleRectBR.x + 10, scaleRectBR.y + 10, 6, 6), string.Empty, StyleSheet.scaleArrowBR);

                var size = 14 / zoomFactor;
                var name = string.Format("<size={0}>{1}</size>", size, group.name);
                GUI.Label(handleRect, name, style);

                EditorGUIUtility.AddCursorRect(handleRect, group.editState == CanvasGroup.EditState.Renaming ? MouseCursor.Text : MouseCursor.Link);
                EditorGUIUtility.AddCursorRect(scaleRectBR, MouseCursor.ResizeUpLeft);

                if ( group.editState == CanvasGroup.EditState.Renaming ) {
                    GUI.SetNextControlName("GroupRename");
                    group.name = EditorGUI.TextField(handleRect, group.name, style);
                    GUI.FocusControl("GroupRename");
                    if ( e.keyCode == KeyCode.Return || ( e.type == EventType.MouseDown && !handleRect.Contains(e.mousePosition) ) ) {
                        group.editState = CanvasGroup.EditState.None;
                        GUIUtility.hotControl = 0;
                        GUIUtility.keyboardControl = 0;
                    }
                }

                if ( e.type == EventType.MouseDown && GraphEditorUtility.allowClick ) {

                    if ( handleRect.Contains(e.mousePosition) ) {

                        Undo.RegisterCompleteObjectUndo(currentGraph, "Move Canvas Group");

                        //calc group nodes
                        tempCanvasGroupNodes = currentGraph.allNodes.Where(n => group.rect.Encapsulates(n.rect)).ToArray();
                        tempCanvasGroupGroups = currentGraph.canvasGroups.Where(c => group.rect.Encapsulates(c.rect)).ToArray();

                        if ( e.button == 1 ) {
                            var menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Rename"), false, () => { group.editState = CanvasGroup.EditState.Renaming; });
                            menu.AddItem(new GUIContent("Edit Color"), false, () =>
                            {
                                DoPopup(() => { group.color = EditorGUILayout.ColorField(group.color); });
                            });
                            menu.AddItem(new GUIContent("Select Nodes"), false, () => { GraphEditorUtility.activeElements = tempCanvasGroupNodes.Cast<IGraphElement>().ToList(); });
                            menu.AddItem(new GUIContent("Delete Group"), false, () =>
                            {
                                currentGraph.canvasGroups.Remove(group);
                                if ( EditorUtility.DisplayDialog("Delete Group", "Delete contained nodes as well?", "Yes", "No") ) {
                                    foreach ( var node in tempCanvasGroupNodes ) {
                                        currentGraph.RemoveNode(node);
                                    }
                                }
                            });
                            GraphEditorUtility.PostGUI += () => { menu.ShowAsContext(); };
                        } else if ( e.button == 0 ) {
                            group.editState = CanvasGroup.EditState.Dragging;
                        }

                        e.Use();
                    }

                    if ( e.button == 0 && scaleRectBR.Contains(e.mousePosition) ) {
                        Undo.RegisterCompleteObjectUndo(currentGraph, "Scale Canvas Group");
                        group.editState = CanvasGroup.EditState.Scaling;
                        e.Use();
                    }
                }

                if ( e.type == EventType.MouseDrag ) {

                    if ( group.editState == CanvasGroup.EditState.Dragging ) {

                        group.rect.position += e.delta;

                        if ( !e.shift ) {
                            for ( var j = 0; j < tempCanvasGroupNodes.Length; j++ ) {
                                tempCanvasGroupNodes[j].position += e.delta;
                            }

                            for ( var j = 0; j < tempCanvasGroupGroups.Length; j++ ) {
                                tempCanvasGroupGroups[j].rect.position += e.delta;
                            }
                        }
                    }

                    if ( group.editState == CanvasGroup.EditState.Scaling ) {
                        group.rect.xMax = Mathf.Max(e.mousePosition.x + 5, group.rect.xMin + 100);
                        group.rect.yMax = Mathf.Max(e.mousePosition.y + 5, group.rect.yMin + 100);
                    }
                }

                if ( e.rawType == EventType.MouseUp ) {
                    group.editState = CanvasGroup.EditState.None;
                }
            }
        }


        //Snap all nodes either to grid if option enabled or to pixel perfect integer position
        static void SnapNodesToGridAndPixel(Graph graph) {
            for ( var i = 0; i < graph.allNodes.Count; i++ ) {
                var node = graph.allNodes[i];
                var pos = node.position;
                if ( Prefs.doSnap ) {
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
        static void HandleMinimapEvents(Rect container) {
            if ( !GraphEditorUtility.allowClick ) { return; }
            var resizeRect = new Rect(container.x, container.y, 6, 6);
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeUpLeft);
            if ( e.type == EventType.MouseDown && e.button == 0 && resizeRect.Contains(e.mousePosition) ) {
                isResizingMinimap = true;
                e.Use();
            }
            if ( e.rawType == EventType.MouseUp ) {
                isResizingMinimap = false;
            }
            if ( isResizingMinimap && e.type == EventType.MouseDrag ) {
                Prefs.minimapSize -= e.delta;
                e.Use();
            }

            if ( Prefs.minimapSize != Prefs.MINIMAP_MIN_SIZE ) {
                EditorGUIUtility.AddCursorRect(container, MouseCursor.MoveArrow);
                if ( e.type == EventType.MouseDown && e.button == 0 && container.Contains(e.mousePosition) ) {
                    var finalBound = ResolveMinimapBoundRect(currentGraph, viewRect);
                    var norm = Rect.PointToNormalized(container, e.mousePosition);
                    var pos = Rect.NormalizedToPoint(finalBound, norm);
                    FocusPosition(pos);
                    isDraggingMinimap = true;
                    e.Use();
                }
                if ( e.rawType == EventType.MouseUp ) {
                    isDraggingMinimap = false;
                }
                if ( isDraggingMinimap && e.type == EventType.MouseDrag ) {
                    var finalBound = ResolveMinimapBoundRect(currentGraph, viewRect);
                    var norm = Rect.PointToNormalized(container, e.mousePosition);
                    var pos = Rect.NormalizedToPoint(finalBound, norm);
                    FocusPosition(pos);
                    e.Use();
                }
            }
        }

        ///after nodes, a cool minimap
        static void DrawMinimap(Rect container) {

            GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.85f);
            GUI.Box(container, string.Empty, StyleSheet.windowShadow);
            GUI.Box(container, currentGraph.allNodes.Count > 0 ? string.Empty : "Minimap", StyleSheet.box);

            if ( Prefs.minimapSize != Prefs.MINIMAP_MIN_SIZE ) {

                var finalBound = ResolveMinimapBoundRect(currentGraph, viewRect);
                var lensRect = viewRect.TransformSpace(finalBound, container);
                GUI.color = new Color(1, 1, 1, 0.8f);
                GUI.Box(lensRect, string.Empty, StyleSheet.box);
                GUI.color = Color.white;
                finalBound = finalBound.ExpandBy(25);

                if ( currentGraph.canvasGroups != null ) {
                    for ( var i = 0; i < currentGraph.canvasGroups.Count; i++ ) {
                        var group = currentGraph.canvasGroups[i];
                        var blipRect = group.rect.TransformSpace(finalBound, container);
                        var blipHeaderRect = Rect.MinMaxRect(blipRect.xMin, blipRect.yMin, blipRect.xMax, blipRect.yMin + 2);
                        var color = group.color != default(Color) ? group.color : Color.gray;
                        color.a = 0.5f;
                        GUI.color = color;
                        GUI.DrawTexture(blipRect, Texture2D.whiteTexture);
                        GUI.DrawTexture(blipHeaderRect, Texture2D.whiteTexture);
                        GUI.color = Color.white;
                    }
                }

                if ( pingValue >= 0 ) {
                    GUI.color = Color.white.WithAlpha(pingValue);
                    var pingBlipRect = pingRect.TransformSpace(finalBound, container);
                    GUI.DrawTexture(pingBlipRect.ExpandBy(2), Texture2D.whiteTexture);
                    GUI.color = Color.white;
                }

                if ( currentGraph.allNodes != null ) {

                    for ( var i = 0; i < currentGraph.allNodes.Count; i++ ) {
                        var node = currentGraph.allNodes[i];
                        for ( var j = 0; j < node.outConnections.Count; j++ ) {
                            var connection = node.outConnections[j];
                            var snp = connection.sourceNode.rect.center.TransformSpace(finalBound, container);
                            var tnp = connection.targetNode.rect.center.TransformSpace(finalBound, container);
                            var sp = connection.startRect.center.TransformSpace(finalBound, container);
                            var tp = connection.endRect.center.TransformSpace(finalBound, container);
                            Handles.color = Application.isPlaying ? StyleSheet.GetStatusColor(connection.status) : Colors.Grey(0.35f);
                            Handles.DrawAAPolyLine(snp, sp, tp, tnp);
                            // Handles.DrawAAPolyLine(snp, tnp);
                            Handles.color = Color.white;
                        }
                    }

                    for ( var i = 0; i < currentGraph.allNodes.Count; i++ ) {
                        var node = currentGraph.allNodes[i];
                        if ( node.isHidden ) { continue; }
                        var blipRect = node.rect.TransformSpace(finalBound, container);

                        if ( Application.isPlaying && node.status != Status.Resting ) {
                            GUI.color = StyleSheet.GetStatusColor(node.status);
                            GUI.DrawTexture(blipRect.ExpandBy(2), Texture2D.whiteTexture);
                        }

                        if ( GraphEditorUtility.activeElement == node || GraphEditorUtility.activeElements.Contains(node) ) {
                            GUI.color = Color.white;
                            GUI.DrawTexture(blipRect.ExpandBy(2), Texture2D.whiteTexture);
                        }

                        GUI.color = node.nodeColor != default(Color) ? node.nodeColor : Color.grey;
                        GUI.DrawTexture(blipRect, Texture2D.whiteTexture);
                        GUI.color = Color.white;
                    }
                }
            }

            var resizeRect = new Rect(container.x, container.y, 6, 6);
            GUI.color = Color.white;
            GUI.Box(resizeRect, string.Empty, StyleSheet.scaleArrowTL);
        }

        //resolves the bounds used in the minimap
        static Rect ResolveMinimapBoundRect(Graph graph, Rect container) {
            var arr1 = new Rect[graph.allNodes.Count];
            for ( var i = 0; i < graph.allNodes.Count; i++ ) {
                arr1[i] = graph.allNodes[i].rect;
            }

            var nBounds = RectUtils.GetBoundRect(arr1);
            var finalBound = nBounds;

            if ( graph.canvasGroups != null && graph.canvasGroups.Count > 0 ) {
                var arr2 = new Rect[graph.canvasGroups.Count];
                for ( var i = 0; i < graph.canvasGroups.Count; i++ ) {
                    arr2[i] = graph.canvasGroups[i].rect;
                }
                var gBounds = RectUtils.GetBoundRect(arr2);
                finalBound = RectUtils.GetBoundRect(nBounds, gBounds);
            }

            finalBound = RectUtils.GetBoundRect(finalBound, container);
            return finalBound;
        }

        ///
        static void DrawPings() {
            if ( pingValue > 0 ) {
                GUI.color = Color.white.WithAlpha(pingValue);
                GUI.Box(pingRect, "", Styles.highlightBox);
                GUI.color = Color.white;
            }
        }

        //an idea but it's taking up space i dont like
        static void ShowConsoleLog() {
            var rect = Rect.MinMaxRect(canvasRect.xMin, canvasRect.yMax + 5, canvasRect.xMax, screenHeight - TOOLBAR_HEIGHT);
            var msg = GraphConsole.GetFirstMessageForGraph(currentGraph);
            if ( msg.IsValid() ) {
                rect.xMin += 2;
                GUI.Label(rect, GraphConsole.GetFormatedGUIContentForMessage(msg), StyleSheet.labelOnCanvas);
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
                if ( GUI.Button(rect, string.Empty, GUIStyle.none) ) {
                    GraphConsole.ShowWindow();
                }
            }
        }

        //this is shown when root graph is null
        static void ShowEmptyGraphGUI() {
            if ( targetOwner != null ) {
                var text = string.Format("The selected {0} does not have a {1} assigned.\n Please create or assign a new one in it's inspector.", targetOwner.GetType().Name, targetOwner.graphType.Name);
                current.ShowNotification(new GUIContent(text));
                return;
            }
            current.ShowNotification(new GUIContent("Please select a GraphOwner GameObject or a Graph Asset."));
        }

    }
}

#endif