using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{


	[Description("Enumerate a value (usualy a list or array) for each of it's elements")]
	[Category("Flow Controllers/Iterators")]
	[ContextDefinedInputs(typeof(IEnumerable))]
	[ContextDefinedOutputs(typeof(object))]
	public class ForEach : FlowControlNode {
		
		private object current;
		private bool broken;
		private ValueInput<IEnumerable> enumerableInput;

		protected override void RegisterPorts(){
			enumerableInput = AddValueInput<IEnumerable>("Value");
			AddValueOutput<object>("Current", ()=> {return current;});
			var fCurrent = AddFlowOutput("Do");
			var fFinish = AddFlowOutput("Done");
			AddFlowInput("In", (f)=>
			{
				var li = enumerableInput.value;
				if (li == null){
					fFinish.Call(f);
					return;
				}

				broken = false;
				f.Break = ()=>{ broken = true; };
				foreach(var o in li){
					if (broken){
						break;
					}
					current = o;
					fCurrent.Call(f);
				}

				f.Break = null;
				fFinish.Call(f);
			});

			AddFlowInput("Break", (f)=>{ broken = true; });
		}

		public override System.Type GetNodeWildDefinitionType() {
			return typeof(IEnumerable);
		}

		public override void OnPortConnected(Port port, Port otherPort){
			if (port == enumerableInput){
				var elementType = otherPort.type.GetEnumerableElementType();
				if (elementType != null){
					ReplaceWith( typeof(ForEach<>).RTMakeGenericType(elementType) );
				}
			}
		}
		
	}

	[Description("Enumerate a value (usualy a list or array) for each of it's elements")]
	[Category("Flow Controllers/Iterators")]
	[ContextDefinedInputs(typeof(IEnumerable<>))]
	[ContextDefinedOutputs(typeof(Wild))]
	[ExposeAsDefinition]
	public class ForEach<T> : FlowControlNode {
		
		private T current;
		private bool broken;

		protected override void RegisterPorts(){
			var list = AddValueInput<IEnumerable<T>>("Value");
			AddValueOutput<T>("Current", ()=> {return current;} );
			var fCurrent = AddFlowOutput("Do");
			var fFinish = AddFlowOutput("Done");
			AddFlowInput("In", (f)=>
			{
				var li = list.value;
				if (li == null){
					fFinish.Call(f);
					return;
				}

				broken = false;
				f.Break = ()=>{ broken = true; };
				foreach(var o in li){
					if (broken){
						break;
					}
					current = o;
					fCurrent.Call(f);
				}
				f.Break = null;
				fFinish.Call(f);
			});

			AddFlowInput("Break", (f)=>{ broken = true; });
		}
	}
}