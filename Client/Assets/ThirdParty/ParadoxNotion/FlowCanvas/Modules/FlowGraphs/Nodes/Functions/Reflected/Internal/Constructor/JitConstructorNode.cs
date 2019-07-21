#if !NET_STANDARD_2_0 && (UNITY_EDITOR || (!ENABLE_IL2CPP && (UNITY_STANDALONE || UNITY_ANDROID || UNITY_WSA)))
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
    public class JitConstructorNode : BaseReflectedConstructorNode
    {
        private static readonly Dictionary<string, UniversalDelegate> Delegates = new Dictionary<string, UniversalDelegate>(StringComparer.Ordinal);
        private static readonly Type[] DynParamTypes = { typeof(UniversalDelegateParam[]) };
        private readonly Type[] tmpTypes = new Type[1];
        private readonly Type[] tmpTypes2 = new Type[2];
        private UniversalDelegate delegat;
        private UniversalDelegateParam[] delegateParams;
        private Action actionCall;

        private void CreateDelegat() {
            var key = ReflectedNodesHelper.GetGeneratedKey(constructorInfo);
            if ( Delegates.TryGetValue(key, out delegat) && delegat != null ) return;

            DynamicMethod dynamicMethod = new DynamicMethod("Constructor_Dynamic", null, DynParamTypes, typeof(JitMethodNode));
            ILGenerator ilGen = dynamicMethod.GetILGenerator();
            int returnId = -1;
            for ( int i = 0; i <= delegateParams.Length - 1; i++ ) {
                var param = delegateParams[i];
                var def = param.paramDef;
                //declare local variables in method
                ilGen.DeclareLocal(param.GetCurrentType());
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
            for ( int i = 0; i <= delegateParams.Length - 1; i++ ) {
                var param = delegateParams[i];
                var def = param.paramDef;
                if ( def.paramMode != ParamMode.Instance && def.paramMode != ParamMode.Result ) {
                    if ( def.paramMode == ParamMode.In ) {
                        //load local variable for call if it simple param
                        ilGen.Emit(OpCodes.Ldloc, i);
                    } else {
                        //load reference to local variable for call if it out or ref param
                        ilGen.Emit(OpCodes.Ldloca, i);
                    }
                }
            }
            ilGen.Emit(OpCodes.Newobj, constructorInfo);
            if ( returnId >= 0 ) {
                //set result of code to local var
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

        protected override bool InitInternal(ConstructorInfo constructor) {
            delegat = null;
            int cnt = paramDefinitions.Count;
            if ( resultDef.paramMode != ParamMode.Undefined ) cnt++;
            delegateParams = new UniversalDelegateParam[cnt];
            int i = 0;
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
                if ( def.paramMode == ParamMode.Result ) {
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