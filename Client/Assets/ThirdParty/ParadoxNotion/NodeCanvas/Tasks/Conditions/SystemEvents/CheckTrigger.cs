using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions{

	[Category("System Events")]
	[EventReceiver("OnTriggerEnter", "OnTriggerExit")]
	public class CheckTrigger : ConditionTask<Collider> {

		public TriggerTypes checkType = TriggerTypes.TriggerEnter;
		public bool specifiedTagOnly;
		[TagField] [ShowIf("specifiedTagOnly", 1)]
		public string objectTag = "Untagged";
		[BlackboardOnly]
		public BBParameter<GameObject> saveGameObjectAs;

		private bool stay;

		protected override string info{
			get {return checkType.ToString() + ( specifiedTagOnly? (" '" + objectTag + "' tag") : "" );}
		}

		protected override bool OnCheck(){
			if (checkType == TriggerTypes.TriggerStay)
				return stay;
			return false;
		}

		public void OnTriggerEnter(Collider other){
			if (!specifiedTagOnly || other.gameObject.tag == objectTag){
				stay = true;
				if (checkType == TriggerTypes.TriggerEnter || checkType == TriggerTypes.TriggerStay){
					saveGameObjectAs.value = other.gameObject;
					YieldReturn(true);
				}
			}
		}

		public void OnTriggerExit(Collider other){
			if (!specifiedTagOnly || other.gameObject.tag == objectTag){
				stay = false;
				if (checkType == TriggerTypes.TriggerExit){
					saveGameObjectAs.value = other.gameObject;				
					YieldReturn(true);
				}
			}
		}
	}
}