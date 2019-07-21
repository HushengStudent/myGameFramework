using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Macros
{

    [DoNotList]
    [Color("ffe4e1")]
    public class MacroNodeWrapper : FlowNode, IGraphAssignable, IUpdatable
    {

        [SerializeField]
        private Macro _macro = null;
        private bool instantiated = false;

        public override string name {
            get { return macro != null ? macro.name : "No Macro"; }
        }

        public override string description {
            get { return _macro != null && !string.IsNullOrEmpty(_macro.comments) ? _macro.comments : base.description; }
        }

        public Macro macro {
            get { return _macro; }
            set
            {
                if ( _macro != value ) {
                    _macro = value;
                    if ( value != null ) {
                        GatherPorts();
                    }
                }
            }
        }

        Graph IGraphAssignable.nestedGraph {
            get { return macro; }
            set { macro = (Macro)value; }
        }

        Graph[] IGraphAssignable.GetInstances() { return instantiated ? new Graph[] { _macro } : new Graph[0]; }

        ///----------------------------------------------------------------------------------------------

        public void CheckInstance() {

            if ( macro == null ) {
                return;
            }

            if ( !instantiated ) {
                instantiated = true;
                macro = Graph.Clone<Macro>(macro);
            }
        }

        void IUpdatable.Update() {
            if ( macro == null || !instantiated ) {
                return;
            }

            macro.UpdateGraph();
        }

        protected override void RegisterPorts() {

            if ( macro == null || macro.entry == null || macro.exit == null ) {
                return;
            }

            for ( var i = 0; i < macro.inputDefinitions.Count; i++ ) {
                var defIn = macro.inputDefinitions[i];
                if ( defIn.type == typeof(Flow) ) {
                    AddFlowInput(defIn.name, (f) => { macro.entryActionMap[defIn.ID](f); }, defIn.ID);
                } else {
                    macro.entryFunctionMap[defIn.ID] = AddValueInput(defIn.name, defIn.type, defIn.ID).GetObjectValue;
                }
            }

            for ( var i = 0; i < macro.outputDefinitions.Count; i++ ) {
                var defOut = macro.outputDefinitions[i];
                if ( defOut.type == typeof(Flow) ) {
                    macro.exitActionMap[defOut.ID] = AddFlowOutput(defOut.name, defOut.ID).Call;
                } else {
                    AddValueOutput(defOut.name, defOut.type, () => { return macro.exitFunctionMap[defOut.ID](); }, defOut.ID);
                }
            }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeInspectorGUI() {

            if ( !Application.isPlaying ) {
                if ( macro != null ) {
                    if ( GUILayout.Button("REFRESH PORTS") ) {
                        GatherPorts();
                    }
                } else {
                    macro = (Macro)UnityEditor.EditorGUILayout.ObjectField("Macro", macro, typeof(Macro), false);
                }
            }

            base.OnNodeInspectorGUI();
        }

#endif
    }
}