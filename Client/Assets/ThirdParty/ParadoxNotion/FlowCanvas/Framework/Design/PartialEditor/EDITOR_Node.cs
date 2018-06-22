#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Editor;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;


namespace NodeCanvas.Framework{

	partial class Node{

		//Class for the nodeports GUI
		class GUIPort{
			public int portIndex{get; private set;}
			public Node parent{get; private set;}
			public Vector2 pos{get; private set;}
			public GUIPort(int index, Node parent, Vector2 pos){
				this.portIndex = index;
				this.parent = parent;
				this.pos = pos;
			}
		}

		[SerializeField]
		private bool _collapsed;
		[SerializeField]
		private Color _color;

		private Vector2 size = minSize;
		private object _icon{get;set;}
		private bool colorLoaded{get;set;}
		private bool hasColorAttribute{get;set;}
		private string hexColor{get;set;}
		private Color colorAttributeColor{get;set;}
		private bool nodeIsPressed{get;set;}

		private const string DEFAULT_HEX_COLOR_LIGHT = "eed9a7";
		private const string DEFAULT_HEX_COLOR_DARK = "333333";
		private static GUIPort clickedPort{get;set;}
		private static int dragDropMisses{get;set;}

		readonly public static Vector2 minSize = new Vector2(100, 20);
		readonly public static Dictionary<Status, Color> statusColors = new Dictionary<Status, Color>()
		{
			{Status.Failure, new Color(1.0f, 0.3f, 0.3f)},
			{Status.Success, new Color(0.4f, 0.7f, 0.2f)},
			{Status.Running, Color.yellow},
			{Status.Resting, new Color(0.7f, 0.7f, 1f, 0.8f)},
			{Status.Error, Color.red},
			{Status.Optional, Color.grey}
		};


		//This is to be able to work with rects which is easier in many cases.
		//Size is temporary to the node since it's auto adjusted thus no need to serialize it
		public Rect rect{
			get { return new Rect(_position.x, _position.y, size.x, size.y); }
			set
			{
				_position = new Vector2(value.x, value.y);
				size = new Vector2(Mathf.Max(value.width, minSize.x), Mathf.Max(value.height, minSize.y) );
			}
		}

		///EDITOR! Active is relevant to the input connections
		public bool isActive{
			get
			{
				for (var i = 0; i < inConnections.Count; i++){
					if (inConnections[i].isActive){
						return true;
					}
				}
				return inConnections.Count == 0;
			}
		}

		///EDITOR! Are children collapsed?
		public bool collapsed{
			get {return _collapsed;}
			set {_collapsed = value;}
		}

		///EDITOR! The custom color set by user.
		public Color customColor{
			get {return _color;}
			set {_color = value;}
		}

		///EDITOR! is the node hidden due to parent has children collapsed or is hidden itself?
		public bool isHidden {
			get
			{
				if (graph.autoSort){
					foreach (var parent in inConnections.Select(c => c.sourceNode)){
						if (parent.ID > this.ID){
							continue;
						}
						if (parent.collapsed || parent.isHidden){
							return true;
						}
					}
				}
				return false;
			}
		}

		///EDITOR! is the node selected or part of the multi selection?
		public bool isSelected{
			get {return GraphEditorUtility.activeElement == this || GraphEditorUtility.activeElements.Contains(this);}
		}

		//Is NC in icon mode && node has an icon?
		private bool showIcon{
			get {return NCPrefs.showIcons && icon != null;}
		}

		//The icon of the node
		public Texture2D icon{
			get
			{
				if (_icon == null){
					if (this is ITaskAssignable){
						var assignable = this as ITaskAssignable;
						_icon = assignable.task != null? assignable.task.icon : null;
					}
					if (_icon == null){
						var iconAtt = this.GetType().RTGetAttribute<IconAttribute>(true);
						_icon = iconAtt != null? UserTypePrefs.GetTypeIcon(iconAtt, this) : null;
					}
					if (_icon == null){
						_icon = new object();
					}
				}
				return _icon as Texture2D;
			}
		}


