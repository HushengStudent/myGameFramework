using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace FlowCanvas.Nodes
{

    [Name("UI Button")]
    [Category("Events/Object/UI")]
    [Description("Called when the target UI Button is clicked")]
    public class UIButtonEvent : EventNode<Button>
    {

        private FlowOutput o;

        public override void OnGraphStarted() {
            ResolveSelf();
            if ( !target.isNull ) {
                target.value.onClick.AddListener(OnClick);
            }
        }

        public override void OnGraphStoped() {
            if ( !target.isNull ) {
                target.value.onClick.RemoveListener(OnClick);
            }
        }

        protected override void RegisterPorts() {
            o = AddFlowOutput("Clicked");
            AddValueOutput<Button>("This", () => { return base.target.value; });
        }

        void OnClick() {
            o.Call(new Flow());
        }
    }
}