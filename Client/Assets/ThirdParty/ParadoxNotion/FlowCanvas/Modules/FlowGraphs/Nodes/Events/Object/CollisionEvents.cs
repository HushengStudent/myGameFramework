using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes
{

    [Name("Collision")]
    [Category("Events/Object")]
    [Description("Called when Collision based events happen on target and expose collision information")]
    public class CollisionEvents : MessageEventNode<Collider>
    {

        private FlowOutput onEnter;
        private FlowOutput onStay;
        private FlowOutput onExit;
        private Collider receiver;
        private Collision collision;

        protected override string[] GetTargetMessageEvents() {
            return new string[] { "OnCollisionEnter", "OnCollisionStay", "OnCollisionExit" };
        }

        protected override void RegisterPorts() {
            onEnter = AddFlowOutput("Enter");
            onStay = AddFlowOutput("Stay");
            onExit = AddFlowOutput("Exit");
            AddValueOutput<Collider>("Receiver", () => { return receiver; });
            AddValueOutput<GameObject>("Other", () => { return collision.gameObject; });
            AddValueOutput<ContactPoint>("Contact Point", () => { return collision.contacts[0]; });
            AddValueOutput<Collision>("Collision Info", () => { return collision; });
        }

        void OnCollisionEnter(ParadoxNotion.Services.MessageRouter.MessageData<Collision> msg) {
            this.receiver = ResolveReceiver(msg.receiver);
            this.collision = msg.value;
            onEnter.Call(new Flow());
        }

        void OnCollisionStay(ParadoxNotion.Services.MessageRouter.MessageData<Collision> msg) {
            this.receiver = ResolveReceiver(msg.receiver);
            this.collision = msg.value;
            onStay.Call(new Flow());
        }

        void OnCollisionExit(ParadoxNotion.Services.MessageRouter.MessageData<Collision> msg) {
            this.receiver = ResolveReceiver(msg.receiver);
            this.collision = msg.value;
            onExit.Call(new Flow());
        }
    }
}