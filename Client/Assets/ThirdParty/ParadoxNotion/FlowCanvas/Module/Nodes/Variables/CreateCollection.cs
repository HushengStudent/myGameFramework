using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Description("Create a collection of <T> objects")]
	[ContextDefinedInputs(typeof(Wild))]
	[ContextDefinedOutputs(typeof(List<>))]
	public class CreateCollection<T> : VariableNode {

		[SerializeField] [ExposeField]
		[GatherPortsCallback] [MinValue(2)] [DelayedField]
		private int _portCount = 4;

		public override void SetVariable(object o){
			//...
		}

		protected override void RegisterPorts(){
			var ins = new List<ValueInput<T>>();
			for (var i = 0; i < _portCount; i++){
				ins.Add( AddValueInput<T>("Element" + i.ToString()) );
			}
			AddValueOutput<T[]>("Collection", ()=> { return ins.Select(p => p.value).ToArray(); });
		}
	}
}