		//The coloring of the node if any.
		public Color nodeColor{
			get
			{
				if (customColor != default(Color)){
					return customColor;
				}

				if (!colorLoaded){
					colorLoaded = true;
					hasColorAttribute = false;
					colorAttributeColor = default(Color);
					hexColor = DEFAULT_HEX_COLOR_LIGHT;
					var cAtt = this.GetType().RTGetAttribute<ColorAttribute>(true);
					if (cAtt != null){
						hasColorAttribute = true;
						colorAttributeColor = Colors.HexToColor(cAtt.hexColor);
						hexColor = cAtt.hexColor;
					}
				}
				return colorAttributeColor;
			}
			private set
			{
				if (customColor != value){
					if (value.a <= 0.2f){
						customColor = default(Color);
						hexColor = DEFAULT_HEX_COLOR_LIGHT;
						return;
					}
					customColor = value;
					var temp = (Color32)value;
					hexColor = (temp.r.ToString("X2") + temp.g.ToString("X2") + temp.b.ToString("X2")).ToLower();
				}
			}
		}

		///----------------------------------------------------------------------------------------------

		//Helper function to create a nested graph for an IGraphAssignable
		protected static void CreateNested<T>(IGraphAssignable parent) where T:Graph{
			var newGraph = EditorUtils.CreateAsset<T>(true);
			if (newGraph != null){
				Undo.RegisterCreatedObjectUndo(newGraph, "CreateNested");
				parent.nestedGraph = newGraph;
			}
		}

		///Get connection information node wise, to show on top of the connection
		virtual public string GetConnectionInfo(int index){	return null; }
		///Extra inspector controls for the provided OUT connection
		virtual public void OnConnectionInspectorGUI(int index){ }

		//The main function for drawing a node's gui.Fires off others.
		public static void ShowNodeGUI(Node node, Rect drawCanvas, bool fullDrawPass, Vector2 canvasMousePos, float zoomFactor){

			if (node.isHidden){
				return;
			}

			if (fullDrawPass || drawCanvas.Overlaps(node.rect)){
				DrawNodeWindow(node, canvasMousePos, zoomFactor);
				DrawNodeTag(node);
				DrawNodeComments(node);
				DrawNodeID(node);
			}

			node.DrawNodeConnections(drawCanvas, fullDrawPass, canvasMousePos, zoomFactor);
		}

		//Draw the window
		static void DrawNodeWindow(Node node, Vector2 canvasMousePos, float zoomFactor){

			///un-colapse children ui
			if (node.collapsed){
				var r = new Rect(node.rect.x, (node.rect.yMax + 10), node.rect.width, 20);
				EditorGUIUtility.AddCursorRect(r, MouseCursor.Link);
				if (GUI.Button(r, "COLLAPSED", CanvasStyles.box)){
					node.collapsed = false;
				}
			}

			GUI.color = node.isActive? Color.white : new Color(0.9f, 0.9f, 0.9f, 0.8f);
			GUI.color = GraphEditorUtility.activeElement == node? new Color(0.9f, 0.9f, 1) : GUI.color;
			node.rect = GUILayout.Window(node.ID, node.rect, (ID)=>{ NodeWindowGUI(node, ID); }, string.Empty, CanvasStyles.window);


			GUI.Box(node.rect, string.Empty, CanvasStyles.windowShadow);
			GUI.color = new Color(1,1,1,0.5f);
			GUI.Box(new Rect(node.rect.x+6, node.rect.y+6, node.rect.width, node.rect.height), string.Empty, CanvasStyles.windowShadow);

			if (Application.isPlaying && node.status != Status.Resting){
				GUI.color = statusColors[node.status];
				GUI.Box(node.rect, string.Empty, CanvasStyles.windowHighlight);
				
			} else {
				
				if (node.isSelected){
					GUI.color = statusColors[Status.Resting];
					GUI.Box(node.rect, string.Empty, CanvasStyles.windowHighlight);
				}
			}

			GUI.color = Color.white;
			if (GraphEditorUtility.allowClick){
				if (zoomFactor == 1f){
					EditorGUIUtility.AddCursorRect(new Rect(node.rect.x, node.rect.y, node.rect.width, node.rect.height ), MouseCursor.Link);
				}
			}
		}


