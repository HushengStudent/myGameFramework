using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("✫ Utility")]
	[Description("Logs the value of a variable in the console")]
	public class DebugLogVariable : ActionTask{

		[BlackboardOnly]
		public BBParameter<object> log;
		public BBParameter<string> prefix;
		public float secondsToRun = 1f;
		public CompactStatus finishStatus = CompactStatus.Success;

		protected override string info{
			get {return "Log '" + log + "'" + (secondsToRun > 0? " for " + secondsToRun + " sec." : ""); }
		}

		protected override void OnExecute(){
			Debug.Log(string.Format("<b>({0}) ({1}) | Var '{2}' = </b> {3}", agent.gameObject.name, prefix.value, log.name, log.value ), agent.gameObject );
		}

		protected override void OnUpdate(){
			if (elapsedTime >= secondsToRun){
				EndAction(finishStatus == CompactStatus.Success? true : false );
			}
		}
	}
}