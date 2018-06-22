using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;

namespace FlowCanvas.Nodes{

	[Name("Mouse2D")]
	[Category("Events/Object")]
	[Description("Called when mouse based operations happen on target 2D collider")]
	public class MouseAgent2DEvents : MessageEventNode<Collider2D> {

		private FlowOutput onEnter;
		private FlowOutput onOver;
		private FlowOutput onExit;
		private FlowOutput onDown;
		private FlowOutput onUp;
		private FlowOutput onDrag;

		private Collider2D receiver;
		private RaycastHit2D hit;

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
			AddValueOutput<Collider2D>("Receiver", ()=>{ return receiver; });
			AddValueOutput<RaycastHit2D>("Info", ()=>{ return hit; });
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
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
		}
	}
}