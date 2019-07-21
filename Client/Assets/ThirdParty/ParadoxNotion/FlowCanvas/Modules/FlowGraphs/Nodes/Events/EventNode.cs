using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{

    [Category("Events")]
    [Color("ff5c5c")]
    [ContextDefinedOutputs(typeof(Flow))]
    [ExecutionPriority(1)]
    ///Base class for event nodes.
    abstract public class EventNode : FlowNode
    {
        public override string name {
            get { return string.Format("➥ {0}", base.name.ToUpper()); }
        }
    }

    ///Base class for event nodes that require a specific target Component or GameObject(use Transform for GameObjects).
    [ContextDefinedOutputs(typeof(Wild))]
    abstract public class EventNode<T> : EventNode where T : Component
    {

        public BBParameter<T> target;

        public override string name {
            get { return string.Format("{0} ({1})", base.name.ToUpper(), target.isNull && !target.useBlackboard ? "Self" : target.ToString()); }
        }

        public override void OnGraphStarted() {
            ResolveSelf();
        }

        ///Resolve component from Self if 'target' is null
        protected void ResolveSelf() {
            if ( target.isNull && !target.useBlackboard ) {
                target.value = graphAgent.GetComponent<T>();
            }
        }
    }

    ///Base class for event nodes with single or multiple target Component(s) that work with MonoBehaviour-based message callbacks (use Transform for GameObjects).
    [ContextDefinedOutputs(typeof(Wild))]
    abstract public class MessageEventNode<T> : EventNode where T : Component
    {

        public enum TargetMode
        {
            SingleTarget = 0,
            MultipleTargets = 1
        }

        public TargetMode targetMode;
        [ShowIf("targetMode", 0)]
        public BBParameter<T> target;
        [ShowIf("targetMode", 1)]
        public BBParameter<List<T>> targets;

        public override string name {
            get
            {
                string text = string.Empty;
                if ( targetMode == TargetMode.SingleTarget ) {
                    text = target.isNull && !target.useBlackboard ? "Self" : target.ToString();
                } else {
                    text = targets.ToString();
                }
                return string.Format("{0} ({1})", base.name.ToUpper(), text);
            }
        }

        ///The event message names to subscribe on the target agent. Null if none required.
        abstract protected string[] GetTargetMessageEvents();

        sealed public override void OnGraphStarted() {
            //SINGLE TARGET
            if ( targetMode == TargetMode.SingleTarget ) {
                if ( target.isNull && !target.useBlackboard ) {
                    target.value = graphAgent.GetComponent<T>();
                }

                if ( target.isNull ) {
                    Fail(string.Format("Target is missing component of type '{0}'", typeof(T).Name));
                    return;
                }

                var targetEvents = GetTargetMessageEvents();
                if ( targetEvents != null && targetEvents.Length != 0 ) {
                    RegisterEvents(target.value, targetEvents);
                }
            }

            //MULTIPLE TARGETS
            if ( targetMode == TargetMode.MultipleTargets ) {
                if ( targets.isNull || targets.value.Count == 0 ) {
                    Fail("No Targets specified");
                    return;
                }

                var targetEvents = GetTargetMessageEvents();
                if ( targetEvents != null && targetEvents.Length != 0 ) {
                    foreach ( var target in targets.value ) {
                        RegisterEvents(target, targetEvents);
                    }
                }
            }
        }

        sealed public override void OnGraphStoped() {
            //SINGLE TARGET
            if ( targetMode == TargetMode.SingleTarget ) {
                UnRegisterEvents(target.value, GetTargetMessageEvents());
            }

            //MULTIPLE TARGETS
            if ( targetMode == TargetMode.MultipleTargets ) {
                var targetEvents = GetTargetMessageEvents();
                foreach ( var target in targets.value ) {
                    UnRegisterEvents(target, targetEvents);
                }
            }
        }

        //Utility to resolve sender T object for better performance
        protected T ResolveReceiver(GameObject receiver) {
            if ( targetMode == TargetMode.SingleTarget ) {
                return target.value;
            }
            return targets.value.Find(t => t.gameObject == receiver);
        }
    }
}