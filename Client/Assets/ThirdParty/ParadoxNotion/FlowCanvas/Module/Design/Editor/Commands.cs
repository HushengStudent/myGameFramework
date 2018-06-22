#if UNITY_EDITOR

using UnityEditor;
using ParadoxNotion.Design;
using NodeCanvas.Editor;
using NodeCanvas.Framework;

namespace FlowCanvas.Editor {

    static class Commands {

		[MenuItem("Tools/ParadoxNotion/FlowCanvas/Create/Scene Global Blackboard")]
		static void CreateGlobalBlackboard(){
			Selection.activeObject = GlobalBlackboard.Create();
		}

		[MenuItem("Tools/ParadoxNotion/FlowCanvas/Preferred Types Editor")]
		static void ShowPrefTypes(){
			PreferedTypesEditorWindow.ShowWindow();
		}

		[MenuItem("Tools/ParadoxNotion/FlowCanvas/Graph Debug Console")]
		static void OpenConsole(){
			GraphConsole.ShowWindow();
		}

	    [MenuItem("Tools/ParadoxNotion/FlowCanvas/External Inspector Panel")]
	    static void ShowExternalInspector(){
	    	ExternalInspectorWindow.ShowWindow();
	    }


		///----------------------------------------------------------------------------------------------

		[MenuItem("Tools/ParadoxNotion/FlowCanvas/Welcome Window")]
		static void ShowWelcome(){
			WelcomeWindow.ShowWindow(typeof(FlowScript));
		}

		[MenuItem("Tools/ParadoxNotion/FlowCanvas/Visit Website")]
		static void VisitWebsite(){
			Help.BrowseURL("http://flowcanvas.paradoxnotion.com");
		}
	}
}

#endif