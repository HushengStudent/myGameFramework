using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.DialogueTrees
{

    [Name("Say")]
    [Description("Make the selected Dialogue Actor talk. You can make the text more dynamic by using variable names in square brackets\ne.g. [myVarName] or [Global/myVarName]")]
    public class StatementNode : DTNode
    {

        [SerializeField]
        private Statement statement = new Statement("This is a dialogue text");

        public override bool requireActorSelection { get { return true; } }

        protected override Status OnExecute(Component agent, IBlackboard bb) {
            var tempStatement = statement.BlackboardReplace(bb);
            DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(finalActor, tempStatement, OnStatementFinish));
            return Status.Running;
        }

        void OnStatementFinish() {
            status = Status.Success;
            DLGTree.Continue();
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            var displayText = statement.text.Length > 30 ? statement.text.Substring(0, 30) + "..." : statement.text;
            GUILayout.Label("\"<i> " + displayText + "</i> \"");
        }

        protected override void OnNodeInspectorGUI() {

            base.OnNodeInspectorGUI();
            var areaStyle = new GUIStyle(GUI.skin.GetStyle("TextArea"));
            areaStyle.wordWrap = true;

            GUILayout.Label("Dialogue Text");
            statement.text = UnityEditor.EditorGUILayout.TextArea(statement.text, areaStyle, GUILayout.Height(100));
            statement.audio = UnityEditor.EditorGUILayout.ObjectField("Audio File", statement.audio, typeof(AudioClip), false) as AudioClip;
            statement.meta = UnityEditor.EditorGUILayout.TextField("Metadata", statement.meta);
        }

#endif
    }
}