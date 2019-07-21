using System.Reflection;
using System.Linq;
using ParadoxNotion;
using ParadoxNotion.Serialization;
using UnityEngine;
using FlowCanvas.Nodes.Legacy;

namespace FlowCanvas.Nodes
{
    ///Wraps a MethodInfo into a FlowGraph node
    public class ReflectedMethodNodeWrapper : ReflectedMethodBaseNodeWrapper
    {

        [SerializeField]
        private SerializedMethodInfo _method;

        protected override SerializedMethodBaseInfo serializedMethodBase {
            get { return _method; }
        }

        private BaseReflectedMethodNode reflectedMethodNode { get; set; }

        private MethodInfo method {
            get { return _method != null ? _method.Get() : null; }
        }

        public override string name {
            get
            {
                if ( method != null ) {
                    var specialType = ReflectionTools.MethodType.Normal;
                    var methodName = method.FriendlyName(out specialType);
                    if ( specialType == ReflectionTools.MethodType.Operator ) {
                        ReflectionTools.op_FriendlyNamesShort.TryGetValue(method.Name, out methodName);
                        return methodName;
                    }
                    methodName = methodName.SplitCamelCase();
                    if ( method.IsGenericMethod ) {
                        methodName += string.Format(" ({0})", method.GetGenericArguments().First().FriendlyName());
                    }
                    if ( !method.IsStatic || method.IsExtensionMethod() ) {
                        return methodName;
                    }
                    return string.Format("{0}.{1}", method.DeclaringType.FriendlyName(), methodName);
                }
                if ( _method != null ) {
                    return string.Format("<color=#ff6457>* Missing Function *\n{0}</color>", _method.GetMethodString());
                }
                return "NOT SET";
            }
        }

        ///Set a new MethodInfo to be used by ReflectedMethodNode
        public override void SetMethodBase(MethodBase newMethod, object instance = null) {
            if ( newMethod is MethodInfo ) {
                SetMethod((MethodInfo)newMethod, instance);
            }
        }

        ///Set a new MethodInfo to be used by ReflectedMethodNode
        void SetMethod(MethodInfo newMethod, object instance = null) {

            //open generic
            if ( newMethod.IsGenericMethodDefinition ) {
                var wildType = newMethod.GetFirstGenericParameterConstraintType();
                newMethod = newMethod.MakeGenericMethod(wildType);
            }

            //drop hierarchy to base definition
            newMethod = newMethod.GetBaseDefinition();

            _method = new SerializedMethodInfo(newMethod);
            _callable = newMethod.ReturnType == typeof(void);
            GatherPorts();

            base.SetDropInstanceReference(newMethod, instance);
            base.SetDefaultParameterValues(newMethod);
        }

        ///When ports connects and is a generic method, try change method to that port type
        public override void OnPortConnected(Port port, Port otherPort) {
            if ( method.IsGenericMethod ) {
                var wildType = method.GetFirstGenericParameterConstraintType();
                var newMethod = FlowNode.TryGetNewGenericMethodForWild(wildType, port.type, otherPort.type, method);
                if ( newMethod != null ) {
                    _method = new SerializedMethodInfo(newMethod);
                    GatherPorts();
                }
            }
        }

        //...
        public override System.Type GetNodeWildDefinitionType() {
            return method.GetFirstGenericParameterConstraintType();
        }

        ///Gather the ports through the wrapper
        protected override void RegisterPorts() {
            if ( method == null ) {
                return;
            }

            var options = new ReflectedMethodRegistrationOptions();
            options.callable = callable;
            options.exposeParams = exposeParams;
            options.exposedParamsCount = exposedParamsCount;

            reflectedMethodNode = BaseReflectedMethodNode.GetMethodNode(method, options);
            if ( reflectedMethodNode != null ) {
                reflectedMethodNode.RegisterPorts(this, options);
            }
        }
    }
}