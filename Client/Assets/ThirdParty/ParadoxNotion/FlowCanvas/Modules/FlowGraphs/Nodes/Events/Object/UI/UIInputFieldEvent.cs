using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace FlowCanvas.Nodes
{

    [Name("UI Field Input")]
    [Category("Events/Object/UI")]
    [Description("Called when the target UI Dropdown value changed.")]
    public class UIInputFieldEvent : EventNode<UnityEngine.UI.InputField>
    {

        private FlowOutput onValueChanged;
        private FlowOutput onEndEdit;
        private string value;

        public override void OnGraphStarted() {
            ResolveSelf();
            if ( !target.isNull ) {
                target.value.onValueChanged.AddListener(OnValueChanged);
                target.value.onEndEdit.AddListener(OnEndEdit);
            }
        }
        public override void OnGraphStoped() {
            if ( !target.isNull ) {
                target.value.onValueChanged.RemoveListener(OnValueChanged);
                target.value.onEndEdit.RemoveListener(OnEndEdit);
            }
        }

        protected override void RegisterPorts() {
            onValueChanged = AddFlowOutput("Value Changed");
            onEndEdit = AddFlowOutput("End Edit");
            AddValueOutput<UnityEngine.UI.InputField>("This", () => { return base.target.value; });
            AddValueOutput<string>("Value", () => { return value; });
        }

        void OnValueChanged(string value) {
            this.value = value;
            onValueChanged.Call(new Flow());
        }

        void OnEndEdit(string value) {
            this.value = value;
            onEndEdit.Call(new Flow());
        }
    }
}