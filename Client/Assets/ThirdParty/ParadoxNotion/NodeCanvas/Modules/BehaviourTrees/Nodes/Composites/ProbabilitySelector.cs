using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Category("Composites")]
    [Description("Select a child to execute based on it's chance to be selected and return Success if it returns Success, otherwise pick another.\nReturns Failure if no child returns Success or a direct 'Failure Chance' is introduced")]
    [Icon("ProbabilitySelector")]
    [Color("b3ff7f")]
    public class ProbabilitySelector : BTComposite
    {

        public List<BBParameter<float>> childWeights = new List<BBParameter<float>>();
        public BBParameter<float> failChance = new BBParameter<float>();

        private float probability;
        private float currentProbability;
        private List<int> failedIndeces = new List<int>();

        public override void OnChildConnected(int index) {
            childWeights.Insert(index, new BBParameter<float> { value = 1, bb = graphBlackboard });
        }

        public override void OnChildDisconnected(int index) {
            childWeights.RemoveAt(index);
        }

        //To create a new probability when BT starts
        public override void OnGraphStarted() {
            OnReset();
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            currentProbability = probability;
            for ( var i = 0; i < outConnections.Count; i++ ) {

                if ( failedIndeces.Contains(i) ) {
                    continue;
                }

                if ( currentProbability > childWeights[i].value ) {
                    currentProbability -= childWeights[i].value;
                    continue;
                }

                status = outConnections[i].Execute(agent, blackboard);
                if ( status == Status.Success || status == Status.Running ) {
                    return status;
                }

                if ( status == Status.Failure ) {
                    failedIndeces.Add(i);
                    var newTotal = GetTotal();
                    for ( var j = 0; j < failedIndeces.Count; j++ ) {
                        newTotal -= childWeights[failedIndeces[j]].value;
                    }
                    probability = Random.Range(0, newTotal);
                    return Status.Running;
                }
            }

            return Status.Failure;
        }

        protected override void OnReset() {
            failedIndeces.Clear();
            probability = Random.Range(0f, GetTotal());
        }


        float GetTotal() {
            var total = failChance.value;
            foreach ( var weight in childWeights ) {
                total += weight.value;
            }
            return total;
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        public override string GetConnectionInfo(int i) {
            return Mathf.Round(( childWeights[i].value / GetTotal() ) * 100) + "%";
        }

        public override void OnConnectionInspectorGUI(int i) {
            NodeCanvas.Editor.BBParameterEditor.ParameterField("Weight", childWeights[i]);
        }

        protected override void OnNodeInspectorGUI() {

            if ( outConnections.Count == 0 ) {
                GUILayout.Label("Make some connections first");
                return;
            }

            var total = GetTotal();
            for ( var i = 0; i < childWeights.Count; i++ ) {
                GUILayout.BeginHorizontal();
                childWeights[i] = (BBParameter<float>)NodeCanvas.Editor.BBParameterEditor.ParameterField("Weight", childWeights[i]);
                GUILayout.Label(Mathf.Round(( childWeights[i].value / total ) * 100) + "%", GUILayout.Width(38));
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            failChance = (BBParameter<float>)NodeCanvas.Editor.BBParameterEditor.ParameterField("Direct Failure Chance", failChance);
            GUILayout.Label(Mathf.Round(( failChance.value / total ) * 100) + "%", GUILayout.Width(38));
            GUILayout.EndHorizontal();
        }

#endif
    }
}