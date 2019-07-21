using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees
{

    [Name("SubTree")]
    [Category("Nested")]
    [Description("SubTree Node can be assigned an entire Sub BehaviorTree. The root node of that behaviour will be considered child node of this node and will return whatever it returns.\nThe target SubTree can also be set by using a Blackboard variable as normal.")]
    [Icon("BT")]
    public class SubTree : BTNode, IGraphAssignable
    {

        [SerializeField]
        private BBParameter<BehaviourTree> _subTree = null;

        private Dictionary<BehaviourTree, BehaviourTree> instances = new Dictionary<BehaviourTree, BehaviourTree>();
        private BehaviourTree currentInstance = null;

        public override string name {
            get { return base.name.ToUpper(); }
        }

        public BehaviourTree subTree {
            get { return _subTree.value; }
            set { _subTree.value = value; }
        }

        Graph IGraphAssignable.nestedGraph {
            get { return subTree; }
            set { subTree = (BehaviourTree)value; }
        }

        Graph[] IGraphAssignable.GetInstances() { return instances.Values.ToArray(); }

        ///----------------------------------------------------------------------------------------------

        protected override Status OnExecute(Component agent, IBlackboard blackboard) {

            if ( subTree == null || subTree.primeNode == null ) {
                return Status.Optional;
            }

            if ( status == Status.Resting || currentInstance.isPaused ) {
                currentInstance = CheckInstance();
                currentInstance.StartGraph(agent, blackboard, false);
            }

            currentInstance.UpdateGraph();
            return currentInstance.rootStatus;
        }

        protected override void OnReset() {
            if ( currentInstance != null ) {
                currentInstance.Stop();
            }
        }

        public override void OnGraphPaused() {
            if ( currentInstance != null ) {
                currentInstance.Pause();
            }
        }

        public override void OnGraphStoped() {
            if ( currentInstance != null ) {
                currentInstance.Stop();
            }
        }

        BehaviourTree CheckInstance() {

            if ( subTree == currentInstance ) {
                return currentInstance;
            }

            BehaviourTree instance = null;
            if ( !instances.TryGetValue(subTree, out instance) ) {
                instance = Graph.Clone<BehaviourTree>(subTree);
                instances[subTree] = instance;
            }

            subTree = instance;
            return instance;
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.Label(string.Format("SubTree\n{0}", _subTree));
            if ( subTree == null ) {
                if ( !Application.isPlaying && GUILayout.Button("CREATE NEW") ) {
                    Node.CreateNested<BehaviourTree>(this);
                }
            }
        }

        protected override void OnNodeInspectorGUI() {

            NodeCanvas.Editor.BBParameterEditor.ParameterField("Behaviour SubTree", _subTree);

            if ( subTree == this.graph ) {
                Debug.LogWarning("You can't have a Graph nested to iteself! Please select another");
                subTree = null;
            }

            if ( subTree != null ) {

                var defParams = subTree.GetDefinedParameters();
                if ( defParams.Length != 0 ) {
                    EditorUtils.TitledSeparator("Defined SubTree Parameters");
                    GUI.color = Color.yellow;
                    UnityEditor.EditorGUILayout.LabelField("Name", "Type");
                    GUI.color = Color.white;
                    var added = new List<string>();
                    foreach ( var bbVar in defParams ) {
                        if ( !added.Contains(bbVar.name) ) {
                            UnityEditor.EditorGUILayout.LabelField(bbVar.name, bbVar.varType.FriendlyName());
                            added.Add(bbVar.name);
                        }
                    }
                    if ( GUILayout.Button("Check/Create Blackboard Variables") ) {
                        subTree.PromoteDefinedParametersToVariables(graphBlackboard);
                    }
                }
            }
        }

#endif
    }
}