		//This is the callback function of the GUILayout.window. Everything here is called INSIDE the node Window callback.
		//The Window ID is the same as the node.ID.
		static void NodeWindowGUI(Node node, int ID){

			var e = Event.current;

			ShowHeader(node);
			ShowPossibleWarningIcon(node);
			HandleEvents(node, e);
			ShowStatusIcons(node);
			ShowBreakpointMark(node);
			ShowNodeContents(node);
			HandleContextMenu(node, e);
			HandleNodePosition(node, e);
		}

		//The title name or icon of the node
		static void ShowHeader(Node node){

			var colorizeHeader = NCPrefs.nodeHeaderStyle == NCPrefs.NodeHeaderStyle.ColorizeHeader;

			//alternative idea where header gets a colored background instead of coloring the text
			if (!node.showIcon || node.iconAlignment != Alignment2x2.Default){
				if (node.name != null){
					var finalTitle = node is IGraphAssignable? string.Format("{{ {0} }}", node.name) : node.name;
					var image = node.showIcon && node.iconAlignment == Alignment2x2.Left? node.icon : null;
					string hex;
					var isProSkin = EditorGUIUtility.isProSkin;
					if (colorizeHeader && node.nodeColor != default(Color)){
						hex = node.nodeColor.grayscale > 0.6f? DEFAULT_HEX_COLOR_DARK : DEFAULT_HEX_COLOR_LIGHT;
					} else {
						hex = isProSkin? node.hexColor : DEFAULT_HEX_COLOR_DARK;
					}

					if (colorizeHeader && node.nodeColor != default(Color)){
						GUI.color = node.nodeColor;
						var headerHeight = node.rect.height <= 35? 35 : 27;
						GUI.Box(new Rect(0,0,node.rect.width, headerHeight), string.Empty, CanvasStyles.windowHeader);
						GUI.color = Color.white;
					}

					var text = string.Format("<b><size=12><color=#{0}>{1}{2}</color></size></b>", hex, (image != null? " " : ""), finalTitle);
					var content = new GUIContent(text, image, node.description);
					GUILayout.Label(content, CanvasStyles.nodeTitle, GUILayout.MaxHeight(23));
				}
			}

			
			if (node.showIcon && (node.iconAlignment == Alignment2x2.Default || node.iconAlignment == Alignment2x2.Bottom) ){ //prefs in icon mode AND has icon
				GUI.color = node.nodeColor.a > 0.2f? node.nodeColor : Color.white;
				if (!EditorGUIUtility.isProSkin){
					var assignable = node as ITaskAssignable;
					IconAttribute att = null;
					if (assignable != null && assignable.task != null){
						att = assignable.task.GetType().RTGetAttribute<IconAttribute>(true);
					}

					if (att == null){
						att = node.GetType().RTGetAttribute<IconAttribute>(true);
					}

					if (att != null){
						if (att.fixedColor == false){
							GUI.color = new Color(0f,0f,0f, 0.7f);
						}
					}
				}
				
				GUI.backgroundColor = Color.clear;
				GUILayout.Box(node.icon, CanvasStyles.box, GUILayout.MaxHeight(32));
				GUI.backgroundColor = Color.white;
				GUI.color = Color.white;
			}

			//horizontal line node color indicator
			if ( !colorizeHeader && node.name != null && node.nodeColor.a > 0.2f && (!node.showIcon || !node.hasColorAttribute) ){
				var lastRect = GUILayoutUtility.GetLastRect();
				var hMargin = EditorGUIUtility.isProSkin? 4 : 1;
				GUILayout.Space(2);
				GUI.color = node.nodeColor;
				GUI.DrawTexture(new Rect(hMargin, lastRect.yMax, node.rect.width - (hMargin * 2), 3), EditorGUIUtility.whiteTexture);
				GUI.color = Color.white;
			}
		}

		///Responsible for showing a warning icon
		static void ShowPossibleWarningIcon(Node node){
			var assignable = node as ITaskAssignable;
			if (assignable != null && assignable.task != null){
				var warning = assignable.task.GetWarning();
				if (warning != null){
			        var errorRect = new Rect(node.rect.width - 21, 5, 16, 16);
		        	GUI.Box(errorRect, Icons.warningIcon, GUIStyle.none);					
				}
			}
		}

