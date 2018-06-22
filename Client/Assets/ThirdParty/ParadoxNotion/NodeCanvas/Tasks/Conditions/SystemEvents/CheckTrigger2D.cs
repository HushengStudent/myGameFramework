using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions{

	[Category("System Events")]
	[Name("Check Trigger 2D")]
	[EventReceiver("OnTriggerEnter2D", "OnTriggerExit2D")]
	public class CheckTrigger2D : ConditionTask<Collider2D> {

		public TriggerTypes CheckType = TriggerTypes.TriggerEnter;
		public bool specifiedTagOnly;
		[TagField]
		public string objectTag = "Untagged";
		[BlackboardOnly]
		public BBParameter<GameObject> saveGameObjectAs;

		private bool stay;

		protected override string info{
			get {return CheckType.ToString() + ( specifiedTagOnly? (" '" + objectTag + "' tag") : "" );}
		}

		protected override bool OnCheck(){
			if (CheckType == TriggerTypes.TriggerStay)
				return stay;
			return false;
		}

		public void OnTriggerEnter2D(Collider2D other){
			
			if (!specifiedTagOnly || other.gameObject.tag == objectTag){
				stay = true;
				if (CheckType == TriggerTypes.TriggerEnter || CheckType == TriggerTypes.TriggerStay){
					saveGameObjectAs.value = other.gameObject;
					YieldReturn(true);
				}
			}
		}

		public void OnTriggerExit2D(Collider2D other){
			
			if (!specifiedTagOnly || other.gameObject.tag == objectTag){
				stay = false;
				if (CheckType == TriggerTypes.TriggerExit){
					saveGameObjectAs.value = other.gameObject;				
					YieldReturn(true);
				}
			}
		}
	}
}