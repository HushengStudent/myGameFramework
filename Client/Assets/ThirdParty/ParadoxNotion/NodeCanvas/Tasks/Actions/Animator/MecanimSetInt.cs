using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Name("Set Parameter Integer")]
	[Category("Animator")]
	[Description("You can either use a parameter name OR hashID. Leave the parameter name empty or none to use hashID instead.")]
	public class MecanimSetInt : ActionTask<Animator> {

		public BBParameter<string> parameter;
		public BBParameter<int> parameterHashID;
		public BBParameter<int> setTo;

		protected override string info{
			get{ return string.Format("Mec.SetInt {0} to {1}", string.IsNullOrEmpty(parameter.value)? parameterHashID.ToString() : parameter.ToString(), setTo ); }
		}

		protected override void OnExecute(){
			if (!string.IsNullOrEmpty(parameter.value)){
				agent.SetInteger(parameter.value, setTo.value);
			} else {
				agent.SetInteger(parameterHashID.value, setTo.value);
			}
			EndAction();
		}
	}
}