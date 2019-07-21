using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Name("Identity", 100)]
    [Description("Use for organization.")]
    public class Dummy : FlowControlNode
    {
        public override string name { get { return null; } }
        protected override void RegisterPorts() {
            var fOut = AddFlowOutput(" ", "Out");
            AddFlowInput(" ", "In", (f) => { fOut.Call(f); });
        }
    }
}