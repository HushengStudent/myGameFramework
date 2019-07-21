using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace FlowCanvas.Nodes
{

    [Name("UI Scrollbar")]
    [Category("Events/Object/UI")]
    [Description("Called when the target UI Scrollbar value changed.")]
    public class UIScrollbarEvent : EventNode<UnityEngine.UI.Scrollbar>
    {

        private FlowOutput o;
        private float value;

        public override void OnGraphStarted() {
            ResolveSelf();
            if ( !target.isNull ) {
                target.value.onValueChanged.AddListener(OnValueChanged);
            }
        }
        public override void OnGraphStoped() {
            if ( !target.isNull ) {
                target.value.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        protected override void RegisterPorts() {
            o = AddFlowOutput("Value Changed");
            AddValueOutput<UnityEngine.UI.Scrollbar>("This", () => { return base.target.value; });
            AddValueOutput<float>("Value", () => { return value; });
        }

        void OnValueChanged(float value) {
            this.value = value;
            o.Call(new Flow());
        }
    }
}