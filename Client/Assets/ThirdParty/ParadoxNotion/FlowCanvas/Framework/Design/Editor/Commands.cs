#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace NodeCanvas.Editor{

	static class Commands {

		[MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/New Task")]
		[MenuItem("Assets/Create/ParadoxNotion/NodeCanvas/New Task")]
		static void ShowTaskWizard(){
			TaskWizardWindow.ShowWindow();
		}

		///----------------------------------------------------------------------------------------------

		[MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/Scene Global Blackboard")]
		static void CreateGlobalBlackboard(){
			Selection.activeObject = GlobalBlackboard.Create();
		}

		[MenuItem("Tools/ParadoxNotion/NodeCanvas/Preferred Types Editor")]
		static void ShowPrefTypes(){
			PreferedTypesEditorWindow.ShowWindow();
		}

		[MenuItem("Tools/ParadoxNotion/NodeCanvas/Graph Debug Console")]
		static void OpenConsole(){
			GraphConsole.ShowWindow();
		}

	    [MenuItem("Tools/ParadoxNotion/NodeCanvas/External Inspector Panel")]
	    static void ShowExternalInspector(){
	    	ExternalInspectorWindow.ShowWindow();
	    }

		///----------------------------------------------------------------------------------------------

		[MenuItem("Tools/ParadoxNotion/NodeCanvas/Welcome Window")]
		static void ShowWelcome(){
			WelcomeWindow.ShowWindow(null);
		}

		[MenuItem("Tools/ParadoxNotion/NodeCanvas/Visit Website")]
		static void VisitWebsite(){
			Help.BrowseURL("http://nodecanvas.paradoxnotion.com");
		}
	}
}

#endif