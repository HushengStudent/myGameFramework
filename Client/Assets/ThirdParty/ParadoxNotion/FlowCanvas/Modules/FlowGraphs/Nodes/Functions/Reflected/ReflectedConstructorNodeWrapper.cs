using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;

namespace FlowCanvas.Nodes
{
    ///Wraps a ConstructorInfo into a FlowGraph node
    public class ReflectedConstructorNodeWrapper : ReflectedMethodBaseNodeWrapper
    {

        [SerializeField]
        private SerializedConstructorInfo _constructor;

        protected override SerializedMethodBaseInfo serializedMethodBase {
            get { return _constructor; }
        }

        private BaseReflectedConstructorNode reflectedConstructorNode { get; set; }
        private ConstructorInfo constructor {
            get { return _constructor != null ? _constructor.Get() : null; }
        }

        public override string name {
            get
            {
                if ( constructor != null ) {
                    return string.Format("New {0} ()", constructor.DeclaringType.FriendlyName());
                }
                if ( _constructor != null ) {
                    return string.Format("<color=#ff6457>* Missing Function *\n{0}</color>", _constructor.GetMethodString());
                }
                return "NOT SET";
            }
        }

#if UNITY_EDITOR
        public override string description {
            get { return constructor != null ? DocsByReflection.GetMemberSummary(constructor.DeclaringType) : "Missing Constructor"; }
        }
#endif

        ///Set a new ConstructorInfo to be used by ReflectedConstructorNode
        public override void SetMethodBase(MethodBase newMethod, object instance = null) {
            if ( newMethod is ConstructorInfo ) {
                SetConstructor((ConstructorInfo)newMethod);
            }
        }

        ///Set a new ConstructorInfo to be used by ReflectedConstructorNode
        void SetConstructor(ConstructorInfo newConstructor) {
            _constructor = new SerializedConstructorInfo(newConstructor);
            GatherPorts();

            base.SetDefaultParameterValues(newConstructor);
        }

        protected override void RegisterPorts() {
            if ( constructor == null ) {
                return;
            }

            var options = new ReflectedMethodRegistrationOptions();
            options.callable = callable;
            options.exposeParams = exposeParams;
            options.exposedParamsCount = exposedParamsCount;

            reflectedConstructorNode = BaseReflectedConstructorNode.GetConstructorNode(constructor, options);
            if ( reflectedConstructorNode != null ) {
                reflectedConstructorNode.RegisterPorts(this, options);
            }
        }
    }
}