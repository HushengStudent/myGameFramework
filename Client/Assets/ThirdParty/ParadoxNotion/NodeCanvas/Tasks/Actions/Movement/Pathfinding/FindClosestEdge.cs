using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

#if UNITY_5_5_OR_NEWER
using NavMesh = UnityEngine.AI.NavMesh;
using NavMeshHit = UnityEngine.AI.NavMeshHit;
#endif

namespace NodeCanvas.Tasks.Actions{

	[Name("Find Closest NavMesh Edge")]
	[Category("Movement/Pathfinding")]
	[Description("Find the closes Navigation Mesh position to the target position")]
	public class FindClosestEdge : ActionTask {

		public BBParameter<Vector3> targetPosition;
		[BlackboardOnly]
		public BBParameter<Vector3> saveFoundPosition;

		private NavMeshHit hit;

		protected override void OnExecute(){
			if (NavMesh.FindClosestEdge(targetPosition.value, out hit, -1)){
				saveFoundPosition.value = hit.position;
				EndAction(true);
			}

			EndAction(false);
		}
	}
}