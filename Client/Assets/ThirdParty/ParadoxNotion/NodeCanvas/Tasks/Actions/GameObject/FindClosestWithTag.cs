using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions{

	[Category("GameObject")]
	[Description("Find the closest game object of tag to the agent")]
	public class FindClosestWithTag : ActionTask<Transform> {

		[TagField] [RequiredField]
		public BBParameter<string> searchTag;
		public BBParameter<bool> ignoreChildren;
		[BlackboardOnly]
		public BBParameter<GameObject> saveObjectAs;
		[BlackboardOnly]
		public BBParameter<float> saveDistanceAs;

		protected override void OnExecute(){

			var found = GameObject.FindGameObjectsWithTag(searchTag.value).ToList();
			if (found.Count == 0){
				saveObjectAs.value = null;
				saveDistanceAs.value = 0;
				EndAction(false);
				return;
			}

			GameObject closest = null;
			var dist = Mathf.Infinity;
			foreach (var go in found){
				
				if (go.transform == agent){
					continue;
				}

				if (ignoreChildren.value && go.transform.IsChildOf(agent)){
					continue;
				}

				var newDist = Vector3.Distance(go.transform.position, agent.position);
				if (newDist < dist){
					dist = newDist;
					closest = go;
				}
			}

			saveObjectAs.value = closest;
			saveDistanceAs.value = dist;
			EndAction();
		}
	}
}