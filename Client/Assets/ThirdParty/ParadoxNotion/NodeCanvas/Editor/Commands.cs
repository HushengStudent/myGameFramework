#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace NodeCanvas.Editor
{

    public static class Commands
    {

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/New Task")]
        [MenuItem("Assets/Create/ParadoxNotion/NodeCanvas/New Task")]
        public static void ShowTaskWizard() {
            TaskWizardWindow.ShowWindow();
        }

        ///----------------------------------------------------------------------------------------------

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/Scene Global Blackboard")]
        public static void CreateGlobalBlackboard() {
            Selection.activeObject = GlobalBlackboard.Create();
        }

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Preferred Types Editor")]
        public static void ShowPrefTypes() {
            TypePrefsEditorWindow.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Graph Debug Console")]
        public static void OpenConsole() {
            GraphConsole.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Graph Element Finder")]
        public static void OpenFinder() {
            GraphFinder.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/External Inspector Panel")]
        public static void ShowExternalInspector() {
            ExternalInspectorWindow.ShowWindow();
        }

        ///----------------------------------------------------------------------------------------------

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Welcome Window")]
        public static void ShowWelcome() {
            WelcomeWindow.ShowWindow(typeof(NodeCanvas.BehaviourTrees.BehaviourTree));
        }

        [MenuItem("Tools/ParadoxNotion/NodeCanvas/Visit Website")]
        public static void VisitWebsite() {
            Help.BrowseURL("http://nodecanvas.paradoxnotion.com");
        }
    }
}

#endif