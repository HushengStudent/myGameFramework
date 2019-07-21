using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using ParadoxNotion.Serialization;

namespace FlowCanvas.Nodes
{

    [Category("Flow Controllers/Switchers")]
    [Description("Branch the Flow based on an enum value.\nPlease connect an Enum first for the options to show, or directly select the enum type with the relevant button bellow.")]
    [ContextDefinedInputs(typeof(System.Enum))]
    public class SwitchEnum : FlowControlNode
    {

        [SerializeField]
        private SerializedTypeInfo _type;

        private System.Type type {
            get { return _type != null ? _type.Get() : null; }
            set
            {
                if ( _type == null || _type.Get() != value ) {
                    _type = new SerializedTypeInfo(value);
                }
            }
        }

        protected override void RegisterPorts() {
            if ( type == null ) {
                type = typeof(System.Enum);
            }

            var selector = AddValueInput(type.Name, type, "Enum");
            if ( type != typeof(System.Enum) ) {
                var enumNames = System.Enum.GetNames(type);
                var cases = new Dictionary<string, FlowOutput>(enumNames.Length);
                for ( var i = 0; i < enumNames.Length; i++ ) {
                    cases[enumNames[i]] = AddFlowOutput(enumNames[i]);
                }
                AddFlowInput("In", (f) =>
                {
                    cases[selector.value.ToString()].Call(f);
                });
            }
        }

        public override System.Type GetNodeWildDefinitionType() {
            return typeof(System.Enum);
        }

        public override void OnPortConnected(Port port, Port otherPort) {
            if ( type == typeof(System.Enum) ) {
                if ( typeof(System.Enum).RTIsAssignableFrom(otherPort.type) ) {
                    type = otherPort.type;
                    GatherPorts();
                }
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeInspectorGUI() {
            base.OnNodeInspectorGUI();
            if ( GUILayout.Button("Select Enum Type") ) {
                EditorUtils.ShowPreferedTypesSelectionMenu(typeof(System.Enum), (t) => { type = t; GatherPorts(); });
            }
        }

#endif
        ///----------------------------------------------------------------------------------------------

    }
}