#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;


namespace NodeCanvas.Editor
{

    /// Editor for IBlackboards
    public static class BlackboardEditor
    {

        private static GUILayoutOption[] layoutOptions = new GUILayoutOption[] { GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true), GUILayout.Height(16) };
        private static Dictionary<IBlackboard, List<Variable>> tempLists = new Dictionary<IBlackboard, List<Variable>>();

        public static Variable pickedVariable { get; private set; }
        public static IBlackboard pickedVariableBlackboard { get; private set; }

        ///Show the variables of blackboard serialized in contextParent
        public static void ShowVariables(IBlackboard bb, UnityEngine.Object contextParent) {

            GUI.skin.label.richText = true;
            var e = Event.current;

            //Begin undo check
            UndoManager.CheckUndo(contextParent, "Blackboard Inspector");

            //Add variable button
            GUI.backgroundColor = Colors.lightBlue;
            if ( GUILayout.Button("Add Variable") ) {
                GetAddVariableMenu(bb, contextParent).ShowAsBrowser("Add Variable");
                Event.current.Use();
            }
            GUI.backgroundColor = Color.white;

            //Simple column header info
            if ( bb.variables.Keys.Count != 0 ) {
                GUILayout.BeginHorizontal();
                GUI.color = Color.yellow;
                GUILayout.Label("Name", layoutOptions);
                GUILayout.Label("Value", layoutOptions);
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            } else {
                EditorGUILayout.HelpBox("Blackboard has no variables", MessageType.Info);
            }


            List<Variable> tempList = null;
            if ( !tempLists.TryGetValue(bb, out tempList) ) {
                tempList = bb.variables.Values.ToList();
                tempLists[bb] = tempList;
            }

            if ( !tempList.SequenceEqual(bb.variables.Values) ) {
                tempList = bb.variables.Values.ToList();
                tempLists[bb] = tempList;
            }

            // //store the names of the variables being used by current graph selection.
            string[] usedVariableNames = null;
            if ( GraphEditorUtility.activeElement != null ) {
                usedVariableNames = Graph.GetParametersInElement(GraphEditorUtility.activeElement).Where(p => p != null && p.isDefined).Select(p => p.name).ToArray();
            }

            //The actual variables reorderable list
            var options = new EditorUtils.ReorderableListOptions();
            options.CustomItemMenu = (i) => { return GetVariableMenu(tempList[i], bb); };
            EditorUtils.ReorderableList(tempList, options, (i, picked) =>
            {
                var data = (Variable)tempList[i];
                if ( data == null ) {
                    GUILayout.Label("NULL Variable!");
                    return;
                }

                var isUsed = usedVariableNames != null ? usedVariableNames.Contains(data.name) : false;
                var missingVariableType = data as MissingVariableType;

                GUILayout.Space(data.varType == typeof(VariableSeperator) ? 5 : 0);
                GUILayout.BeginHorizontal();

                if ( missingVariableType == null ) {
                    //Name of the variable GUI control
                    if ( !Application.isPlaying ) {
                        if ( picked && data.varType != typeof(VariableSeperator) ) {
                            pickedVariable = data;
                            pickedVariableBlackboard = bb;
                        }

                        GUI.color = Color.white;
                        GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f, 0.3f);

                        //Make name field red if same name exists
                        if ( tempList.Where(v => v != data).Select(v => v.name).Contains(data.name) ) {
                            GUI.backgroundColor = Color.red;
                        }

                        GUI.enabled = !data.isProtected;
                        if ( data.varType != typeof(VariableSeperator) ) {
                            data.name = EditorGUILayout.DelayedTextField(data.name, layoutOptions);
                            EditorGUI.indentLevel = 0;

                        } else {

                            var separator = (VariableSeperator)data.value;
                            if ( separator == null ) {
                                separator = new VariableSeperator();
                            }

                            if ( separator.isEditingName ) {
                                data.name = EditorGUILayout.DelayedTextField(data.name, layoutOptions);
                            } else {
                                GUI.color = Color.yellow;
                                GUILayout.Label(string.Format("<b>{0}</b>", data.name).ToUpper(), layoutOptions);
                                GUI.color = Color.white;
                            }

                            if ( !separator.isEditingName ) {
                                if ( e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 2 && GUILayoutUtility.GetLastRect().Contains(e.mousePosition) ) {
                                    separator.isEditingName = true;
                                    GUIUtility.keyboardControl = 0;
                                }
                            }

                            if ( separator.isEditingName ) {
                                if ( ( e.isKey && e.keyCode == KeyCode.Return ) || ( e.rawType == EventType.MouseUp && !GUILayoutUtility.GetLastRect().Contains(e.mousePosition) ) ) {
                                    separator.isEditingName = false;
                                    GUIUtility.keyboardControl = 0;
                                }
                            }

                            data.value = separator;
                        }

                        GUI.enabled = true;
                        GUI.backgroundColor = Color.white;

                    } else {

                        //Don't allow name edits in play mode. Instead show just a label
                        if ( data.varType != typeof(VariableSeperator) ) {
                            GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f);
                            GUI.color = new Color(0.8f, 0.8f, 1f);
                            GUILayout.Label(data.name, layoutOptions);
                        } else {
                            GUI.color = Color.yellow;
                            GUILayout.Label(string.Format("<b>{0}</b>", data.name.ToUpper()), layoutOptions);
                            GUI.color = Color.white;
                        }
                    }


                    //reset coloring
                    GUI.color = Color.white;
                    GUI.backgroundColor = Color.white;

                    //Highlight used variable by selection?
                    if ( isUsed ) {
                        var highRect = GUILayoutUtility.GetLastRect();
                        highRect.xMin += 2;
                        highRect.xMax -= 2;
                        highRect.yMax -= 4;
                        GUI.Box(highRect, string.Empty, Styles.highlightBox);
                    }

                    //Show the respective data GUI
                    ShowDataGUI(data, bb, contextParent, layoutOptions);

                } else {

                    var internalTypeName = missingVariableType.missingType;
                    internalTypeName = internalTypeName.Substring(internalTypeName.IndexOf("`1") + 2);
                    GUILayout.Label(data.name, layoutOptions);
                    GUILayout.Label(string.Format("<color=#ff6457>* {0} *</color>", internalTypeName), layoutOptions);
                }

                //closure
                GUI.color = Color.white;
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();

            }, contextParent);

            //reset coloring
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;

            if ( GUI.changed || e.rawType == EventType.MouseUp ) {
                EditorApplication.delayCall += () => { ResetPick(); };
                //reconstruct the dictionary
                try { bb.variables = tempList.ToDictionary(d => d.name, d => d); }
                catch { Debug.LogError("Blackboard has duplicate names!"); }
            }

            //Check dirty
            UndoManager.CheckDirty(contextParent);
        }


        ///reset pick info
        public static void ResetPick() {
            pickedVariable = null;
            pickedVariableBlackboard = null;
        }


        //show variable data
        static void ShowDataGUI(Variable data, IBlackboard bb, UnityEngine.Object contextParent, GUILayoutOption[] layoutOptions) {
            //Bind info or value GUI control
            if ( data.hasBinding ) {
                var idx = data.propertyPath.LastIndexOf('.');
                var typeName = data.propertyPath.Substring(0, idx);
                var memberName = data.propertyPath.Substring(idx + 1);
                GUI.color = new Color(0.8f, 0.8f, 1);
                GUILayout.Label(string.Format(".{0} ({1})", memberName, typeName.Split('.').Last()), layoutOptions);
                GUI.color = Color.white;
            } else {
                GUI.enabled = !data.isProtected;
                data.value = VariableField(data, contextParent, layoutOptions);
                GUI.enabled = true;
                GUI.backgroundColor = Color.white;
            }
        }

        ///Return get add variable menu
        static GenericMenu GetAddVariableMenu(IBlackboard bb, UnityEngine.Object contextParent) {
            System.Action<System.Type> AddNewVariable = (t) =>
            {
                Undo.RecordObject(contextParent, "Variable Added");
                var name = "my" + t.FriendlyName();
                while ( bb.GetVariable(name) != null ) {
                    name += ".";
                }
                bb.AddVariable(name, t);
            };

            System.Action<PropertyInfo> AddBoundProp = (p) =>
            {
                Undo.RecordObject(contextParent, "Variable Added");
                var newVar = bb.AddVariable(p.Name, p.PropertyType);
                newVar.BindProperty(p);
            };

            System.Action<FieldInfo> AddBoundField = (f) =>
            {
                Undo.RecordObject(contextParent, "Variable Added");
                var newVar = bb.AddVariable(f.Name, f.FieldType);
                newVar.BindProperty(f);
            };

            var menu = new GenericMenu();
            menu = EditorUtils.GetPreferedTypesSelectionMenu(typeof(object), AddNewVariable, menu, "New", true);

            if ( bb.propertiesBindTarget != null ) {
                foreach ( var comp in bb.propertiesBindTarget.GetComponents(typeof(Component)).Where(c => c.hideFlags == 0) ) {
                    menu = EditorUtils.GetInstanceFieldSelectionMenu(comp.GetType(), typeof(object), AddBoundField, menu, "Bound (Self)");
                    menu = EditorUtils.GetInstancePropertySelectionMenu(comp.GetType(), typeof(object), AddBoundProp, false, false, menu, "Bound (Self)");
                }
            }

            foreach ( var type in TypePrefs.GetPreferedTypesList(typeof(object)) ) {
                menu = EditorUtils.GetStaticFieldSelectionMenu(type, typeof(object), AddBoundField, menu, "Bound (Static)");
                menu = EditorUtils.GetStaticPropertySelectionMenu(type, typeof(object), AddBoundProp, false, false, menu, "Bound (Static)");
            }


            menu.AddSeparator("/");
            menu.AddItem(new GUIContent("Add Header Separator"), false, () => { bb.AddVariable("Separator (Double Click To Rename)", new VariableSeperator()); });
            return menu;
        }

        ///Get a generic menu per variable
        static GenericMenu GetVariableMenu(Variable data, IBlackboard bb) {
            var menu = new GenericMenu();
            if ( data.varType == typeof(VariableSeperator) ) {
                menu.AddItem(new GUIContent("Rename"), false, () => { ( data.value as VariableSeperator ).isEditingName = true; });
                menu.AddItem(new GUIContent("Remove"), false, () => { bb.RemoveVariable(data.name); });
                return menu;
            }

            System.Action<PropertyInfo> SelectProp = (p) => { data.BindProperty(p); };
            System.Action<FieldInfo> SelectField = (f) => { data.BindProperty(f); };

            menu.AddDisabledItem(new GUIContent(string.Format("Type: {0}", data.varType.FriendlyName())));

            if ( bb.propertiesBindTarget != null ) {
                foreach ( var comp in bb.propertiesBindTarget.GetComponents(typeof(Component)).Where(c => c.hideFlags != HideFlags.HideInInspector) ) {
                    menu = EditorUtils.GetInstanceFieldSelectionMenu(comp.GetType(), data.varType, SelectField, menu, "Bind (Self)");
                    menu = EditorUtils.GetInstancePropertySelectionMenu(comp.GetType(), data.varType, SelectProp, false, false, menu, "Bind (Self)");
                }
            }

            foreach ( var type in TypePrefs.GetPreferedTypesList() ) {
                menu = EditorUtils.GetStaticFieldSelectionMenu(type, data.varType, SelectField, menu, "Bind (Static)");
                menu = EditorUtils.GetStaticPropertySelectionMenu(type, data.varType, SelectProp, false, false, menu, "Bind (Static)");
            }

            menu.AddItem(new GUIContent("Duplicate"), false, () =>
            {
                var dup = bb.AddVariable(data.name + '.', data.varType);
                dup.value = data.value;
            });

            menu.AddItem(new GUIContent("Protected"), data.isProtected, () => { data.isProtected = !data.isProtected; });

            menu.AddSeparator("/");
            if ( data.hasBinding ) {
                menu.AddItem(new GUIContent("UnBind"), false, () => { data.UnBindProperty(); });
            } else {
                menu.AddDisabledItem(new GUIContent("UnBind"));
            }

            menu.AddItem(new GUIContent("Delete Variable"), false, () =>
            {
                if ( EditorUtility.DisplayDialog("Delete Variable '" + data.name + "'", "Are you sure?", "Yes", "No") ) {
                    bb.RemoveVariable(data.name);
                    GUIUtility.hotControl = 0;
                    GUIUtility.keyboardControl = 0;
                }
            });
            return menu;
        }

        //While there is a similar method in EditorUtils, due to layouting and especialy no prefix name, this has to be redone a bit differently
        static object VariableField(Variable data, UnityEngine.Object context, GUILayoutOption[] layoutOptions) {

            var o = data.value;
            var t = data.varType;

            if ( t == typeof(VariableSeperator) ) {
                GUILayout.Space(0);
                return o;
            }

            //Check scene object type for UnityObjects. Consider Interfaces as scene object type. Assume that user uses interfaces with UnityObjects
            if ( typeof(UnityEngine.Object).IsAssignableFrom(t) || t.IsInterface ) {
                var isSceneObjectType = ( typeof(Component).IsAssignableFrom(t) || t == typeof(GameObject) || t.IsInterface );
                return EditorGUILayout.ObjectField((UnityEngine.Object)o, t, isSceneObjectType, layoutOptions);
            }

            //Check Type second
            if ( t == typeof(System.Type) ) {
                return EditorUtils.Popup<System.Type>(string.Empty, (System.Type)o, TypePrefs.GetPreferedTypesList(true), layoutOptions);
            }

            t = o != null ? o.GetType() : t;
            if ( t.IsAbstract ) {
                GUILayout.Label(string.Format("({0})", t.FriendlyName()), layoutOptions);
                return o;
            }

            if ( o == null && !t.IsAbstract && !t.IsInterface && ( t.GetConstructor(System.Type.EmptyTypes) != null || t.IsArray ) ) {
                if ( GUILayout.Button("(null) Create", layoutOptions) ) {
                    if ( t.IsArray ) {
                        return System.Array.CreateInstance(t.GetElementType(), 0);
                    }
                    return System.Activator.CreateInstance(t);
                }
                return o;
            }

            if ( t == typeof(bool) ) {
                return EditorGUILayout.Toggle((bool)o, layoutOptions);
            }

            if ( t == typeof(Color) ) {
                return EditorGUILayout.ColorField((Color)o, layoutOptions);
            }

            if ( t == typeof(AnimationCurve) ) {
                return EditorGUILayout.CurveField((AnimationCurve)o, layoutOptions);
            }

            if ( t.IsSubclassOf(typeof(System.Enum)) ) {
                if ( t.IsDefined(typeof(System.FlagsAttribute), true) ) {
#if UNITY_2017_3_OR_NEWER
                    return EditorGUILayout.EnumFlagsField(GUIContent.none, (System.Enum)o, layoutOptions);
#else
					return EditorGUILayout.EnumMaskPopup(GUIContent.none, (System.Enum)o, layoutOptions);
#endif
                }
                return EditorGUILayout.EnumPopup((System.Enum)o, layoutOptions);
            }

            if ( t == typeof(float) ) {
                GUI.backgroundColor = TypePrefs.GetTypeColor(t);
                return EditorGUILayout.FloatField((float)o, layoutOptions);
            }

            if ( t == typeof(int) ) {
                GUI.backgroundColor = TypePrefs.GetTypeColor(t);
                return EditorGUILayout.IntField((int)o, layoutOptions);
            }

            if ( t == typeof(string) ) {
                GUI.backgroundColor = TypePrefs.GetTypeColor(t);
                return EditorGUILayout.TextField((string)o, layoutOptions);
            }

            if ( t == typeof(long) ) {
                GUI.backgroundColor = TypePrefs.GetTypeColor(t);
                return EditorGUILayout.LongField((long)o, layoutOptions);
            }

            if ( t == typeof(double) ) {
                GUI.backgroundColor = TypePrefs.GetTypeColor(t);
                return EditorGUILayout.DoubleField((double)o, layoutOptions);
            }

            if ( t == typeof(Vector2) ) {
                return EditorGUILayout.Vector2Field(string.Empty, (Vector2)o, layoutOptions);
            }

            if ( t == typeof(Vector3) ) {
                return EditorGUILayout.Vector3Field(string.Empty, (Vector3)o, layoutOptions);
            }

            if ( t == typeof(Vector4) ) {
                return EditorGUILayout.Vector4Field(string.Empty, (Vector4)o, layoutOptions);
            }

            if ( t == typeof(Quaternion) ) {
                var q = (Quaternion)o;
                var v = new Vector4(q.x, q.y, q.z, q.w);
                v = EditorGUILayout.Vector4Field(string.Empty, v, layoutOptions);
                return new Quaternion(v.x, v.y, v.z, v.w);
            }

            if ( t == typeof(LayerMask) ) {
                return EditorUtils.LayerMaskField(string.Empty, (LayerMask)o, layoutOptions);
            }

            //If some other type, show it in the generic object editor window
            if ( GUILayout.Button(string.Format("{0} {1}", t.FriendlyName(), ( o is IList ) ? ( (IList)o ).Count.ToString() : string.Empty), layoutOptions) ) {
                GenericInspectorWindow.Show(data.ID, o, t, context);
            }

            //if we are externaly inspecting value and it's this one, get value from the external editor. This is basicaly done for structs
            if ( GenericInspectorWindow.current != null && GenericInspectorWindow.current.inspectedID == data.ID ) {
                return GenericInspectorWindow.current.value;
            }

            return o;
        }
    }
}

#endif