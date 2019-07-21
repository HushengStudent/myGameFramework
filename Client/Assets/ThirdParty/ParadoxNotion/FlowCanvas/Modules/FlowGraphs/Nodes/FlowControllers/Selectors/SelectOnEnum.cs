using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;

namespace FlowCanvas.Nodes
{

    [ExposeAsDefinition]
    [ContextDefinedInputs(typeof(Wild), typeof(System.Enum))]
    [ContextDefinedOutputs(typeof(Wild))]
    [Category("Flow Controllers/Selectors")]
    [Description("Select a Result value out of the input cases provided, based on an Enum")]
    public class SelectOnEnum<T> : FlowControlNode
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
                var cases = new ValueInput<T>[enumNames.Length];
                for ( var i = 0; i < enumNames.Length; i++ ) {
                    cases[i] = AddValueInput<T>("Is " + enumNames[i], enumNames[i]);
                }

                AddValueOutput<T>("Result", "Value", () =>
                {
                    var enumValue = selector.value;
                    var index = (int)System.Enum.Parse(enumValue.GetType(), enumValue.ToString());
                    return cases[index].value;
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