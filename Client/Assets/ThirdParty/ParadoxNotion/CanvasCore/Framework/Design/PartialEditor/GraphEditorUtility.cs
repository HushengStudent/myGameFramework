#if UNITY_EDITOR

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace NodeCanvas.Editor
{

    public static class GraphEditorUtility
    {

        public static event System.Action<IGraphElement> onActiveElementChanged;
        public static event System.Action PostGUI;
        public static bool allowClick = true;

        //Invoke PostGUI
        public static void InvokePostGUI() {
            if ( PostGUI != null ) {
                PostGUI();
                PostGUI = null;
            }
        }

        ///----------------------------------------------------------------------------------------------

        private static IGraphElement _activeElement;
        private static List<IGraphElement> _activeElements = new List<IGraphElement>();

        ///Selected Node or Connection
        public static IGraphElement activeElement {
            get
            {
                if ( activeElements.Count > 1 ) {
                    return null;
                }
                if ( activeElements.Count == 1 ) {
                    return activeElements[0];
                }
                return _activeElement;
            }
            set
            {
                if ( !activeElements.Contains(value) ) {
                    activeElements.Clear();
                }
                _activeElement = value;
                GUIUtility.keyboardControl = 0;
                UnityEditor.SceneView.RepaintAll(); //for gizmos
                if ( onActiveElementChanged != null ) {
                    onActiveElementChanged(value);
                }
            }
        }

        ///multiple selected Node or Connection
        public static List<IGraphElement> activeElements {
            get { return _activeElements; }
            set
            {
                if ( value != null && value.Count == 1 ) {
                    activeElement = value[0];
                    value.Clear();
                }
                _activeElements = value != null ? value : new List<IGraphElement>();
            }
        }

        ///Selected Node if any
        public static Node activeNode {
            get { return activeElement as Node; }
        }

        ///Selected Connection if any
        public static Connection activeConnection {
            get { return activeElement as Connection; }
        }

        ///----------------------------------------------------------------------------------------------

        ///Returns the extension at which the graph will be saved with if exported to JSON
        public static string GetGraphJSONFileExtension(this Graph graph) {
            return graph.GetType().Name.GetCapitals();
        }

        ///Returns the selected nodes if any or all graph nodes
        public static Node[] GetSelectedOrAll(this Graph graph) {
            if ( activeNode != null ) { return new Node[] { activeNode }; }
            if ( activeElements.Count > 0 ) { return activeElements.OfType<Node>().ToArray(); }
            return graph.allNodes.ToArray();
        }

        ///Make a deep copy of provided graph asset along with it's sub-graphs.
        public static Graph DeepCopy(Graph root) {
            if ( root == null ) {
                return null;
            }

            var path = EditorUtility.SaveFilePanelInProject("Copy of " + root.name, root.name + "_duplicate.asset", "asset", string.Empty);
            if ( string.IsNullOrEmpty(path) ) {
                return null;
            }

            var copy = (Graph)ScriptableObject.CreateInstance(root.GetType());
            AssetDatabase.CreateAsset(copy, path);
            EditorUtility.CopySerialized(root, copy);

            //make use of IGraphAssignable interface to find nodes that represent a sub-graph.
            foreach ( var subGraphNode in copy.allNodes.OfType<IGraphAssignable>() ) {
                if ( subGraphNode.nestedGraph != null ) {
                    //duplicate the existing sub-graph and assign the copy to node.
                    subGraphNode.nestedGraph = DeepCopy(subGraphNode.nestedGraph);
                }
            }

            copy.Validate();
            AssetDatabase.SaveAssets();
            return copy;
        }
    }
}

#endif