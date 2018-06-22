using System;
using System.Collections.Generic;
using System.Reflection;

namespace FlowCanvas.Nodes
{
    public abstract class BaseReflectedConstructorNode
    {
        protected static event Func<ConstructorInfo, BaseReflectedConstructorNode> OnGetAotReflectedConstructorNode;

        public static BaseReflectedConstructorNode GetConstructorNode(ConstructorInfo targetConstructor, ReflectedMethodRegistrationOptions options)
        {
            ParametresDef paramDef;
            if (!ReflectedNodesHelper.InitParams(targetConstructor, out paramDef)) return null;
#if UNITY_EDITOR || (!ENABLE_IL2CPP && (UNITY_STANDALONE || UNITY_ANDROID || UNITY_WSA))
            var jit = new JitConstructorNode();
            jit.options = options;
            if (jit.Init(targetConstructor, paramDef))
            {
                return jit;
            }
#endif
            if (OnGetAotReflectedConstructorNode != null)
            {
                var eventAot = OnGetAotReflectedConstructorNode(targetConstructor);
                if (eventAot != null)
                {
                    eventAot.options = options;
                    if (eventAot.Init(targetConstructor, paramDef))
                    {
                        return eventAot;
                    }
                }
            }
            var aot = new PureReflectionConstructorNode();
            aot.options = options;
            return aot.Init(targetConstructor, paramDef) ? aot : null;
        }

        protected ConstructorInfo constructorInfo;
        protected List<ParamDef> paramDefinitions;
        protected ParamDef instanceDef;
        protected ParamDef resultDef;
        protected ReflectedMethodRegistrationOptions options;

        protected bool Init(ConstructorInfo constructor, ParametresDef parametres)
        {
            if (constructor == null || constructor.ContainsGenericParameters || constructor.IsGenericMethodDefinition) return false;
            paramDefinitions = parametres.paramDefinitions == null ? new List<ParamDef>() : parametres.paramDefinitions;
            instanceDef = parametres.instanceDef;
            resultDef = parametres.resultDef;
            constructorInfo = constructor;
            return InitInternal(constructor);
        }

        protected abstract bool InitInternal(ConstructorInfo constructor);

        public abstract void RegisterPorts(FlowNode node, ReflectedMethodRegistrationOptions options);
    }
}