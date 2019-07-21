using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("Step Sequencer")]
    [Category("Composites")]
    [Description("Executes AND immediately returns children node status ONE-BY-ONE. Step Sequencer always moves forward by one and loops it's index")]
    [Icon("StepIterator")]
    [Color("bf7fff")]
    public class StepIterator : BTComposite
    {

        private int current;

        public override void OnGraphStarted() {
            current = 0;
        }

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {
            current = current % outConnections.Count;
            return outConnections[current].Execute(agent, blackboard);
        }

        protected override void OnReset() {
            current++;
        }
    }
}