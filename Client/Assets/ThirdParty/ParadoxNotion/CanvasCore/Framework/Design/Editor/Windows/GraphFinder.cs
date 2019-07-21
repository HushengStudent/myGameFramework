#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace NodeCanvas.Editor
{

    public class GraphFinder : EditorWindow
    {

        private static HierarchyTree.Element graphElement;
        private static HierarchyTree.Element lastHoverElement;
        private static string search;
        private static Vector2 scrollPos;
        private static bool willRepaint;
        private static int indent;
        const int INDENT_WIDTH = 25;
        const int INDENT_START = 1;

        ///----------------------------------------------------------------------------------------------

        ///Show the finder window
        public static void ShowWindow() {
            GetWindow<GraphFinder>().Show();
        }

        //...
        void OnEnable() {
            titleContent = new GUIContent("Finder", StyleSheet.canvasIcon);

            wantsMouseMove = true;
            wantsMouseEnterLeaveWindow = true;
            GraphEditor.onCurrentGraphChanged -= GraphChanged;
            GraphEditor.onCurrentGraphChanged += GraphChanged;
            Graph.onGraphSerialized -= OnGraphSerialized;
            Graph.onGraphSerialized += OnGraphSerialized;
            GraphEditorUtility.onActiveElementChanged -= OnActiveElementChanged;
            GraphEditorUtility.onActiveElementChanged += OnActiveElementChanged;

            graphElement = null;
            search = null;
            lastHoverElement = null;
            scrollPos = default(Vector2);
            willRepaint = true;

            if ( GraphEditor.currentGraph != null ) {
                FetchElements(GraphEditor.currentGraph);
            }
        }

        //...
        void OnDisable() {
            GraphEditor.onCurrentGraphChanged -= GraphChanged;
            Graph.onGraphSerialized -= OnGraphSerialized;
            GraphEditorUtility.onActiveElementChanged -= OnActiveElementChanged;
        }

        //...
        void GraphChanged(Graph graph) {
            FetchElements(graph);
            search = null;
            GUIUtility.hotControl = 0;
            GUIUtility.keyboardControl = 0;
        }

        //...
        void OnGraphSerialized(Graph graph) {
            if ( GraphEditor.current != null && GraphEditor.currentGraph == graph ) {
                FetchElements(graph);
            }
        }

        //...
        void OnActiveElementChanged(IGraphElement element) {
            willRepaint = true;
        }

        ///----------------------------------------------------------------------------------------------

        //Initialize the graph elements
        void FetchElements(Graph graph) {
            graphElement = graph.GetFlatGraphHierarchy();
            willRepaint = true;
        }

        //...
        void Update() {
            if ( willRepaint ) {
                willRepaint = false;
                Repaint();
            }
        }

        //...
        void OnGUI() {
            if ( EditorGUIUtility.isProSkin ) {
                GUI.Box(new Rect(0, 0, Screen.width, Screen.height), string.Empty, Styles.shadowedBackground);
            }

            if ( GraphEditor.current == null ) {
                ShowNotification(new GUIContent("No Graph is currently open in the Graph Editor"));
                return;
            } else {
                RemoveNotification();
            }

            EditorGUILayout.HelpBox("A 'flat' structure of the graph, including nodes, connections, tasks and parameters.\nThis is not a hierarchical tree representation of the graph.\nUse this utility window to quickly search, find and jump focus to the related element.", MessageType.Info);

            GUILayout.BeginHorizontal();
            search = EditorUtils.SearchField(search);
            Prefs.finderShowTypeNames = EditorGUILayout.ToggleLeft("Show Type Names", Prefs.finderShowTypeNames, GUILayout.Width(130));
            GUILayout.EndHorizontal();

            if ( graphElement == null ) {
                return;
            }

            EditorUtils.BoldSeparator();

            GUILayout.Label(string.Format("<size=12><b> ROOT</b></size>", GraphEditor.currentGraph.name));

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            indent = INDENT_START;
            DoElement(graphElement);
            indent = INDENT_START;
            EditorGUILayout.EndScrollView();

            if ( Event.current.type == EventType.MouseLeaveWindow ) {
                willRepaint = true;
            }
        }

        //...
        void DoElement(HierarchyTree.Element element, Rect parentElementRect = default(Rect)) {

            if ( element.children == null ) { return; }


            foreach ( var child in element.children ) {

                var elementRect = default(Rect);

                if ( child.reference == null ) { continue; }

                //Dont show undefined parameters.
                //TODO: I dont like this "special case" here
                if ( child.reference is BBParameter ) {
                    var bbPram = (BBParameter)child.reference;
                    if ( !bbPram.isDefined ) { continue; }
                }

                var toString = child.reference.ToString();
                var typeName = child.reference.GetType().FriendlyName();
                var searchText = toString + " " + typeName;

                if ( string.IsNullOrEmpty(search) || StringUtils.SearchMatch(search, searchText) ) {

                    GUI.color = EditorGUIUtility.isProSkin ? Color.white : Colors.Grey(0.8f);
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Space(indent * INDENT_WIDTH);
                    GUI.color = Color.white;


                    var displayText = string.Format("<b>{0}</b>{1}", toString, Prefs.finderShowTypeNames ? " (" + typeName + ")" : string.Empty);
                    GUILayout.Label(string.Format("<size=9>{0}</size>", displayText));
                    GUILayout.EndHorizontal();

                    elementRect = GUILayoutUtility.GetLastRect();

                    EditorGUIUtility.AddCursorRect(elementRect, MouseCursor.Link);
                    if ( elementRect.Contains(Event.current.mousePosition) ) {
                        if ( child != lastHoverElement ) {
                            lastHoverElement = child;
                            willRepaint = true;
                            PingElement(child);
                        }
                        GUI.color = new Color(0.5f, 0.5f, 1, 0.3f);
                        GUI.DrawTexture(elementRect, EditorGUIUtility.whiteTexture);
                        GUI.color = Color.white;
                        if ( Event.current.type == EventType.MouseDown ) {
                            FocusElement(child);
                            Event.current.Use();
                        }
                    }

                    if ( GraphEditorUtility.activeElement == child.reference ) {
                        GUI.color = new Color(0.5f, 0.5f, 1, 0.1f);
                        GUI.DrawTexture(elementRect, EditorGUIUtility.whiteTexture);
                        GUI.color = Color.white;
                    }
                }

                indent++;
                DoElement(child, elementRect);
                indent--;

                if ( elementRect != default(Rect) ) {
                    var rootOrNotParentHidden = indent == INDENT_START || parentElementRect != default(Rect);

                    var lineVer = new Rect();
                    lineVer.xMin = elementRect.xMin + ( indent * INDENT_WIDTH ) - ( INDENT_WIDTH / 2 );
                    lineVer.width = 2;
                    lineVer.yMin = parentElementRect.yMax + 6;
                    lineVer.yMax = elementRect.yMax - ( elementRect.height / 2 );

                    var lineHor = new Rect();
                    lineHor.xMin = rootOrNotParentHidden ? lineVer.xMin : ( INDENT_WIDTH / 2 );
                    lineHor.xMax = lineVer.xMin + ( INDENT_WIDTH / 2 );
                    lineHor.yMin = lineVer.yMax - 2;
                    lineHor.height = 2;

                    GUI.color = Colors.Grey(EditorGUIUtility.isProSkin ? 0.6f : 0.3f);
                    if ( rootOrNotParentHidden ) {
                        GUI.DrawTexture(lineVer, Texture2D.whiteTexture);
                    }
                    GUI.DrawTexture(lineHor, Texture2D.whiteTexture);
                    GUI.color = Color.white;
                }


                if ( indent == INDENT_START && string.IsNullOrEmpty(search) ) {
                    EditorUtils.Separator();
                }
            }
        }

        ///Ping element. User hover.
        void PingElement(HierarchyTree.Element e) {
            var element = e.GetFirstParentReferenceOfType<IGraphElement>();
            EditorApplication.delayCall += () => GraphEditor.PingElement(element);
        }

        ///Focus element. This also Pings it. User click.
        void FocusElement(HierarchyTree.Element e) {
            var element = e.GetFirstParentReferenceOfType<IGraphElement>();
            EditorApplication.delayCall += () => GraphEditor.FocusElement(element, true);
        }
    }
}

#endif