using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;

namespace FlowCanvas.Nodes
{

    [ExposeAsDefinition]
    [ContextDefinedInputs(typeof(Wild), typeof(GameObject))]
    [ContextDefinedOutputs(typeof(Wild))]
    [Category("Flow Controllers/Selectors")]
    [Description("Select a Result value out of the input cases provided, based on a GameObject's Tag")]
    [HasRefreshButton]
    public class SelectOnTag<T> : FlowControlNode
    {

        //serialized since tags are fetched in editor
        [SerializeField]
        private string[] _tagNames = null;

        protected override void RegisterPorts() {

#if UNITY_EDITOR
            _tagNames = UnityEditorInternal.InternalEditorUtility.tags;
#endif

            var selector = AddValueInput<GameObject>("Value");
            var cases = new ValueInput<T>[_tagNames.Length];
            for ( var i = 0; i < cases.Length; i++ ) {
                cases[i] = AddValueInput<T>("Is " + _tagNames[i], i.ToString());
            }

            AddValueOutput<T>("Result", "Value", () =>
            {
                var tagValue = selector.value.tag;
                for ( var i = 0; i < _tagNames.Length; i++ ) {
                    if ( _tagNames[i] == tagValue ) {
                        return cases[i].value;
                    }
                }
                return default(T);
            });
        }
    }
}