		//Handles events, Mouse downs, ups etc.
		static void HandleEvents(Node node, Event e){

			//Node click
			if (e.type == EventType.MouseDown && GraphEditorUtility.allowClick && e.button != 2){

	    		Undo.RegisterCompleteObjectUndo(node.graph, "Move Node");

				if (!e.control){
					GraphEditorUtility.activeElement = node;
				}

				if (e.control){
					if (node.isSelected) { GraphEditorUtility.activeElements.Remove(node); }
					else { GraphEditorUtility.activeElements.Add(node); }
				}

				if (e.button == 0){
					node.nodeIsPressed = true;
				}

				//Double click
				if (e.button == 0 && e.clickCount == 2){
		    		if (node is IGraphAssignable && (node as IGraphAssignable).nestedGraph != null ){
	    				node.graph.currentChildGraph = (node as IGraphAssignable).nestedGraph;
	    				node.nodeIsPressed = false;
		    		} else if (node is ITaskAssignable && (node as ITaskAssignable).task != null){
		    			EditorUtils.OpenScriptOfType((node as ITaskAssignable).task.GetType());
		    		} else {
			    		EditorUtils.OpenScriptOfType(node.GetType());
		    		}
		    		e.Use();
		    	}

		    	node.OnNodePicked();
			}

	    	//Mouse up
	    	if (e.type == EventType.MouseUp){
	    		node.nodeIsPressed = false;
	    		if (node.graph.autoSort){
	    			node.SortConnectionsByPositionX();
	    		}
	    		node.OnNodeReleased();
	    	}			
		}

		//Shows the icons relative to the current node status
		static void ShowStatusIcons(Node node){
	        if (Application.isPlaying && node.status != Status.Resting){
		        var markRect = new Rect(5, 5, 15, 15);
		        if (node.status == Status.Success){
		        	GUI.color = statusColors[Status.Success];
		        	GUI.Box(markRect, string.Empty, CanvasStyles.checkMark);

		        } else if (node.status == Status.Running){
		        	GUI.Box(markRect, string.Empty, CanvasStyles.clockMark);

		        } else if (node.status == Status.Failure){
		        	GUI.color = statusColors[Status.Failure];
		        	GUI.Box(markRect, string.Empty, CanvasStyles.xMark);
		        }
		    }
		}

		//Shows the breakpoint mark icon if node is set as a breakpoint
		static void ShowBreakpointMark(Node node){
			if (node.isBreakpoint){
				var rect = new Rect( node.rect.width-15, 5, 12, 12);
				GUI.DrawTexture(rect, Icons.redCircle);
			}
		}

		//Shows the actual node contents GUI
		static void ShowNodeContents(Node node){
	        
	        GUI.color = Color.white;
	        GUI.skin.label.richText = true;
	        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

			node.OnNodeGUI();

			if (node is ITaskAssignable){
				var task = (node as ITaskAssignable).task;
				if (task == null){
					GUILayout.Label("No Task");
				} else {
					GUILayout.Label(NCPrefs.showTaskSummary? task.summaryInfo : string.Format("<b>{0}</b>", task.name));
				}
			}

			GUI.skin.label.alignment = TextAnchor.UpperLeft;
		}

		//Handles and shows the right click mouse button for the node context menu
		static void HandleContextMenu(Node node, Event e){
			var isContextClick = (e.type == EventType.MouseUp && e.button == 1) || ( /*e.control && e.type == EventType.MouseUp ||*/ e.type == EventType.ContextClick);
		    if (GraphEditorUtility.allowClick && isContextClick){
		    	GenericMenu menu;
		    	if (GraphEditorUtility.activeElements.Count > 1){
					menu = GetNodeMenu_Multi(node.graph);
		    	} else {
					menu = GetNodeMenu_Single(node);
		    	}
				if (menu != null){
					//show in PostGUI due to zoom factor
					GraphEditorUtility.PostGUI += ()=> { menu.ShowAsContext(); };
		            e.Use();
				}
		    }
		}

