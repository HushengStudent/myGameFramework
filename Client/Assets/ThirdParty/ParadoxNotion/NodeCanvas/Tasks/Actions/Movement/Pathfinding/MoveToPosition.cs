using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

#if UNITY_5_5_OR_NEWER
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;
#endif

namespace NodeCanvas.Tasks.Actions{

	[Name("Seek (Vector3)")]
	[Category("Movement/Pathfinding")]
	public class MoveToPosition : ActionTask<NavMeshAgent> {

		public BBParameter<Vector3> targetPosition;
		public BBParameter<float> speed = 4;
		public float keepDistance = 0.1f;

		private Vector3? lastRequest;

		protected override string info{
			get {return "Seek " + targetPosition;}
		}

		protected override void OnExecute(){
			agent.speed = speed.value;
			if ( Vector3.Distance(agent.transform.position, targetPosition.value) < agent.stoppingDistance + keepDistance ){
				EndAction(true);
				return;
			}
		}

		protected override void OnUpdate(){
			if (lastRequest != targetPosition.value){
				if ( !agent.SetDestination( targetPosition.value) ){
					EndAction(false);
					return;
				}
			}

			lastRequest = targetPosition.value;

			if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + keepDistance){
				EndAction(true);
			}
		}

		protected override void OnStop(){
			if (lastRequest != null && agent.gameObject.activeSelf){
				agent.ResetPath();
			}
			lastRequest = null;
		}

		protected override void OnPause(){
			OnStop();
		}
	}
}