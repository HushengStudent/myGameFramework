using System.Collections.Generic;
using System.Linq;
using ParadoxNotion;
using NodeCanvas.Framework;


namespace NodeCanvas.BehaviourTrees
{

    /// Super Base class for BehaviourTree nodes that can live within a BehaviourTree Graph.
    abstract public class BTNode : Node
    {

        sealed public override System.Type outConnectionType { get { return typeof(BTConnection); } }
        public override int maxInConnections { get { return 1; } }
        public override int maxOutConnections { get { return 0; } }
        public override bool allowAsPrime { get { return true; } }
        public override Alignment2x2 commentsAlignment { get { return Alignment2x2.Bottom; } }
        public override Alignment2x2 iconAlignment { get { return Alignment2x2.Default; } }

        ///Add a child node to this node connected to the specified child index
        public T AddChild<T>(int childIndex) where T : BTNode {
            if ( outConnections.Count >= maxOutConnections && maxOutConnections != -1 ) {
                return null;
            }
            var child = graph.AddNode<T>();
            graph.ConnectNodes(this, child, childIndex);
            return child;
        }

        ///Add a child node to this node connected last
        public T AddChild<T>() where T : BTNode {
            if ( outConnections.Count >= maxOutConnections && maxOutConnections != -1 ) {
                return null;
            }
            var child = graph.AddNode<T>();
            graph.ConnectNodes(this, child);
            return child;
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override UnityEditor.GenericMenu OnContextMenu(UnityEditor.GenericMenu menu) {
            menu.AddItem(new UnityEngine.GUIContent("Breakpoint"), isBreakpoint, () => { isBreakpoint = !isBreakpoint; });
            return ParadoxNotion.Design.EditorUtils.GetTypeSelectionMenu(typeof(BTDecorator), (t) => { this.DecorateWith(t); }, menu, "Decorate");
        }

#endif
    }
}