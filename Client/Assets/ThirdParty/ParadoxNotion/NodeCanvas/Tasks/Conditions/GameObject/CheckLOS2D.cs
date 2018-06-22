using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions{

	[Name("Target In Line Of Sight 2D")]
	[Category("GameObject")]
	[Description("Check of agent is in line of sight with target by doing a linecast and optionaly save the distance")]
	public class CheckLOS2D : ConditionTask<Transform> {

		[RequiredField]
		public BBParameter<GameObject> LOSTarget;
		public BBParameter<LayerMask> layerMask = (LayerMask)(-1);
		[BlackboardOnly]
		public BBParameter<float> saveDistanceAs;

		[GetFromAgent]
		protected Collider2D agentCollider;
		private RaycastHit2D[] hits;

		protected override string info{
			get {return "LOS with " + LOSTarget.ToString();}
		}

		protected override bool OnCheck(){
			hits = Physics2D.LinecastAll(agent.position, LOSTarget.value.transform.position, layerMask.value);
			foreach (var collider in hits.Select(h => h.collider)){
				if (collider != agentCollider && collider != LOSTarget.value.GetComponent<Collider2D>()){
					return false;
				}
			}
			return true;
		}

		public override void OnDrawGizmosSelected(){
			if (agent && LOSTarget.value)
				Gizmos.DrawLine(agent.position, LOSTarget.value.transform.position);
		}
	}
}