using ParadoxNotion.Design;
using ParadoxNotion.Serialization;

namespace FlowCanvas.Nodes{

	[Name("Latch Condition")]
	[Category("Flow Controllers/Flow Convert")]
	[Description("Convert a Flow signal to boolean value")]
	[ContextDefinedOutputs(typeof(bool))]
	
	[DeserializeFrom("FlowCanvas.Nodes.Latch")]
	public class LatchBool : FlowControlNode {
		private bool latched;
		protected override void RegisterPorts(){
			var o = AddFlowOutput("Out");
			AddFlowInput("True", (f)=> { latched = true; o.Call(f); });
			AddFlowInput("False", (f)=> { latched = false; o.Call(f); });
			AddValueOutput<bool>("Value", ()=> { return latched; });
		}
	}
}