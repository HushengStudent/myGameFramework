using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Category("Flow Controllers/Switchers")]
    [Description("Branch the Flow based on the tag of a GameObject value")]
    [ContextDefinedInputs(typeof(GameObject))]
    [HasRefreshButton]
    public class SwitchTag : FlowControlNode
    {

        //serialized since tags are fetched in editor
        [SerializeField]
        private string[] _tagNames = null;

        protected override void RegisterPorts() {

#if UNITY_EDITOR
            _tagNames = UnityEditorInternal.InternalEditorUtility.tags;
#endif

            var selector = AddValueInput<GameObject>("Value");
            var cases = new FlowOutput[_tagNames.Length];
            for ( var i = 0; i < cases.Length; i++ ) {
                cases[i] = AddFlowOutput(_tagNames[i], i.ToString());
            }

            AddFlowInput("In", (f) =>
            {
                for ( var i = 0; i < _tagNames.Length; i++ ) {
                    if ( _tagNames[i] == selector.value.tag ) {
                        cases[i].Call(f);
                    }
                }
            });
        }
    }
}