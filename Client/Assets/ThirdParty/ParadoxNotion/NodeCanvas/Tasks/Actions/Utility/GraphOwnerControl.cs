using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections;

namespace NodeCanvas.Tasks.Actions{

	[Name("Control Graph Owner")]
	[Category("✫ Utility")]
	[Description("Start, Resume, Pause, Stop a GraphOwner's behaviour")]
	public class GraphOwnerControl : ActionTask<GraphOwner> {

		public enum Control
		{
			StartBehaviour,
			StopBehaviour,
			PauseBehaviour
		}

		public Control control = Control.StartBehaviour;
		public bool waitActionFinish = true;

		protected override string info{
			get {return agentInfo + "." + control.ToString();}
		}

		protected override void OnExecute(){

			if (control == Control.StartBehaviour){
				if (waitActionFinish){
					agent.StartBehaviour( (s)=> { EndAction(s); } );
				} else {
					agent.StartBehaviour();
					EndAction();
				}
				return;
			}

			//in case target is this owner, we must yield 1 frame before pausing/stoppping
			if (agent == ownerAgent){ StartCoroutine(YieldDo()); }
			else { Do(); }
		}

		IEnumerator YieldDo(){
			yield return null;
			Do();
		}

		void Do(){
			if (control == Control.StopBehaviour){
				EndAction();
				agent.StopBehaviour();
			}

			if (control == Control.PauseBehaviour){
				EndAction();
				agent.PauseBehaviour();
			}			
		}

		protected override void OnStop(){
			if (waitActionFinish && control == Control.StartBehaviour){
				agent.StopBehaviour();
			}
		}
	}
}