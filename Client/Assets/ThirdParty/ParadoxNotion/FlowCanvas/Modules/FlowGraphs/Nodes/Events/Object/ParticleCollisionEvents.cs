#if UNITY_5_4_OR_NEWER
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

namespace FlowCanvas.Nodes
{

    [Name("Particle Collision")]
    [Category("Events/Object")]
    [Description("Called when any Particle System collided with the target collider object")]
    public class ParticleCollisionEvents : MessageEventNode<Collider>
    {

        private FlowOutput onCollision;
        private Collider receiver;
        private ParticleSystem particle;
        private List<ParticleCollisionEvent> collisionEvents;

        protected override string[] GetTargetMessageEvents() {
            return new string[] { "OnParticleCollision" };
        }

        protected override void RegisterPorts() {
            onCollision = AddFlowOutput("On Particle Collision");
            AddValueOutput<Collider>("Receiver", () => { return receiver; });
            AddValueOutput<ParticleSystem>("Particle System", () => { return particle; });
            AddValueOutput<Vector3>("Collision Point", () => { return collisionEvents[0].intersection; });
            AddValueOutput<Vector3>("Collision Normal", () => { return collisionEvents[0].normal; });
            AddValueOutput<Vector3>("Collision Velocity", () => { return collisionEvents[0].velocity; });
        }

        void OnParticleCollision(ParadoxNotion.Services.MessageRouter.MessageData<GameObject> msg) {
            this.receiver = ResolveReceiver(msg.receiver);
            this.particle = msg.value.GetComponent<ParticleSystem>();
            this.collisionEvents = new List<ParticleCollisionEvent>();
            if ( particle != null ) {
                particle.GetCollisionEvents(receiver.gameObject, collisionEvents);
            }
            onCollision.Call(new Flow());
        }
    }
}
#endif