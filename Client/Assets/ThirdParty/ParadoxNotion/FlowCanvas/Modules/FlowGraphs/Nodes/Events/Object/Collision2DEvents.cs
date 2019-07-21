using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes
{

    [Name("Collision2D")]
    [Category("Events/Object")]
    [Description("Called when 2D Collision based events happen on target and expose collision information")]
    public class Collision2DEvents : MessageEventNode<Collider2D>
    {

        private FlowOutput onEnter;
        private FlowOutput onStay;
        private FlowOutput onExit;
        private Collider2D receiver;
        private Collision2D collision;

        protected override string[] GetTargetMessageEvents() {
            return new string[] { "OnCollisionEnter2D", "OnCollisionStay2D", "OnCollisionExit2D" };
        }

        protected override void RegisterPorts() {
            onEnter = AddFlowOutput("Enter");
            onStay = AddFlowOutput("Stay");
            onExit = AddFlowOutput("Exit");
            AddValueOutput<Collider2D>("Receiver", () => { return receiver; });
            AddValueOutput<GameObject>("Other", () => { return collision.gameObject; });
            AddValueOutput<ContactPoint2D>("Contact Point", () => { return collision.contacts[0]; });
            AddValueOutput<Collision2D>("Collision Info", () => { return collision; });
        }

        void OnCollisionEnter2D(ParadoxNotion.Services.MessageRouter.MessageData<Collision2D> msg) {
            this.receiver = ResolveReceiver(msg.receiver);
            this.collision = msg.value;
            onEnter.Call(new Flow());
        }

        void OnCollisionStay2D(ParadoxNotion.Services.MessageRouter.MessageData<Collision2D> msg) {
            this.receiver = ResolveReceiver(msg.receiver);
            this.collision = msg.value;
            onStay.Call(new Flow());
        }

        void OnCollisionExit2D(ParadoxNotion.Services.MessageRouter.MessageData<Collision2D> msg) {
            this.receiver = ResolveReceiver(msg.receiver);
            this.collision = msg.value;
            onExit.Call(new Flow());
        }
    }
}