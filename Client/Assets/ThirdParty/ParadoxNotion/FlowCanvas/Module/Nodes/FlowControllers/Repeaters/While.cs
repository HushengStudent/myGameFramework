using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Name("While True")]
	[Category("Flow Controllers/Repeaters")]
	[Description("Once called, will continuously call 'Do' while the input boolean condition is true. Once condition becomes or is false, 'Done' is called")]
	[ContextDefinedInputs(typeof(bool))]
	public class While : FlowControlNode {
		
		private UnityEngine.Coroutine coroutine;

		public override void OnGraphStarted(){
			coroutine = null;
		}

		public override void OnGraphStoped(){
			if (coroutine != null){
				StopCoroutine(coroutine);
				coroutine = null;
			}
		}

		protected override void RegisterPorts(){
			var c = AddValueInput<bool>("Condition");
			var fCurrent = AddFlowOutput("Do");
			var fFinish = AddFlowOutput("Done");
			AddFlowInput("In", (f)=> {
				if (coroutine == null){
					coroutine = StartCoroutine( DoWhile(fCurrent, fFinish, f, c) );
				}
			});
		}

		IEnumerator DoWhile(FlowOutput fCurrent, FlowOutput fFinish, Flow f, ValueInput<bool> condition){
			var active = true;
			f.Break = ()=>{ active = false; };
			while (active && condition.value){
				while(graph.isPaused){
					yield return null;
				}
				fCurrent.Call(f);
				yield return null;
			}

			coroutine = null;
			f.Break = null;
			fFinish.Call(f);
		}
	}
}