using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Category("Flow Controllers/Togglers")]
	[Description("Flip Flops between the 2 outputs each time In is called")]
	[ContextDefinedOutputs(typeof(bool))]
	public class FlipFlop : FlowControlNode {
		
		public bool isFlip = true;
		private bool original;

		public override string name {
			get {return base.name + " " + (isFlip? "[FLIP]" : "[FLOP]");}
		}

		public override void OnGraphStarted(){ original = isFlip; }
		public override void OnGraphStoped(){ isFlip = original; }

		protected override void RegisterPorts(){
			var fFlip = AddFlowOutput("Flip");
			var fFlop = AddFlowOutput("Flop");
			AddFlowInput("In", (f)=> {
				f.Call(isFlip? fFlip : fFlop);
				isFlip = !isFlip;
			});
			AddFlowInput("Reset", (f)=>{ isFlip = false; });
			AddValueOutput<bool>("Is Flip", ()=>{ return isFlip; });
		}
	}
}