		//Returns multi node context menu
		static GenericMenu GetNodeMenu_Multi(Graph graph){
			var menu = new GenericMenu();
			menu.AddItem (new GUIContent ("Duplicate Selected Nodes"), false, ()=>
				{
					var newNodes = Graph.CloneNodes(GraphEditorUtility.activeElements.OfType<Node>().ToList(), graph);
					GraphEditorUtility.activeElements = newNodes.Cast<object>().ToList();
				});
			menu.AddItem (new GUIContent ("Copy Selected Nodes"), false, ()=>{ CopyBuffer.Set<Node[]>( Graph.CloneNodes(GraphEditorUtility.activeElements.OfType<Node>().ToList() ).ToArray() );  });
			menu.AddSeparator("/");
			menu.AddItem (new GUIContent ("Delete Selected Nodes"), false, ()=>{ foreach (Node n in GraphEditorUtility.activeElements.ToArray()) graph.RemoveNode(n); });
			return menu;
		}

		//Returns single node context menu
		static GenericMenu GetNodeMenu_Single(Node node){
			var menu = new GenericMenu();
			if (node.graph.primeNode != node && node.allowAsPrime){
				menu.AddItem (new GUIContent("Set Start"), false, ()=> {node.graph.primeNode = node;});
			}

			if (node is IGraphAssignable){
				menu.AddItem (new GUIContent("Edit Nested (Double Click)"), false, ()=> {node.graph.currentChildGraph = (node as IGraphAssignable).nestedGraph; } );
			}
			
			menu.AddItem (new GUIContent("Duplicate (CTRL+D)"), false, ()=> { GraphEditorUtility.activeElement = node.Duplicate(node.graph); });
			menu.AddItem( new GUIContent("Copy Node"), false, ()=> { CopyBuffer.Set<Node[]>( new Node[]{node} ); } );

			if (node.inConnections.Count > 0){
				menu.AddItem (new GUIContent(node.isActive? "Disable" : "Enable"), false, ()=> {node.SetActive(!node.isActive);});
			}

			if (node.graph.autoSort && node.outConnections.Count > 0){
				menu.AddItem (new GUIContent(node.collapsed? "Expand Children" : "Collapse Children"), false, ()=> { node.collapsed = !node.collapsed; });
			}

			if (node is ITaskAssignable){
				var assignable = node as ITaskAssignable;
				if (assignable.task != null){
					menu.AddItem (new GUIContent("Copy Assigned Task"), false, ()=> { CopyBuffer.Set<Task>(assignable.task); });
				} else {
					menu.AddDisabledItem(new GUIContent("Copy Assigned Task"));
				}

				if (CopyBuffer.Has<Task>()) {
					menu.AddItem (new GUIContent("Paste Assigned Task"), false, ()=>
					{
						if (assignable.task == CopyBuffer.Peek<Task>()){
							return;
						}

						if (assignable.task != null){
							if (!EditorUtility.DisplayDialog("Paste Task", string.Format("Node already has a Task assigned '{0}'. Replace assigned task with pasted task '{1}'?", assignable.task.name, CopyBuffer.Peek<Task>().name), "YES", "NO")){
								return;
							}
						}

						try {assignable.task = CopyBuffer.Get<Task>().Duplicate(node.graph);}
						catch {ParadoxNotion.Services.Logger.LogWarning("Can't paste Task here. Incombatible Types", "Editor", node);}
					});

				} else {
					menu.AddDisabledItem(new GUIContent("Paste Assigned Task"));
				}
			}
			
			menu = node.OnContextMenu(menu);
			if (menu != null){
				menu.AddSeparator("/");
				menu.AddItem (new GUIContent("Delete (DEL)"), false, ()=> {node.graph.RemoveNode(node);});
			}
			return menu;
		}

		//basicaly handles the node position and draging etc
		static void HandleNodePosition(Node node, Event e){

		    if (GraphEditorUtility.allowClick && e.button != 2){
	    		//drag all selected nodes
	    		if (e.type == EventType.MouseDrag && GraphEditorUtility.activeElements.Count > 1){
	    			for (var i = 0; i < GraphEditorUtility.activeElements.Count; i++){
	    				((Node)GraphEditorUtility.activeElements[i]).position += e.delta;
	    			}
	    			return;
	    		}

	    		if (node.nodeIsPressed){
		    		var hierarchicalMove = NCPrefs.hierarchicalMove != e.shift;
		    		//snap to grid
			    	if (!hierarchicalMove && NCPrefs.doSnap && GraphEditorUtility.activeElements.Count == 0){
			    		node.position = new Vector2( Mathf.Round(node.position.x/15) * 15, Mathf.Round(node.position.y/15) * 15 );
					}		

			    	//recursive drag
			    	if (node.graph.autoSort && e.type == EventType.MouseDrag){
				    	if (hierarchicalMove || node.collapsed ){
				    		RecursivePanNode(node, e.delta);
				    	}
			    	}
		    	}

		    	//this drag
				GUI.DragWindow();
		    }			
		}

