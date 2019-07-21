#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using System.Linq;

namespace NodeCanvas.Editor
{

    ///Toolbar
    public partial class GraphEditor
    {

        //This is called outside Begin/End Windows from GraphEditor.
        public static void ShowToolbar(Graph graph) {

            var owner = graph.agent != null && graph.agent is GraphOwner && ( graph.agent as GraphOwner ).graph == graph ? (GraphOwner)graph.agent : null;

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUI.backgroundColor = Color.white.WithAlpha(0.5f);

            ///----------------------------------------------------------------------------------------------
            ///Left side
            ///----------------------------------------------------------------------------------------------

            if ( GUILayout.Button("File", EditorStyles.toolbarDropDown, GUILayout.Width(50)) ) {
                GetToolbarMenu_File(graph, owner).ShowAsContext();
            }

            if ( GUILayout.Button("Edit", EditorStyles.toolbarDropDown, GUILayout.Width(50)) ) {
                GetToolbarMenu_Edit(graph, owner).ShowAsContext();
            }

            if ( GUILayout.Button("Prefs", EditorStyles.toolbarDropDown, GUILayout.Width(50)) ) {
                GetToolbarMenu_Prefs(graph, owner).ShowAsContext();
            }

            // var customMenu = GetToolbarMenu_Custom(graph, owner);
            // if ( customMenu.GetItemCount() > 0 ) {
            //     if ( GUILayout.Button("More", EditorStyles.toolbarDropDown, GUILayout.Width(50)) ) {
            //         customMenu.ShowAsContext();
            //     }
            // }

            GUILayout.Space(10);

            if ( owner != null && GUILayout.Button("Select Owner", EditorStyles.toolbarButton, GUILayout.Width(80)) ) {
                Selection.activeObject = owner;
                EditorGUIUtility.PingObject(owner);
            }

            if ( EditorUtility.IsPersistent(graph) && GUILayout.Button("Select Graph", EditorStyles.toolbarButton, GUILayout.Width(80)) ) {
                Selection.activeObject = graph;
                EditorGUIUtility.PingObject(graph);
            }

            GUILayout.Space(10);

            if ( GUILayout.Button(new GUIContent(StyleSheet.log, "Open Graph Console"), EditorStyles.toolbarButton, GUILayout.MaxHeight(12)) ) {
                GraphConsole.ShowWindow();
            }

            if ( GUILayout.Button(new GUIContent(StyleSheet.lens, "Open Graph Finder"), EditorStyles.toolbarButton, GUILayout.MaxHeight(12)) ) {
                GraphFinder.ShowWindow();
            }

            GUILayout.Space(10);

            ///----------------------------------------------------------------------------------------------

            graph.CallbackOnGraphEditorToolbar();

            ///----------------------------------------------------------------------------------------------
            ///Mid
            ///----------------------------------------------------------------------------------------------

            GUILayout.Space(10);
            GUILayout.FlexibleSpace();

            //...

            //...

            GUILayout.FlexibleSpace();
            GUILayout.Space(10);

            ///----------------------------------------------------------------------------------------------
            ///Right side
            ///----------------------------------------------------------------------------------------------

            GUI.backgroundColor = Color.clear;
            GUI.color = new Color(1, 1, 1, 0.3f);
            GUILayout.Label(string.Format("{0} @NodeCanvas Framework v{1}", graph.GetType().Name, NodeCanvas.Framework.Internal.GraphSerializationData.FRAMEWORK_VERSION), EditorStyles.toolbarButton);
            GUILayout.Space(10);
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;

            //GRAPHOWNER JUMP SELECTION
            if ( owner != null ) {
                if ( GUILayout.Button(string.Format("[{0}]", owner.gameObject.name), EditorStyles.toolbarDropDown, GUILayout.Width(120)) ) {
                    var menu = new GenericMenu();
                    foreach ( var _o in Object.FindObjectsOfType<GraphOwner>().OrderBy(x => x.gameObject != owner.gameObject) ) {
                        var o = _o;
                        menu.AddItem(new GUIContent(o.gameObject.name + "/" + o.GetType().Name), o == owner, () => { SetReferences(o); });
                    }
                    menu.ShowAsContext();
                }
            }

            Prefs.isEditorLocked = GUILayout.Toggle(Prefs.isEditorLocked, "Lock", EditorStyles.toolbarButton);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;
        }

