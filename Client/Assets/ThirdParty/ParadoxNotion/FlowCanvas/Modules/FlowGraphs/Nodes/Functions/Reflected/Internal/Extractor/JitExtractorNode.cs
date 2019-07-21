#if !NET_STANDARD_2_0 && (UNITY_EDITOR || (!ENABLE_IL2CPP && (UNITY_STANDALONE || UNITY_ANDROID || UNITY_WSA)))
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
    public class JitExtractorNode : BaseReflectedExtractorNode
    {
        private static readonly Type[] DynParamTypes = { typeof(UniversalDelegateParam[]) };
        private readonly Type[] tmpTypes = new Type[1];
        private UniversalDelegateParam[] delegateParams;

        private void CreateDelegates() {
            var instanceId = -1;
            for ( var i = 0; i <= delegateParams.Length - 1; i++ ) {
                var param = delegateParams[i];
                if ( param == null ) continue;
                var def = param.paramDef;
                if ( def.paramMode == ParamMode.Instance ) instanceId = i;

            }
            for ( var i = 0; i <= delegateParams.Length - 1; i++ ) {
                var param = delegateParams[i];
                if ( param == null ) continue;
                var def = param.paramDef;

                var field = def.presentedInfo as FieldInfo;
                var method = def.presentedInfo as MethodInfo;

                if ( def.paramMode == ParamMode.Instance ) continue;
                if ( field != null && field.IsStatic && field.IsReadOnly() ) continue;
                var dynamicMethod = new DynamicMethod(TargetType.Name + "_" + def.portId + "_Extractor", null, DynParamTypes, typeof(JitFieldNode));
                var ilGen = dynamicMethod.GetILGenerator();

                var instLocId = -1;
                var curLocId = 0;

                if ( instanceId >= 0 ) {
                    var instanceParam = delegateParams[instanceId];
                    ilGen.DeclareLocal(instanceParam.GetCurrentType());
                    instLocId++;
                    curLocId++;
                }

                ilGen.DeclareLocal(param.GetCurrentType());

                if ( instanceId >= 0 ) {
                    var instanceParam = delegateParams[instanceId];
                    //load first argument of method to stack, in our situation it "delegateParams" array
                    ilGen.Emit(OpCodes.Ldarg, 0);
                    //load current array index to stack
                    ilGen.Emit(OpCodes.Ldc_I4, instLocId);
                    //load reference for delegateParams[i] to stack
                    ilGen.Emit(OpCodes.Ldelem_Ref);
                    //load value for delegateParams[i].value to stack
                    ilGen.Emit(OpCodes.Ldfld, instanceParam.ValueField);
                    //set local variable to loaded value
                    ilGen.Emit(OpCodes.Stloc, instLocId);
                }

                if ( field != null ) {
                    if ( instanceId >= 0 ) {
                        //load local var for get field value
                        ilGen.Emit(OpCodes.Ldloc, instLocId);
                        ilGen.Emit(OpCodes.Ldfld, field);
                    } else {
                        ilGen.Emit(OpCodes.Ldsfld, field);
                    }
                }
                if ( method != null ) {
                    if ( instanceId >= 0 ) {
                        //load local var for get field value
                        ilGen.Emit(delegateParams[instanceId].GetCurrentType().RTIsValueType() ? OpCodes.Ldloca : OpCodes.Ldloc, instLocId);
                    }
                    if ( instanceId < 0 || delegateParams[instanceId].GetCurrentType().RTIsValueType() ) {
                        //use Call opcode, because value types and statics methods cannot be virtual or overrided, result (if exist) will stored into stack
                        ilGen.Emit(OpCodes.Call, method);
                    } else {
                        //use Callvirt opcode, because non value types and non statics methods can be virtual or overrided, result (if exist) will stored into stack
                        ilGen.Emit(OpCodes.Callvirt, method);
                    }
                }

                // Set local var to value of target field
                ilGen.Emit(OpCodes.Stloc, curLocId);
                //load first argument of method to stack, in our situation it "delegateParams" array
                ilGen.Emit(OpCodes.Ldarg, 0);
                //load current array index to stack
                ilGen.Emit(OpCodes.Ldc_I4, curLocId);
                //load reference for delegateParams[i] to stack
                ilGen.Emit(OpCodes.Ldelem_Ref);
                //load value for local val
                ilGen.Emit(OpCodes.Ldloc, curLocId);
                //set local variable to loaded value
                ilGen.Emit(OpCodes.Stfld, param.ValueField);

                ilGen.Emit(OpCodes.Ret);
                param.referencedDelegate = (UniversalDelegate)dynamicMethod.CreateDelegate(typeof(UniversalDelegate));
                param.referencedParams = instanceId >= 0 ? new[] { delegateParams[instanceId], param } : new[] { param };
            }
        }

        private void Call(UniversalDelegateParam targetParam) {
            if ( targetParam != null && targetParam.referencedDelegate != null && targetParam.referencedParams != null ) {
                for ( int i = 0; i <= delegateParams.Length - 1; i++ ) {
                    var param = delegateParams[i];
                    if ( param != null && param.paramDef.paramMode == ParamMode.Instance ) {
                        param.SetFromInput();
                        break;
                    }
                }
                targetParam.referencedDelegate(targetParam.referencedParams);
            }
        }

        protected override bool InitInternal() {
            var cnt = 0;
            if ( Params.instanceDef.paramMode == ParamMode.Instance ) {
                cnt++;
            }
            var list = Params.paramDefinitions ?? new List<ParamDef>();
            for ( var i = 0; i <= list.Count - 1; i++ ) {
                var def = list[i];
                if ( def.paramMode != ParamMode.Out || def.presentedInfo == null ) continue;
                cnt++;
            }
            delegateParams = new UniversalDelegateParam[cnt];
            var k = 0;
            if ( Params.instanceDef.paramMode == ParamMode.Instance ) {
                tmpTypes[0] = TargetType;
                delegateParams[k] = (UniversalDelegateParam)typeof(UniversalDelegateParam<>).RTMakeGenericType(tmpTypes).CreateObjectUninitialized();
                delegateParams[k].paramDef = Params.instanceDef;
                k++;
            }
            for ( var i = 0; i <= list.Count - 1; i++ ) {
                var def = list[i];
                if ( def.paramMode != ParamMode.Out || def.presentedInfo == null ) continue;
                tmpTypes[0] = def.paramType;
                delegateParams[k] = (UniversalDelegateParam)typeof(UniversalDelegateParam<>).RTMakeGenericType(tmpTypes).CreateObjectUninitialized();
                delegateParams[k].paramDef = def;
                k++;
            }
            try {
                CreateDelegates();
            }
            catch {
                return false;
            }
            return true;
        }

        public override void RegisterPorts(FlowNode node) {
            for ( var i = 0; i <= delegateParams.Length - 1; i++ ) {
                var param = delegateParams[i];
                if ( param != null ) {
                    var def = param.paramDef;
                    if ( def.paramMode == ParamMode.Instance ) {
                        param.RegisterAsInput(node);
                    }
                    if ( def.paramMode == ParamMode.Out ) {
                        var info = def.presentedInfo as FieldInfo;
                        if ( info != null && info.IsStatic && info.IsReadOnly() ) {
                            param.SetFromValue(info.GetValue(null));
                            param.RegisterAsOutput(node);
                        } else {
                            param.RegisterAsOutput(node, Call);
                        }
                    }
                }
            }
        }
    }
}
#endif