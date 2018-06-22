using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Name("Coroutine")]
	[Category("Flow Controllers/Repeaters")]
	[Description("Start a Coroutine that will repeat until Break is signaled")]
	public class CoroutineState : FlowControlNode {

		private bool active = false;
		UnityEngine.Coroutine coroutine = null;

		public override void OnGraphStoped(){
			if (coroutine != null){
				StopCoroutine(coroutine);
				active = false;
			}
		}

		protected override void RegisterPorts(){
			var fStarted = AddFlowOutput("Start");
			var fUpdate = AddFlowOutput("Update");
			var fFinish = AddFlowOutput("Finish");
			AddFlowInput("Start", (f)=> {
				if (!active){
					active = true;
					coroutine = StartCoroutine(DoRepeat(fStarted, fUpdate, fFinish, f));
				}
			});
			AddFlowInput("Break", (f)=> {
				active = false;
			});
		}


		IEnumerator DoRepeat(FlowOutput fStarted, FlowOutput fUpdate, FlowOutput fFinish, Flow f){
			f.Break = ()=>{ active = false; };
			fStarted.Call(f);
			while(active){
				while(graph.isPaused){
					yield return null;
				}
				fUpdate.Call(f);
				yield return null;
			}
			f.Break = null;
			fFinish.Call(f);
		}
	}
}