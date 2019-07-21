using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
    [Category("Flow Controllers/Iterators")]
    [Description("Can be used within a For Loop or For Each node, to Break the iteration.")]
    public class Break : FlowControlNode
    {
        protected override void RegisterPorts() {
            AddFlowInput("Break", (f) => { f.Break(this); });
        }
    }
}