#if UNITY_EDITOR || (!ENABLE_IL2CPP && (UNITY_STANDALONE || UNITY_ANDROID || UNITY_WSA))
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
    public class JitFieldNode : BaseReflectedFieldNode
    {
        private static readonly Dictionary<string, UniversalDelegate> GetDelegates = new Dictionary<string, UniversalDelegate>(StringComparer.Ordinal);
        private static readonly Dictionary<string, UniversalDelegate> SetDelegates = new Dictionary<string, UniversalDelegate>(StringComparer.Ordinal);
        private static readonly Type[] DynParamTypes = { typeof(UniversalDelegateParam[]) };
        private readonly Type[] tmpTypes = new Type[1];
        private UniversalDelegate getDelegat;
        private UniversalDelegate setDelegat;
        private UniversalDelegateParam[] delegateParams;
        private Action getValue;
        private bool isConstant;

        private void CreateDelegates()
        {
            var key = ReflectedNodesHelper.GetGeneratedKey(fieldInfo);
            if (!GetDelegates.TryGetValue(key, out getDelegat) || getDelegat == null)
            {
                DynamicMethod dynamicMethod = new DynamicMethod(fieldInfo.Name + "_DynamicGet", null, DynParamTypes, typeof(JitFieldNode));
                ILGenerator ilGen = dynamicMethod.GetILGenerator();
                int instanceId = -1;
                int returnId = -1;
                int localIds = 0;
                int instanceLocId = -1;
                int returnLocId = -1;
                for (int i = 0; i <= delegateParams.Length - 1; i++)
                {
                    var param = delegateParams[i];
                    var def = param.paramDef;
                    if (def.paramMode == ParamMode.Instance)
                    {
                        instanceId = i;
                        ilGen.DeclareLocal(def.paramType);
                        instanceLocId = localIds;
                        localIds++;
                    }
                    if (def.paramMode == ParamMode.Result)
                    {
                        returnId = i;
                        ilGen.DeclareLocal(def.paramType);
                        returnLocId = localIds;
                        localIds++;
                    }
                }

                if (instanceId >= 0)
                {
                    //load first argument of method to stack, in our situation it "delegateParams" array
                    ilGen.Emit(OpCodes.Ldarg, 0);
                    //load current array index to stack
                    ilGen.Emit(OpCodes.Ldc_I4, instanceId);
                    //load reference for delegateParams[i] to stack
                    ilGen.Emit(OpCodes.Ldelem_Ref);
                    //load value for delegateParams[i].value to stack
                    ilGen.Emit(OpCodes.Ldfld, delegateParams[instanceId].ValueField);
                    //set local variable to loaded value
                    ilGen.Emit(OpCodes.Stloc, instanceLocId);
                    //load local var for get field value
                    ilGen.Emit(OpCodes.Ldloc, instanceLocId);
                    ilGen.Emit(OpCodes.Ldfld, fieldInfo);
                }
                else
                {
                    ilGen.Emit(OpCodes.Ldsfld, fieldInfo);
                }
                
                
                if (returnId >= 0)
                {
                    //Set local var to value of target field
                    ilGen.Emit(OpCodes.Stloc, returnLocId);
                    //load first argument of method to stack, in our situation it "delegateParams" array
                    ilGen.Emit(OpCodes.Ldarg, 0);
                    //load current array index to stack
                    ilGen.Emit(OpCodes.Ldc_I4, returnId);
                    //load reference for delegateParams[i] to stack
                    ilGen.Emit(OpCodes.Ldelem_Ref);
                    //load value for local val
                    ilGen.Emit(OpCodes.Ldloc, returnLocId);
                    //set local variable to loaded value
                    ilGen.Emit(OpCodes.Stfld, delegateParams[returnId].ValueField);
                }

                ilGen.Emit(OpCodes.Ret);

                getDelegat = (UniversalDelegate)dynamicMethod.CreateDelegate(typeof(UniversalDelegate));

                GetDelegates[key] = getDelegat;
            }
            if ((!SetDelegates.TryGetValue(key, out setDelegat) || setDelegat == null) && !fieldInfo.IsReadOnly())
            {
                DynamicMethod dynamicMethod = new DynamicMethod(fieldInfo.Name + "_DynamicSet", null, DynParamTypes, typeof(JitFieldNode));
                ILGenerator ilGen = dynamicMethod.GetILGenerator();
                int instanceId = -1;
                int valueId = -1;
                int localIds = 0;
                int instanceLocId = -1;
                int valueLocId = -1;
                for (int i = 0; i <= delegateParams.Length - 1; i++)
                {
                    var param = delegateParams[i];
                    var def = param.paramDef;
                    //declare local variables in method
                    if (def.paramMode == ParamMode.Instance)
                    {
                        instanceId = i;
                        ilGen.DeclareLocal(def.paramType);
                        instanceLocId = localIds;
                        localIds++;
                    }
                    if (def.paramMode == ParamMode.Result)
                    {
                        valueId = i;
                        ilGen.DeclareLocal(def.paramType);
                        valueLocId = localIds;
                        localIds++;
                    }
                }

                if (instanceId >= 0)
                {
                    //load first argument of method to stack, in our situation it "delegateParams" array
                    ilGen.Emit(OpCodes.Ldarg, 0);
                    //load current array index to stack
                    ilGen.Emit(OpCodes.Ldc_I4, instanceId);
                    //load reference for delegateParams[i] to stack
                    ilGen.Emit(OpCodes.Ldelem_Ref);
                    //load value for delegateParams[i].value to stack
                    ilGen.Emit(OpCodes.Ldfld, delegateParams[instanceId].ValueField);
                    //set local variable to loaded value
                    ilGen.Emit(OpCodes.Stloc, instanceLocId);
                }

                if (valueId >= 0)
                {
                    //load first argument of method to stack, in our situation it "delegateParams" array
                    ilGen.Emit(OpCodes.Ldarg, 0);
                    //load current array index to stack
                    ilGen.Emit(OpCodes.Ldc_I4, valueId);
                    //load reference for delegateParams[i] to stack
                    ilGen.Emit(OpCodes.Ldelem_Ref);
                    //load value for delegateParams[i].value to stack
                    ilGen.Emit(OpCodes.Ldfld, delegateParams[valueId].ValueField);
                    //set local variable to loaded value
                    ilGen.Emit(OpCodes.Stloc, valueLocId);
                    if (instanceId >= 0)
                    {
                        //set instance for set field value
                        ilGen.Emit(delegateParams[instanceId].GetCurrentType().RTIsValueType() ? OpCodes.Ldloca : OpCodes.Ldloc, instanceLocId);
                        //set new value for field
                        ilGen.Emit(OpCodes.Ldloc, valueLocId);
                        //set field value
                        ilGen.Emit(OpCodes.Stfld, fieldInfo);
                    }
                    else
                    {
                        //set new value for field
                        ilGen.Emit(OpCodes.Ldloc, valueLocId);
                        //set field value
                        ilGen.Emit(OpCodes.Stsfld, fieldInfo);
                    }
                }

                if (instanceId >= 0)
                {
                    //load first argument of method to stack, in our situation it "delegateParams" array
                    ilGen.Emit(OpCodes.Ldarg, 0);
                    //load current array index to stack
                    ilGen.Emit(OpCodes.Ldc_I4, instanceId);
                    //load reference for delegateParams[i] to stack
                    ilGen.Emit(OpCodes.Ldelem_Ref);
                    //load value for local val
                    ilGen.Emit(OpCodes.Ldloc, instanceLocId);
                    //set local variable to loaded value
                    ilGen.Emit(OpCodes.Stfld, delegateParams[instanceId].ValueField);
                }

                ilGen.Emit(OpCodes.Ret);

                setDelegat = (UniversalDelegate)dynamicMethod.CreateDelegate(typeof(UniversalDelegate));

                SetDelegates[key] = setDelegat;
            }
        }

        protected override bool InitInternal(FieldInfo field)
        {
            isConstant = field.IsStatic && field.IsReadOnly();
            if (isConstant)
            {
                delegateParams = new UniversalDelegateParam[1];
                tmpTypes[0] = field.FieldType;
                delegateParams[0] = (UniversalDelegateParam)typeof(UniversalDelegateParam<>).RTMakeGenericType(tmpTypes).CreateObjectUninitialized();
                delegateParams[0].paramDef = resultDef;
                delegateParams[0].SetFromValue(field.GetValue(null));
                return true;
            }
            getDelegat = null;
            setDelegat = null;
            int cnt = 0;
            if (instanceDef.paramMode != ParamMode.Undefined) cnt++;
            if (resultDef.paramMode != ParamMode.Undefined) cnt++;
            delegateParams = new UniversalDelegateParam[cnt];
            int i = 0;
            if (instanceDef.paramMode != ParamMode.Undefined)
            {
                tmpTypes[0] = instanceDef.paramType;
                delegateParams[i] = (UniversalDelegateParam)typeof(UniversalDelegateParam<>).RTMakeGenericType(tmpTypes).CreateObjectUninitialized();
                delegateParams[i].paramDef = instanceDef;
                i++;
            }
            if (resultDef.paramMode != ParamMode.Undefined)
            {
                tmpTypes[0] = resultDef.paramType;
                delegateParams[i] = (UniversalDelegateParam)typeof(UniversalDelegateParam<>).RTMakeGenericType(tmpTypes).CreateObjectUninitialized();
                delegateParams[i].paramDef = resultDef;
            }
            try
            {
                CreateDelegates();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void SetValue()
        {
            if (setDelegat != null && !fieldInfo.IsReadOnly())
            {
                for (int i = 0; i <= delegateParams.Length - 1; i++)
                {
                    delegateParams[i].SetFromInput();
                }
                setDelegat(delegateParams);
            }
        }

        private void GetValue()
        {
            if (getDelegat != null)
            {
                for (int i = 0; i <= delegateParams.Length - 1; i++)
                {
                    delegateParams[i].SetFromInput();
                }
                getDelegat(delegateParams);
            }
        }

        public override void RegisterPorts(FlowNode node, ReflectedFieldNodeWrapper.AccessMode accessMode)
        {
            if (isConstant)
            {
                delegateParams[0].RegisterAsOutput(node);
            }
            if (getValue == null) getValue = GetValue;
            if (accessMode == ReflectedFieldNodeWrapper.AccessMode.SetField && !fieldInfo.IsReadOnly())
            {
                var output = node.AddFlowOutput(" ");
                node.AddFlowInput(" ", flow =>
                {
                    SetValue();
                    output.Call(flow);
                });
            }
            for (int i = 0; i <= delegateParams.Length - 1; i++)
            {
                var param = delegateParams[i];
                var def = param.paramDef;
                if (def.paramMode == ParamMode.Instance)
                {
                    param.RegisterAsInput(node);
                    if (accessMode == ReflectedFieldNodeWrapper.AccessMode.SetField && !fieldInfo.IsReadOnly())
                    {
                        param.RegisterAsOutput(node);
                    }
                }
                if (def.paramMode == ParamMode.Result)
                {
                    if (accessMode == ReflectedFieldNodeWrapper.AccessMode.SetField && !fieldInfo.IsReadOnly())
                    {
                        param.RegisterAsInput(node);
                    }
                    else
                    {
                        param.RegisterAsOutput(node, getValue);
                    }
                }
            }
        }
    }
}
#endif