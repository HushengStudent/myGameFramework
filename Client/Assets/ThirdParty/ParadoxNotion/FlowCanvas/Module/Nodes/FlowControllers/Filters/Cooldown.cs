using System.Collections;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{
	
	[Category("Flow Controllers/Filters")]
	[Description("Filters OUT so that it can't be called very frequently")]
	[ContextDefinedInputs(typeof(float))]
	[ContextDefinedOutputs(typeof(float))]
	public class Cooldown : FlowControlNode {
		
		private float current = 0;
		private Coroutine coroutine;
		
		public override string name{
			get {return base.name + string.Format(" [{0}]", current.ToString("0.0"));}
		}

		public override void OnGraphStarted(){
			current = 0;
			coroutine = null;
		}

		public override void OnGraphStoped(){
			if (coroutine != null){
				StopCoroutine(coroutine);
				coroutine = null;
				current = 0;
			}
		}

		protected override void RegisterPorts(){
			var o = AddFlowOutput("Out");
			var ready = AddFlowOutput("Ready");
			var time = AddValueInput<float>("Time");
			AddValueOutput<float>("Current", ()=>{ return Mathf.Max(current, 0); } );
			AddFlowInput("In", (f)=>
			{
				if (current <= 0 && coroutine == null){
					current = time.value;
					coroutine = StartCoroutine(CountDown(ready, f));
					o.Call(f);
				}
			});

			AddFlowInput("Cancel", (f)=>
			{
				if (coroutine != null){
					StopCoroutine(coroutine);
					coroutine = null;
					current = 0;
				}
			});
		}

		IEnumerator CountDown(FlowOutput ready, Flow f){
			while (current > 0){
				while (graph.isPaused){
					yield return null;
				}
				current -= Time.deltaTime;
				yield return null;
			}
			coroutine = null;
			ready.Call(f);
		}
	}
}