using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Name("Switch Condition")]
	[Category("Flow Controllers/Switchers")]
	[Description("Branch the Flow based on a conditional boolean value")]
	[ContextDefinedInputs(typeof(bool))]
	public class SwitchBool : FlowControlNode {
		protected override void RegisterPorts(){
			var c = AddValueInput<bool>("Condition");
			var fTrue = AddFlowOutput("True");
			var fFalse = AddFlowOutput("False");
			AddFlowInput("In", (f)=> { f.Call(c.value? fTrue : fFalse); });
		}
	}
}