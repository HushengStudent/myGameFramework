using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.DialogueTrees
{

    [Icon("Condition")]
    [Name("Task Condition")]
    [Category("Branch")]
    [Description("Execute the first child node if a Condition is true, or the second one if that Condition is false. The Actor selected is used for the Condition check")]
    [Color("b3ff7f")]
    public class ConditionNode : DTNode, ITaskAssignable<ConditionTask>
    {

        [SerializeField]
        private ConditionTask _condition;

        public ConditionTask condition {
            get { return _condition; }
            set { _condition = value; }
        }

        public Task task {
            get { return condition; }
            set { condition = (ConditionTask)value; }
        }

        public override int maxOutConnections { get { return 2; } }
        public override bool requireActorSelection { get { return true; } }

        protected override Status OnExecute(Component agent, IBlackboard bb) {

            if ( outConnections.Count == 0 ) {
                return Error("There are no connections on the Dialogue Condition Node");
            }

            if ( condition == null ) {
                return Error("There is no Conidition on the Dialoge Condition Node");
            }

            var isSuccess = condition.CheckCondition(finalActor.transform, graphBlackboard);
            status = isSuccess ? Status.Success : Status.Failure;
            DLGTree.Continue(isSuccess ? 0 : 1);
            return status;
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        public override string GetConnectionInfo(int i) {
            return i == 0 ? "Then" : "Else";
        }

        protected override void OnNodeGUI() {
            if ( outConnections.Count == 0 ) {
                GUILayout.Label("No Outcomes Connected");
            }
        }

#endif
    }
}