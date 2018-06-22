using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

#if UNITY_5_5_OR_NEWER
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;
#endif

namespace NodeCanvas.Tasks.Actions{

	[Category("Movement/Pathfinding")]
	[Description("Move Randomly or Progressively between various game object positions taken from the list provided")]
	public class Patrol : ActionTask<NavMeshAgent> {

		public enum PatrolMode{
			Progressive,
			Random
		}

		[RequiredField]
		public BBParameter<List<GameObject>> targetList;
		public BBParameter<PatrolMode> patrolMode = PatrolMode.Random;
		public BBParameter<float> speed = 4;
		public float keepDistance = 0.1f;

		private int index = -1;
		private Vector3? lastRequest;

		protected override string info{
			get {return string.Format("{0} Patrol {1}", patrolMode, targetList);}
		}

		protected override void OnExecute(){

			if (targetList.value.Count == 0){
				EndAction(false);
				return;
			}

			if (targetList.value.Count == 1){

				index = 0;

			} else {

				if (patrolMode.value == PatrolMode.Random){
					var newIndex = index;
					while(newIndex == index){
						newIndex = Random.Range(0, targetList.value.Count);
					}
					index = newIndex;
				}

				if (patrolMode.value == PatrolMode.Progressive) {
					index = (int)Mathf.Repeat(index + 1, targetList.value.Count);
				}
			}

			var targetGo = targetList.value[index];
			if (targetGo == null){
				Debug.LogWarning("List's game object is null on MoveToFromList Action");
				EndAction(false);
				return;
			}

			var targetPos = targetGo.transform.position;

			agent.speed = speed.value;
			if ( (agent.transform.position - targetPos).magnitude < agent.stoppingDistance + keepDistance){
				EndAction(true);
				return;
			}
		}

		protected override void OnUpdate(){
			var targetPos = targetList.value[index].transform.position;
			if (lastRequest != targetPos){
				if ( !agent.SetDestination( targetPos) ){
					EndAction(false);
					return;
				}
			}

			lastRequest = targetPos;

			if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + keepDistance){
				EndAction(true);			
			}
		}

		protected override void OnPause(){ OnStop(); }
		protected override void OnStop(){
			if (lastRequest != null && agent.gameObject.activeSelf){
				agent.ResetPath();
			}
			lastRequest = null;
		}

		public override void OnDrawGizmosSelected(){
			if (agent && targetList.value != null){
				foreach (var go in targetList.value){
					if (go != null){
						Gizmos.DrawSphere(go.transform.position, 0.1f);
					}
				}
			}
		}
	}
}