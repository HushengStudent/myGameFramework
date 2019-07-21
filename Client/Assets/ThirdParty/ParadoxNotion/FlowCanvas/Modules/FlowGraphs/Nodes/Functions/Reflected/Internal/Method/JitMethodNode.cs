#if !NET_STANDARD_2_0 && (UNITY_EDITOR || (!ENABLE_IL2CPP && (UNITY_STANDALONE || UNITY_ANDROID || UNITY_WSA)))
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
    public class JitMethodNode : BaseReflectedMethodNode
    {
        private static readonly Dictionary<string, UniversalDelegate> Delegates = new Dictionary<string, UniversalDelegate>(StringComparer.Ordinal);
        private static readonly Type[] DynParamTypes = { typeof(UniversalDelegateParam[]) };
        private readonly Type[] tmpTypes = new Type[1];
        private readonly Type[] tmpTypes2 = new Type[2];
        private UniversalDelegate delegat;
        private UniversalDelegateParam[] delegateParams;
        private Action actionCall;

        private void CreateDelegat() {
            var key = ReflectedNodesHelper.GetGeneratedKey(methodInfo);
            if ( Delegates.TryGetValue(key, out delegat) && delegat != null ) return;

            DynamicMethod dynamicMethod = new DynamicMethod(methodInfo.Name + "_Dynamic", null, DynParamTypes, typeof(JitMethodNode));
            ILGenerator ilGen = dynamicMethod.GetILGenerator();
            int instanceId = -1;
            int returnId = -1;
            for ( int i = 0; i <= delegateParams.Length - 1; i++ ) {
                var param = delegateParams[i];
                var def = param.paramDef;
                //declare local variables in method
                ilGen.DeclareLocal(param.GetCurrentType());
                if ( def.paramMode == ParamMode.Instance ) instanceId = i;
                if ( def.paramMode == ParamMode.Result ) returnId = i;
            }
            //store values to local variables
            for ( int i = 0; i <= delegateParams.Length - 1; i++ ) {
                var param = delegateParams[i];
                //load first argument of method to stack, in our situation it "delegateParams" array
                ilGen.Emit(OpCodes.Ldarg, 0);
                //load current array index to stack
                ilGen.Emit(OpCodes.Ldc_I4, i);
                //load reference for delegateParams[i] to stack
                ilGen.Emit(OpCodes.Ldelem_Ref);
                //load value for delegateParams[i].value to stack
                ilGen.Emit(OpCodes.Ldfld, param.ValueField);
                //set local variable to loaded value
                ilGen.Emit(OpCodes.Stloc, i);
            }
            if ( instanceId >= 0 ) {
                //load instance local to stack for use call on it
                ilGen.Emit(delegateParams[instanceId].GetCurrentType().RTIsValueType() ? OpCodes.Ldloca : OpCodes.Ldloc, instanceId);
            }
            for ( int i = 0; i <= delegateParams.Length - 1; i++ ) {
                var param = delegateParams[i];
                var def = param.paramDef;
                if ( def.paramMode != ParamMode.Instance && def.paramMode != ParamMode.Result ) {
                    ilGen.Emit(def.paramMode == ParamMode.In ? OpCodes.Ldloc : OpCodes.Ldloca, i);
                }
            }
            if ( instanceId < 0 || delegateParams[instanceId].GetCurrentType().RTIsValueType() ) {
                //use Call opcode, because value types and statics methods cannot be virtual or overrided, result (if exist) will stored into stack
                ilGen.Emit(OpCodes.Call, methodInfo);
            } else {
                //use Callvirt opcode, because non value types and non statics methods can be virtual or overrided, result (if exist) will stored into stack
                ilGen.Emit(OpCodes.Callvirt, methodInfo);
            }
            if ( returnId >= 0 ) {
                //set result of code to loacal var
                ilGen.Emit(OpCodes.Stloc, returnId);
            }
            //return local vals to our array
            for ( int i = 0; i <= delegateParams.Length - 1; i++ ) {
                var param = delegateParams[i];
                //load first argument of method to stack, in our sytuation it "delegateParams" array
                ilGen.Emit(OpCodes.Ldarg, 0);
                //load current array index to stack
                ilGen.Emit(OpCodes.Ldc_I4, i);
                //load reference for delegateParams[i] to stack
                ilGen.Emit(OpCodes.Ldelem_Ref);
                //load value for local val
                ilGen.Emit(OpCodes.Ldloc, i);
                //set local variable to loaded value
                ilGen.Emit(OpCodes.Stfld, param.ValueField);
            }
            ilGen.Emit(OpCodes.Ret);
            delegat = (UniversalDelegate)dynamicMethod.CreateDelegate(typeof(UniversalDelegate));
            Delegates[key] = delegat;
        }

        private void Call() {
            if ( delegat != null ) {
                for ( int i = 0; i <= delegateParams.Length - 1; i++ ) {
                    delegateParams[i].SetFromInput();
                }
                delegat(delegateParams);
            }
        }

        protected override bool InitInternal(MethodInfo method) {
            delegat = null;
            int cnt = paramDefinitions.Count;
            if ( instanceDef.paramMode != ParamMode.Undefined ) cnt++;
            if ( resultDef.paramMode != ParamMode.Undefined ) cnt++;
            delegateParams = new UniversalDelegateParam[cnt];
            int i = 0;
            if ( instanceDef.paramMode != ParamMode.Undefined ) {
                tmpTypes[0] = instanceDef.paramType;
                delegateParams[i] = (UniversalDelegateParam)typeof(UniversalDelegateParam<>).RTMakeGenericType(tmpTypes).CreateObjectUninitialized();
                delegateParams[i].paramDef = instanceDef;
                i++;
            }
            if ( resultDef.paramMode != ParamMode.Undefined ) {
                tmpTypes[0] = resultDef.paramType;
                delegateParams[i] = (UniversalDelegateParam)typeof(UniversalDelegateParam<>).RTMakeGenericType(tmpTypes).CreateObjectUninitialized();
                delegateParams[i].paramDef = resultDef;
                i++;
            }
            for ( int j = 0; j <= paramDefinitions.Count - 1; j++ ) {
                if ( options.exposeParams && paramDefinitions[j].isParamsArray ) {
                    tmpTypes2[0] = paramDefinitions[j].paramType;
                    tmpTypes2[1] = paramDefinitions[j].arrayType;
                    delegateParams[i] = (UniversalDelegateParam)typeof(UniversalDelegateParam<,>).RTMakeGenericType(tmpTypes2).CreateObjectUninitialized();
                    delegateParams[i].paramsArrayNeeded = options.exposeParams;
                    delegateParams[i].paramsArrayCount = options.exposedParamsCount;
                } else {
                    tmpTypes[0] = paramDefinitions[j].paramType;
                    delegateParams[i] = (UniversalDelegateParam)typeof(UniversalDelegateParam<>).RTMakeGenericType(tmpTypes).CreateObjectUninitialized();
                }
                delegateParams[i].paramDef = paramDefinitions[j];
                i++;
            }
            try {
                CreateDelegat();
            }
            catch {
                return false;
            }
            return true;
        }

        public override void RegisterPorts(FlowNode node, ReflectedMethodRegistrationOptions options) {
            if ( actionCall == null ) actionCall = Call;
            if ( options.callable ) {
                var output = node.AddFlowOutput(" ");
                node.AddFlowInput(" ", flow =>
                {
                    Call();
                    output.Call(flow);
                });
            }
            for ( int i = 0; i <= delegateParams.Length - 1; i++ ) {
                var param = delegateParams[i];
                var def = param.paramDef;
                if ( def.paramMode == ParamMode.Instance ) {
                    param.RegisterAsInput(node);
                    if ( options.callable ) {
                        param.RegisterAsOutput(node);
                    }
                } else if ( def.paramMode == ParamMode.Result ) {
                    param.RegisterAsOutput(node, options.callable ? null : actionCall);
                } else {
                    if ( def.paramMode == ParamMode.Ref ) {
                        param.RegisterAsInput(node);
                        param.RegisterAsOutput(node, options.callable ? null : actionCall);
                    } else if ( def.paramMode == ParamMode.In ) {
                        param.RegisterAsInput(node);
                    } else if ( def.paramMode == ParamMode.Out ) {
                        param.RegisterAsOutput(node, options.callable ? null : actionCall);
                    }
                }
            }
        }
    }
}
#endif