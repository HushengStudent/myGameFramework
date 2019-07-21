using System;
using System.Collections.Generic;
using System.Reflection;

namespace FlowCanvas.Nodes
{
    public abstract class BaseReflectedMethodNode
    {
        protected static event Func<MethodInfo, BaseReflectedMethodNode> OnGetAotReflectedMethodNode;

        public static BaseReflectedMethodNode GetMethodNode(MethodInfo targetMethod, ReflectedMethodRegistrationOptions options) {
            ParametresDef paramDef;
            if ( !ReflectedNodesHelper.InitParams(targetMethod, out paramDef) ) return null;
#if !NET_STANDARD_2_0 && (UNITY_EDITOR || (!ENABLE_IL2CPP && (UNITY_STANDALONE || UNITY_ANDROID || UNITY_WSA)))
            var jit = new JitMethodNode();
            jit.options = options;
            if ( jit.Init(targetMethod, paramDef) ) {
                return jit;
            }
#endif
            if ( OnGetAotReflectedMethodNode != null ) {
                var eventAot = OnGetAotReflectedMethodNode(targetMethod);
                if ( eventAot != null ) {
                    eventAot.options = options;
                    if ( eventAot.Init(targetMethod, paramDef) ) {
                        return eventAot;
                    }
                }
            }
            var aot = new PureReflectedMethodNode();
            aot.options = options;
            return aot.Init(targetMethod, paramDef) ? aot : null;
        }

        protected MethodInfo methodInfo;
        protected List<ParamDef> paramDefinitions;
        protected ParamDef instanceDef;
        protected ParamDef resultDef;
        protected ReflectedMethodRegistrationOptions options;

        protected bool Init(MethodInfo method, ParametresDef parametres) {
            if ( method == null || method.ContainsGenericParameters || method.IsGenericMethodDefinition ) return false;
            paramDefinitions = parametres.paramDefinitions == null ? new List<ParamDef>() : parametres.paramDefinitions;
            instanceDef = parametres.instanceDef;
            resultDef = parametres.resultDef;
            methodInfo = method;
            return InitInternal(method);
        }

        protected abstract bool InitInternal(MethodInfo method);

        public abstract void RegisterPorts(FlowNode node, ReflectedMethodRegistrationOptions options);
    }
}
