#if UNITY_EDITOR

using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace NodeCanvas.Editor
{

    //Panels
    partial class GraphEditor
    {

        private static float inspectorPanelHeight;
        private static float blackboardPanelHeight;
        private static bool isResizingInspectorPanel;
        private static bool isResizingBlackboardPanel;
        private static Vector2 inspectorPanelScrollPos;
        private static Vector2 blackboardPanelScrollPos;

        //This is called outside of windows
        static void ShowPanels(Graph graph, Vector2 canvasMousePos) {
            ShowGraphCommentsGUI(graph, canvasMousePos);
            var panel1 = ShowInspectorGUIPanel(graph, canvasMousePos).ExpandBy(14);
            var panel2 = ShowBlackboardGUIPanel(graph, canvasMousePos).ExpandBy(14);
            GraphEditorUtility.allowClick = !panel1.Contains(e.mousePosition) && !panel2.Contains(e.mousePosition);
        }

        //Show the comments window
        static void ShowGraphCommentsGUI(Graph graph, Vector2 canvasMousePos) {
            if ( Prefs.showComments && !string.IsNullOrEmpty(graph.comments) ) {
                GUI.backgroundColor = new Color(1f, 1f, 1f, 0.3f);
                GUI.Box(new Rect(10, screenHeight - 100, 330, 70), graph.comments, StyleSheet.commentsBox);
                GUI.backgroundColor = Color.white;
            }
        }

        //This is the window shown at the top left with a GUI for extra editing opions of the selected node.
        static Rect ShowInspectorGUIPanel(Graph graph, Vector2 canvasMousePos) {
            var inspectorPanel = default(Rect);
            if ( ( GraphEditorUtility.activeNode == null && GraphEditorUtility.activeConnection == null ) || Prefs.useExternalInspector ) {
                return inspectorPanel;
            }

            inspectorPanel.x = 10;
            inspectorPanel.y = 30;
            inspectorPanel.width = Prefs.inspectorPanelWidth;
            inspectorPanel.height = inspectorPanelHeight;

            var resizeRect = Rect.MinMaxRect(inspectorPanel.xMax - 2, inspectorPanel.yMin, inspectorPanel.xMax + 2, inspectorPanel.yMax);
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeHorizontal);
            if ( e.type == EventType.MouseDown && resizeRect.Contains(e.mousePosition) ) { isResizingInspectorPanel = true; e.Use(); }
            if ( isResizingInspectorPanel && e.type == EventType.Layout ) { Prefs.inspectorPanelWidth += e.delta.x; }
            if ( e.rawType == EventType.MouseUp ) { isResizingInspectorPanel = false; }

            var headerRect = new Rect(inspectorPanel.x, inspectorPanel.y, inspectorPanel.width, 30);
            EditorGUIUtility.AddCursorRect(headerRect, MouseCursor.Link);
            if ( GUI.Button(headerRect, string.Empty, StyleSheet.button) ) {
                Prefs.showNodePanel = !Prefs.showNodePanel;
            }

            GUI.Box(inspectorPanel, string.Empty, StyleSheet.windowShadow);
            var title = GraphEditorUtility.activeNode != null ? GraphEditorUtility.activeNode.name : "Connection";
            if ( Prefs.showNodePanel ) {

                var viewRect = new Rect(inspectorPanel.x, inspectorPanel.y, inspectorPanel.width + 18, screenHeight - inspectorPanel.y - 30);
                inspectorPanelScrollPos = GUI.BeginScrollView(viewRect, inspectorPanelScrollPos, inspectorPanel);

                GUILayout.BeginArea(inspectorPanel, title, StyleSheet.editorPanel);
                GUILayout.Space(5);

                if ( GraphEditorUtility.activeNode != null ) {
                    Node.ShowNodeInspectorGUI(GraphEditorUtility.activeNode);
                } else if ( GraphEditorUtility.activeConnection != null ) {
                    Connection.ShowConnectionInspectorGUI(GraphEditorUtility.activeConnection);
                }

                EditorUtils.EndOfInspector();
                if ( e.type == EventType.Repaint ) {
                    inspectorPanelHeight = GUILayoutUtility.GetLastRect().yMax + 5;
                }

                GUILayout.EndArea();
                GUI.EndScrollView();

                if ( GUI.changed ) {
                    EditorUtility.SetDirty(graph);
                }

            } else {

                inspectorPanelHeight = 55;
                GUILayout.BeginArea(inspectorPanel, title, StyleSheet.editorPanel);
                GUI.color = new Color(1, 1, 1, 0.2f);
                if ( GUILayout.Button("...", StyleSheet.button) ) {
                    Prefs.showNodePanel = true;
                }
                GUILayout.EndArea();
                GUI.color = Color.white;
            }

            return inspectorPanel;
        }


        //Show the target blackboard window
        static Rect ShowBlackboardGUIPanel(Graph graph, Vector2 canvasMousePos) {
            var blackboardPanel = default(Rect);
            if ( graph.blackboard == null ) {
                return blackboardPanel;
            }

            blackboardPanel.xMin = screenWidth - Prefs.blackboardPanelWidth;
            blackboardPanel.yMin = 30;
            blackboardPanel.xMax = screenWidth - 20;
            blackboardPanel.height = blackboardPanelHeight;

            var resizeRect = Rect.MinMaxRect(blackboardPanel.xMin - 2, blackboardPanel.yMin, blackboardPanel.xMin + 2, blackboardPanel.yMax);
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeHorizontal);
            if ( e.type == EventType.MouseDown && resizeRect.Contains(e.mousePosition) ) { isResizingBlackboardPanel = true; e.Use(); }
            if ( isResizingBlackboardPanel && e.type == EventType.Layout ) { Prefs.blackboardPanelWidth -= e.delta.x; }
            if ( e.rawType == EventType.MouseUp ) { isResizingBlackboardPanel = false; }

            var headerRect = new Rect(blackboardPanel.x, blackboardPanel.y, blackboardPanel.width, 30);
            EditorGUIUtility.AddCursorRect(headerRect, MouseCursor.Link);
            if ( GUI.Button(headerRect, string.Empty, StyleSheet.button) ) {
                Prefs.showBlackboard = !Prefs.showBlackboard;
            }

            GUI.Box(blackboardPanel, string.Empty, StyleSheet.windowShadow);
            var title = graph.blackboard == graph.localBlackboard ? string.Format("Local {0} Variables", graph.GetType().Name) : "Variables";
            if ( Prefs.showBlackboard ) {

                var viewRect = new Rect(blackboardPanel.x, blackboardPanel.y, blackboardPanel.width + 16, screenHeight - blackboardPanel.y - 30);
                var r = new Rect(blackboardPanel.x - 3, blackboardPanel.y, blackboardPanel.width, blackboardPanel.height);
                blackboardPanelScrollPos = GUI.BeginScrollView(viewRect, blackboardPanelScrollPos, r);

                GUILayout.BeginArea(blackboardPanel, title, StyleSheet.editorPanel);
                GUILayout.Space(5);

                BlackboardEditor.ShowVariables(graph.blackboard, graph.blackboard == graph.localBlackboard ? graph : graph.blackboard as Object);
                EditorUtils.EndOfInspector();
                if ( e.type == EventType.Repaint ) {
                    blackboardPanelHeight = GUILayoutUtility.GetLastRect().yMax + 5;
                }
                GUILayout.EndArea();
                GUI.EndScrollView();

            } else {

                blackboardPanelHeight = 55;
                GUILayout.BeginArea(blackboardPanel, title, StyleSheet.editorPanel);
                GUI.color = new Color(1, 1, 1, 0.2f);
                if ( GUILayout.Button("...", StyleSheet.button) ) {
                    Prefs.showBlackboard = true;
                }
                GUILayout.EndArea();
                GUI.color = Color.white;
            }

            if ( graph.canAcceptVariableDrops && BlackboardEditor.pickedVariable != null && BlackboardEditor.pickedVariableBlackboard == graph.blackboard ) {
                GUI.Label(new Rect(e.mousePosition.x + 15, e.mousePosition.y, 100, 18), "Drop Variable", StyleSheet.labelOnCanvas);
                if ( e.type == EventType.MouseUp && !blackboardPanel.Contains(e.mousePosition) ) {
                    graph.CallbackOnVariableDropInGraph(BlackboardEditor.pickedVariable, canvasMousePos);
                    BlackboardEditor.ResetPick();
                }
            }

            return blackboardPanel;
        }

    }
}

#endif