using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Name("Latch Integer")]
	[Category("Flow Controllers/Flow Convert")]
	[Description("Convert a Flow signal to an integer value")]
	[ContextDefinedOutputs(typeof(int))]
	public class LatchInt : FlowControlNode {
		
		[SerializeField] [ExposeField]
		[GatherPortsCallback] [MinValue(2)] [DelayedField]
		private int _portCount = 4;
		private int latched;

		protected override void RegisterPorts(){
			var o = AddFlowOutput("Out");
			for (int _i = 0; _i < _portCount; _i++){
				var i = _i;
				AddFlowInput(i.ToString(), (f)=>{ latched = i; o.Call(f); });
			}
			AddValueOutput<int>("Value", ()=> { return latched; });
		}
	}
}