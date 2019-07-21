using System.Reflection;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
    public class PureReflectedExtractorNode : BaseReflectedExtractorNode
    {
        private static readonly object[] EmptyParams = new object[0];
        private ValueInput instanceInput;

        protected override bool InitInternal() {
            instanceInput = null;
            return true;
        }

        private ValueHandlerObject GetPortHandler(FieldInfo info) {
            if ( info != null ) {
                return () =>
                {
                    var inst = instanceInput != null ? instanceInput.value : null;
                    return info.GetValue(inst);
                };
            }
            return null;
        }

        private ValueHandlerObject GetPortHandler(MethodInfo info) {
            if ( info != null ) {
                return () =>
                {
                    var inst = instanceInput != null ? instanceInput.value : null;
                    return info.Invoke(inst, EmptyParams);
                };
            }
            return null;
        }

        public override void RegisterPorts(FlowNode node) {
            instanceInput = null;
            var instance = Params.instanceDef;
            if ( instance.paramMode != ParamMode.Undefined ) {
                instanceInput = node.AddValueInput(instance.portName, instance.paramType, instance.portId);
            }
            var list = Params.paramDefinitions;
            if ( list == null ) return;
            for ( var i = 0; i <= list.Count - 1; i++ ) {
                var def = list[i];
                if ( def.paramMode != ParamMode.Out ) continue;
                ValueHandlerObject handler = null;
                var field = def.presentedInfo as FieldInfo;
                if ( field != null ) handler = GetPortHandler(field);
                var method = def.presentedInfo as MethodInfo;
                if ( method != null ) handler = GetPortHandler(method);
                if ( handler != null ) {
                    node.AddValueOutput(def.portName, def.paramType, handler, def.portId);
                }
            }
        }
    }
}
