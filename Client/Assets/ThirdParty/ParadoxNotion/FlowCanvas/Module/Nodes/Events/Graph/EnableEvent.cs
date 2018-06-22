using System.Collections;
using ParadoxNotion.Design;


namespace FlowCanvas.Nodes{

	[Name("On Enable", 8)]
	[Category("Events/Graph")]
	[Description("Called when the Graph is enabled")]
	public class EnableEvent : EventNode {

		private FlowOutput enable;

		public override void OnGraphStarted(){
			enable.Call( new Flow() );
		}

		protected override void RegisterPorts(){
			enable = AddFlowOutput("Out");
		}
	}
}