		//The comments of the node sitting next or bottom of it
		static void DrawNodeComments(Node node){

			if ( !NCPrefs.showComments || string.IsNullOrEmpty(node.comments)){
				return;
			}

			var commentsRect = new Rect();
			var style = CanvasStyles.textArea;
			var size = style.CalcSize(new GUIContent(node.comments));

			if (node.commentsAlignment == Alignment2x2.Top){
				size.y = style.CalcHeight(new GUIContent(node.comments), node.rect.width);
				commentsRect = new Rect(node.rect.x, node.rect.y - size.y, node.rect.width, size.y -2);
			}
			if (node.commentsAlignment == Alignment2x2.Bottom){
				size.y = style.CalcHeight(new GUIContent(node.comments), node.rect.width);
				commentsRect = new Rect(node.rect.x, node.rect.yMax + 5, node.rect.width, size.y);
			}
			if (node.commentsAlignment == Alignment2x2.Left){
				var width = Mathf.Min(size.x, node.rect.width * 2);
				commentsRect = new Rect(node.rect.xMin - width, node.rect.yMin, width, node.rect.height);
			}
			if (node.commentsAlignment == Alignment2x2.Right){
				commentsRect = new Rect(node.rect.xMax + 5, node.rect.yMin, Mathf.Min(size.x, node.rect.width * 2), node.rect.height);
			}

			GUI.color = new Color(1,1,1,0.6f);
			GUI.backgroundColor = new Color(1f,1f,1f,0.2f);
			GUI.Box(commentsRect, node.comments, CanvasStyles.textArea);
			GUI.backgroundColor = Color.white;
			GUI.color = Color.white;
		}

		//Shows the tag label on the left of the node if it is tagged
		static void DrawNodeTag(Node node){
			if (!string.IsNullOrEmpty(node.tag)){
				var size = CanvasStyles.label.CalcSize(new GUIContent(node.tag));
				var tagRect = new Rect(node.rect.x - size.x -10, node.rect.y, size.x, size.y);
				GUI.Label(tagRect, node.tag, CanvasStyles.label);
				tagRect.width = Icons.tagIcon.width;
				tagRect.height = Icons.tagIcon.height;
				tagRect.y += tagRect.height + 5;
				tagRect.x = node.rect.x - 22;
				GUI.DrawTexture(tagRect, Icons.tagIcon);
			}
		}

		//Show the Node ID, mostly for debug purposes
		static void DrawNodeID(Node node){
			if (NCPrefs.showNodeIDs){
				var rect = new Rect(node.rect.x, node.rect.y - 18, node.rect.width, 18);
				GUI.color = Color.grey;
				GUI.Label(rect, string.Format("<size=9>#{0}</size>", node.ID.ToString() ), CanvasStyles.label );
				GUI.color = Color.white;
			}
		}

		//Function to pan the node with children recursively
		static void RecursivePanNode(Node node, Vector2 delta){
			node.position += delta;
			for (var i= 0; i < node.outConnections.Count; i++){
				var child = node.outConnections[i].targetNode;
				if (child.ID > node.ID){
					RecursivePanNode(child, delta);
				}
			}
		}

