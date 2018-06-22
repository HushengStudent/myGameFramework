using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using ParadoxNotion.Services;
using Logger = ParadoxNotion.Services.Logger;

namespace NodeCanvas.Tasks.Conditions{

	[Category("✫ Utility")]
	[Description("Check if an event is received and return true for one frame")]
	[EventReceiver("OnCustomEvent")]
	public class CheckEvent : ConditionTask<GraphOwner> {

		[RequiredField]
		public BBParameter<string> eventName;

		protected override string info{ get {return "[" + eventName.ToString() + "]"; } }
		protected override bool OnCheck(){ return false; }
		public void OnCustomEvent(EventData receivedEvent){
			if (isActive && receivedEvent.name.ToUpper() == eventName.value.ToUpper()){

				#if UNITY_EDITOR
				if (NodeCanvas.Editor.NCPrefs.logEvents){
					Logger.Log(string.Format("Event Received from ({0}): '{1}'", agent.gameObject.name, receivedEvent.name), "Event", this);
				}
				#endif			

				YieldReturn(true);
			}
		}
		
#if UNITY_EDITOR
		protected override void OnTaskInspectorGUI(){
			base.OnTaskInspectorGUI();
			if (Application.isPlaying && GUILayout.Button("Debug Receive Event")){
				SendEvent(eventName.value);
			}
		}
#endif

	}


	[Category("✫ Utility")]
	[Description("Check if an event is received and return true for one frame. Optionaly save the received event's value")]
	[EventReceiver("OnCustomEvent")]
	public class CheckEvent<T> : ConditionTask<GraphOwner> {

		[RequiredField]
		public BBParameter<string> eventName;
		[BlackboardOnly]
		public BBParameter<T> saveEventValue;

		protected override string info{ get {return string.Format("Event [{0}]\n{1} = EventValue", eventName, saveEventValue);} }
		protected override bool OnCheck(){ return false; }
		public void OnCustomEvent(EventData receivedEvent){
			if (isActive && receivedEvent.name.ToUpper() == eventName.value.ToUpper()){
				if (receivedEvent.value is T){
					saveEventValue.value = (T)receivedEvent.value;
				}

				#if UNITY_EDITOR
				if (NodeCanvas.Editor.NCPrefs.logEvents){
					Logger.Log(string.Format("Event Received from ({0}): '{1}'", agent.gameObject.name, receivedEvent.name), "Event", this);
				}
				#endif			
				
				YieldReturn(true);
			}
		}		

#if UNITY_EDITOR
		protected override void OnTaskInspectorGUI(){
			base.OnTaskInspectorGUI();
			if (Application.isPlaying && GUILayout.Button("Debug Receive Event")){
				SendEvent<T>(eventName.value, default(T));
			}
		}
#endif

	}
}