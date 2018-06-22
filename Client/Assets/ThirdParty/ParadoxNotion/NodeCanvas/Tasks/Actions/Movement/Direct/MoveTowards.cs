using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions{

	[Category("Movement/Direct")]
	[Description("Moves the agent towards to target per frame without pathfinding")]
	public class MoveTowards : ActionTask<Transform> {

		[RequiredField]
		public BBParameter<GameObject> target;
		public BBParameter<float> speed = 2;
		public BBParameter<float> stopDistance = 0.1f;
		public bool waitActionFinish;

		protected override void OnUpdate(){
			if ( (agent.position - target.value.transform.position).magnitude <= stopDistance.value ){
				EndAction();
				return;
			}

			agent.position = Vector3.MoveTowards(agent.position, target.value.transform.position, speed.value * Time.deltaTime);
			if (!waitActionFinish){
				EndAction();
			}
		}
	}
}