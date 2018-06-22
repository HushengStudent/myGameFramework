#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


namespace ParadoxNotion.Design{

	///A generic popup editor
	public class GenericInspectorWindow : EditorWindow{

		public static GenericInspectorWindow current{get; private set;}
		public string inspectedID{get; private set;}
		public object value{get; private set;}

		private System.Type targetType;
		private Object context;
		private Vector2 scrollPos;

		//...
		void OnEnable(){
	        titleContent = new GUIContent("Object Editor");
			current = this;
			#if UNITY_2017_2_OR_NEWER
			EditorApplication.playModeStateChanged += PlayModeChange;
			#else
			EditorApplication.playmodeStateChanged += PlayModeChange;
			#endif
		}

		//...
		void OnDisable(){
			#if UNITY_2017_2_OR_NEWER
			EditorApplication.playModeStateChanged -= PlayModeChange;
			#else
			EditorApplication.playmodeStateChanged -= PlayModeChange;
			#endif
			current = null;
		}

#if UNITY_2017_2_OR_NEWER
		void PlayModeChange(PlayModeStateChange state){ Close(); }
#else
		void PlayModeChange(){ Close(); }
#endif

		//...
		void OnGUI(){

			if (targetType == null){
				return;
			}

			//Begin undo check
			GUI.skin.label.richText = true;
			UndoManager.CheckUndo(context, "Blackboard External Inspector");

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("<size=14><b>{0}</b></size>", targetType.FriendlyName()) );
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			value = EditorUtils.ReflectedFieldInspector(targetType.FriendlyName(), value, targetType, null, null);
			GUILayout.EndScrollView();
			Repaint();

			//Check dirty
			UndoManager.CheckDirty(context);
		}

		///Open utility window to inspect target object of type in context.
		///ID is simply a way for external types to track what is inspected on their own and whether or not they should care.
		public static void Show(string inspectedID, object o, System.Type t, Object context){
			var window = current == null? CreateInstance<GenericInspectorWindow>() : current;
			window.inspectedID = inspectedID;
			window.value = o;
			window.targetType = t;
			window.context = context;
			window.ShowUtility();
		}
	}
}

#endif