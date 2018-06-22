using System.Collections.Generic;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Category("Flow Controllers/Flow Convert")]
	[Description("Reads a named parameter from the incomming Flow and returns it's value.\nFlow parameters can be set with a WriteFlowParameter node.\nFlow parameters are temporary variables that exist only in the context of the same Flow.")]
	[ContextDefinedOutputs(typeof(Wild))]
	public class ReadFlowParameter<T> : FlowControlNode {

		private T flowValue;
		protected override void RegisterPorts(){
			var o = AddFlowOutput("Out");
			var pName = AddValueInput<string>("Name");
			AddValueOutput<T>("Value", ()=>{ return flowValue; });
			AddFlowInput("In", (f)=>
			{
				flowValue = f.ReadParameter<T>(pName.value);
				o.Call(f);
			});
		}
	}

	[Category("Flow Controllers/Flow Convert")]
	[Description("Writes (or creates) a named parameter to the incomming Flow, which you can later read with a ReadFlowParameter node.\nFlow parameters are temporary variables that exist only in the context of the same Flow.")]
	[ContextDefinedInputs(typeof(Wild))]
	public class WriteFlowParameter<T> : FlowControlNode {

		protected override void RegisterPorts(){
			var o = AddFlowOutput("Out");
			var pName = AddValueInput<string>("Name");
			var pValue = AddValueInput<T>("Value");
			AddFlowInput("In", (f)=>
			{
				f.WriteParameter<T>(pName.value, pValue.value);
				o.Call(f);
			});
		}
	}
}