using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Name("On Start", 9)]
    [Category("Events/Graph")]
    [Description("Called only once and the first time the Graph is enabled. This called after all OnAwake events and before OnEnable events of the graph.")]
    [ExecutionPriority(9)]
    public class StartEvent : EventNode
    {

        private FlowOutput start;
        private bool called = false;

        public override void OnGraphStarted() {
            if ( !called ) {
                called = true;
                start.Call(new Flow());
            }
        }

        protected override void RegisterPorts() {
            start = AddFlowOutput("Once");
        }
    }
}