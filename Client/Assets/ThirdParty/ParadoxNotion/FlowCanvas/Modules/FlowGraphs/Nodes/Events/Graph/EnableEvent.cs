using System.Collections;
using ParadoxNotion.Design;


namespace FlowCanvas.Nodes
{

    [Name("On Enable", 8)]
    [Category("Events/Graph")]
    [Description("Called whenever the Graph is enabled")]
    [ExecutionPriority(8)]
    public class EnableEvent : EventNode
    {

        private FlowOutput enable;

        public override void OnGraphStarted() {
            enable.Call(new Flow());
        }

        protected override void RegisterPorts() {
            enable = AddFlowOutput("Out");
        }
    }
}