using System.Linq;
using UnityEngine;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{

    [Name("GOTO Label Definition", 1)]
    [Description("A Flow Control Label definition. Can be called with the GOTO node.")]
    [Category("Flow Controllers/GOTO")]
    [Color("ff5c5c")]
    [ContextDefinedOutputs(typeof(Flow))]
    public class GoToLabel : FlowControlNode, IEditorMenuCallbackReceiver
    {

        [Tooltip("The identifier name of the label")]
        [DelayedField]
        public string identifier = "MY_LABEL";
        [HideInInspector]
        public FlowOutput port { get; private set; }

        public override string name {
            get { return string.Format("[ {0} ]", identifier.ToUpper()); }
        }

        protected override void RegisterPorts() {
            port = AddFlowOutput(" ");
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
        void IEditorMenuCallbackReceiver.OnMenu(UnityEditor.GenericMenu menu, Vector2 pos, Port contextPort, object dropInstance) {
            if ( contextPort == null || contextPort is FlowOutput ) {
                menu.AddItem(new GUIContent(string.Format("Flow Controllers/GOTO/GOTO '{0}'", identifier)), false, () => { flowGraph.AddFlowNode<GoToStatement>(pos, contextPort, dropInstance).SetTarget(this); });
            }
        }
#endif
        ///----------------------------------------------------------------------------------------------

    }


    ///----------------------------------------------------------------------------------------------

    [DoNotList]
    [Description("Routes the Flow to the target GOTO label.")]
    [ContextDefinedInputs(typeof(Flow))]
    public class GoToStatement : FlowControlNode
    {

        [SerializeField]
        private string _targetLabelUID;
        private string targetLabelUID {
            get { return _targetLabelUID; }
            set { _targetLabelUID = value; }
        }

        private object _targetLabel;
        private GoToLabel targetLabel {
            get
            {
                if ( _targetLabel == null ) {
                    _targetLabel = graph.GetAllNodesOfType<GoToLabel>().FirstOrDefault(i => i.UID == targetLabelUID);
                    if ( _targetLabel == null ) { _targetLabel = new object(); }
                }
                return _targetLabel as GoToLabel;
            }
            set { _targetLabel = value; }
        }

        public override string name {
            get { return string.Format("GOTO {0}", targetLabel != null ? targetLabel.ToString() : "NONE"); }
        }

        public void SetTarget(GoToLabel target) {
            _targetLabelUID = target != null ? target.UID : null;
            _targetLabel = target != null ? target : null;
            GatherPorts();
        }

        protected override void RegisterPorts() {
            AddFlowInput(" ", (f) => { if ( targetLabel != null ) targetLabel.port.Call(f); });
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
        protected override void OnNodeInspectorGUI() {
            var labels = graph.GetAllNodesOfType<GoToLabel>();
            var newInput = EditorUtils.Popup<GoToLabel>("GOTO Label Target", targetLabel, labels);
            if ( newInput != targetLabel ) {
                if ( newInput == null ) {
                    SetTarget(null);
                    return;
                }
                SetTarget(newInput);
            }
        }
#endif
        ///----------------------------------------------------------------------------------------------

    }
}