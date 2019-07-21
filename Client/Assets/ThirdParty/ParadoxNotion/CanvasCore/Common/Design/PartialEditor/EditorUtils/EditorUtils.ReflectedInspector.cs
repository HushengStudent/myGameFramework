#if UNITY_EDITOR

using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;


namespace ParadoxNotion.Design
{

    ///Automatic Inspector functions
	partial class EditorUtils
    {

        ///Show an automatic editor GUI inspector for target object, taking into account drawer attributes
        public static void ReflectedObjectInspector(object target) {

            if ( target == null ) {
                return;
            }

            var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for ( var i = 0; i < fields.Length; i++ ) {
                var field = fields[i];

                //hide type altogether?
                if ( field.FieldType.IsDefined(typeof(HideInInspector), true) ) {
                    continue;
                }

                //inspect only public fields or private fields with the [ExposeField] attribute
                if ( field.IsPublic || field.IsDefined(typeof(ExposeFieldAttribute), true) ) {
                    var attributes = field.GetCustomAttributes(true);
                    //Hide field?
                    if ( attributes.Any(a => a is HideInInspector) ) {
                        continue;
                    }
                    field.SetValue(target, ReflectedFieldInspector(field.Name, field.GetValue(target), field.FieldType, field, target, attributes));
                }
            }
        }


        ///Show an arbitrary field type editor. Passing a FieldInfo will also check for attributes.
        public static object ReflectedFieldInspector(string name, object value, Type t, FieldInfo field = null, object context = null, object[] attributes = null) {
            var content = new GUIContent(name.SplitCamelCase());
            if ( attributes != null ) {
                ///Create proper GUIContent
                var nameAtt = attributes.FirstOrDefault(a => a is NameAttribute) as NameAttribute;
                if ( nameAtt != null ) { content.text = nameAtt.name; }

                var tooltipAtt = attributes.FirstOrDefault(a => a is TooltipAttribute) as TooltipAttribute;
                if ( tooltipAtt != null ) { content.tooltip = tooltipAtt.tooltip; }
            }

            return ReflectedFieldInspector(content, value, t, field, context, attributes);
        }

        ///Show an arbitrary field type editor. Passing a FieldInfo will also check for attributes.
        public static object ReflectedFieldInspector(GUIContent content, object value, Type t, FieldInfo field = null, object context = null, object[] attributes = null) {

            if ( t == null ) {
                GUILayout.Label("NO TYPE PROVIDED!");
                return value;
            }

            ///Use drawers
            var drawerAttributes = attributes != null ? attributes.OfType<DrawerAttribute>().OrderBy(a => a.priority).ToArray() : null;
            var objectDrawer = PropertyDrawerFactory.GetObjectDrawer(t);
            return objectDrawer.DrawGUI(content, value, field, context, drawerAttributes);
        }


        ///Draws an Editor field for object of type directly
        public static object DrawEditorFieldDirect(GUIContent content, object value, Type t, FieldInfo field = null, object context = null, object[] attributes = null) {

            if ( typeof(UnityObject).IsAssignableFrom(t) == false && t != typeof(Type) ) {
                //Check abstract
                if ( ( value != null && value.GetType().IsAbstract ) || ( value == null && t.IsAbstract ) ) {
                    EditorGUILayout.LabelField(content, new GUIContent(string.Format("Abstract ({0})", t.FriendlyName())));
                    return value;
                }

                //Auto create instance for some types
                if ( value == null && t != typeof(object) && !t.IsAbstract && !t.IsInterface ) {
                    if ( t.GetConstructor(Type.EmptyTypes) != null || t.IsArray ) {
                        if ( t.IsArray ) { value = Array.CreateInstance(t.GetElementType(), 0); } else { value = Activator.CreateInstance(t); }
                    }
                }
            }

            //Check the type
            if ( typeof(UnityObject).IsAssignableFrom(t) ) {
                if ( t == typeof(Component) && ( value is Component ) ) {
                    return ComponentField(content, (Component)value, typeof(Component));
                }
                return EditorGUILayout.ObjectField(content, (UnityObject)value, t, true);
            }

