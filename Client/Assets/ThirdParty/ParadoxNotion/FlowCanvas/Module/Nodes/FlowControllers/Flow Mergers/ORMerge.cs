using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Name("OR")]
	[Category("Flow Controllers/Flow Merge")]
	[Description("Calls Out when either input is called")]
	public class ORMerge : FlowControlNode {

		[SerializeField] [ExposeField]
		[GatherPortsCallback] [MinValue(2)] [DelayedField]
		private int _portCount = 2;

		private FlowOutput fOut;
		private int lastFrameCall;
		
		protected override void RegisterPorts(){
			fOut = AddFlowOutput("Out");
			for (var _i = 0; _i < _portCount; _i++){
				var i = _i;
				AddFlowInput(i.ToString(), (f)=> { Check(i, f); } );
			}
		}

		void Check(int index, Flow f){
			if (Time.frameCount != lastFrameCall){
				lastFrameCall = Time.frameCount;
				fOut.Call(f);
			}
		}
	}
}