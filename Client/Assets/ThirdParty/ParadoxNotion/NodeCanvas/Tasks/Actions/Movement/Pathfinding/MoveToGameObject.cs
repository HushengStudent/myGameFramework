using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

#if UNITY_5_5_OR_NEWER
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;
#endif

namespace NodeCanvas.Tasks.Actions{

	[Name("Seek (GameObject)")]
	[Category("Movement/Pathfinding")]
	public class MoveToGameObject : ActionTask<NavMeshAgent> {

		[RequiredField]
		public BBParameter<GameObject> target;
		public BBParameter<float> speed = 4;
		public float keepDistance = 0.1f;

		private Vector3? lastRequest;

		protected override string info{
			get {return "Seek " + target;}
		}

		protected override void OnExecute(){
			agent.speed = speed.value;
			if ( Vector3.Distance(agent.transform.position, target.value.transform.position) < agent.stoppingDistance + keepDistance ){
				EndAction(true);
				return;
			}
		}

		protected override void OnUpdate(){
			var pos = target.value.transform.position;
			if (lastRequest != pos){
				if ( !agent.SetDestination( pos) ){
					EndAction(false);
					return;
				}
			}

			lastRequest = pos;

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

		public override void OnDrawGizmos(){
			if (target.value != null){
				Gizmos.DrawWireSphere(target.value.transform.position, keepDistance);
			}
		}
	}
}