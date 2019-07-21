#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;


namespace NodeCanvas.Editor
{

    [CustomEditor(typeof(Graph), true)]
    public class GraphInspector : UnityEditor.Editor
    {

        private Graph graph {
            get { return target as Graph; }
        }

        private string fileExtension {
            get { return graph.GetType().Name.GetCapitals(); }
        }

        public override void OnInspectorGUI() {

            UndoManager.CheckUndo(this, "Graph Inspector");

            GUI.skin.label.richText = true;
            ShowBasicGUI();
            ShowDefinedParametersGUI();

            UndoManager.CheckDirty(this);
        }


        //name, description, edit button
        public void ShowBasicGUI() {
            GUILayout.Space(10);
            graph.category = GUILayout.TextField(graph.category);
            EditorUtils.TextFieldComment(graph.category, "Category...");

            graph.comments = GUILayout.TextArea(graph.comments, GUILayout.Height(45));
            EditorUtils.TextFieldComment(graph.comments, "Comments...");

            GUI.backgroundColor = Colors.lightBlue;
            if ( GUILayout.Button(string.Format("EDIT {0}", graph.GetType().Name.SplitCamelCase().ToUpper())) ) {
                GraphEditor.OpenWindow(graph);
            }
            GUI.backgroundColor = Color.white;
        }

        //List of defined parameters in graph
        public void ShowDefinedParametersGUI() {

            var varInfo = new Dictionary<string, System.Type>();
            var occurencies = new Dictionary<string, int>();
            var duplicateTypes = new Dictionary<System.Type, string>();

            foreach ( var bbVar in graph.GetDefinedParameters() ) {

                if ( varInfo.ContainsKey(bbVar.name) && varInfo[bbVar.name] != bbVar.varType ) {
                    duplicateTypes[bbVar.varType] = bbVar.name;
                    continue;
                }

                varInfo[bbVar.name] = bbVar.varType;
                if ( !occurencies.ContainsKey(bbVar.name) ) {
                    occurencies[bbVar.name] = 0;
                }
                occurencies[bbVar.name]++;
            }

            EditorUtils.TitledSeparator("Defined Blackboard Parameters");

            if ( varInfo.Count == 0 ) {
                EditorGUILayout.HelpBox("The graph has no defined Blackboard Parameters", MessageType.None);
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true));
            GUI.color = Color.yellow;
            GUILayout.Label("Name");
            GUI.color = Color.white;
            foreach ( var name in varInfo.Keys ) {
                GUILayout.Label(name);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true));
            GUI.color = Color.yellow;
            GUILayout.Label("Type");
            GUI.color = Color.white;
            foreach ( var type in varInfo.Values ) {
                GUILayout.Label(type.FriendlyName());
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true));
            GUI.color = Color.yellow;
            GUILayout.Label("Occurencies");
            GUI.color = Color.white;
            foreach ( var occ in occurencies.Values ) {
                GUILayout.Label(occ.ToString());
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if ( duplicateTypes.Count > 0 ) {
                EditorUtils.Separator();
                GUILayout.Label("Duplicate Types");
                foreach ( var pair in duplicateTypes ) {
                    EditorGUILayout.LabelField(pair.Value, pair.Key.FriendlyName());
                }
            }
        }
    }
}

#endif