using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;
using Logger = ParadoxNotion.Services.Logger;

namespace FlowCanvas.Nodes{

	[Name("Custom Event", 100)]
	[Description("Called when a custom event is received on target.\n- To send an event from a graph use the SendEvent node.\n- To send an event from code use:'FlowScriptController.SendEvent(string)'")]
	[Category("Events/Custom")]
	public class CustomEvent : MessageEventNode<GraphOwner> {

		[RequiredField] [DelayedField]
		public BBParameter<string> eventName = "EventName";

		private FlowOutput onReceived;
		private GraphOwner receiver;

		public override string name{
			get {return base.name + string.Format(" [ <color=#DDDDDD>{0}</color> ]", eventName); }
		}

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnCustomEvent" };
		}

		protected override void RegisterPorts(){
			onReceived = AddFlowOutput("Received");
			AddValueOutput<GraphOwner>("Receiver", ()=>{ return receiver; });
		}

		public void OnCustomEvent(ParadoxNotion.Services.MessageRouter.MessageData<EventData> msg){
			if (msg.value.name == eventName.value){
				this.receiver = ResolveReceiver(msg.receiver);
				
				#if UNITY_EDITOR
				if (NodeCanvas.Editor.NCPrefs.logEvents){
					Logger.Log(string.Format("Event Received from ({0}): '{1}'", receiver.name, msg.value.name), "Event", this);
				}
				#endif

				onReceived.Call(new Flow());
			}
		}
	}


	[Name("Custom Event", 100)]
	[Description("Called when a custom event is received on target.\n- To send an event from a graph use the SendEvent node.\n- To send an event from code use:'FlowScriptController.SendEvent(string)'")]
	[Category("Events/Custom")]
	[ContextDefinedOutputs(typeof(Wild))]
	public class CustomEvent<T> : MessageEventNode<GraphOwner> {

		[RequiredField]
		public BBParameter<string> eventName = "EventName";
		private FlowOutput onReceived;
		private T receivedValue;
		private GraphOwner receiver;

		public override string name{
			get {return base.name + string.Format(" [ <color=#DDDDDD>{0}</color> ]", eventName); }
		}

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnCustomEvent" };
		}

		protected override void RegisterPorts(){
			onReceived = AddFlowOutput("Received");
			AddValueOutput<GraphOwner>("Receiver", ()=>{ return receiver; });
			AddValueOutput<T>("Event Value", ()=> { return receivedValue; });
		}

		public void OnCustomEvent(ParadoxNotion.Services.MessageRouter.MessageData<EventData> msg){
			if (msg.value.name == eventName.value){
				this.receiver = ResolveReceiver(msg.receiver);
				if (msg.value is EventData<T>){
					receivedValue = (msg.value as EventData<T>).value;
				}

				#if UNITY_EDITOR
				if (NodeCanvas.Editor.NCPrefs.logEvents){
					Logger.Log(string.Format("Event Received from ({0}): '{1}'", receiver.name, msg.value.name), "Event", this);
				}
				#endif

				onReceived.Call(new Flow());
			}
		}
	}

}