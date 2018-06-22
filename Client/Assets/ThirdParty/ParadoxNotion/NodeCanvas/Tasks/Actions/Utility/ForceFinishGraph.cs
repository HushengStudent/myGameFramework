using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions{

	[Category("✫ Utility")]
	[Description("Force Finish the current graph this Task is assigned to.")]
	public class ForceFinishGraph : ActionTask{
		
		public CompactStatus finishStatus = CompactStatus.Success;

		protected override void OnExecute(){
			var graph = ownerSystem as Graph;
			if (graph != null){
				graph.Stop(finishStatus == CompactStatus.Success);
			}
			EndAction(graph != null);
		}
	}
}