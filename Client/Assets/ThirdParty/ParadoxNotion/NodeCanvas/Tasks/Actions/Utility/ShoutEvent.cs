using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions{

	[Category("✫ Utility")]
	[Description("Sends an event to all GraphOwners within range of the agent and over time like a shockwave.")]
	public class ShoutEvent : ActionTask<Transform> {

		[RequiredField]
		public BBParameter<string> eventName;
		public BBParameter<float> shoutRange = 10;
		public BBParameter<float> completionTime = 1;

		private GraphOwner[] owners;
		private List<GraphOwner> receivedOwners = new List<GraphOwner>();
		private float traveledDistance;

		protected override string info{
			get { return string.Format("Shout Event [{0}]", eventName.ToString()); }
		}

		protected override void OnExecute(){
			owners = Object.FindObjectsOfType<GraphOwner>();
			receivedOwners.Clear();
		}

		protected override void OnUpdate(){

			traveledDistance = Mathf.Lerp(0, shoutRange.value, elapsedTime/completionTime.value );
			foreach (var owner in owners){
				var distance = (agent.position - owner.transform.position).magnitude;
				if ( distance <= traveledDistance && !receivedOwners.Contains(owner) ){
					owner.SendEvent(eventName.value);
					receivedOwners.Add(owner);
				}
			}

			if (elapsedTime >= completionTime.value){
				EndAction();
			}
		}

		public override void OnDrawGizmosSelected(){
			if (agent != null){
				Gizmos.color = new Color(1,1,1,0.2f);
				Gizmos.DrawWireSphere(agent.position, traveledDistance);
				Gizmos.DrawWireSphere(agent.position, shoutRange.value);
			}			
		}
	}
}