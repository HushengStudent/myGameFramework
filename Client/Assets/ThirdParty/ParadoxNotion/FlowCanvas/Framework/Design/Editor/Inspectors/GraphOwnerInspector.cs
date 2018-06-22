#if UNITY_EDITOR

using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace NodeCanvas.Editor{


    [InitializeOnLoad]
    public class HierarchyIcons {

        static HierarchyIcons() {
            EditorApplication.hierarchyWindowItemOnGUI += ShowIcon;
			if (NCPrefs.highlightOwnersInHierarchy){
				GraphOwner.onOwnerBehaviourStateChange += delegate { EditorApplication.RepaintHierarchyWindow(); };
			}
        }

        static void ShowIcon(int ID, Rect r) {
            var go = EditorUtility.InstanceIDToObject(ID) as GameObject;
            if (go == null) return;
            var owner = go.GetComponent<GraphOwner>();
            if (owner == null) return;
            var prefix = string.Empty;
			if (NCPrefs.highlightOwnersInHierarchy){
				if (owner.isRunning){ prefix = "►"; }
				if (owner.isPaused){ prefix = "||"; }
			}
            var content = new GUIContent( prefix + "♟");
            var size = GUI.skin.GetStyle("label").CalcSize(content);
            r.x = r.xMax - size.x;
            r.width = size.x;
            GUI.color = Color.white;
			if (NCPrefs.highlightOwnersInHierarchy && (owner.isRunning || owner.isPaused) ){
				GUI.color = Color.yellow;
			}
            GUI.Label(r, content);
            GUI.color = Color.white;
        }
    }



	[CustomEditor(typeof(GraphOwner), true)]
	public class GraphOwnerInspector : UnityEditor.Editor {

		private GraphOwner owner{ get {return (GraphOwner)target;} }
		
		//destroy local graph when owner gets destroyed
		void OnDestroy(){
			if (owner == null && owner.graph != null){
				if (owner.graphIsBound){
					Undo.DestroyObjectImmediate(owner.graph);
				}
			}
		}

		//create new graph asset and assign it to owner
		Graph NewAsAsset(){
			var newGraph = (Graph)EditorUtils.CreateAsset(owner.graphType, true);
			if (newGraph != null){
				Undo.RecordObject(owner, "New Asset Graph");
				owner.graph = newGraph;
				EditorUtility.SetDirty(owner);
				EditorUtility.SetDirty(newGraph);
				AssetDatabase.SaveAssets();
			}
			return newGraph;			
		}

		//create new local graph and assign it to owner
		Graph NewAsBound(){
			var newGraph = (Graph)ScriptableObject.CreateInstance(owner.graphType);
			Undo.RecordObject(owner, "New Bound Graph");
			owner.SetBoundGraphReference(newGraph);
			EditorUtility.SetDirty(owner);
			return newGraph;
		}

		//Bind graph to owner
		void AssetToBound(){
			Undo.RecordObject(owner, "Bind Asset Graph");
			owner.SetBoundGraphReference(owner.graph);
			EditorUtility.SetDirty(owner);
		}

		
		public override void OnInspectorGUI(){

			UndoManager.CheckUndo(owner, "Graph Owner Inspector");
			if (owner.graph != null && owner.graphIsBound){
				UndoManager.CheckUndo(owner.graph, "Graph Owner Inspector");
			}

			var ownerPeristant = EditorUtility.IsPersistent(owner);
			var label = owner.graphType.Name.SplitCamelCase();

			if (owner.graph == null){
				EditorGUILayout.HelpBox(owner.GetType().Name + " needs a " + label + ".\nAssign or Create a new one...", MessageType.Info);
				if (!Application.isPlaying && GUILayout.Button("CREATE NEW")){
					
					Graph newGraph = null;
					if (EditorUtility.DisplayDialog("Create Graph", "Create a Bound or an Asset Graph?\n\n"+
                        "Bound Graph is saved with the GraphOwner and you can use direct scene references within it.\n\n"+
                        "Asset Graph is an asset file and can be reused amongst any number of GraphOwners.\n\n"+
                        "You can convert from one type to the other at any time.",
                        "Bound", "Asset")){

						newGraph = NewAsBound();

					} else {

						newGraph = NewAsAsset();
					}

					if (newGraph != null){
						GraphEditor.OpenWindow(owner);
					}
				}

				owner.graph = (Graph)EditorGUILayout.ObjectField(label, owner.graph, owner.graphType, false);
				if (GUI.changed){
					owner.Validate();
					EditorUtility.SetDirty(owner);
				}
				return;
			}

			GUILayout.Space(10);


			//Graph comments ONLY if Bound graph
			if (owner.graphIsBound){
				owner.graph.comments = GUILayout.TextArea(owner.graph.comments, GUILayout.Height(45));
				EditorUtils.TextFieldComment(owner.graph.comments, "Graph comments...");
			}

			//Open behaviour
			GUI.backgroundColor = Colors.lightBlue;
			if (GUILayout.Button( ("Edit " + owner.graphType.Name.SplitCamelCase()).ToUpper() )){
				GraphEditor.OpenWindow(owner);
			}
			GUI.backgroundColor = Color.white;
		
			if (!Application.isPlaying){

				if (!owner.graphIsBound && GUILayout.Button("Bind Graph")){
					if (EditorUtility.DisplayDialog("Bind Graph", "This will make a local copy of the graph, bound to the owner.\n\nThis allows you to make local changes and assign scene object references directly.\n\nNote that you can also use scene object references through the use of Blackboard Variables.\n\nBind Graph?", "YES", "NO")){
						AssetToBound();
					}
				}

				//Reference graph
				if (!owner.graphIsBound){

					owner.graph = (Graph)EditorGUILayout.ObjectField(label, owner.graph, owner.graphType, true);

				} else {

					if (GUILayout.Button("Delete Bound Graph")){
						if (EditorUtility.DisplayDialog("Delete Bound Graph", "Are you sure?", "YES", "NO")){
							Object.DestroyImmediate(owner.graph, true);
							Undo.RecordObject(owner, "Delete Bound Graph");
							owner.SetBoundGraphReference(null);
							EditorUtility.SetDirty(owner);
						}
					}
				}
			}



			//basic options
			// owner.blackboard = (Blackboard)EditorGUILayout.ObjectField("Blackboard", owner.blackboard as Blackboard, typeof(Blackboard), true);
			owner.enableAction = (GraphOwner.EnableAction)EditorGUILayout.EnumPopup("On Enable", owner.enableAction);
			owner.disableAction = (GraphOwner.DisableAction)EditorGUILayout.EnumPopup("On Disable", owner.disableAction);


			EditorUtils.Separator();

			//derived GUI
			OnExtraOptions();

			//execution debug controls
			if (Application.isPlaying && owner.graph != null && !ownerPeristant){
				var pressed = new GUIStyle(GUI.skin.GetStyle("button"));
				pressed.normal.background = pressed.active.background;

				GUILayout.BeginHorizontal("box");
				GUILayout.FlexibleSpace();

				if (GUILayout.Button(Icons.playIcon, owner.isRunning || owner.isPaused? pressed : (GUIStyle)"button")){
					if (owner.isRunning || owner.isPaused) owner.StopBehaviour();
					else owner.StartBehaviour();
				}

				if (GUILayout.Button(Icons.pauseIcon, owner.isPaused? pressed : (GUIStyle)"button")){	
					if (owner.isPaused) owner.StartBehaviour();
					else owner.PauseBehaviour();
				}

				OnGrapOwnerControls();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}

			EditorUtils.ReflectedObjectInspector(owner);
			EditorUtils.EndOfInspector();

			UndoManager.CheckDirty(owner);
			if (owner.graph != null && owner.graphIsBound){
				UndoManager.CheckDirty(owner.graph);
			}
		}

		virtual protected void OnExtraOptions(){}
		virtual protected void OnGrapOwnerControls(){}
	}
}

#endif