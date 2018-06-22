#if UNITY_EDITOR

using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEditor;
using UnityEngine;

namespace NodeCanvas.Editor{

	public class ExternalInspectorWindow : EditorWindow {

		private object currentSelection;
		private Vector2 scrollPos;

	    public static void ShowWindow() {
	        var window = GetWindow(typeof(ExternalInspectorWindow)) as ExternalInspectorWindow;
	        window.Show();
			NCPrefs.useExternalInspector = true;
	    }

		void OnEnable(){
	        titleContent = new GUIContent("NC Inspector");
		}

		void OnDestroy(){
			NCPrefs.useExternalInspector = false;
		}

		void Update(){
			if (currentSelection != GraphEditorUtility.activeElement){
				Repaint();
			}
		}

		void OnGUI(){

			if (GraphEditor.currentGraph == null){
				GUILayout.Label("No current NodeCanvas Graph open");				
				return;
			}
				
			if (EditorApplication.isCompiling){
				ShowNotification(new GUIContent("Compiling Please Wait..."));
				return;			
			}

			currentSelection = GraphEditorUtility.activeElement;

			if (currentSelection == null){
				GUILayout.Label("No Node Selected in Canvas");
				return;
			}

			scrollPos = GUILayout.BeginScrollView(scrollPos);

			if (currentSelection is Node){
				var node = (Node)currentSelection;
				Title(node.name);
				Node.ShowNodeInspectorGUI(node);
			}
			
			if (currentSelection is Connection){
				Title("Connection");
				(currentSelection as Connection).ShowConnectionInspectorGUI();
			}

			EditorUtils.EndOfInspector();
			GUILayout.EndScrollView();
		}

		void Title(string text){
			GUILayout.Space(5);
			GUILayout.BeginHorizontal("box", GUILayout.Height(28));
			GUILayout.FlexibleSpace();
			GUILayout.Label("<b><size=16>" + text + "</size></b>");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			EditorUtils.BoldSeparator();
		}
	}
}

#endif