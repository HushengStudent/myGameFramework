using System;
using System.Reflection;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
    public abstract class UniversalDelegateParam
    {
        public ParamDef paramDef;
        public bool paramsArrayNeeded;
        public int paramsArrayCount;
        public UniversalDelegate referencedDelegate;
        public UniversalDelegateParam[] referencedParams;
        public abstract Type GetCurrentType();
        public abstract void RegisterAsInput(FlowNode node);
        public abstract void RegisterAsOutput(FlowNode node);
        public abstract void RegisterAsOutput(FlowNode node, Action beforeReturn);
        public abstract void RegisterAsOutput(FlowNode node, Action<UniversalDelegateParam> beforeReturn);
        public abstract void SetFromInput();
        public abstract void SetFromValue(object value);
        public abstract FieldInfo ValueField { get; }
    }


    public class UniversalDelegateParam<T> : UniversalDelegateParam
    {
        public T value;
        private ValueInput<T> valueInput;
        // ReSharper disable once StaticMemberInGenericType
        private static FieldInfo _fieldInfo;

        //required for activator
        // ReSharper disable once EmptyConstructor
        public UniversalDelegateParam()
        {
        }

        public override Type GetCurrentType()
        {
            return typeof(T);
        }

        public override void RegisterAsInput(FlowNode node)
        {
            if (paramDef.paramMode == ParamMode.Instance || paramDef.paramMode == ParamMode.In ||
                paramDef.paramMode == ParamMode.Ref || paramDef.paramMode == ParamMode.Result)
            {
                valueInput = node.AddValueInput<T>(paramDef.portName, paramDef.portId);
            }
        }

        private void RegisterAsOutputInternal(FlowNode node, Delegate beforeReturn)
        {
            if (paramDef.paramMode == ParamMode.Instance || paramDef.paramMode == ParamMode.Out ||
                paramDef.paramMode == ParamMode.Ref || paramDef.paramMode == ParamMode.Result)
            {
                ValueHandler<T> handler = () =>
                {
                    var simpleAct = beforeReturn as Action;
                    if (simpleAct != null) simpleAct();
                    var selfAct = beforeReturn as Action<UniversalDelegateParam>;
                    if (selfAct != null) selfAct(this);
                    return value;
                };
                node.AddValueOutput(paramDef.portName, handler, paramDef.portId);
            }
        }

        public override void RegisterAsOutput(FlowNode node)
        {
            RegisterAsOutputInternal(node, null);
        }

        public override void RegisterAsOutput(FlowNode node, Action beforeReturn)
        {
            RegisterAsOutputInternal(node, beforeReturn);
        }

        public override void RegisterAsOutput(FlowNode node, Action<UniversalDelegateParam> beforeReturn)
        {
            RegisterAsOutputInternal(node, beforeReturn);
        }

        public override void SetFromInput()
        {
            if (valueInput != null) value = valueInput.value;
        }

        public override void SetFromValue(object newValue)
        {
            value = (T) newValue;
        }

        public override FieldInfo ValueField
        {
            get { return _fieldInfo ?? (_fieldInfo = GetType().RTGetField("value")); }
        }
    }

    public class UniversalDelegateParam<TArray, TValue> : UniversalDelegateParam<TArray>
    {
        private ValueInput<TValue>[] valueInputs;

        public override void RegisterAsInput(FlowNode node)
        {
            valueInputs = null;
            if (paramsArrayNeeded && paramsArrayCount >= 0)
            {
                valueInputs = new ValueInput<TValue>[paramsArrayCount];

                for (var i = 0; i <= paramsArrayCount - 1; i++)
                {
                    valueInputs[i] = node.AddValueInput<TValue>(paramDef.portName + " #" + i, paramDef.portId + i);
                }
            }
            else
            {
                base.RegisterAsInput(node);
            }
        }

        public override void SetFromInput()
        {
            if (paramsArrayNeeded && paramsArrayCount >= 0 && valueInputs != null && valueInputs.Length == paramsArrayCount)
            {
                var valueArray = new TValue[paramsArrayCount];
                for (var i = 0; i <= paramsArrayCount - 1; i++)
                {
                    valueArray[i] = valueInputs[i].value;
                }
                try
                {
                    value = (TArray)(object)valueArray;
                }
                catch
                {
                    base.SetFromInput();
                }
            }
            else
            {
                base.SetFromInput();
            }
        }
    }
}