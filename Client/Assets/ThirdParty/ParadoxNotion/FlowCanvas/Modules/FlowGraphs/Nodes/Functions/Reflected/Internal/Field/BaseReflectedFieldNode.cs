using System;
using System.Reflection;

namespace FlowCanvas.Nodes
{
    public abstract class BaseReflectedFieldNode
    {
        protected static event Func<FieldInfo, BaseReflectedFieldNode> OnGetAotReflectedFieldNode;

        public static BaseReflectedFieldNode GetFieldNode(FieldInfo targetField) {
            ParametresDef paramDef;
            if ( !ReflectedNodesHelper.InitParams(targetField, out paramDef) ) return null;
#if !NET_STANDARD_2_0 && (UNITY_EDITOR || (!ENABLE_IL2CPP && (UNITY_STANDALONE || UNITY_ANDROID || UNITY_WSA)))
            var jit = new JitFieldNode();
            if ( jit.Init(targetField, paramDef) ) {
                return jit;
            }
#endif
            if ( OnGetAotReflectedFieldNode != null ) {
                var eventAot = OnGetAotReflectedFieldNode(targetField);
                if ( eventAot != null && eventAot.Init(targetField, paramDef) ) {
                    return eventAot;
                }
            }
            var aot = new PureReflectedFieldNode();
            return aot.Init(targetField, paramDef) ? aot : null;
        }

        protected FieldInfo fieldInfo;
        protected ParamDef instanceDef;
        protected ParamDef resultDef;

        protected bool Init(FieldInfo field, ParametresDef parametres) {
            if ( field == null || field.FieldType.ContainsGenericParameters || field.FieldType.IsGenericTypeDefinition ) return false;
            instanceDef = parametres.instanceDef;
            resultDef = parametres.resultDef;
            fieldInfo = field;
            return InitInternal(fieldInfo);
        }

        protected abstract bool InitInternal(FieldInfo field);

        public abstract void RegisterPorts(FlowNode node, ReflectedFieldNodeWrapper.AccessMode accessMode);
    }
}