		//The inspector of the node shown in the editor panel or else.
		static public void ShowNodeInspectorGUI(Node node){

			UndoManager.CheckUndo(node.graph, "Node Inspector");

			if (NCPrefs.showNodeInfo){
				GUI.backgroundColor = Colors.lightBlue;
				EditorGUILayout.HelpBox(node.description, MessageType.None);
				GUI.backgroundColor = Color.white;
			}

			GUILayout.BeginHorizontal();
			if (!node.showIcon && node.allowAsPrime){
				node.customName = EditorGUILayout.TextField(node.customName);
				EditorUtils.TextFieldComment(node.customName, "Name...");
			}

			node.tag = EditorGUILayout.TextField(node.tag);
			EditorUtils.TextFieldComment(node.tag, "Tag...");

			if (!node.hasColorAttribute){
				node.nodeColor = EditorGUILayout.ColorField(node.nodeColor, GUILayout.Width(30));
			}

			GUILayout.EndHorizontal();

			GUI.color = new Color(1,1,1,0.5f);
			node.comments = EditorGUILayout.TextArea(node.comments);
			GUI.color = Color.white;
			EditorUtils.TextFieldComment(node.comments, "Comments...");

			EditorUtils.Separator();
			node.OnNodeInspectorGUI();
			TaskAssignableGUI(node);

			if (GUI.changed){ //minimize node so that GUILayour brings it back to correct scale
				node.rect = new Rect( node.rect.x, node.rect.y, Node.minSize.x, Node.minSize.y );
			}

			UndoManager.CheckDirty(node.graph);
		}


		//If the node implements ITaskAssignable this is shown for the user to assign a task
		static void TaskAssignableGUI(Node node){

			if (node is ITaskAssignable){

				var assignable = (node as ITaskAssignable);
				System.Type taskType = null;
				foreach(var iType in node.GetType().GetInterfaces()) {
				    if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(ITaskAssignable<>)){
				        taskType = iType.GetGenericArguments()[0];
				        break;
				    }
				}

				if (taskType != null){
					TaskEditor.TaskFieldMulti(assignable.task, node.graph, taskType, (t)=> {node._icon = null; assignable.task = t;} );
				}
			}
		}


		//Activates/Deactivates all inComming connections
		void SetActive(bool active){

			if (isChecked){
				return;
			}
			
			isChecked = true;

			//just for visual feedback
			if (!active){
				GraphEditorUtility.activeElement = null;
			}

			Undo.RecordObject(graph, "SetActive");

			//disable all incomming
			foreach (var cIn in inConnections){
				cIn.isActive = active;
			}
			
			//disable all outgoing
			foreach (var cOut in outConnections){
				cOut.isActive = active;
			}

			//if child is still considered active(= at least 1 incomming is active), continue else SetActive child as well
			foreach (var child in outConnections.Select(c => c.targetNode)){
				
				if (child.isActive == !active){
					continue;
				}

				child.SetActive(active);
			}

			isChecked = false;
		}


		//Sorts the connections based on the child nodes and this node X position. Possible only when not in play mode
		void SortConnectionsByPositionX(){
			if (!Application.isPlaying){
				
				if (isChecked){
					return;
				}

				isChecked = true;
				outConnections = outConnections.OrderBy(c => c.targetNode.rect.center.x ).ToList();
				foreach(var connection in inConnections.ToArray()){
					connection.sourceNode.SortConnectionsByPositionX();
				}
				isChecked = false;
			}
		}


		///Draw an automatic editor inspector for this node.
		protected void DrawDefaultInspector(){
			EditorUtils.ReflectedObjectInspector(this);
		}
		
		//Editor. When the node is picked
		virtual protected void OnNodePicked(){}
		//Editor. When the node is released (mouse up)
		virtual protected void OnNodeReleased(){}
		///Editor. Override to show controls within the node window
		virtual protected void OnNodeGUI(){}
		//Editor. Override to show controls within the inline inspector or leave it to show an automatic editor
		virtual protected void OnNodeInspectorGUI(){ DrawDefaultInspector(); }
		//Editor. Override to add more entries to the right click context menu of the node
		virtual protected GenericMenu OnContextMenu(GenericMenu menu){ return menu; }

