#if UNITY_EDITOR

using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEditor;
using UnityEngine;

namespace NodeCanvas.Editor
{

    public class ExternalInspectorWindow : EditorWindow
    {

        private object currentSelection;
        private Vector2 scrollPos;

        public static void ShowWindow() {
            var window = GetWindow(typeof(ExternalInspectorWindow)) as ExternalInspectorWindow;
            window.Show();
            Prefs.useExternalInspector = true;
        }

        void OnEnable() {
            titleContent = new GUIContent("Inspector", StyleSheet.canvasIcon);
        }

        void OnDestroy() {
            Prefs.useExternalInspector = false;
        }

        void Update() {
            if ( currentSelection != GraphEditorUtility.activeElement ) {
                Repaint();
            }
        }

        void OnGUI() {

            if ( EditorGUIUtility.isProSkin ) {
                GUI.Box(new Rect(0, 0, Screen.width, Screen.height), string.Empty, Styles.shadowedBackground);
            }

            if ( GraphEditor.currentGraph == null ) {
                GUILayout.Label("No current NodeCanvas Graph open");
                return;
            }

            if ( EditorApplication.isCompiling ) {
                ShowNotification(new GUIContent("Compiling Please Wait..."));
                return;
            }

            currentSelection = GraphEditorUtility.activeElement;

            if ( currentSelection == null ) {
                GUILayout.Label("No Node Selected in Canvas");
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            if ( currentSelection is Node ) {
                var node = (Node)currentSelection;
                Title(node.name);
                Node.ShowNodeInspectorGUI(node);
            }

            if ( currentSelection is Connection ) {
                Title("Connection");
                Connection.ShowConnectionInspectorGUI(currentSelection as Connection);
            }

            EditorUtils.EndOfInspector();
            GUILayout.EndScrollView();
        }

        void Title(string text) {
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