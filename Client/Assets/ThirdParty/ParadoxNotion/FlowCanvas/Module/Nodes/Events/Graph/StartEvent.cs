using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Name("On Start", 9)]
	[Category("Events/Graph")]
	[Description("Called only once and the first time the Graph is enabled.\nThis is called 1 frame after all Awake events are called.")]
	public class StartEvent : EventNode {

		private FlowOutput start;
		private bool called = false;

		public override void OnGraphStarted(){
			if (!called){
				called = true;
				StartCoroutine(DelayCall());
			}			
		}

		IEnumerator DelayCall(){
			yield return null;
			start.Call( new Flow() );
		}

		protected override void RegisterPorts(){
			start = AddFlowOutput("Once");
		}
	}
}