            if ( t == typeof(Type) ) {
                return Popup<Type>(content, (Type)value, TypePrefs.GetPreferedTypesList(true));
            }

            if ( t == typeof(string) ) {
                return EditorGUILayout.TextField(content, (string)value);
            }

            if ( t == typeof(char) ) {
                var c = (char)value;
                var s = c.ToString();
                s = EditorGUILayout.TextField(content, s);
                return string.IsNullOrEmpty(s) ? (char)c : (char)s[0];
            }

            if ( t == typeof(bool) ) {
                return EditorGUILayout.Toggle(content, (bool)value);
            }

            if ( t == typeof(int) ) {
                return EditorGUILayout.IntField(content, (int)value);
            }

            if ( t == typeof(float) ) {
                return EditorGUILayout.FloatField(content, (float)value);
            }

            if ( t == typeof(byte) ) {
                return Convert.ToByte(Mathf.Clamp(EditorGUILayout.IntField(content, (byte)value), 0, 255));
            }

            if ( t == typeof(long) ) {
                return EditorGUILayout.LongField(content, (long)value);
            }

            if ( t == typeof(double) ) {
                return EditorGUILayout.DoubleField(content, (double)value);
            }

            if ( t == typeof(Vector2) ) {
                return EditorGUILayout.Vector2Field(content, (Vector2)value);
            }

            if ( t == typeof(Vector3) ) {
                return EditorGUILayout.Vector3Field(content, (Vector3)value);
            }

            if ( t == typeof(Vector4) ) {
                return EditorGUILayout.Vector4Field(content, (Vector4)value);
            }

            if ( t == typeof(Quaternion) ) {
                var quat = (Quaternion)value;
                var vec4 = new Vector4(quat.x, quat.y, quat.z, quat.w);
                vec4 = EditorGUILayout.Vector4Field(content, vec4);
                return new Quaternion(vec4.x, vec4.y, vec4.z, vec4.w);
            }

            if ( t == typeof(Color) ) {
                return EditorGUILayout.ColorField(content, (Color)value);
            }

            if ( t == typeof(Rect) ) {
                return EditorGUILayout.RectField(content, (Rect)value);
            }

            if ( t == typeof(AnimationCurve) ) {
                return EditorGUILayout.CurveField(content, (AnimationCurve)value);
            }

            if ( t == typeof(Bounds) ) {
                return EditorGUILayout.BoundsField(content, (Bounds)value);
            }

            if ( t == typeof(LayerMask) ) {
                return LayerMaskField(content, (LayerMask)value);
            }

            if ( t.IsSubclassOf(typeof(System.Enum)) ) {
                if ( t.IsDefined(typeof(FlagsAttribute), true) ) {
#if UNITY_2017_3_OR_NEWER
                    return EditorGUILayout.EnumFlagsField(content, (System.Enum)value);
#else
					return EditorGUILayout.EnumMaskPopup(content, (System.Enum)value);
#endif
                }
                return EditorGUILayout.EnumPopup(content, (System.Enum)value);
            }

            if ( typeof(IList).IsAssignableFrom(t) ) {
                return ListEditor(content, (IList)value, t, field, context, attributes);
            }

            if ( typeof(IDictionary).IsAssignableFrom(t) ) {
                return DictionaryEditor(content, (IDictionary)value, t, field, context, attributes);
            }

            //show nested class members recursively
            if ( value != null && ( t.IsClass || t.IsValueType ) ) {

                if ( EditorGUI.indentLevel <= 8 ) {
                    GUILayout.BeginVertical();
                    EditorGUILayout.LabelField(content, new GUIContent(string.Format("({0})", t.FriendlyName())));
                    EditorGUI.indentLevel++;
                    ReflectedObjectInspector(value);
                    EditorGUI.indentLevel--;
                    GUILayout.EndVertical();
                }

            } else {

                EditorGUILayout.LabelField(content, new GUIContent(string.Format("NonInspectable ({0})", t.FriendlyName())));
            }

            return value;
        }
    }
}

#endif
