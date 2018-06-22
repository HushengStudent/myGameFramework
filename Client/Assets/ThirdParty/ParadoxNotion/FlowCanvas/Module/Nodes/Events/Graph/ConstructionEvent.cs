using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Name("On Awake", 10)]
	[Category("Events/Graph")]
	[Description("Called only once and the first time the Graph is enabled.\nUse this only for initialization of this graph.")]
	public class ConstructionEvent : EventNode {

		private FlowOutput awake;
		private bool called = false;

		public override void OnGraphStarted(){
			if (!called){
				called = true;
				awake.Call( new Flow() );
			}			
		}

		protected override void RegisterPorts(){
			awake = AddFlowOutput("Once");
		}
	}
}