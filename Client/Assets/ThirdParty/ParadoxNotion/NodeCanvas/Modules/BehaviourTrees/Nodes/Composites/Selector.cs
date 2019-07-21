using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Selector", 9)]
    [Category("Composites")]
    [Description("Execute the child nodes in order or randonly until the first that returns Success and return Success as well. If none returns Success, then returns Failure.\nIf is Dynamic, then higher priority children Status are revaluated and if one returns Success the Selector will select that one and bail out immediately in Success too")]
    [Icon("Selector")]
    [Color("b3ff7f")]
    public class Selector : BTComposite
    {

        public bool dynamic;
        public bool random;

        private int lastRunningNodeIndex;

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            for ( var i = dynamic ? 0 : lastRunningNodeIndex; i < outConnections.Count; i++ ) {

                // if ( i < lastRunningNodeIndex ) {
                //     outConnections[i].Reset();
                // }

                status = outConnections[i].Execute(agent, blackboard);

                switch ( status ) {
                    case Status.Running:

                        if ( dynamic && i < lastRunningNodeIndex ) {
                            outConnections[lastRunningNodeIndex].Reset();
                        }

                        lastRunningNodeIndex = i;
                        return Status.Running;

                    case Status.Success:

                        if ( dynamic && i < lastRunningNodeIndex ) {
                            for ( var j = i + 1; j <= lastRunningNodeIndex; j++ ) {
                                outConnections[j].Reset();
                            }
                        }

                        return Status.Success;
                }
            }

            return Status.Failure;
        }

        protected override void OnReset() {
            lastRunningNodeIndex = 0;
            if ( random ) {
                outConnections = Shuffle(outConnections);
            }
        }

        public override void OnChildDisconnected(int index) {
            if ( index != 0 && index == lastRunningNodeIndex )
                lastRunningNodeIndex--;
        }

        public override void OnGraphStarted() {
            OnReset();
        }

        //Fisher-Yates shuffle algorithm
        private List<Connection> Shuffle(List<Connection> list) {
            for ( var i = list.Count - 1; i > 0; i-- ) {
                var j = (int)Mathf.Floor(Random.value * ( i + 1 ));
                var temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }

            return list;
        }


        /////////////////////////////////////////
        /////////GUI AND EDITOR STUFF////////////
        /////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            if ( dynamic )
                GUILayout.Label("<b>DYNAMIC</b>");
            if ( random )
                GUILayout.Label("<b>RANDOM</b>");
        }

#endif
    }
}