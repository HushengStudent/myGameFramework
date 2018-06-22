using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;

namespace FlowCanvas.Nodes{

	[Name("Mouse")]
	[Category("Events/Object")]
	[Description("Called when mouse based operations happen on target collider")]
	public class MouseAgentEvents : MessageEventNode<Collider> {

		private FlowOutput onEnter;
		private FlowOutput onOver;
		private FlowOutput onExit;
		private FlowOutput onDown;
		private FlowOutput onUp;
		private FlowOutput onDrag;
		
		private Collider receiver;
		private RaycastHit hit;

		protected override string[] GetTargetMessageEvents(){
			return new string[]{"OnMouseEnter", "OnMouseOver", "OnMouseExit", "OnMouseDown", "OnMouseUp", "OnMouseDrag"};
		}

		protected override void RegisterPorts(){
			onDown  = AddFlowOutput("Down");
			onUp    = AddFlowOutput("Up");
			onEnter = AddFlowOutput("Enter");
			onOver  = AddFlowOutput("Over");
			onExit  = AddFlowOutput("Exit");
			onDrag  = AddFlowOutput("Drag");
			AddValueOutput<Collider>("Receiver", ()=>{ return receiver; });
			AddValueOutput<RaycastHit>("Info", ()=>{ return hit; });
		}

		void OnMouseEnter(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.receiver = ResolveReceiver(msg.receiver);
			StoreHit();
			onEnter.Call(new Flow());
		}

		void OnMouseOver(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.receiver = ResolveReceiver(msg.receiver);
			StoreHit();
			onOver.Call(new Flow());
		}

		void OnMouseExit(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.receiver = ResolveReceiver(msg.receiver);
			StoreHit();
			onExit.Call(new Flow());
		}

		void OnMouseDown(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.receiver = ResolveReceiver(msg.receiver);
			StoreHit();
			onDown.Call(new Flow());
		}

		void OnMouseUp(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.receiver = ResolveReceiver(msg.receiver);
			StoreHit();
			onUp.Call(new Flow());
		}

		void OnMouseDrag(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.receiver = ResolveReceiver(msg.receiver);
			StoreHit();
			onDrag.Call(new Flow());
		}

		void StoreHit(){
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity);
		}
	}
}