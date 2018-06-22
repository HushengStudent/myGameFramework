using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Name("Set Parameter Float")]
	[Category("Animator")]
	[Description("You can either use a parameter name OR hashID. Leave the parameter name empty or none to use hashID instead.")]
	public class MecanimSetFloat : ActionTask<Animator> {

		public BBParameter<string> parameter;
		public BBParameter<int> parameterHashID;
		public BBParameter<float> setTo;
		[SliderField(0, 1)]
		public float transitTime = 0.25f;

		private float currentValue;

		protected override string info{
			get{ return string.Format("Mec.SetFloat {0} to {1}", string.IsNullOrEmpty(parameter.value)? parameterHashID.ToString() : parameter.ToString(), setTo ); }
		}


		protected override void OnExecute(){

			if (transitTime <= 0){
				Set(setTo.value);
				EndAction();
				return;
			}

			currentValue = Get();
		}

		protected override void OnUpdate(){
			Set(Mathf.Lerp(currentValue, setTo.value, elapsedTime/transitTime));
			if (elapsedTime >= transitTime){
				EndAction(true);
			}
		}

		float Get(){
			if (!string.IsNullOrEmpty(parameter.value)){
				return agent.GetFloat(parameter.value);
			}
			return agent.GetFloat(parameterHashID.value);
		}

		void Set(float newValue){
			if (!string.IsNullOrEmpty(parameter.value)){
				agent.SetFloat(parameter.value, newValue);
				return;
			}
			agent.SetFloat(parameterHashID.value, newValue);
		}
	}
}