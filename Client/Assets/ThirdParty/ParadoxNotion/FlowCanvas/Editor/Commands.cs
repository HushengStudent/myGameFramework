#if UNITY_EDITOR

using UnityEditor;
using ParadoxNotion.Design;
using NodeCanvas.Editor;
using NodeCanvas.Framework;

namespace FlowCanvas.Editor
{

    public static class Commands
    {

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Create/Scene Global Blackboard")]
        public static void CreateGlobalBlackboard() {
            Selection.activeObject = GlobalBlackboard.Create();
        }

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Preferred Types Editor")]
        public static void ShowPrefTypes() {
            TypePrefsEditorWindow.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Graph Debug Console")]
        public static void OpenConsole() {
            GraphConsole.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Graph Element Finder")]
        public static void OpenFinder() {
            GraphFinder.ShowWindow();
        }

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/External Inspector Panel")]
        public static void ShowExternalInspector() {
            ExternalInspectorWindow.ShowWindow();
        }


        ///----------------------------------------------------------------------------------------------

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Welcome Window")]
        public static void ShowWelcome() {
            WelcomeWindow.ShowWindow(typeof(FlowScript));
        }

        [MenuItem("Tools/ParadoxNotion/FlowCanvas/Visit Website")]
        public static void VisitWebsite() {
            Help.BrowseURL("http://flowcanvas.paradoxnotion.com");
        }
    }
}

#endif