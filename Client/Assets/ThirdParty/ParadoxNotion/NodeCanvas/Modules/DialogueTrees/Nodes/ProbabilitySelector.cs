using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.DialogueTrees
{

    [Category("Branch")]
    [Description("Select a child to execute based on it's chance to be selected. An optional pre-Condition Task can be assigned to filter the child in or out of the selection probability.\nThe actor selected will be used for the condition checks.")]
    [Icon("ProbabilitySelector")]
    [Color("b3ff7f")]
    public class ProbabilitySelector : DTNode, ISubParametersContainer, ISubTasksContainer
    {

        public class Option
        {
            public BBParameter<float> weight;
            public ConditionTask condition;
            public Option(float weightValue, IBlackboard bbValue) {
                weight = new BBParameter<float> { value = weightValue, bb = bbValue };
                condition = null;
            }
        }

        [SerializeField]
        private List<Option> childOptions = new List<Option>();
        private List<int> successIndeces;

        public override int maxOutConnections { get { return -1; } }

        public Task[] GetSubTasks() {
            return childOptions.Select(o => o.condition).ToArray();
        }

        public BBParameter[] GetSubParameters() {
            return childOptions.Select(o => o.weight).ToArray();
        }

        public override void OnChildConnected(int index) {
            childOptions.Insert(index, new Option(1, graphBlackboard));
        }

        public override void OnChildDisconnected(int index) {
            childOptions.RemoveAt(index);
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            successIndeces = new List<int>();
            for ( var i = 0; i < outConnections.Count; i++ ) {
                var condition = childOptions[i].condition;
                if ( condition == null || condition.CheckCondition(finalActor.transform, blackboard) ) {
                    successIndeces.Add(i);
                }
            }

            var probability = Random.Range(0f, GetTotal());
            for ( var i = 0; i < outConnections.Count; i++ ) {

                if ( !successIndeces.Contains(i) ) {
                    continue;
                }

                if ( probability > childOptions[i].weight.value ) {
                    probability -= childOptions[i].weight.value;
                    continue;
                }

                DLGTree.Continue(i);
                return Status.Success;
            }

            return Status.Failure;
        }

        float GetTotal() {
            var total = 0f;
            for ( var i = 0; i < childOptions.Count; i++ ) {
                var option = childOptions[i];
                if ( successIndeces == null || successIndeces.Contains(i) ) {
                    total += option.weight.value;
                    continue;
                }
            }
            return total;
        }

        protected override void OnReset() {
            successIndeces = null;
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        public override string GetConnectionInfo(int i) {
            var result = childOptions[i].condition != null ? childOptions[i].condition.summaryInfo + "\n" : string.Empty;
            if ( successIndeces == null || successIndeces.Contains(i) ) {
                return result + Mathf.Round(( childOptions[i].weight.value / GetTotal() ) * 100) + "%";
            }
            return result + "Condition Failed";
        }

        public override void OnConnectionInspectorGUI(int i) {
            DrawOptionGUI(i, GetTotal());
        }

        protected override void OnNodeInspectorGUI() {

            base.OnNodeInspectorGUI();

            if ( outConnections.Count == 0 ) {
                GUILayout.Label("Make some connections first");
                return;
            }

            var total = GetTotal();
            for ( var i = 0; i < childOptions.Count; i++ ) {
                DrawOptionGUI(i, total);
            }
        }

        void DrawOptionGUI(int i, float total) {
            EditorUtils.BoldSeparator();
            NodeCanvas.Editor.TaskEditor.TaskFieldMulti<ConditionTask>(childOptions[i].condition, DLGTree, (c) => { childOptions[i].condition = c; });
            EditorUtils.Separator();
            GUILayout.BeginHorizontal();
            NodeCanvas.Editor.BBParameterEditor.ParameterField("Weight", childOptions[i].weight);
            GUILayout.Label(Mathf.Round(( childOptions[i].weight.value / total ) * 100) + "%", GUILayout.Width(38));
            GUILayout.EndHorizontal();
        }

#endif
    }
}