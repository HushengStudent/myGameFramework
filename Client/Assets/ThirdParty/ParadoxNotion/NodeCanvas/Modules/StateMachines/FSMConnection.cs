using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.StateMachines
{

    ///The connection object for FSM nodes. AKA Transitions
    public class FSMConnection : Connection, ITaskAssignable<ConditionTask>
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


        ///Perform the transition disregarding whether or not the condition (if any) is valid
        public void PerformTransition() {
            ( graph as FSM ).EnterState((FSMState)targetNode);
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        public override ParadoxNotion.PlanarDirection direction {
            get { return ParadoxNotion.PlanarDirection.Auto; }
        }

        public override TipConnectionStyle tipConnectionStyle {
            get { return TipConnectionStyle.Arrow; }
        }

        protected override string GetConnectionInfo() {
            if ( condition == null ) { return "OnFinish"; }
            return condition.summaryInfo;
        }

        protected override void OnConnectionInspectorGUI() {
            NodeCanvas.Editor.TaskEditor.TaskFieldMulti<ConditionTask>(condition, graph, (c) => { condition = c; });
        }

#endif
    }
}