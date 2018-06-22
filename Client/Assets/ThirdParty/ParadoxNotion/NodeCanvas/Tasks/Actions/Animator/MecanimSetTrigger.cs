using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Name("Set Parameter Trigger")]
	[Category("Animator")]
	[Description("You can either use a parameter name OR hashID. Leave the parameter name empty or none to use hashID instead.")]
	public class MecanimSetTrigger : ActionTask<Animator> {

		public BBParameter<string> parameter;
		public BBParameter<int> parameterHashID;

		protected override string info{
			get{ return string.Format("Mec.SetTrigger {0}", string.IsNullOrEmpty(parameter.value)? parameterHashID.ToString() : parameter.ToString() ); }			
		}

		protected override void OnExecute(){
			if (!string.IsNullOrEmpty(parameter.value)){
				agent.SetTrigger(parameter.value);
			} else {
				agent.SetTrigger(parameterHashID.value);
			}
			EndAction();
		}
	}
}