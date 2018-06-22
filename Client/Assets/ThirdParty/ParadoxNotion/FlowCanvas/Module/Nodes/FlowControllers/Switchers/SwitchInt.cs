using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Name("Switch Integer")]
	[Category("Flow Controllers/Switchers")]
	[Description("Branch the Flow based on an integer value. The Default output is called when the Index value is out of range.")]
	[ContextDefinedInputs(typeof(int))]
	public class SwitchInt : FlowControlNode {

		[SerializeField] [ExposeField]
		[GatherPortsCallback] [MinValue(2)] [DelayedField]
		private int _portCount = 4;

		protected override void RegisterPorts(){
			var index = AddValueInput<int>("Index");
			var outs = new List<FlowOutput>();
			for (var i = 0; i < _portCount; i++){
				outs.Add( AddFlowOutput(i.ToString()) );
			}
			var def = AddFlowOutput("Default");
			AddFlowInput("In", (Flow f)=>
			{
				var i = index.value;
				if (i >= 0 && i < outs.Count){
					outs[i].Call(f);
				} else {
					def.Call(f);
				}
			});
		}
	}
}