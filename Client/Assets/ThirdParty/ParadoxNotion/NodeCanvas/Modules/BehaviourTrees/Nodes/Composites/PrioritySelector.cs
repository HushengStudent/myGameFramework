using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Category("Composites")]
    [Description("Used for Utility AI, the Priority Selector executes the child with the highest priority weight. If it fails, the Priority Selector will continue with the next highest priority child until one Succeeds, or until all Fail (similar to how a normal Selector does).")]
    [Icon("Priority")]
    [Color("b3ff7f")]
    public class PrioritySelector : BTComposite
    {

        public List<BBParameter<float>> priorities = new List<BBParameter<float>>();

        private List<Connection> orderedConnections = new List<Connection>();
        private int current = 0;

        public override void OnChildConnected(int index) {
            priorities.Insert(index, new BBParameter<float> { value = 1, bb = graphBlackboard });
        }

        public override void OnChildDisconnected(int index) {
            priorities.RemoveAt(index);
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( status == Status.Resting ) {
                orderedConnections = outConnections.OrderBy(c => priorities[outConnections.IndexOf(c)].value).Reverse().ToList();
            }

            for ( var i = current; i < orderedConnections.Count; i++ ) {
                status = orderedConnections[i].Execute(agent, blackboard);
                if ( status == Status.Success ) {
                    return Status.Success;
                }

                if ( status == Status.Running ) {
                    current = i;
                    return Status.Running;
                }
            }

            return Status.Failure;
        }

        protected override void OnReset() {
            current = 0;
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        public override string GetConnectionInfo(int i) {
            return priorities[i].ToString();
        }

        public override void OnConnectionInspectorGUI(int i) {
            priorities[i] = (BBParameter<float>)NodeCanvas.Editor.BBParameterEditor.ParameterField("Priority Weight", priorities[i]);
        }

        protected override void OnNodeInspectorGUI() {
            for ( var i = 0; i < priorities.Count; i++ )
                priorities[i] = (BBParameter<float>)NodeCanvas.Editor.BBParameterEditor.ParameterField("Priority Weight", priorities[i]);
        }

#endif
    }
}