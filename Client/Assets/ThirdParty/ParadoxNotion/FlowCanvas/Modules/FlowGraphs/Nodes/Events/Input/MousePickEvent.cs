using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace FlowCanvas.Nodes
{

    [Name("Mouse Pick")]
    [Category("Events/Input")]
    [Description("Called when any collider is clicked with the specified mouse button. PickInfo contains the information of the raycast event")]
    public class MousePickEvent : EventNode, IUpdatable
    {

        public enum ButtonKeys
        {
            Left = 0,
            Right = 1,
            Middle = 2
        }

        public BBParameter<ButtonKeys> buttonKey;
        public BBParameter<LayerMask> mask = new BBParameter<LayerMask>(-1);

        private FlowOutput o;
        private RaycastHit hit;

        public override string name {
            get { return string.Format("{0} [{1}]", base.name, buttonKey); }
        }

        protected override void RegisterPorts() {
            o = AddFlowOutput("Object Picked");
            AddValueOutput<RaycastHit>("Pick Info", () => { return hit; });
        }

        public void Update() {
            if ( Input.GetMouseButtonDown((int)buttonKey.value) ) {
                if ( Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask.value) ) {
                    o.Call(new Flow());
                }
            }
        }
    }
}