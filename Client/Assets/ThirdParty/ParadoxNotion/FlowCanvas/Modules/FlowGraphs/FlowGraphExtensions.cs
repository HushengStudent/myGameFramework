#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
using System.Linq;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using NodeCanvas.Editor;
using FlowCanvas.Nodes;
using FlowCanvas.Macros;


namespace FlowCanvas
{

    public static class FlowGraphExtensions
    {

        //...
        public static T AddFlowNode<T>(this FlowGraph graph, Vector2 pos, Port sourcePort, object dropInstance) where T : FlowNode {
            return (T)AddFlowNode(graph, typeof(T), pos, sourcePort, dropInstance);
        }

        //...
        public static FlowNode AddFlowNode(this FlowGraph graph, System.Type type, Vector2 pos, Port sourcePort, object dropInstance) {
            if ( type.IsGenericTypeDefinition ) { type = type.MakeGenericType(type.GetFirstGenericParameterConstraintType()); }
            var node = (FlowNode)graph.AddNode(type, pos);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        ///----------------------------------------------------------------------------------------------

        //...
        public static CustomObjectWrapper AddObjectWrapper(this FlowGraph graph, System.Type type, Vector2 pos, Port sourcePort, UnityEngine.Object dropInstance) {
            var node = (CustomObjectWrapper)graph.AddNode(type, pos);
            node.SetTarget(dropInstance);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static VariableNode AddVariableGet(this FlowGraph graph, System.Type varType, string varName, Vector2 pos, Port sourcePort, object dropInstance) {
            var genericType = typeof(GetVariable<>).MakeGenericType(varType);
            var node = (VariableNode)graph.AddNode(genericType, pos);
            genericType.GetMethod("SetTargetVariableName").Invoke(node, new object[] { varName });
            Finalize(node, sourcePort, dropInstance);
            if ( dropInstance != null ) {
                node.SetVariable(dropInstance);
            }
            return node;
        }

        //...
        public static FlowNode AddVariableSet(this FlowGraph graph, System.Type varType, string varName, Vector2 pos, Port sourcePort, object dropInstance) {
            var genericType = typeof(SetVariable<>).MakeGenericType(varType);
            var node = (FlowNode)graph.AddNode(genericType, pos);
            genericType.GetMethod("SetTargetVariableName").Invoke(node, new object[] { varName });
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static FlowNode AddSimplexNode(this FlowGraph graph, System.Type type, Vector2 pos, Port sourcePort, object dropInstance) {
            if ( type.IsGenericTypeDefinition ) { type = type.MakeGenericType(type.GetFirstGenericParameterConstraintType()); }
            var genericType = typeof(SimplexNodeWrapper<>).MakeGenericType(type);
            var node = (FlowNode)graph.AddNode(genericType, pos);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static MacroNodeWrapper AddMacroNode(this FlowGraph graph, Macro m, Vector2 pos, Port sourcePort, object dropInstance) {
            var node = graph.AddNode<MacroNodeWrapper>(pos);
            node.macro = (Macro)m;
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        ///----------------------------------------------------------------------------------------------

        //...
        public static ReflectedConstructorNodeWrapper AddContructorNode(this FlowGraph graph, ConstructorInfo c, Vector2 pos, Port sourcePort, object dropInstance) {
            var node = graph.AddNode<ReflectedConstructorNodeWrapper>(pos);
            node.SetMethodBase(c);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static ReflectedMethodNodeWrapper AddMethodNode(this FlowGraph graph, MethodInfo m, Vector2 pos, Port sourcePort, object dropInstance) {
            var node = graph.AddNode<ReflectedMethodNodeWrapper>(pos);
            node.SetMethodBase(m);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static ReflectedFieldNodeWrapper AddFieldGetNode(this FlowGraph graph, FieldInfo f, Vector2 pos, Port sourcePort, object dropInstance) {
            var node = graph.AddNode<ReflectedFieldNodeWrapper>(pos);
            node.SetField(f, ReflectedFieldNodeWrapper.AccessMode.GetField);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static ReflectedFieldNodeWrapper AddFieldSetNode(this FlowGraph graph, FieldInfo f, Vector2 pos, Port sourcePort, object dropInstance) {
            var node = graph.AddNode<ReflectedFieldNodeWrapper>(pos);
            node.SetField(f, ReflectedFieldNodeWrapper.AccessMode.SetField);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static UnityEventAutoCallbackEvent AddUnityEventAutoCallbackNode(this FlowGraph graph, FieldInfo field, Vector2 pos, Port sourcePort, object dropInstance) {
            var node = graph.AddNode<UnityEventAutoCallbackEvent>(pos);
            node.SetEvent(field, dropInstance);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static CSharpAutoCallbackEvent AddCSharpEventAutoCallbackNode(this FlowGraph graph, EventInfo info, Vector2 pos, Port sourcePort, object dropInstance) {
            var node = graph.AddNode<CSharpAutoCallbackEvent>(pos);
            node.SetEvent(info, dropInstance);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        public static GetSharpEvent AddCSharpGetNode(this FlowGraph graph, EventInfo info, Vector2 pos, Port sourcePort, object dropInstance) {
            var node = graph.AddNode<GetSharpEvent>(pos);
            node.SetEvent(info);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static FlowNode AddSimplexExtractorNode(this FlowGraph graph, System.Type type, Vector2 pos, Port sourcePort, object dropInstance) {
            var simplexWrapper = typeof(SimplexNodeWrapper<>).MakeGenericType(type);
            var node = (FlowNode)graph.AddNode(simplexWrapper, pos);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }

        //...
        public static FlowNode AddReflectedExtractorNode(this FlowGraph graph, System.Type type, Vector2 pos, Port sourcePort, object dropInstance) {
            var genericType = typeof(ReflectedExtractorNodeWrapper<>).MakeGenericType(type);
            var node = (FlowNode)graph.AddNode(genericType, pos);
            Finalize(node, sourcePort, dropInstance);
            return node;
        }


        ///----------------------------------------------------------------------------------------------

        public static void Finalize(FlowNode node, Port sourcePort, object dropInstance) {
            FinalizeConnection(sourcePort, node);
            DropInstance(node, dropInstance);
            Select(node);
        }

        //...
        static void FinalizeConnection(Port sourcePort, FlowNode targetNode) {
            if ( sourcePort == null || targetNode == null ) {
                return;
            }

            Port source = null;
            Port target = null;

            if ( sourcePort is ValueOutput || sourcePort is FlowOutput ) {
                source = sourcePort;
                target = targetNode.GetFirstInputOfType(sourcePort.type);
            } else {
                source = targetNode.GetFirstOutputOfType(sourcePort.type);
                target = sourcePort;
            }

            BinderConnection.Create(source, target);
        }

        //...
        static void DropInstance(FlowNode targetNode, object dropInstance) {
            if ( targetNode == null || dropInstance == null ) {
                return;
            }

            //dont set instance if it's 'Self'
            if ( dropInstance is UnityEngine.Object ) {
                var ownerGO = targetNode.graph.agent != null ? targetNode.graph.agent.gameObject : null;
                if ( ownerGO != null ) {
                    var dropGO = dropInstance as GameObject;
                    if ( dropGO == ownerGO ) {
                        return;
                    }
                    var dropComp = dropInstance as Component;
                    if ( dropComp != null && dropComp.gameObject == ownerGO ) {
                        return;
                    }
                }
            }

            var instancePort = targetNode.GetFirstInputOfType(dropInstance.GetType()) as ValueInput;
            if ( instancePort != null ) {
                instancePort.serializedValue = dropInstance;
            }
        }

        //...
        static void Select(FlowNode targetNode) {
            GraphEditorUtility.activeElement = targetNode;
        }

        ///----------------------------------------------------------------------------------------------


        ///Returns all nodes' menu
        public static UnityEditor.GenericMenu GetFullNodesMenu(this FlowGraph flowGraph, Vector2 mousePos, Port context, Object dropInstance) {
            var menu = new UnityEditor.GenericMenu();
            if ( context is ValueInput || context is ValueOutput ) {
                menu = flowGraph.AppendTypeReflectionNodesMenu(menu, context.type, "", mousePos, context, dropInstance);
            }
            menu = flowGraph.AppendFlowNodesMenu(menu, "", mousePos, context, dropInstance);
            menu = flowGraph.AppendSimplexNodesMenu(menu, "Functions/Implemented", mousePos, context, dropInstance);
            menu = flowGraph.AppendAllReflectionNodesMenu(menu, "Functions/Reflected", mousePos, context, dropInstance);
            menu = flowGraph.AppendVariableNodesMenu(menu, "Variables", mousePos, context, dropInstance);
            menu = flowGraph.AppendMacroNodesMenu(menu, "MACROS", mousePos, context, dropInstance);
            menu = flowGraph.AppendMenuCallbackReceivers(menu, "", mousePos, context, dropInstance);
            return menu;
        }


        ///----------------------------------------------------------------------------------------------

        //very special case. Used in AppendFlowNodesMenu bellow.
        static System.Type[] AlterTypesDefinition(System.Type[] types, System.Type input) {
            if ( input != null && input.IsGenericType && types != null ) {
                var concreteTypes = new System.Type[types.Length];
                var genericArg1 = input.GetGenericArguments()[0];
                for ( var i = 0; i < types.Length; i++ ) {
                    var t = types[i];
                    if ( t.IsGenericTypeDefinition ) {
                        concreteTypes[i] = t.MakeGenericType(genericArg1);
                        continue;
                    }
                    if ( t == typeof(Wild) ) {
                        concreteTypes[i] = genericArg1;
                    }
                }
                return concreteTypes;
            }
            return types;
        }

        //FlowNode
        public static UnityEditor.GenericMenu AppendFlowNodesMenu(this FlowGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance) {
            var infos = EditorUtils.GetScriptInfosOfType(typeof(FlowNode));
            var generalized = new List<System.Type>();
            foreach ( var _info in infos ) {
                var info = _info;
                if ( sourcePort != null ) {

                    if ( generalized.Contains(info.originalType) ) {
                        continue;
                    }

                    if ( sourcePort.IsValuePort() ) {
                        if ( info.originalType.IsGenericTypeDefinition ) {
                            var genericInfo = info.MakeGenericInfo(sourcePort.type);
                            if ( genericInfo != null ) {
                                info = genericInfo;
                                generalized.Add(info.originalType);
                            }
                        }
                    }

                    var definedInputTypesAtts = info.type.RTGetAttributesRecursive<FlowNode.ContextDefinedInputsAttribute>();
                    var definedOutputTypesAtts = info.type.RTGetAttributesRecursive<FlowNode.ContextDefinedOutputsAttribute>();
                    System.Type[] concreteInputTypes = null;
                    if ( definedInputTypesAtts.Length > 0 ) {
                        concreteInputTypes = definedInputTypesAtts.Select(att => att.types).Aggregate((x, y) => { return x.Union(y).ToArray(); });
                        concreteInputTypes = AlterTypesDefinition(concreteInputTypes, info.type);
                    }
                    System.Type[] concreteOutputTypes = null;
                    if ( definedOutputTypesAtts.Length > 0 ) {
                        concreteOutputTypes = definedOutputTypesAtts.Select(att => att.types).Aggregate((x, y) => { return x.Union(y).ToArray(); });
                        concreteOutputTypes = AlterTypesDefinition(concreteOutputTypes, info.type);
                    }

                    if ( sourcePort is ValueOutput || sourcePort is FlowOutput ) {
                        if ( concreteInputTypes == null || !concreteInputTypes.Any(t => t != null && t.IsAssignableFrom(sourcePort.type)) ) {
                            continue;
                        }
                    }

                    if ( sourcePort is ValueInput || sourcePort is FlowInput ) {
                        if ( concreteOutputTypes == null || !concreteOutputTypes.Any(t => t != null && sourcePort.type.IsAssignableFrom(t)) ) {
                            continue;
                        }
                    }
                }
                var category = string.Join("/", new string[] { baseCategory, info.category, info.name }).TrimStart('/');
                menu.AddItem(new GUIContent(category), false, (o) => { graph.AddFlowNode((System.Type)o, pos, sourcePort, dropInstance); }, info.type);
            }
            return menu;
        }

        ///Simplex Nodes
        public static UnityEditor.GenericMenu AppendSimplexNodesMenu(this FlowGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance) {
            var infos = EditorUtils.GetScriptInfosOfType(typeof(SimplexNode));
            var generalized = new List<System.Type>();
            foreach ( var _info in infos ) {
                var info = _info;
                if ( sourcePort != null ) {

                    if ( generalized.Contains(info.originalType) ) {
                        continue;
                    }

                    if ( sourcePort.IsValuePort() ) {
                        if ( info.originalType.IsGenericTypeDefinition ) {
                            var genericInfo = info.MakeGenericInfo(sourcePort.type);
                            if ( genericInfo != null ) {
                                info = genericInfo;
                                generalized.Add(info.originalType);
                            }
                        }
                    }


                    var outProperties = info.type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    var method = info.type.GetMethod("Invoke");
                    if ( method != null ) {
                        if ( sourcePort is ValueOutput ) {
                            var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                            if ( parameterTypes.Length == 0 || !parameterTypes.Any(t => t.IsAssignableFrom(sourcePort.type)) ) {
                                continue;
                            }
                        }
                        if ( sourcePort is ValueInput ) {
                            if ( !sourcePort.type.IsAssignableFrom(method.ReturnType) && !outProperties.Any(p => sourcePort.type.IsAssignableFrom(p.PropertyType)) ) {
                                continue;
                            }
                        }
                        if ( sourcePort is FlowOutput || sourcePort is FlowInput ) {
                            if ( method.ReturnType != typeof(void) && method.ReturnType != typeof(System.Collections.IEnumerator) ) {
                                continue;
                            }
                            if ( info.type.IsSubclassOf(typeof(ExtractorNode)) ) {
                                continue;
                            }
                        }
                    }
                }

                var category = string.Join("/", new string[] { baseCategory, info.category, info.name }).TrimStart('/');
                menu.AddItem(new GUIContent(category), false, (o) => { graph.AddSimplexNode((System.Type)o, pos, sourcePort, dropInstance); }, info.type);
            }
            return menu;
        }


        ///All reflection type nodes
        public static UnityEditor.GenericMenu AppendAllReflectionNodesMenu(this FlowGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance) {
            foreach ( var type in TypePrefs.GetPreferedTypesList() ) {
                menu = graph.AppendTypeReflectionNodesMenu(menu, type, baseCategory, pos, sourcePort, dropInstance);
            }
            return menu;
        }

        ///Refletion nodes on a type
        public static UnityEditor.GenericMenu AppendTypeReflectionNodesMenu(this FlowGraph graph, UnityEditor.GenericMenu menu, System.Type type, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance) {

            if ( !string.IsNullOrEmpty(baseCategory) ) {
                baseCategory += "/";
            }

            var typeCategory = baseCategory + type.FriendlyName();

            if ( !type.IsPrimitive ) {
                if ( sourcePort == null || ( sourcePort is ValueOutput && type.IsAssignableFrom(sourcePort.type) ) ) {
                    var extractName = string.Format("Extract {0}", type.FriendlyName());
                    menu.AddItem(new GUIContent(typeCategory + "/" + extractName), false, (o) => { graph.AddReflectedExtractorNode(type, pos, sourcePort, dropInstance); }, type);
                }
            }

            if ( sourcePort is ValueInput ) {
                if ( sourcePort.type == type ) {
                    var portValue = ( sourcePort as ValueInput ).serializedValue;
                    menu.AddItem(new GUIContent("Make Constant Variable"), false, (o) => { graph.AddVariableGet((System.Type)o, null, pos, sourcePort, portValue); }, type);
                    menu.AddItem(new GUIContent("Make Linked Variable"), false, (o) =>
                    {
                        var bbVar = graph.blackboard.AddVariable(sourcePort.name, sourcePort.type);
                        if ( bbVar != null ) {
                            graph.AddVariableGet((System.Type)o, bbVar.name, pos, sourcePort, portValue);
                        }
                    }, type);
                    menu.AddSeparator("/");
                }
            }


            //Constructors
            if ( !type.IsAbstract && !type.IsInterface && !type.IsPrimitive && type != typeof(string) ) {
                foreach ( var _c in type.RTGetConstructors() ) {
                    var c = _c;
                    if ( !c.IsPublic || c.IsObsolete() ) {
                        continue;
                    }

                    if ( sourcePort is FlowInput || sourcePort is FlowOutput ) {
                        continue;
                    }

                    var parameters = c.GetParameters();
                    if ( sourcePort is ValueOutput ) {
                        if ( !parameters.Any(p => p.ParameterType.IsAssignableFrom(sourcePort.type)) ) {
                            continue;
                        }
                    }

                    if ( sourcePort is ValueInput ) {
                        if ( !sourcePort.type.IsAssignableFrom(type) ) {
                            continue;
                        }
                    }

                    var categoryName = typeCategory + "/" + "Constructors/";
                    var name = categoryName + c.SignatureName();

                    if ( typeof(Component).IsAssignableFrom(type) ) {
                        if ( type == typeof(Transform) ) {
                            continue;
                        }
                        var stubType = typeof(AddComponent<>).MakeGenericType(type);
                        menu.AddItem(new GUIContent(name), false, (o) => { graph.AddSimplexNode(stubType, pos, sourcePort, dropInstance); }, c);
                        continue;
                    }

                    if ( typeof(ScriptableObject).IsAssignableFrom(type) ) {
                        var stubType = typeof(NewScriptableObject<>).MakeGenericType(type);
                        menu.AddItem(new GUIContent(name), false, (o) => { graph.AddSimplexNode(stubType, pos, sourcePort, dropInstance); }, c);
                        continue;
                    }

                    //exclude types like Mathf, Random, Time etc (they are not static)
                    if ( !TypePrefs.functionalTypesBlacklist.Contains(type) ) {
                        menu.AddItem(new GUIContent(name), false, (o) => { graph.AddContructorNode((ConstructorInfo)o, pos, sourcePort, dropInstance); }, c);
                    }
                }
            }

            //Methods
            var methods = type.RTGetMethods().ToList();
            methods.AddRange(type.GetExtensionMethods());
            foreach ( var _m in methods.OrderBy(_m => !_m.IsStatic).OrderBy(_m => _m.GetMethodSpecialType()).OrderBy(_m => _m.DeclaringType != type) ) {
                var m = _m;
                if ( !m.IsPublic || m.IsObsolete() ) {
                    continue;
                }

                //convertions are handled automatically at a connection level
                if ( m.Name == "op_Implicit" || m.Name == "op_Explicit" ) {
                    continue;
                }

                var isGeneric = m.IsGenericMethod && m.GetGenericArguments().Length == 1;
                if ( isGeneric ) {
                    if ( sourcePort != null && sourcePort.IsValuePort() ) {
                        if ( m.CanBeMadeGenericWith(sourcePort.type) ) {
                            m = m.MakeGenericMethod(sourcePort.type);
                        } else {
                            continue;
                        }
                    }
                }

                var parameters = m.GetParameters();
                if ( sourcePort is ValueOutput ) {
                    if ( type != sourcePort.type || m.IsStatic ) {
                        if ( !parameters.Any(p => p.ParameterType.IsAssignableFrom(sourcePort.type)) ) {
                            continue;
                        }
                    }
                }

                if ( sourcePort is ValueInput ) {
                    if ( !sourcePort.type.IsAssignableFrom(m.ReturnType) && !parameters.Any(p => p.IsOut && sourcePort.type.IsAssignableFrom(p.ParameterType)) ) {
                        continue;
                    }
                }

                if ( sourcePort is FlowInput || sourcePort is FlowOutput ) {
                    if ( m.ReturnType != typeof(void) ) {
                        continue;
                    }
                }

                var categoryName = typeCategory;
                var signatureName = m.SignatureName();
                var specialType = m.GetMethodSpecialType();
                if ( specialType == ReflectionTools.MethodType.Normal ) {
                    categoryName += "/Methods/";
                }
                if ( specialType == ReflectionTools.MethodType.PropertyAccessor ) {
                    categoryName += "/Properties/";
                }
                if ( specialType == ReflectionTools.MethodType.Operator ) {
                    categoryName += "/Operators/";
                }
                if ( specialType == ReflectionTools.MethodType.Event ) {
                    categoryName += "/Events/";
                }

                //Unity Event special case
                if ( typeof(UnityEventBase).IsAssignableFrom(m.ReturnType) && specialType == ReflectionTools.MethodType.PropertyAccessor ) {
                    categoryName = typeCategory + "/Events/";
                }

                var isExtension = m.IsExtensionMethod();
                if ( m.DeclaringType != type ) { categoryName += isExtension ? "Extensions/" : "Inherited/"; }
                var name = categoryName + signatureName;
                menu.AddItem(new GUIContent(name), false, (o) => { graph.AddMethodNode((MethodInfo)o, pos, sourcePort, dropInstance); }, m);
            }

            //Fields
            foreach ( var _f in type.RTGetFields() ) {
                var f = _f;
                if ( !f.IsPublic || f.IsObsolete() ) {
                    continue;
                }

                var isReadOnly = f.IsReadOnly();
                var isConstant = f.IsConstant();

                if ( sourcePort is ValueOutput ) {
                    if ( type != sourcePort.type || isConstant ) {
                        if ( isReadOnly || !f.FieldType.IsAssignableFrom(sourcePort.type) ) {
                            continue;
                        }
                    }
                }

                if ( sourcePort is ValueInput ) {
                    if ( !sourcePort.type.IsAssignableFrom(f.FieldType) ) {
                        continue;
                    }
                }

                var isUnityEvent = typeof(UnityEventBase).IsAssignableFrom(f.FieldType);
                var categoryName = typeCategory + ( isUnityEvent ? "/Events/" : "/Fields/" );
                if ( f.DeclaringType != type ) { categoryName += "Inherited/"; }

                //Unity Event
                if ( isUnityEvent ) {
                    menu.AddItem(new GUIContent(categoryName + f.Name + " (Auto Subscribe)"), false, (o) => { graph.AddUnityEventAutoCallbackNode((FieldInfo)o, pos, sourcePort, dropInstance); }, f);
                    menu.AddItem(new GUIContent(categoryName + f.Name + " (Get Reference)"), false, (o) => { graph.AddFieldGetNode((FieldInfo)o, pos, sourcePort, dropInstance); }, f);
                    continue;
                }

                var nameForGet = categoryName + ( isConstant ? "constant " + f.Name : "Get " + f.Name );
                menu.AddItem(new GUIContent(nameForGet), false, (o) => { graph.AddFieldGetNode((FieldInfo)o, pos, sourcePort, dropInstance); }, f);

                if ( !isReadOnly ) {
                    var nameForSet = categoryName + "Set " + f.Name;
                    menu.AddItem(new GUIContent(nameForSet), false, (o) => { graph.AddFieldSetNode((FieldInfo)o, pos, sourcePort, dropInstance); }, f);
                }
            }


            //C# Events
            foreach ( var _info in type.RTGetEvents() ) {
                var info = _info;
                var categoryName = typeCategory + "/Events/";
                menu.AddItem(new GUIContent(categoryName + info.Name + " (Auto Subscribe)"), false, (o) => { graph.AddCSharpEventAutoCallbackNode((EventInfo)o, pos, sourcePort, dropInstance); }, info);
                menu.AddItem(new GUIContent(categoryName + info.Name + " (Get Reference)"), false, (o) => { graph.AddCSharpGetNode((EventInfo)o, pos, sourcePort, dropInstance); }, info);
            }

            return menu;
        }





        ///Variable based nodes
        public static UnityEditor.GenericMenu AppendVariableNodesMenu(this FlowGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance) {
            if ( !string.IsNullOrEmpty(baseCategory) ) {
                baseCategory += "/";
            }

            var variables = new Dictionary<IBlackboard, List<Variable>>();
            if ( graph.blackboard != null ) {
                variables[graph.blackboard] = graph.blackboard.variables.Values.ToList();
            }
            foreach ( var globalBB in GlobalBlackboard.allGlobals ) {
                variables[globalBB] = globalBB.variables.Values.ToList();
            }

            foreach ( var pair in variables ) {
                foreach ( var _bbVar in pair.Value ) {
                    var bb = pair.Key;
                    var bbVar = _bbVar;
                    var category = baseCategory + "Blackboard/" + ( bb == graph.blackboard ? "" : bb.name + "/" );
                    var fullName = bb == graph.blackboard ? bbVar.name : string.Format("{0}/{1}", bb.name, bbVar.name);

                    if ( sourcePort == null || ( sourcePort is ValueInput && sourcePort.type.IsAssignableFrom(bbVar.varType) ) ) {
                        var getName = string.Format("{0}Get '{1}'", category, bbVar.name);
                        menu.AddItem(new GUIContent(getName, null, "Get Variable"), false, () => { graph.AddVariableGet(bbVar.varType, fullName, pos, sourcePort, dropInstance); });
                    }
                    if ( sourcePort == null || sourcePort is FlowOutput || ( sourcePort is ValueOutput && bbVar.varType.IsAssignableFrom(sourcePort.type) ) ) {
                        var setName = string.Format("{0}Set '{1}'", category, bbVar.name);
                        menu.AddItem(new GUIContent(setName, null, "Set Variable"), false, () => { graph.AddVariableSet(bbVar.varType, fullName, pos, sourcePort, dropInstance); });
                    }
                }
            }
            return menu;
        }

        ///Macro Nodes
        public static UnityEditor.GenericMenu AppendMacroNodesMenu(this FlowGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance) {
            var projectMacroGUIDS = UnityEditor.AssetDatabase.FindAssets("t:Macro");
            foreach ( var guid in projectMacroGUIDS ) {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var macro = (Macro)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Macro));

                if ( sourcePort is ValueOutput || sourcePort is FlowOutput ) {
                    if ( !macro.inputDefinitions.Select(d => d.type).Any(d => d.IsAssignableFrom(sourcePort.type)) ) {
                        continue;
                    }
                }

                if ( sourcePort is ValueInput || sourcePort is FlowInput ) {
                    if ( !macro.outputDefinitions.Select(d => d.type).Any(d => sourcePort.type.IsAssignableFrom(d)) ) {
                        continue;
                    }
                }

                var category = baseCategory + ( !string.IsNullOrEmpty(macro.category) ? "/" + macro.category : "" );
                var name = category + "/" + macro.name;

                var content = new GUIContent(name, null, macro.comments);
                if ( macro != graph ) {
                    menu.AddItem(content, false, () => { graph.AddMacroNode(macro, pos, sourcePort, dropInstance); });
                } else {
                    menu.AddDisabledItem(content);
                }
            }

            if ( sourcePort == null ) {
                menu.AddItem(new GUIContent("MACROS/Create New...", null, "Create a new macro"), false, () =>
                {
                    var newMacro = EditorUtils.CreateAsset<Macro>();
                    if ( newMacro != null ) {
                        var wrapper = graph.AddNode<MacroNodeWrapper>(pos);
                        wrapper.macro = newMacro;
                    }
                });
            }
            return menu;
        }

        ///Nodes can post menu by themselves as well.
        public static UnityEditor.GenericMenu AppendMenuCallbackReceivers(this FlowGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance) {
            foreach ( var node in graph.allNodes.OfType<IEditorMenuCallbackReceiver>() ) {
                node.OnMenu(menu, pos, sourcePort, dropInstance);
            }
            return menu;
        }

        ///----------------------------------------------------------------------------------------------
        ///----------------------------------------------------------------------------------------------

        ///Convert nodes to macro (with a bit of hocus pocus)
        public static void ConvertNodesToMacro(List<Node> originalNodes) {

            if ( originalNodes == null || originalNodes.Count == 0 ) {
                return;
            }

            if ( !UnityEditor.EditorUtility.DisplayDialog("Convert to Macro", "This will create a new Macro out of the nodes.\nPlease note that since Macros are assets, Scene Object references will not be saved.\nThe Macro can NOT be unpacked later on.\nContinue?", "Yes", "No!") ) {
                return;
            }

            //create asset
            var newMacro = EditorUtils.CreateAsset<Macro>();
            if ( newMacro == null ) {
                return;
            }

            //undo
            var graph = (FlowScriptBase)originalNodes[0].graph;
            graph.RecordUndo("Convert To Macro");

            //clone nodes
            var cloned = Graph.CloneNodes(originalNodes, newMacro, -newMacro.translation);

            //clear initial "example" ports
            newMacro.inputDefinitions.Clear();
            newMacro.outputDefinitions.Clear();

            //cache used ports
            var inputMergeMapSource = new Dictionary<Port, Port>();
            var inputMergeMapTarget = new Dictionary<Port, Port>();

            var outputMergeMapTarget = new Dictionary<Port, Port>();
            var outputMergeMapSource = new Dictionary<Port, Port>();


            //relink copied nodes to inside macro entry/exit
            for ( var i = 0; i < originalNodes.Count; i++ ) {
                var originalNode = originalNodes[i];
                //create macro entry node port definitions and link those to input ports of cloned nodes inside
                foreach ( var originalInputConnection in originalNode.inConnections.OfType<BinderConnection>() ) {
                    //only do stuff if link source node is not part of the clones
                    if ( originalNodes.Contains(originalInputConnection.sourceNode) ) {
                        continue;
                    }
                    Port defSourcePort = null;
                    //merge same input ports and same target ports
                    if ( !inputMergeMapSource.TryGetValue(originalInputConnection.sourcePort, out defSourcePort) ) {
                        if ( !inputMergeMapTarget.TryGetValue(originalInputConnection.targetPort, out defSourcePort) ) {
                            //remark: we use sourcePort.type instead of target port type, so that connections remain assignable
                            var def = new DynamicPortDefinition(originalInputConnection.targetPort.name, originalInputConnection.sourcePort.type);
                            defSourcePort = newMacro.AddInputDefinition(def);
                            inputMergeMapTarget[originalInputConnection.targetPort] = defSourcePort;
                        }
                        inputMergeMapSource[originalInputConnection.sourcePort] = defSourcePort;
                    }

                    if ( defSourcePort.CanAcceptConnections() ) { //check this for case of merged FlowPorts
                        var targetPort = ( cloned[i] as FlowNode ).GetInputPort(originalInputConnection.targetPortID);
                        BinderConnection.Create(defSourcePort, targetPort);
                    }
                }

                //create macro exit node port definitions and link those to output ports of cloned nodes inside
                foreach ( var originalOutputConnection in originalNode.outConnections.OfType<BinderConnection>() ) {
                    //only do stuff if link target node is not part of the clones
                    if ( originalNodes.Contains(originalOutputConnection.targetNode) ) {
                        continue;
                    }
                    Port defTargetPort = null;
                    //merge same input ports and same target ports
                    if ( !outputMergeMapTarget.TryGetValue(originalOutputConnection.targetPort, out defTargetPort) ) {
                        if ( !outputMergeMapSource.TryGetValue(originalOutputConnection.sourcePort, out defTargetPort) ) {
                            var def = new DynamicPortDefinition(originalOutputConnection.sourcePort.name, originalOutputConnection.sourcePort.type);
                            defTargetPort = newMacro.AddOutputDefinition(def);
                            outputMergeMapSource[originalOutputConnection.sourcePort] = defTargetPort;
                        }
                        outputMergeMapTarget[originalOutputConnection.targetPort] = defTargetPort;
                    }

                    if ( defTargetPort.CanAcceptConnections() ) { //check this for case of merged ValuePorts
                        var sourcePort = ( cloned[i] as FlowNode ).GetOutputPort(originalOutputConnection.sourcePortID);
                        BinderConnection.Create(sourcePort, defTargetPort);
                    }
                }
            }

            //Delete originals
            var originalBounds = RectUtils.GetBoundRect(originalNodes.Select(n => n.rect).ToArray());
            foreach ( var node in originalNodes.ToArray() ) {
                graph.RemoveNode(node, false);
            }

            //Create MacroWrapper. Relink macro wrapper to outside nodes
            var wrapperPos = originalBounds.center;
            wrapperPos.x = (int)wrapperPos.x;
            wrapperPos.y = (int)wrapperPos.y;
            var wrapper = graph.AddMacroNode(newMacro, wrapperPos, null, null);
            wrapper.GatherPorts();
            foreach ( var pair in inputMergeMapSource ) {
                var source = pair.Key;
                var target = wrapper.GetInputPort(pair.Value.ID);
                BinderConnection.Create(source, target);
            }
            foreach ( var pair in outputMergeMapTarget ) {
                var source = wrapper.GetOutputPort(pair.Value.ID);
                var target = pair.Key;
                BinderConnection.Create(source, target);
            }

            //organize a bit
            var clonedBounds = RectUtils.GetBoundRect(cloned.Select(n => n.rect).ToArray());
            newMacro.entry.position = new Vector2((int)( clonedBounds.xMin - 300 ), (int)clonedBounds.yMin);
            newMacro.exit.position = new Vector2((int)( clonedBounds.xMax + 300 ), (int)clonedBounds.yMin);
            newMacro.translation = -newMacro.entry.position + new Vector2(300, 300);
            //

            //validate and save
            newMacro.Validate();
            UnityEditor.AssetDatabase.SaveAssets();
        }

    }
}

#endif