		//Draw the connections line from this node, to all of its children. This is the default hierarchical tree style. Override in each system's base node class.
		virtual protected void DrawNodeConnections(Rect drawCanvas, bool fullDrawPass, Vector2 canvasMousePos, float zoomFactor){

			var e = Event.current;

			//Receive connections first
			if (clickedPort != null && e.type == EventType.MouseUp && e.button == 0){
		
				if (rect.Contains(e.mousePosition)){
					graph.ConnectNodes(clickedPort.parent, this, clickedPort.portIndex);
					clickedPort = null;
					e.Use();

				} else {

					dragDropMisses ++;

					if (dragDropMisses == graph.allNodes.Count && clickedPort != null){

						var source = clickedPort.parent;
						var index = clickedPort.portIndex;
						var pos = e.mousePosition;						
						clickedPort = null;
						
						System.Action<System.Type> Selected = delegate(System.Type type){
							var newNode = graph.AddNode(type, pos);
							graph.ConnectNodes(source, newNode, index);
							newNode.SortConnectionsByPositionX();
							GraphEditorUtility.activeElement = newNode;
						};

						var menu = EditorUtils.GetTypeSelectionMenu(graph.baseNodeType, Selected);
						if (zoomFactor == 1 && NCPrefs.useBrowser){
							menu.ShowAsBrowser(string.Format("Add {0} Node", graph.GetType().Name), graph.baseNodeType );
						} else {
							GraphEditorUtility.PostGUI += ()=> { menu.ShowAsContext(); };
						}
						e.Use();							
					}
				}
			}

			if (maxOutConnections == 0){
				return;
			}
			
			if (fullDrawPass || drawCanvas.Overlaps(rect)){

				var nodeOutputBox = new Rect(rect.x, rect.yMax - 4, rect.width, 12);
				GUI.Box(nodeOutputBox, string.Empty, CanvasStyles.nodePortContainer);

				//draw the ports
				if (outConnections.Count < maxOutConnections || maxOutConnections == -1){
					for (var i = 0; i < outConnections.Count + 1; i++){
						var portRect = new Rect(0, 0, 10, 10);
						portRect.center = new Vector2(((rect.width / (outConnections.Count + 1)) * (i + 0.5f)) + rect.xMin, rect.yMax + 6);
						GUI.Box(portRect, string.Empty, CanvasStyles.nodePortEmpty);

						if (collapsed){
							continue;
						}

						if (GraphEditorUtility.allowClick){
							//start a connection by clicking a port
							EditorGUIUtility.AddCursorRect(portRect, MouseCursor.ArrowPlus);
							if (e.type == EventType.MouseDown && e.button == 0 && portRect.Contains(e.mousePosition)){
								dragDropMisses = 0;
								clickedPort = new GUIPort(i, this, portRect.center);
								e.Use();
							}
						}
					}
				}
			}


			//draw the new drag&drop connection line
			if (clickedPort != null && clickedPort.parent == this){
				var yDiff = (clickedPort.pos.y - e.mousePosition.y) * 0.5f;
				yDiff = e.mousePosition.y > clickedPort.pos.y? -yDiff : yDiff;
				var tangA = new Vector2(0, yDiff);
				var tangB = tangA * -1;
				Handles.DrawBezier(clickedPort.pos, e.mousePosition, clickedPort.pos + tangA , e.mousePosition + tangB, new Color(0.5f,0.5f,0.8f,0.8f), null, 3);
			}


			//draw all connected lines
			for (var i = 0; i < outConnections.Count; i++){
				
				var connection = outConnections[i];
				if (connection != null){

					var sourcePos = new Vector2(((rect.width / (outConnections.Count + 1)) * (i + 1) ) + rect.xMin, rect.yMax + 6);
					var targetPos = new Vector2(connection.targetNode.rect.center.x, connection.targetNode.rect.y);

					var sourcePortRect = new Rect(0,0,12,12);
					sourcePortRect.center = sourcePos;

					var targetPortRect = new Rect(0,0,15,15);
					targetPortRect.center = targetPos;

					var boundRect = RectUtils.GetBoundRect(sourcePortRect, targetPortRect);
					if (fullDrawPass || drawCanvas.Overlaps(boundRect)){

						GUI.Box(sourcePortRect, string.Empty, CanvasStyles.nodePortConnected);
				
						if (collapsed || connection.targetNode.isHidden){
							continue;
						}

						connection.DrawConnectionGUI(sourcePos, targetPos);

						if (GraphEditorUtility.allowClick){
							//On right click disconnect connection from the source.
							if (e.type == EventType.ContextClick && sourcePortRect.Contains(e.mousePosition)){
								graph.RemoveConnection(connection);
								e.Use();
								return;
							}

							//On right click disconnect connection from the target.
							if (e.type == EventType.ContextClick && targetPortRect.Contains(e.mousePosition)){
								graph.RemoveConnection(connection);
								e.Use();
								return;
							}
						}
					}

				}
			}
		}

	}
}

#endif
