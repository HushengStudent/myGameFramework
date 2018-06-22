using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("Physics")]
	[Description("Get hit info for ALL objects in the linecast, in Lists")]
	public class GetLinecastInfo2DAll : ActionTask<Transform> {

		[RequiredField]
		public BBParameter<GameObject> target;
		public LayerMask mask = -1;
		[BlackboardOnly]
		public BBParameter<List<GameObject>> saveHitGameObjectsAs;
		[BlackboardOnly]
		public BBParameter<List<float>> saveDistancesAs;
		[BlackboardOnly]
		public BBParameter<List<Vector3>> savePointsAs;
		[BlackboardOnly]
		public BBParameter<List<Vector3>> saveNormalsAs;

		private RaycastHit2D[] hits;

		protected override void OnExecute(){

            hits = Physics2D.LinecastAll(agent.position, target.value.transform.position, mask);

            if (hits.Length > 0) {
                saveHitGameObjectsAs.value = hits.Select(h => h.collider.gameObject).ToList();
                saveDistancesAs.value = hits.Select(h => h.fraction).ToList();
                savePointsAs.value = hits.Select(h => h.point).Cast<Vector3>().ToList();
                saveNormalsAs.value = hits.Select(h => h.normal).Cast<Vector3>().ToList();
                EndAction(true);
                return;
            }

            EndAction(false);
		}

		public override void OnDrawGizmosSelected(){
			if (agent && target.value)
				Gizmos.DrawLine(agent.position, target.value.transform.position);
		}	
	}
}