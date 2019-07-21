using System.Collections;
using ParadoxNotion.Design;


namespace FlowCanvas.Nodes
{

    [Name("On Disable", 4)]
    [Category("Events/Graph")]
    [Description("Called whenever the Graph is Disabled")]
    [ExecutionPriority(4)]
    public class DisableEvent : EventNode
    {

        private FlowOutput disable;

        public override void OnGraphStoped() {
            disable.Call(new Flow());
        }

        protected override void RegisterPorts() {
            disable = AddFlowOutput("Out");
        }
    }
}