using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using System.Linq;

namespace NodeCanvas.DialogueTrees
{

    [Name("JUMP")]
    [Description("Select a target node to jump to.\nFor your convenience in identifying nodes in the dropdown, please give a Tag name to the nodes you want to use in this way.")]
    [Category("Control")]
    [Icon("Set")]
    [Color("00b9e8")]
    public class Jumper : DTNode
    {

        [SerializeField]
        private string _sourceNodeUID;
        private string sourceNodeUID {
            get { return _sourceNodeUID; }
            set { _sourceNodeUID = value; }
        }

        private object _sourceNode;
        private DTNode sourceNode {
            get
            {
                if ( _sourceNode == null ) {
                    _sourceNode = graph.allNodes.OfType<DTNode>().FirstOrDefault(n => n.UID == sourceNodeUID);
                    if ( _sourceNode == null ) {
                        _sourceNode = new object();
                    }
                }
                return _sourceNode as DTNode;
            }
            set { _sourceNode = value; }
        }

        public override int maxOutConnections { get { return 0; } }
        public override bool requireActorSelection { get { return false; } }

        protected override Status OnExecute(Component agent, IBlackboard bb) {

            if ( sourceNode == null ) {
                return Error("Target Node of Jumper node is null");
            }

            DLGTree.EnterNode(sourceNode);
            return Status.Success;
        }


        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnNodeGUI() {
            GUILayout.Label(string.Format("<b>{0}</b>", sourceNode != null ? sourceNode.ToString() : "NONE"));
        }

        protected override void OnNodeInspectorGUI() {
            var currentEntry = graph.allNodes.OfType<DTNode>().FirstOrDefault(n => n.UID == sourceNodeUID);
            var newEntry = EditorUtils.Popup<DTNode>("Target Node", currentEntry, graph.allNodes.OfType<DTNode>().ToList());
            if ( newEntry != currentEntry ) {
                sourceNodeUID = newEntry != null ? newEntry.UID : null;
                sourceNode = newEntry;
            }

            if ( sourceNode != null && GUILayout.Button("Select Target") ) {
                NodeCanvas.Editor.GraphEditorUtility.activeElement = sourceNode;
            }
        }

#endif
    }
}