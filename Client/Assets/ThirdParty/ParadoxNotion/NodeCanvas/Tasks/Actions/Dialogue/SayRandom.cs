using UnityEngine;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using NodeCanvas.DialogueTrees;

namespace NodeCanvas.Tasks.Actions
{

    [Category("Dialogue")]
    [Icon("Dialogue")]
    [Description("A random statement will be chosen each time for the actor to say")]
    public class SayRandom : ActionTask<IDialogueActor>
    {

        public List<Statement> statements = new List<Statement>();

        protected override void OnExecute() {
            var index = Random.Range(0, statements.Count);
            var statement = statements[index];
            var tempStatement = statement.BlackboardReplace(blackboard);
            var info = new SubtitlesRequestInfo(agent, tempStatement, EndAction);
            DialogueTree.RequestSubtitles(info);
        }


        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI() {

            if ( GUILayout.Button("Add Statement") ) {
                statements.Add(new Statement(""));
            }

            foreach ( var statement in statements.ToArray() ) {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Space(2);
                statement.text = UnityEditor.EditorGUILayout.TextArea(statement.text, (GUIStyle)"textField", GUILayout.Height(50));
                statement.audio = (AudioClip)UnityEditor.EditorGUILayout.ObjectField("Audio Clip", statement.audio, typeof(AudioClip), false);
                statement.meta = UnityEditor.EditorGUILayout.TextField("Meta", statement.meta);
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                if ( GUILayout.Button("X") ) {
                    statements.Remove(statement);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

#endif
    }
}