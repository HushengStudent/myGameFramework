using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.StateMachines
{

    [Name("Any State")]
    [Description("The Transitions of this node will constantly be checked. If any becomes true, the target connected State will Enter regardless of the current State. This node can have no incomming transitions.")]
    [Color("b3ff7f")]
    public class AnyState : FSMState, IUpdatable
    {

        public bool dontRetriggerStates = false;

        public override string name {
            get { return "FROM ANY STATE"; }
        }

        public override int maxInConnections { get { return 0; } }
        public override int maxOutConnections { get { return -1; } }
        public override bool allowAsPrime { get { return false; } }

        new public void Update() {

            if ( outConnections.Count == 0 ) {
                return;
            }

            status = Status.Running;

            for ( var i = 0; i < outConnections.Count; i++ ) {

                var connection = (FSMConnection)outConnections[i];
                var condition = connection.condition;

                if ( !connection.isActive || condition == null ) {
                    continue;
                }

                if ( dontRetriggerStates ) {
                    if ( FSM.currentState == (FSMState)connection.targetNode && FSM.currentState.status == Status.Running ) {
                        continue;
                    }
                }

                if ( condition.CheckCondition(graphAgent, graphBlackboard) ) {
                    FSM.EnterState((FSMState)connection.targetNode);
                    connection.status = Status.Success; //editor vis
                    return;
                }

                connection.status = Status.Failure; //editor vis
            }
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            base.OnNodeGUI();
            if ( dontRetriggerStates ) {
                UnityEngine.GUILayout.Label("<b>[NO RETRIGGER]</b>");
            }
        }

        protected override void OnNodeInspectorGUI() {

            ShowBaseFSMInspectorGUI();
            if ( outConnections.Find(c => ( c as FSMConnection ).condition == null) != null ) {
                UnityEditor.EditorGUILayout.HelpBox("This is not a state and as such it never finish, thus OnFinish transitions are never called.\nPlease add a condition in all transitions of this node.", UnityEditor.MessageType.Warning);
            }

            dontRetriggerStates = UnityEditor.EditorGUILayout.ToggleLeft("Don't Retrigger Running States", dontRetriggerStates);
        }

#endif
    }
}