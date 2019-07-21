using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using System.Linq.Expressions;

namespace FlowCanvas
{

    ///Responsible for internal -connection level- value conversions
	[ParadoxNotion.Design.SpoofAOT]
    public static class TypeConverter
    {

        ///Returns a function that can convert from source type to target type with given func as the current value
        public static ValueHandler<T> GetConverterFuncFromTo<T>(Type sourceType, Type targetType, ValueHandler<object> func) {

            //assignables
            if ( targetType.RTIsAssignableFrom(sourceType) ) {
                return () => { return (T)func(); };
            }

            //convertibles
            if ( typeof(IConvertible).RTIsAssignableFrom(targetType) && typeof(IConvertible).RTIsAssignableFrom(sourceType) ) {
                return () => { return (T)Convert.ChangeType(func(), targetType); };
            }

            ///CUSTOM CONVENIENCE CONVERSIONS
            ///----------------------------------------------------------------------------------------------

            //from anything to string
            if ( targetType == typeof(string) ) {
                return () => { try { return (T)(object)( func().ToString() ); } catch { return default(T); } };
            }

            //from anything to Type
            if ( targetType == typeof(Type) ) {
                return () => { try { return (T)(object)func().GetType(); } catch { return default(T); } };
            }

            //from convertible to Vector3
            if ( targetType == typeof(Vector3) && typeof(IConvertible).RTIsAssignableFrom(sourceType) ) {
                return () =>
                {
                    var f = (float)Convert.ChangeType(func(), typeof(float));
                    return (T)(object)new Vector3(f, f, f);
                };
            }

            //from UnityObject to bool
            if ( targetType == typeof(bool) && typeof(UnityEngine.Object).RTIsAssignableFrom(sourceType) ) {
                return () => { return (T)(object)( func() as UnityEngine.Object != null ); };
            }

            ///----------------------------------------------------------------------------------------------

            //upcasting
            if ( targetType.RTIsSubclassOf(sourceType) ) {
                return () => { return (T)func(); };
            }

            //handles implicit/explicit and prety much everything else.
            //invoke done with reflection to support all platforms. 
            UnaryExpression exp = null;
            if ( ReflectionTools.CanConvert(sourceType, targetType, out exp) ) {
                return () => { try { return (T)exp.Method.Invoke(null, ReflectionTools.SingleTempArgsArray(func())); } catch { return default(T); } };
            }

            ///----------------------------------------------------------------------------------------------

            //from component to Vector3 (position)
            if ( targetType == typeof(Vector3) && typeof(Component).RTIsAssignableFrom(sourceType) ) {
                return () => { try { return (T)(object)( ( func() as Component ).transform.position ); } catch { return default(T); } };
            }

            //from gameobject to Vector3 (position)
            if ( targetType == typeof(Vector3) && sourceType == typeof(GameObject) ) {
                return () => { try { return (T)(object)( ( func() as GameObject ).transform.position ); } catch { return default(T); } };
            }

            //from component to Quaternion (rotation)
            if ( targetType == typeof(Quaternion) && typeof(Component).RTIsAssignableFrom(sourceType) ) {
                return () => { try { return (T)(object)( ( func() as Component ).transform.rotation ); } catch { return default(T); } };
            }

            //from gameobject to Quaternion (rotation)
            if ( targetType == typeof(Quaternion) && sourceType == typeof(GameObject) ) {
                return () => { try { return (T)(object)( ( func() as GameObject ).transform.rotation ); } catch { return default(T); } };
            }

            ///----------------------------------------------------------------------------------------------

            //from component to component
            if ( typeof(Component).RTIsAssignableFrom(targetType) && typeof(Component).RTIsAssignableFrom(sourceType) ) {
                return () => { try { return (T)(object)( ( func() as Component ).GetComponent(targetType) ); } catch { return default(T); } };
            }

            //from gameobject to component
            if ( typeof(Component).RTIsAssignableFrom(targetType) && sourceType == typeof(GameObject) ) {
                return () => { try { return (T)(object)( ( func() as GameObject ).GetComponent(targetType) ); } catch { return default(T); } };
            }

            //from component to gameobject
            if ( targetType == typeof(GameObject) && typeof(Component).RTIsAssignableFrom(sourceType) ) {
                return () => { try { return (T)(object)( ( func() as Component ).gameObject ); } catch { return default(T); } };
            }

            ///----------------------------------------------------------------------------------------------

            //From IEnumerable to IEnumerable for Lists and Arrays
            if ( typeof(IEnumerable).RTIsAssignableFrom(sourceType) && typeof(IEnumerable).RTIsAssignableFrom(targetType) ) {
                try {
                    var elementFrom = sourceType.RTIsArray() ? sourceType.GetElementType() : sourceType.GetGenericArguments().Single();
                    var elementTo = targetType.RTIsArray() ? targetType.GetElementType() : targetType.GetGenericArguments().Single();
                    if ( elementTo.RTIsAssignableFrom(elementFrom) ) {
                        var listType = typeof(List<>).RTMakeGenericType(elementTo);
                        return () =>
                        {
                            var list = (IList)System.Activator.CreateInstance(listType);
                            foreach ( var o in (IEnumerable)func() ) { list.Add(o); }
                            return (T)list;
                        };
                    }
                }
                catch { return null; }
            }

            return null;
        }

        ///Is there a convertion available from source type and to target type?
        public static bool HasConvertion(Type sourceType, Type targetType) {

            //Flow only connect to Flow
            if ( sourceType == typeof(Flow) && sourceType != targetType ) {
                return false;
            }

            return GetConverterFuncFromTo<object>(sourceType, targetType, null) != null;
        }

        ///Rarely used
        public static T QuickConvert<T>(object obj) { return (T)QuickConvert(obj, typeof(T)); }
        public static object QuickConvert(object obj, Type type) {
            if ( obj == null || type == null ) { return null; }
            var func = GetConverterFuncFromTo<object>(obj.GetType(), type, () => { return obj; });
            return func();
        }
    }
}