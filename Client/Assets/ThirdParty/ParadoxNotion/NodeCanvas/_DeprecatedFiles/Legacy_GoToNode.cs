using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.DialogueTrees
{

    [Name("GO TO")]
    [Category("Control")]
    [Description("Jump to another Dialogue node. Usefull if that other node is far away to connect, but otherwise it's exactly the same.\n\nPlease enable 'Show Node IDs' in Editor Prefs for convenience")]
    [Icon("Set")]
    [Color("00b9e8")]
    [System.Obsolete("Use Jumpers instead")]
    public class GoToNode : DTNode
    {

        [SerializeField]
        private DTNode _targetNode = null;

        public override int maxOutConnections { get { return 0; } }
        public override bool requireActorSelection { get { return false; } }

        protected override Status OnExecute(Component agent, IBlackboard bb) {
            if ( _targetNode == null ) {
                return Error("Target node of GOTO node is null");
            }

            DLGTree.EnterNode(_targetNode);
            return Status.Success;
        }


        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.Label(string.Format("<b> < #{0} > </b>", ( _targetNode != null ? _targetNode.ID.ToString() : "NONE" )));
        }

        protected override void OnNodeInspectorGUI() {
            if ( GUILayout.Button("Set Target Node") ) {

                UnityEditor.GenericMenu.MenuFunction2 Selected = (object o) =>
                {
                    _targetNode = (DTNode)o;
                };

                var menu = new UnityEditor.GenericMenu();
                foreach ( DTNode node in graph.allNodes ) {
                    if ( node != this ) {
                        menu.AddItem(new GUIContent("#" + node.ID.ToString()), false, Selected, node);
                    }
                }
                menu.ShowAsContext();
                Event.current.Use();
            }

            if ( _targetNode != null && GUILayout.Button("Select Target Node") ) {
                NodeCanvas.Editor.GraphEditorUtility.activeElement = _targetNode;
            }
        }

#endif
    }
}