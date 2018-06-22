using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes{

	[Name("Animator")]
	[Category("Events/Object")]
	[Description("Calls Animator based events. Note that using this node will override root motion as usual, but you can call 'Apply Builtin Root Motion' to get it back.")]
	public class AnimatorEvents : MessageEventNode<Animator> {

		private FlowOutput onAnimatorMove;
		private FlowOutput onAnimatorIK;
		private Animator receiver;
		private int layerIndex;

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnAnimatorIK", "OnAnimatorMove" };
		}

		protected override void RegisterPorts(){
			onAnimatorMove = AddFlowOutput("On Animator Move");
			onAnimatorIK = AddFlowOutput("On Animator IK");
			AddValueOutput<Animator>("Receiver", ()=>{ return this.receiver; });
			AddValueOutput<int>("Layer Index", ()=>{ return layerIndex; });
		}

		void OnAnimatorMove(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.receiver = ResolveReceiver(msg.receiver);
			onAnimatorMove.Call(new Flow());
		}

		void OnAnimatorIK(ParadoxNotion.Services.MessageRouter.MessageData<int> msg){
			this.receiver = ResolveReceiver(msg.receiver);
			this.layerIndex = msg.value;
			onAnimatorIK.Call(new Flow());
		}
	}
}