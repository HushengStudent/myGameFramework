using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Parallel", 8)]
    [Category("Composites")]
    [Description("Execute all child nodes once but simultaneously and return Success or Failure depending on the selected ParallelPolicy.\nIf set to Repeat, child nodes are repeated until the Policy set is met, or until all children have had a chance to complete at least once.")]
    [Icon("Parallel")]
    [Color("ff64cb")]
    public class Parallel : BTComposite
    {

        public enum ParallelPolicy
        {
            FirstFailure,
            FirstSuccess,
            FirstSuccessOrFailure
        }

        public ParallelPolicy policy = ParallelPolicy.FirstFailure;
        [Name("Repeat")]
        public bool dynamic;

        private readonly List<Connection> finishedConnections = new List<Connection>();

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            var defferedStatus = Status.Resting;
            for ( var i = 0; i < outConnections.Count; i++ ) {

                if ( !dynamic && finishedConnections.Contains(outConnections[i]) ) {
                    continue;
                }

                if ( outConnections[i].status != Status.Running && finishedConnections.Contains(outConnections[i]) ) {
                    outConnections[i].Reset();
                }

                status = outConnections[i].Execute(agent, blackboard);

                if ( defferedStatus == Status.Resting ) {
                    if ( status == Status.Failure && ( policy == ParallelPolicy.FirstFailure || policy == ParallelPolicy.FirstSuccessOrFailure ) ) {
                        defferedStatus = Status.Failure;
                    }

                    if ( status == Status.Success && ( policy == ParallelPolicy.FirstSuccess || policy == ParallelPolicy.FirstSuccessOrFailure ) ) {
                        defferedStatus = Status.Success;
                    }
                }

                if ( status != Status.Running && !finishedConnections.Contains(outConnections[i]) ) {
                    finishedConnections.Add(outConnections[i]);
                }
            }

            if ( defferedStatus != Status.Resting ) {
                ResetRunning();
                return defferedStatus;
            }

            if ( finishedConnections.Count == outConnections.Count ) {
                ResetRunning();
                switch ( policy ) {
                    case ParallelPolicy.FirstFailure:
                        return Status.Success;
                    case ParallelPolicy.FirstSuccess:
                        return Status.Failure;
                }
            }

            return Status.Running;
        }

        protected override void OnReset() {
            finishedConnections.Clear();
        }

        void ResetRunning() {
            for ( var i = 0; i < outConnections.Count; i++ ) {
                if ( outConnections[i].status == Status.Running ) {
                    outConnections[i].Reset();
                }
            }
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.Label(( dynamic ? "<b>REPEAT</b>\n" : "" ) + policy.ToString().SplitCamelCase());
        }

#endif
    }
}