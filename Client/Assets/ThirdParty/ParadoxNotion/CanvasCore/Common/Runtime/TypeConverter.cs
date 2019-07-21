using UnityEngine;
using System;

namespace ParadoxNotion
{

    ///Auto "Convenience Converters" from type to type (boxing).
    ///This includes unconventional data conversions like for example GameObject to Vector3 (by transform.position).
    public static class TypeConverter
    {
        ///Custom Converter delegate
        public delegate Func<object, object> CustomConverter(Type fromType, Type toType);
        ///Subscribe custom converter
        public static event CustomConverter customConverter;

        ///Returns a function that can convert provided first arg value from type to type
        public static Func<object, object> Get(Type fromType, Type toType) {

            // Custom Converter
            if ( customConverter != null ) {
                var converter = customConverter(fromType, toType);
                if ( converter != null ) {
                    return converter;
                }
            }

            // Normal assignment.
            if ( toType.RTIsAssignableFrom(fromType) ) {
                return (value) => value;
            }

            // Anything to string
            if ( toType == typeof(string) ) {
                return (value) => value != null ? value.ToString() : "NULL";
            }

            // Convertible to convertible.
            if ( typeof(IConvertible).RTIsAssignableFrom(toType) && typeof(IConvertible).RTIsAssignableFrom(fromType) ) {
                return (value) => { try { return Convert.ChangeType(value, toType); } catch { return !toType.RTIsAbstract() ? Activator.CreateInstance(toType) : null; } };
            }

            // Unity Object to bool.
            if ( typeof(UnityEngine.Object).RTIsAssignableFrom(fromType) && toType == typeof(bool) ) {
                return (value) => value != null;
            }

            // GameObject to Component.
            if ( fromType == typeof(GameObject) && typeof(Component).RTIsAssignableFrom(toType) ) {
                return (value) => value as GameObject != null ? ( value as GameObject ).GetComponent(toType) : null;
            }

            // Component to GameObject.
            if ( typeof(Component).RTIsAssignableFrom(fromType) && toType == typeof(GameObject) ) {
                return (value) => value as Component != null ? ( value as Component ).gameObject : null;
            }

            // Component to Component.
            if ( typeof(Component).RTIsAssignableFrom(fromType) && typeof(Component).RTIsAssignableFrom(toType) ) {
                return (value) => value as Component != null ? ( value as Component ).gameObject.GetComponent(toType) : null;
            }

            // GameObject to Interface
            if ( fromType == typeof(GameObject) && toType.RTIsInterface() ) {
                return (value) => value as GameObject != null ? ( value as GameObject ).GetComponent(toType) : null;
            }

            // Component to Interface
            if ( typeof(Component).RTIsAssignableFrom(fromType) && toType.RTIsInterface() ) {
                return (value) => value as Component != null ? ( value as Component ).gameObject.GetComponent(toType) : null;
            }

            // GameObject to Vector3 (position).
            if ( fromType == typeof(GameObject) && toType == typeof(Vector3) ) {
                return (value) => { return value as GameObject != null ? ( value as GameObject ).transform.position : Vector3.zero; };
            }

            // Component to Vector3 (position).
            if ( typeof(Component).RTIsAssignableFrom(fromType) && toType == typeof(Vector3) ) {
                return (value) => { return value as Component != null ? ( value as Component ).transform.position : Vector3.zero; };
            }

            // GameObject to Quaternion (rotation).
            if ( fromType == typeof(GameObject) && toType == typeof(Quaternion) ) {
                return (value) => { return value as GameObject != null ? ( value as GameObject ).transform.rotation : Quaternion.identity; };
            }

            // Component to Quaternion (rotation).
            if ( typeof(Component).RTIsAssignableFrom(fromType) && toType == typeof(Quaternion) ) {
                return (value) => { return value as Component != null ? ( value as Component ).transform.rotation : Quaternion.identity; };
            }

            // Quaternion to Vector3 (Euler angles).
            if ( fromType == typeof(Quaternion) && toType == typeof(Vector3) ) {
                return (value) => ( (Quaternion)value ).eulerAngles;
            }

            // Vector3 (Euler angles) to Quaternion.
            if ( fromType == typeof(Vector3) && toType == typeof(Quaternion) ) {
                return (value) => Quaternion.Euler((Vector3)value);
            }

            // Vector2 to Vector3.
            if ( fromType == typeof(Vector2) && toType == typeof(Vector3) ) {
                return (value) => (Vector3)(Vector2)value;
            }

            // Vector3 to Vector2.
            if ( fromType == typeof(Vector3) && toType == typeof(Vector2) ) {
                return (value) => (Vector2)(Vector3)value;
            }

            return null;
        }

        ///Returns whether a conversion exists
        public static bool CanConvert(Type fromType, Type toType) {
            return Get(fromType, toType) != null;
        }
    }
}