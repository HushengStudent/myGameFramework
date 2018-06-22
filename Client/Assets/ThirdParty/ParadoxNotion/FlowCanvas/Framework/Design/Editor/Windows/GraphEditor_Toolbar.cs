using UnityEngine;
using UnityEditor;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace NodeCanvas.Editor {

    ///Toolbar
    public partial class GraphEditor {

		//This is called outside Begin/End Windows from GraphEditor.
		public static void ShowToolbar(Graph graph){

			var owner = graph.agent != null && graph.agent is GraphOwner && (graph.agent as GraphOwner).graph == graph? (GraphOwner)graph.agent : null;

			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			GUI.backgroundColor = new Color(1f,1f,1f,0.5f);

			///----------------------------------------------------------------------------------------------
			///Left side
			///----------------------------------------------------------------------------------------------

			if (GUILayout.Button("File", EditorStyles.toolbarDropDown, GUILayout.Width(50))){
				GetToolbarMenu_File(graph, owner).ShowAsContext();
			}

			if (GUILayout.Button("Edit", EditorStyles.toolbarDropDown, GUILayout.Width(50))){
				GetToolbarMenu_Edit(graph, owner).ShowAsContext();
			}

			if (GUILayout.Button("Prefs", EditorStyles.toolbarDropDown, GUILayout.Width(50))){
				GetToolbarMenu_Prefs(graph, owner).ShowAsContext();
			}

			GUILayout.Space(10);

			if (owner != null && GUILayout.Button("Select Owner", EditorStyles.toolbarButton, GUILayout.Width(80))){
				Selection.activeObject = owner;
				EditorGUIUtility.PingObject(owner);
			}

			if (EditorUtility.IsPersistent(graph) && GUILayout.Button("Select Graph", EditorStyles.toolbarButton, GUILayout.Width(80))){
				Selection.activeObject = graph;
				EditorGUIUtility.PingObject(graph);
			}

			GUILayout.Space(10);

			if (GUILayout.Button("Open Console", EditorStyles.toolbarButton, GUILayout.Width(90))){
				var type = ReflectionTools.GetType("NodeCanvas.Editor.GraphConsole");
				var method = type.GetMethod("ShowWindow");
				method.Invoke(null, null);
			}

			///----------------------------------------------------------------------------------------------
			///Mid
			///----------------------------------------------------------------------------------------------

			GUILayout.Space(10);
			GUILayout.FlexibleSpace();

			//TODO: implement search
			// EditorUtils.SearchField(null);

			GUILayout.FlexibleSpace();
			GUILayout.Space(10);

			///----------------------------------------------------------------------------------------------
			///Right side
			///----------------------------------------------------------------------------------------------

			GUI.backgroundColor = Color.clear;
			GUI.color = new Color(1,1,1,0.3f);
			GUILayout.Label(string.Format("{0} @NodeCanvas Framework v{1}", graph.GetType().Name, NodeCanvas.Framework.Internal.GraphSerializationData.FRAMEWORK_VERSION), EditorStyles.toolbarButton);
			GUILayout.Space(10);
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;

			//GRAPHOWNER JUMP SELECTION
			if (owner != null){
				if (GUILayout.Button(string.Format("[{0}]", owner.gameObject.name), EditorStyles.toolbarDropDown, GUILayout.Width(120))){
					var menu = new GenericMenu();
					foreach(var _o in Object.FindObjectsOfType<GraphOwner>()){
						var o = _o;
						menu.AddItem (new GUIContent(o.GetType().Name + "s/" + o.gameObject.name), false, ()=> { Selection.activeGameObject = o.gameObject; SetReferences(o); });
					}
					menu.ShowAsContext();
				}
			}

			NCPrefs.isLocked = GUILayout.Toggle(NCPrefs.isLocked, "Lock", EditorStyles.toolbarButton);
			GUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;
			GUI.color = Color.white;
		}

		///----------------------------------------------------------------------------------------------

		//FILE MENU
		static GenericMenu GetToolbarMenu_File(Graph graph, GraphOwner owner){
			var menu = new GenericMenu();
			menu.AddItem (new GUIContent("Clear"), false, ()=>
			{
				if (EditorUtility.DisplayDialog("Clear Canvas", "This will delete all nodes of the currently viewing graph!\nAre you sure?", "YES", "NO!")){
					graph.ClearGraph();
				}
			});

			menu.AddItem (new GUIContent ("Import JSON"), false, ()=>
			{
				if (graph.allNodes.Count > 0 && !EditorUtility.DisplayDialog("Import Graph", "All current graph information will be lost. Are you sure?", "YES", "NO")){
					return;
				}

				var path = EditorUtility.OpenFilePanel( string.Format("Import '{0}' Graph", graph.GetType().Name), "Assets", graph.GetGraphJSONFileExtension());
				if (!string.IsNullOrEmpty(path)){
					if ( graph.Deserialize( System.IO.File.ReadAllText(path), true, null ) == null){ //true: validate, null: graph._objectReferences
						EditorUtility.DisplayDialog("Import Failure", "Please read the logs for more information", "OK", string.Empty);
					}
				}
			});

			menu.AddItem (new GUIContent ("Export JSON"), false, ()=>
			{
				var path = EditorUtility.SaveFilePanelInProject (string.Format("Export '{0}' Graph", graph.GetType().Name), string.Empty, graph.GetGraphJSONFileExtension(), string.Empty);
				if (!string.IsNullOrEmpty(path)){
					System.IO.File.WriteAllText( path, graph.Serialize(true, null) ); //true: pretyJson, null: graph._objectReferences
					AssetDatabase.Refresh();
				}
			});

			menu.AddItem (new GUIContent("Show JSON"), false, ()=>
			{
				ParadoxNotion.Serialization.JSONSerializer.ShowData( graph.Serialize(true, null), graph.name );
			});

			return menu;
		}

		///----------------------------------------------------------------------------------------------

		//EDIT MENU
		static GenericMenu GetToolbarMenu_Edit(Graph graph, GraphOwner owner){
			var menu = new GenericMenu();
			//Bind
			if (!Application.isPlaying && owner != null && !owner.graphIsBound){
				menu.AddItem(new GUIContent("Bind To Owner"), false, ()=>
				{
					if (EditorUtility.DisplayDialog("Bind Graph", "This will make a local copy of the graph, bound to the owner.\n\nThis allows you to make local changes and assign scene object references directly.\n\nNote that you can also use scene object references through the use of Blackboard Variables.\n\nBind Graph?", "YES", "NO")){
						Undo.RecordObject(owner, "New Local Graph");
						owner.SetBoundGraphReference(owner.graph);
						EditorUtility.SetDirty(owner);
					}
				});
			}
			else menu.AddDisabledItem(new GUIContent("Bind To Owner"));

			//Save to asset
			if (owner != null && owner.graphIsBound){
				menu.AddItem(new GUIContent("Save To Asset"), false, ()=>
				{
					var newGraph = (Graph)EditorUtils.CreateAsset(graph.GetType(), true);
					if (newGraph != null){
						EditorUtility.CopySerialized(graph, newGraph);
						newGraph.Validate();
						AssetDatabase.SaveAssets();
					}
				});
			}
			else menu.AddDisabledItem(new GUIContent("Save To Asset"));

			//Create defined vars
			if (graph.blackboard != null){
				menu.AddItem(new GUIContent("Promote Defined Parameters To Variables"), false, ()=>
				{
					if (EditorUtility.DisplayDialog("Promote Defined Parameters", "This will fill the current Blackboard with a Variable for each defined Parameter in the graph.\nContinue?", "YES", "NO")){
						graph.PromoteDefinedParametersToVariables(graph.blackboard);
					}
				});
			}
			else menu.AddDisabledItem(new GUIContent("Promote Defined Parameters To Variables"));

			return menu;
		}

		///----------------------------------------------------------------------------------------------

		//PREFS MENU
		static GenericMenu GetToolbarMenu_Prefs(Graph graph, GraphOwner owner){
			var menu = new GenericMenu();
			menu.AddItem (new GUIContent ("Use Node Browser"), NCPrefs.useBrowser, ()=> {NCPrefs.useBrowser = !NCPrefs.useBrowser;});
			menu.AddItem (new GUIContent ("Show Icons"), NCPrefs.showIcons, ()=>
				{
					NCPrefs.showIcons = !NCPrefs.showIcons;
					foreach(var node in graph.allNodes){ node.rect = new Rect( node.position.x, node.position.y, Node.minSize.x, Node.minSize.y ); }
				});
			menu.AddItem (new GUIContent ("Show Node Help"), NCPrefs.showNodeInfo, ()=> {NCPrefs.showNodeInfo = !NCPrefs.showNodeInfo;});
			menu.AddItem (new GUIContent ("Show Comments"), NCPrefs.showComments, ()=> {NCPrefs.showComments = !NCPrefs.showComments;});
			menu.AddItem (new GUIContent ("Show Summary Info"), NCPrefs.showTaskSummary, ()=> {NCPrefs.showTaskSummary = !NCPrefs.showTaskSummary;});
			menu.AddItem (new GUIContent ("Show Node IDs"), NCPrefs.showNodeIDs, ()=> {NCPrefs.showNodeIDs = !NCPrefs.showNodeIDs;});
			menu.AddItem (new GUIContent ("Grid Snap"), NCPrefs.doSnap, ()=> {NCPrefs.doSnap = !NCPrefs.doSnap;});
			menu.AddItem (new GUIContent ("Log Events"), NCPrefs.logEvents, ()=>{ NCPrefs.logEvents = !NCPrefs.logEvents; });
			menu.AddItem (new GUIContent ("Breakpoints Pause Editor"), NCPrefs.breakpointPauseEditor, ()=> {NCPrefs.breakpointPauseEditor = !NCPrefs.breakpointPauseEditor;});
			menu.AddItem (new GUIContent ("Highlight Active In Hierarchy"), NCPrefs.highlightOwnersInHierarchy, ()=> {NCPrefs.highlightOwnersInHierarchy = !NCPrefs.highlightOwnersInHierarchy;});
			if (graph.autoSort){
				menu.AddItem (new GUIContent ("Automatic Hierarchical Move"), NCPrefs.hierarchicalMove, ()=> {NCPrefs.hierarchicalMove = !NCPrefs.hierarchicalMove;});
			}
			menu.AddItem (new GUIContent ("Connection Style/Curved"), NCPrefs.connectionStyle == NCPrefs.ConnectionStyle.Curved, ()=> {NCPrefs.connectionStyle = NCPrefs.ConnectionStyle.Curved;});
			menu.AddItem (new GUIContent ("Connection Style/Stepped"), NCPrefs.connectionStyle == NCPrefs.ConnectionStyle.Stepped, ()=> {NCPrefs.connectionStyle = NCPrefs.ConnectionStyle.Stepped;});
			menu.AddItem (new GUIContent ("Connection Style/Linear"), NCPrefs.connectionStyle == NCPrefs.ConnectionStyle.Linear, ()=> {NCPrefs.connectionStyle = NCPrefs.ConnectionStyle.Linear;});
			menu.AddItem (new GUIContent ("Node Header Style/Colorize Header"), NCPrefs.nodeHeaderStyle == NCPrefs.NodeHeaderStyle.ColorizeHeader, ()=> {NCPrefs.nodeHeaderStyle = NCPrefs.NodeHeaderStyle.ColorizeHeader;});
			menu.AddItem (new GUIContent ("Node Header Style/Colorize Title"), NCPrefs.nodeHeaderStyle == NCPrefs.NodeHeaderStyle.ColorizeTitle, ()=> {NCPrefs.nodeHeaderStyle = NCPrefs.NodeHeaderStyle.ColorizeTitle;});
			menu.AddItem( new GUIContent ("Open Preferred Types Editor..."), false, ()=>{PreferedTypesEditorWindow.ShowWindow();} );			
			return menu;
		}

	}
}