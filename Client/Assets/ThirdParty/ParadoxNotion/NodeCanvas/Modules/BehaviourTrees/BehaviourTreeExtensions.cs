#if UNITY_EDITOR

using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Linq;

namespace NodeCanvas.BehaviourTrees
{

    public static class BehaviourTreeExtensions
    {

        ///Replace the node with another
        public static Node ReplaceWith(this Node node, System.Type t) {

            var newNode = node.graph.AddNode(t, node.position);
            foreach ( var c in node.inConnections.ToArray() ) {
                c.SetTargetNode(newNode);
            }

            foreach ( var c in node.outConnections.ToArray() ) {
                c.SetSourceNode(newNode);
            }

            if ( node.graph.primeNode == node ) {
                node.graph.primeNode = newNode;
            }

            if ( node is ITaskAssignable && newNode is ITaskAssignable ) {
                var assignableNode = node as ITaskAssignable;
                var assignableNewNode = newNode as ITaskAssignable;
                try { assignableNewNode.task = assignableNode.task; }
                catch { /* cant assign */ }
            }

            node.graph.RemoveNode(node);
            return newNode;
        }

        ///Create a new SubTree out of the branch of the provided root node
        public static BehaviourTree ConvertToSubTree(this BTNode root) {

            if ( !UnityEditor.EditorUtility.DisplayDialog("Convert to SubTree", "This will create a new SubTree out of this branch.\nThe SubTree can NOT be unpacked later on.\nAre you sure?", "Yes", "No!") ) {
                return null;
            }

            var newBT = EditorUtils.CreateAsset<BehaviourTree>();
            if ( newBT == null ) {
                return null;
            }

            var subTreeNode = root.graph.AddNode<SubTree>(root.position);
            subTreeNode.subTree = newBT;

            for ( var i = 0; i < root.inConnections.Count; i++ ) {
                root.inConnections[i].SetTargetNode(subTreeNode);
            }

            root.inConnections.Clear();

            newBT.primeNode = DuplicateBranch(root, newBT);
            DeleteBranch(root);

            UnityEditor.AssetDatabase.SaveAssets();
            return newBT;
        }

        ///Delete the whole branch of provided root node along with the root node
        public static void DeleteBranch(this BTNode root) {
            var graph = root.graph;
            foreach ( var node in root.GetAllChildNodesRecursively(true).ToArray() ) {
                graph.RemoveNode(node);
            }
        }

        ///Duplicate a node along with all children hierarchy
        public static Node DuplicateBranch(this BTNode root, Graph targetGraph) {

            if ( targetGraph == null ) {
                return null;
            }

            var newNode = root.Duplicate(targetGraph);
            var dupConnections = new List<Connection>();
            for ( var i = 0; i < root.outConnections.Count; i++ ) {
                dupConnections.Add(root.outConnections[i].Duplicate(newNode, DuplicateBranch((BTNode)root.outConnections[i].targetNode, targetGraph)));
            }
            newNode.outConnections.Clear();
            foreach ( var c in dupConnections ) {
                newNode.outConnections.Add(c);
            }
            return newNode;
        }

        ///Decorates BT node with decorator type
        public static Node DecorateWith(this BTNode node, System.Type t) {
            var newNode = node.graph.AddNode(t, node.position + new UnityEngine.Vector2(0, -80));
            if ( node.inConnections.Count == 0 ) {
                node.graph.ConnectNodes(newNode, node);
            } else {
                var parent = node.inConnections[0].sourceNode;
                var parentConnection = node.inConnections[0];
                var index = parent.outConnections.IndexOf(parentConnection);
                node.graph.RemoveConnection(parentConnection);
                node.graph.ConnectNodes(newNode, node);
                node.graph.ConnectNodes(parent, newNode, index);
                NodeCanvas.Editor.GraphEditorUtility.activeElement = newNode;
            }
            return newNode;
        }

        ///Fetch all child nodes of the node recursively, optionaly including this.
        ///In other words, this fetches the whole branch.
        public static List<BTNode> GetAllChildNodesRecursively(this BTNode root, bool includeThis) {

            var childList = new List<BTNode>();
            if ( includeThis ) {
                childList.Add(root);
            }

            foreach ( BTNode child in root.outConnections.Select(c => c.targetNode) ) {
                childList.AddRange(child.GetAllChildNodesRecursively(true));
            }

            return childList;
        }

        ///Fetch all child nodes of this node with their depth in regards to this node.
        ///So, first level children will have a depth of 1 while second level a depth of 2
        public static Dictionary<BTNode, int> GetAllChildNodesWithDepthRecursively(this BTNode root, bool includeThis, int startIndex) {

            var childList = new Dictionary<BTNode, int>();
            if ( includeThis ) {
                childList[root] = startIndex;
            }

            foreach ( BTNode child in root.outConnections.Select(c => c.targetNode) ) {
                foreach ( var pair in child.GetAllChildNodesWithDepthRecursively(true, startIndex + 1) ) {
                    childList[pair.Key] = pair.Value;
                }
            }

            return childList;
        }

    }
}

#endif