        ///----------------------------------------------------------------------------------------------

        //FILE MENU
        static GenericMenu GetToolbarMenu_File(Graph graph, GraphOwner owner) {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Clear"), false, () =>
           {
               if ( EditorUtility.DisplayDialog("Clear Canvas", "This will delete all nodes of the currently viewing graph!\nAre you sure?", "YES", "NO!") ) {
                   graph.ClearGraph();
               }
           });

            menu.AddItem(new GUIContent("Import JSON"), false, () =>
          {
              if ( graph.allNodes.Count > 0 && !EditorUtility.DisplayDialog("Import Graph", "All current graph information will be lost. Are you sure?", "YES", "NO") ) {
                  return;
              }

              var path = EditorUtility.OpenFilePanel(string.Format("Import '{0}' Graph", graph.GetType().Name), "Assets", graph.GetGraphJSONFileExtension());
              if ( !string.IsNullOrEmpty(path) ) {
                  if ( graph.Deserialize(System.IO.File.ReadAllText(path), true, null) == null ) { //true: validate, null: graph._objectReferences
                      EditorUtility.DisplayDialog("Import Failure", "Please read the logs for more information", "OK", string.Empty);
                  }
              }
          });

            menu.AddItem(new GUIContent("Export JSON"), false, () =>
          {
              var path = EditorUtility.SaveFilePanelInProject(string.Format("Export '{0}' Graph", graph.GetType().Name), string.Empty, graph.GetGraphJSONFileExtension(), string.Empty);
              if ( !string.IsNullOrEmpty(path) ) {
                  System.IO.File.WriteAllText(path, graph.Serialize(true, null)); //true: pretyJson, null: graph._objectReferences
                  AssetDatabase.Refresh();
              }
          });

            menu.AddItem(new GUIContent("Show JSON"), false, () =>
           {
               ParadoxNotion.Serialization.JSONSerializer.ShowData(graph.Serialize(true, null), graph.name);
           });

            return menu;
        }

        ///----------------------------------------------------------------------------------------------

        //EDIT MENU
        static GenericMenu GetToolbarMenu_Edit(Graph graph, GraphOwner owner) {
            var menu = new GenericMenu();
            //Bind
            if ( !Application.isPlaying && owner != null && !owner.graphIsBound ) {
                menu.AddItem(new GUIContent("Bind To Owner"), false, () =>
                {
                    if ( EditorUtility.DisplayDialog("Bind Graph", "This will make a local copy of the graph, bound to the owner.\n\nThis allows you to make local changes and assign scene object references directly.\n\nNote that you can also use scene object references through the use of Blackboard Variables.\n\nBind Graph?", "YES", "NO") ) {
                        Undo.RecordObject(owner, "New Local Graph");
                        owner.SetBoundGraphReference(owner.graph);
                        EditorUtility.SetDirty(owner);
                    }
                });
            } else menu.AddDisabledItem(new GUIContent("Bind To Owner"));

            //Save to asset
            if ( owner != null && owner.graphIsBound ) {
                menu.AddItem(new GUIContent("Save To Asset"), false, () =>
                {
                    var newGraph = (Graph)EditorUtils.CreateAsset(graph.GetType());
                    if ( newGraph != null ) {
                        EditorUtility.CopySerialized(graph, newGraph);
                        newGraph.Validate();
                        AssetDatabase.SaveAssets();
                    }
                });
            } else menu.AddDisabledItem(new GUIContent("Save To Asset"));

            //Create defined vars
            if ( graph.blackboard != null ) {
                menu.AddItem(new GUIContent("Promote Defined Parameters To Variables"), false, () =>
                {
                    if ( EditorUtility.DisplayDialog("Promote Defined Parameters", "This will fill the current Blackboard with a Variable for each defined Parameter in the graph.\nContinue?", "YES", "NO") ) {
                        graph.PromoteDefinedParametersToVariables(graph.blackboard);
                    }
                });
            } else menu.AddDisabledItem(new GUIContent("Promote Defined Parameters To Variables"));

            // menu.AddItem(new GUIContent("Force Deserialize"), false, () =>
            // {
            //     graph.Deserialize();
            //     graph.Validate();
            //     GraphEditorUtility.activeElement = null;
            // });

            return menu;
        }

