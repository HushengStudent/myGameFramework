using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace FlowCanvas.Nodes{

	[Name("UI Pointer")]
	[Category("Events/Object/UI")]
	[Description("Calls UI Pointer based events on target. The Unity Event system has to be set through 'GameObject/UI/Event System'")]
	public class UIPointerEvents : MessageEventNode<Transform> {

		private FlowOutput onPointerDown;
		private FlowOutput onPointerPressed;
		private FlowOutput onPointerUp;
		private FlowOutput onPointerEnter;
		private FlowOutput onPointerExit;
		private FlowOutput onPointerClick;
		private GameObject receiver;
		private PointerEventData eventData;

		private bool updatePressed = false;

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnPointerEnter", "OnPointerExit", "OnPointerDown", "OnPointerUp", "OnPointerClick" };
		}

		protected override void RegisterPorts(){
			onPointerClick = AddFlowOutput("Click");
			onPointerDown  = AddFlowOutput("Down");
			onPointerPressed= AddFlowOutput("Pressed");
			onPointerUp    = AddFlowOutput("Up");
			onPointerEnter = AddFlowOutput("Enter");
			onPointerExit  = AddFlowOutput("Exit");
			AddValueOutput<GameObject>("Receiver", ()=>{ return receiver; });
			AddValueOutput<PointerEventData>("Event Data", ()=> { return eventData; });
		}

		void OnPointerDown(ParadoxNotion.Services.MessageRouter.MessageData<PointerEventData> msg){
			this.receiver = ResolveReceiver(msg.receiver).gameObject;
			this.eventData = msg.value;
			onPointerDown.Call(new Flow());
			updatePressed = true;
			StartCoroutine(UpdatePressed());
		}

		void OnPointerUp(ParadoxNotion.Services.MessageRouter.MessageData<PointerEventData> msg){
			this.receiver = ResolveReceiver(msg.receiver).gameObject;
			this.eventData = msg.value;
			onPointerUp.Call(new Flow());
			updatePressed = false;
		}


		IEnumerator UpdatePressed(){
			while(updatePressed){
				onPointerPressed.Call(new Flow());
				yield return null;
			}
		}

		void OnPointerEnter(ParadoxNotion.Services.MessageRouter.MessageData<PointerEventData> msg){
			this.receiver = ResolveReceiver(msg.receiver).gameObject;
			this.eventData = msg.value;
			onPointerEnter.Call(new Flow());
		}

		void OnPointerExit(ParadoxNotion.Services.MessageRouter.MessageData<PointerEventData> msg){
			this.receiver = ResolveReceiver(msg.receiver).gameObject;
			this.eventData = msg.value;
			onPointerExit.Call(new Flow());
		}

		void OnPointerClick(ParadoxNotion.Services.MessageRouter.MessageData<PointerEventData> msg){
			this.receiver = ResolveReceiver(msg.receiver).gameObject;
			this.eventData = msg.value;
			onPointerClick.Call(new Flow());
		}
	}
}