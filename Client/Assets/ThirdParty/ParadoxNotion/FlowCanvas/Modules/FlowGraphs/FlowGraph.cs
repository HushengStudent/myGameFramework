using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using ParadoxNotion;
using NodeCanvas.Framework;
using FlowCanvas.Macros;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using Logger = ParadoxNotion.Services.Logger;

namespace FlowCanvas
{

    ///Base class for flow graphs.
    [GraphInfo(
        packageName = "FlowCanvas",
        docsURL = "http://flowcanvas.paradoxnotion.com/documentation/",
        resourcesURL = "http://flowcanvas.paradoxnotion.com/downloads/",
        forumsURL = "http://flowcanvas.paradoxnotion.com/forums-page/"
        )]
    [System.Serializable]
    abstract public class FlowGraph : Graph
    {

        private bool hasInitialized;
        private List<IUpdatable> updatableNodes;
        private Dictionary<string, IInvokable> functions;
        private Dictionary<System.Type, Component> cachedAgentComponents;

        public override System.Type baseNodeType { get { return typeof(FlowNode); } }
        public override bool useLocalBlackboard { get { return false; } }
        public override bool requiresAgent { get { return false; } }
        public override bool requiresPrimeNode { get { return false; } }
        public override bool isTree { get { return false; } }
        public override bool canAcceptVariableDrops { get { return true; } }

        ///Calls and returns a value of a custom function in the flowgraph
        public T CallFunction<T>(string name, params object[] args) {
            return (T)CallFunction(name, args);
        }

        ///Calls and returns a value of a custom function in the flowgraph
        public object CallFunction(string name, params object[] args) {
            IInvokable func = null;
            if ( functions.TryGetValue(name, out func) ) {
                return func.Invoke(args);
            }
            return null;
        }

        ///Calls a custom function in the flowgraph async. When the function is done, it will callback with return value
        public void CallFunctionAsync(string name, System.Action<object> callback, params object[] args) {
            IInvokable func = null;
            if ( functions.TryGetValue(name, out func) ) {
                func.InvokeAsync(callback, args);
            }
        }

        ///Returns cached component type from graph agent
        public UnityEngine.Object GetAgentComponent(System.Type type) {
            if ( agent == null ) { return null; }
            if ( type == typeof(GameObject) ) { return agent.gameObject; }
            if ( type == typeof(Transform) ) { return agent.transform; }
            if ( type == typeof(Component) ) { return agent; }

            if ( cachedAgentComponents == null ) {
                cachedAgentComponents = new Dictionary<System.Type, Component>();
            }

            Component component = null;
            if ( cachedAgentComponents.TryGetValue(type, out component) ) {
                return component;
            }

            if ( typeof(Component).RTIsAssignableFrom(type) || type.RTIsInterface() ) {
                component = agent.GetComponent(type);
            }

            return cachedAgentComponents[type] = component;
        }

        //...
        protected override void OnGraphStarted() {

            if ( !hasInitialized ) {
                updatableNodes = new List<IUpdatable>();
                functions = new Dictionary<string, IInvokable>(System.StringComparer.Ordinal);
            }

            for ( var i = 0; i < allNodes.Count; i++ ) {
                var node = allNodes[i];
                if ( node is MacroNodeWrapper ) {
                    var macroWrapper = (MacroNodeWrapper)node;
                    if ( macroWrapper.macro != null ) {
                        macroWrapper.CheckInstance();
                        macroWrapper.macro.StartGraph(agent, blackboard, false, null);
                    }
                }

                if ( !hasInitialized ) {
                    if ( node is IUpdatable ) {
                        updatableNodes.Add((IUpdatable)node);
                    }
                    if ( node is IInvokable ) {
                        var func = (IInvokable)node;
                        functions[func.GetInvocationID()] = func;
                    }
                }
            }


            if ( !hasInitialized ) {
                //2nd pass after macros have been init
                for ( var i = 0; i < allNodes.Count; i++ ) {
                    if ( allNodes[i] is FlowNode ) {
                        var flowNode = (FlowNode)allNodes[i];
                        flowNode.AssignSelfInstancePort();
                        flowNode.BindPorts();
                    }
                }
            }

            hasInitialized = true;
        }

        //Update IUpdatable nodes. Basicaly for events like Input, Update etc
        //This is the only thing that updates per-frame
        protected override void OnGraphUpdate() {
            if ( updatableNodes != null && updatableNodes.Count > 0 ) {
                for ( var i = 0; i < updatableNodes.Count; i++ ) {
                    updatableNodes[i].Update();
                }
            }
        }

        //...
        protected override void OnGraphStoped() {
            for ( var i = 0; i < allNodes.Count; i++ ) {
                var node = allNodes[i];
                if ( node is MacroNodeWrapper ) {
                    var macroWrapper = (MacroNodeWrapper)node;
                    if ( macroWrapper.macro != null ) {
                        macroWrapper.macro.Stop();
                    }
                }
            }
        }

