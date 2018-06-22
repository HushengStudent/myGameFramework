using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Name("Set IK")]
	[Category("Animator")]
	[EventReceiver("OnAnimatorIK")]
	public class MecanimSetIK : ActionTask<Animator> {

		public AvatarIKGoal IKGoal;
		[RequiredField]
		public BBParameter<GameObject> goal;
		public BBParameter<float> weight;

		protected override string info{
			get{return "Set '" + IKGoal + "' " + goal;}
		}

		public void OnAnimatorIK(){

			agent.SetIKPositionWeight(IKGoal, weight.value);
			agent.SetIKPosition(IKGoal, goal.value.transform.position);
			EndAction();
		}
	}
}