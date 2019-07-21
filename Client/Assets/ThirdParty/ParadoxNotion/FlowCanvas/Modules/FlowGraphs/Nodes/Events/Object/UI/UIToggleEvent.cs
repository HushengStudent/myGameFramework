using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace FlowCanvas.Nodes
{

    [Name("UI Toggle")]
    [Category("Events/Object/UI")]
    [Description("Called when the target UI Toggle value changed.")]
    public class UIToggleEvent : EventNode<UnityEngine.UI.Toggle>
    {

        private FlowOutput o;
        private bool state;

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
            AddValueOutput<UnityEngine.UI.Toggle>("This", () => { return base.target.value; });
            AddValueOutput<bool>("Value", () => { return state; });
        }

        void OnValueChanged(bool state) {
            this.state = state;
            o.Call(new Flow());
        }
    }
}