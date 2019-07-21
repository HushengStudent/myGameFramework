#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace NodeCanvas.Editor
{

    //Events
    partial class GraphEditor
    {

        private static bool mouse2Down;

        ///Graph events BEFORE nodes
        static void HandlePreNodesGraphEvents(Graph graph, Vector2 canvasMousePos) {

            if ( e.button == 2 && ( e.type == EventType.MouseDown || e.type == EventType.MouseUp ) ) {
                Undo.RecordObject(graph, "Graph Pan");
            }

            if ( e.type == EventType.MouseUp || e.type == EventType.KeyUp ) {
                SnapNodesToGridAndPixel(graph);
            }

            if ( e.type == EventType.KeyDown && e.keyCode == KeyCode.F && GUIUtility.keyboardControl == 0 ) {
                FocusSelection();
            }

            if ( e.type == EventType.MouseDown && e.button == 2 && e.clickCount == 2 ) {
                FocusPosition(ViewToCanvas(e.mousePosition));
            }

            if ( e.type == EventType.ScrollWheel && GraphEditorUtility.allowClick ) {
                if ( canvasRect.Contains(e.mousePosition) ) {
                    var zoomDelta = e.shift ? 0.1f : 0.25f;
                    ZoomAt(e.mousePosition, -e.delta.y > 0 ? zoomDelta : -zoomDelta);
                }
            }

            if ( e.type == EventType.MouseDrag && e.alt && e.button == 1 ) {
                ZoomAt(new Vector2(screenWidth / 2, screenHeight / 2), e.delta.x / 100);
                e.Use();
            }

            if ( ( e.button == 2 && e.type == EventType.MouseDrag && canvasRect.Contains(e.mousePosition) ) ||
                ( ( e.type == EventType.MouseDown || e.type == EventType.MouseDrag ) && e.alt && e.isMouse ) ) {
                pan += e.delta;
                smoothPan = null;
                smoothZoomFactor = null;
                e.Use();
            }

            if ( e.type == EventType.MouseDown && e.button == 2 ) { mouse2Down = true; }
            if ( e.type == EventType.MouseUp && e.button == 2 ) { mouse2Down = false; }
            if ( e.alt || mouse2Down ) {
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, screenWidth, screenHeight), MouseCursor.Pan);
            }
        }

        ///----------------------------------------------------------------------------------------------

        ///Graph events AFTER nodes
        static void HandlePostNodesGraphEvents(Graph graph, Vector2 canvasMousePos) {

            //Shortcuts
            if ( GUIUtility.keyboardControl == 0 ) {

                //Copy/Cut/Paste
                if ( e.type == EventType.ValidateCommand /*|| e.type == EventType.Used*/) {
                    if ( e.commandName == "Copy" || e.commandName == "Cut" ) {
                        List<Node> selection = null;
                        if ( GraphEditorUtility.activeNode != null ) {
                            selection = new List<Node> { GraphEditorUtility.activeNode };
                        }
                        if ( GraphEditorUtility.activeElements != null && GraphEditorUtility.activeElements.Count > 0 ) {
                            selection = GraphEditorUtility.activeElements.Cast<Node>().ToList();
                        }
                        if ( selection != null ) {
                            CopyBuffer.Set<Node[]>(Graph.CloneNodes(selection).ToArray());
                            if ( e.commandName == "Cut" ) {
                                foreach ( Node node in selection ) { graph.RemoveNode(node); }
                            }
                        }
                        e.Use();
                    }
                    if ( e.commandName == "Paste" ) {
                        if ( CopyBuffer.Has<Node[]>() ) {
                            TryPasteNodesInGraph(graph, CopyBuffer.Get<Node[]>(), canvasMousePos + new Vector2(500, 500) / graph.zoomFactor);
                        }
                        e.Use();
                    }
                }

                if ( e.type == EventType.KeyUp ) {

                    //Delete
                    if ( e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace ) {

                        if ( GraphEditorUtility.activeElements != null && GraphEditorUtility.activeElements.Count > 0 ) {
                            foreach ( var obj in GraphEditorUtility.activeElements.ToArray() ) {
                                if ( obj is Node ) {
                                    graph.RemoveNode(obj as Node);
                                }
                                if ( obj is Connection ) {
                                    graph.RemoveConnection(obj as Connection);
                                }
                            }
                            GraphEditorUtility.activeElements = null;
                        }

                        if ( GraphEditorUtility.activeNode != null ) {
                            graph.RemoveNode(GraphEditorUtility.activeNode);
                            GraphEditorUtility.activeElement = null;
                        }

                        if ( GraphEditorUtility.activeConnection != null ) {
                            graph.RemoveConnection(GraphEditorUtility.activeConnection);
                            GraphEditorUtility.activeElement = null;
                        }
                        e.Use();
                    }

                    //Duplicate
                    if ( e.keyCode == KeyCode.D && e.control ) {
                        if ( GraphEditorUtility.activeElements != null && GraphEditorUtility.activeElements.Count > 0 ) {
                            TryPasteNodesInGraph(graph, GraphEditorUtility.activeElements.OfType<Node>().ToArray(), default(Vector2));
                        }
                        if ( GraphEditorUtility.activeNode != null ) {
                            GraphEditorUtility.activeElement = GraphEditorUtility.activeNode.Duplicate(graph);
                        }
                        //Connections can't be duplicated by themselves. They do so as part of multiple node duplication.
                        e.Use();
                    }
                }
            }

            //No panel is obscuring
            if ( GraphEditorUtility.allowClick ) {
                //'Tilt' or 'Space' keys, opens up the complete context menu browser
                if ( e.type == EventType.KeyDown && !e.shift && ( e.keyCode == KeyCode.BackQuote || e.keyCode == KeyCode.Space ) ) {
                    if ( GUIUtility.keyboardControl == 0 ) {
                        GenericMenuBrowser.Show(GetAddNodeMenu(graph, canvasMousePos), e.mousePosition, string.Format("Add {0} Node", graph.GetType().FriendlyName()), graph.baseNodeType);
                        e.Use();
                    }
                }

                //Right click canvas context menu. Basicaly for adding new nodes.
                if ( e.type == EventType.ContextClick && !e.alt ) {
                    var menu = GetAddNodeMenu(graph, canvasMousePos);
                    if ( CopyBuffer.Has<Node[]>() && CopyBuffer.Peek<Node[]>()[0].GetType().IsSubclassOf(graph.baseNodeType) ) {
                        menu.AddSeparator("/");
                        var copiedNodes = CopyBuffer.Get<Node[]>();
                        if ( copiedNodes.Length == 1 ) {
                            menu.AddItem(new GUIContent(string.Format("Paste Node ({0})", copiedNodes[0].GetType().FriendlyName())), false, () => { TryPasteNodesInGraph(graph, copiedNodes, canvasMousePos); });
                        } else if ( copiedNodes.Length > 1 ) {
                            menu.AddItem(new GUIContent(string.Format("Paste Nodes ({0})", copiedNodes.Length.ToString())), false, () => { TryPasteNodesInGraph(graph, copiedNodes, canvasMousePos); });
                        }
                    }

                    menu.ShowAsBrowser(e.mousePosition, string.Format("Add {0} Node", graph.GetType().FriendlyName()), graph.baseNodeType);
                    e.Use();
                }
            }
        }

        ///----------------------------------------------------------------------------------------------

        //Paste nodes in this graph
        static void TryPasteNodesInGraph(Graph graph, Node[] nodes, Vector2 originPosition) {
            var newNodes = Graph.CloneNodes(nodes.ToList(), graph, originPosition);
            GraphEditorUtility.activeElements = newNodes.Cast<IGraphElement>().ToList();
        }

        ///The final generic menu used for adding nodes in the canvas
        static GenericMenu GetAddNodeMenu(Graph graph, Vector2 canvasMousePos) {
            System.Action<System.Type> Selected = (type) => { GraphEditorUtility.activeElement = graph.AddNode(type, canvasMousePos); };
            var menu = EditorUtils.GetTypeSelectionMenu(graph.baseNodeType, Selected);
            menu = graph.CallbackOnCanvasContextMenu(menu, canvasMousePos);
            return menu;
        }
    }
}

#endif