        ///----------------------------------------------------------------------------------------------



        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        //Append menu items in canvas right click context menu (provided menu is completely overriden here)
        protected override UnityEditor.GenericMenu OnCanvasContextMenu(UnityEditor.GenericMenu menu, Vector2 mousePos) {
            return FlowGraphExtensions.GetFullNodesMenu(this, mousePos, null, null);
        }

        //Append ConvertToMacro feature
        protected override UnityEditor.GenericMenu OnNodesContextMenu(UnityEditor.GenericMenu menu, Node[] nodes) {
            menu.AddItem(new GUIContent("Convert To Macro"), false, () => { FlowGraphExtensions.ConvertNodesToMacro(nodes.ToList()); });
            return menu;
        }

        //Append buttons to toolbar
        protected override void OnGraphEditorToolbar() {
            if ( GUILayout.Button(new GUIContent(NodeCanvas.Editor.StyleSheet.verboseLevel1, "Minimize All/Selected"), UnityEditor.EditorStyles.toolbarButton, GUILayout.MaxHeight(12)) ) {
                foreach ( var node in NodeCanvas.Editor.GraphEditorUtility.GetSelectedOrAll(this).OfType<FlowNode>() ) {
                    node.verboseLevel = Node.VerboseLevel.Compact;
                }
            }
            if ( GUILayout.Button(new GUIContent(NodeCanvas.Editor.StyleSheet.verboseLevel2, "Show Connected Ports Only On All/Selected"), UnityEditor.EditorStyles.toolbarButton, GUILayout.MaxHeight(12)) ) {
                foreach ( var node in NodeCanvas.Editor.GraphEditorUtility.GetSelectedOrAll(this).OfType<FlowNode>() ) {
                    node.verboseLevel = Node.VerboseLevel.Partial;
                }
            }
            if ( GUILayout.Button(new GUIContent(NodeCanvas.Editor.StyleSheet.verboseLevel3, "Maximize All/Selected\nHold S Key over a node to temporarily maximize it"), UnityEditor.EditorStyles.toolbarButton, GUILayout.MaxHeight(12)) ) {
                foreach ( var node in NodeCanvas.Editor.GraphEditorUtility.GetSelectedOrAll(this).OfType<FlowNode>() ) {
                    node.verboseLevel = Node.VerboseLevel.Full;
                }
            }
        }

        //Create and set a UnityObject variable node on drop
        protected override void OnDropAccepted(Object o, Vector2 mousePos) {

            if ( o == null ) {
                return;
            }

            if ( UnityEditor.EditorUtility.IsPersistent(this) && !UnityEditor.EditorUtility.IsPersistent(o) ) {
                Logger.Log("This Graph is an asset. The dragged object is a scene reference. The reference will not persist!", "Editor");
            }

            //macro?
            if ( o is Macro ) {
                this.AddMacroNode((Macro)o, mousePos, null, null);
                return;
            }

            var targetType = o.GetType();
            var menu = new UnityEditor.GenericMenu();
            menu = AppendDragAndDropObjectMenu(menu, o, "", mousePos);
            menu.AddSeparator("/");
            menu = this.AppendTypeReflectionNodesMenu(menu, targetType, "", mousePos, null, o);
            if ( o is GameObject ) {
                foreach ( var component in ( o as GameObject ).GetComponents<Component>().Where(c => c.hideFlags == 0) ) {
                    var cType = component.GetType();
                    menu = AppendDragAndDropObjectMenu(menu, component, cType.Name + "/", mousePos);
                    menu = this.AppendTypeReflectionNodesMenu(menu, cType, "", mousePos, null, component);
                }
            }

            ParadoxNotion.Design.EditorUtils.ShowAsBrowser(menu, "Add Node For Drag & Drop Instance");
            Event.current.Use();
        }

        //Used above for convenience
        UnityEditor.GenericMenu AppendDragAndDropObjectMenu(UnityEditor.GenericMenu menu, UnityEngine.Object o, string category, Vector2 mousePos) {
            var targetType = o.GetType();
            foreach ( var _wrapperType in CustomObjectWrapper.FindCustomObjectWrappersForType(targetType) ) {
                var wrapperType = _wrapperType;
                menu.AddItem(new GUIContent(string.Format(category + "Add Wrapper ({0})", wrapperType.FriendlyName())), false, (x) => { this.AddObjectWrapper(wrapperType, mousePos, null, (UnityEngine.Object)x); }, o);
            }
            menu.AddItem(new GUIContent(string.Format(category + "Make Variable ({0})", targetType.FriendlyName())), false, (x) => { this.AddVariableGet(targetType, null, mousePos, null, x); }, o);
            return menu;
        }

        ///Show Get/Set variable menu
        protected override void OnVariableDropInGraph(Variable variable, Vector2 mousePos) {
            if ( variable != null ) {
                var menu = new UnityEditor.GenericMenu();
                menu.AddItem(new GUIContent("Get " + variable.name), false, () => { this.AddVariableGet(variable.varType, variable.name, mousePos, null, null); });
                menu.AddItem(new GUIContent("Set " + variable.name), false, () => { this.AddVariableSet(variable.varType, variable.name, mousePos, null, null); });
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

#endif
        ///----------------------------------------------------------------------------------------------

    }
}