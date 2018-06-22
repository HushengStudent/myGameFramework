using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions{

	[Category("✫ Utility")]
	[Description("Send a Graph Event to multiple gameobjects which should have a GraphOwner component attached.")]
	public class SendEventToObjects : ActionTask{

		[RequiredField]
		public BBParameter<List<GameObject>> targetObjects;
		[RequiredField]
		public BBParameter<string> eventName;

		protected override string info{
			get {return string.Format("Send Event [{0}] to {1}", eventName, targetObjects);}
		}

		protected override void OnExecute(){
			
			foreach(var target in targetObjects.value){
				if (target != null){
					var owner = target.GetComponent<GraphOwner>();
					if (owner != null){
						owner.SendEvent(eventName.value);
					}
				}
			}

			EndAction();
		}
	}

	[Category("✫ Utility")]
	[Description("Send a Graph Event to multiple gameobjects which should have a GraphOwner component attached.")]
	public class SendEventToObjects<T> : ActionTask{

		[RequiredField]
		public BBParameter<List<GameObject>> targetObjects;
		[RequiredField]
		public BBParameter<string> eventName;
		public BBParameter<T> eventValue;

		protected override string info{
			get {return string.Format("Send Event [{0}]({1}) to {2}", eventName, eventValue, targetObjects);}
		}

		protected override void OnExecute(){
			
			foreach(var target in targetObjects.value){
				var owner = target.GetComponent<GraphOwner>();
				if (owner != null){
					owner.SendEvent<T>(eventName.value, eventValue.value);
				}
			}

			EndAction();
		}
	}
}