        ///----------------------------------------------------------------------------------------------

        //PREFS MENU
        static GenericMenu GetToolbarMenu_Prefs(Graph graph, GraphOwner owner) {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Show Icons"), Prefs.showIcons, () => { Prefs.showIcons = !Prefs.showIcons; });
            menu.AddItem(new GUIContent("Show Node Help"), Prefs.showNodeInfo, () => { Prefs.showNodeInfo = !Prefs.showNodeInfo; });
            menu.AddItem(new GUIContent("Show Comments"), Prefs.showComments, () => { Prefs.showComments = !Prefs.showComments; });
            menu.AddItem(new GUIContent("Show Summary Info"), Prefs.showTaskSummary, () => { Prefs.showTaskSummary = !Prefs.showTaskSummary; });
            menu.AddItem(new GUIContent("Show Node IDs"), Prefs.showNodeIDs, () => { Prefs.showNodeIDs = !Prefs.showNodeIDs; });
            menu.AddItem(new GUIContent("Grid Snap"), Prefs.doSnap, () => { Prefs.doSnap = !Prefs.doSnap; });
            menu.AddItem(new GUIContent("Log Events"), Prefs.logEvents, () => { Prefs.logEvents = !Prefs.logEvents; });
            menu.AddItem(new GUIContent("Log Dynamic Parameters Info"), Prefs.logDynamicParametersInfo, () => { Prefs.logDynamicParametersInfo = !Prefs.logDynamicParametersInfo; });
            menu.AddItem(new GUIContent("Breakpoints Pause Editor"), Prefs.breakpointPauseEditor, () => { Prefs.breakpointPauseEditor = !Prefs.breakpointPauseEditor; });
            menu.AddItem(new GUIContent("Show Hierarchy Icons"), Prefs.showHierarchyIcons, () => { Prefs.showHierarchyIcons = !Prefs.showHierarchyIcons; });
            if ( graph.isTree ) {
                menu.AddItem(new GUIContent("Automatic Hierarchical Move"), Prefs.hierarchicalMove, () => { Prefs.hierarchicalMove = !Prefs.hierarchicalMove; });
            }
            menu.AddItem(new GUIContent("Open Preferred Types Editor..."), false, () => { TypePrefsEditorWindow.ShowWindow(); });
            return menu;
        }

        /*
        //CUSTOM MENU
        static GenericMenu GetToolbarMenu_Custom(Graph graph, GraphOwner owner) {
            var menu = new GenericMenu();
            var methods = graph.GetType().RTGetMethods();
            for ( var i = 0; i < methods.Length; i++ ) {
                var method = methods[i];
                if ( method.GetParameters().Length == 0 ) {
                    var menuAtt = method.RTGetAttribute<ToolbarMenuItemAttribute>(true);
                    if ( menuAtt != null ) {
                        var instance = method.IsStatic ? null : graph;
                        menu.AddItem(new GUIContent(menuAtt.path), false, () => { method.Invoke(instance, null); });
                    }
                }

            }
            return menu;
        }
        */

    }
}

#endif