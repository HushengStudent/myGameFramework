using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ParadoxNotion.Serialization.FullSerializer.Internal;

namespace ParadoxNotion.Serialization.FullSerializer
{

    /// MetaType contains metadata about a type. This is used by the reflection serializer.
    public class fsMetaType
    {

        private static Dictionary<fsConfig, Dictionary<Type, fsMetaType>> _configMetaTypes = new Dictionary<fsConfig, Dictionary<Type, fsMetaType>>();

        /// Return MetaType
        public static fsMetaType Get(fsConfig config, Type type) {
            Dictionary<Type, fsMetaType> metaTypes;
            if ( _configMetaTypes.TryGetValue(config, out metaTypes) == false ) {
                metaTypes = _configMetaTypes[config] = new Dictionary<Type, fsMetaType>();
            }

            fsMetaType metaType;
            if ( metaTypes.TryGetValue(type, out metaType) == false ) {
                metaType = new fsMetaType(config, type);
                metaTypes[type] = metaType;
            }

            return metaType;
        }

        /// Clears out the cached type results. Useful if some prior assumptions become invalid, ie, the default member
        /// serialization mode.
        public static void ClearCache() {
            _configMetaTypes = new Dictionary<fsConfig, Dictionary<Type, fsMetaType>>();
        }

        ///----------------------------------------------------------------------------------------------

        private delegate object ObjectGenerator();

        private ObjectGenerator generator;
        public Type reflectedType { get; private set; }
        public fsMetaProperty[] Properties { get; private set; }


        ///----------------------------------------------------------------------------------------------

        //...
        private fsMetaType(fsConfig config, Type reflectedType) {
            this.reflectedType = reflectedType;
            this.generator = GetGenerator(reflectedType);

            var properties = new List<fsMetaProperty>();
            CollectProperties(config, properties, reflectedType);
            this.Properties = properties.ToArray();
        }

        //...
        static void CollectProperties(fsConfig config, List<fsMetaProperty> properties, Type reflectedType) {
            FieldInfo[] fields = reflectedType.GetFields(ReflectionTools.FLAGS_ALL_DECLARED);
            for ( var i = 0; i < fields.Length; i++ ) {
                var field = fields[i];

                // We don't serialize members annotated with any of the ignore serialize attributes
                if ( config.IgnoreSerializeAttributes.Any(t => field.RTIsDefined(t, true)) ) {
                    continue;
                }

                if ( CanSerializeField(config, field) ) {
                    properties.Add(new fsMetaProperty(config, field));
                }
            }

            if ( reflectedType.BaseType != null ) {
                CollectProperties(config, properties, reflectedType.BaseType);
            }
        }

        //...
        static bool CanSerializeField(fsConfig config, FieldInfo field) {

            // We don't serialize delegates
            if ( typeof(Delegate).IsAssignableFrom(field.FieldType) ) {
                return false;
            }

            // We don't serialize static fields
            if ( field.IsStatic ) {
                return false;
            }

            // We don't serialize compiler generated fields.
            if ( field.RTIsDefined(typeof(CompilerGeneratedAttribute), false) ) {
                return false;
            }

            return field.IsPublic || config.SerializeAttributes.Any(t => field.RTIsDefined(t, true));
        }

        /// Create generator
        static ObjectGenerator GetGenerator(Type reflectedType) {

            if ( reflectedType.IsInterface || reflectedType.IsAbstract ) {
                return () => { throw new Exception("Cannot create an instance of an interface or abstract type for " + reflectedType); };
            }

            if ( typeof(UnityEngine.ScriptableObject).IsAssignableFrom(reflectedType) ) {
                return () => { return UnityEngine.ScriptableObject.CreateInstance(reflectedType); };
            }

            if ( reflectedType.IsArray ) {
                // we have to start with a size zero array otherwise it will have invalid data inside of it
                return () => { return Array.CreateInstance(reflectedType.GetElementType(), 0); };
            }

            if ( reflectedType == typeof(string) ) {
                return () => { return string.Empty; };
            }

            if ( !HasDefaultConstructor(reflectedType) ) {
                return () => { return System.Runtime.Serialization.FormatterServices.GetSafeUninitializedObject(reflectedType); };
            }

            return () => { return Activator.CreateInstance(reflectedType, /*nonPublic:*/ true); };
        }

        //...
        static bool HasDefaultConstructor(Type reflectedType) {
            // arrays are considered to have a default constructor
            if ( reflectedType.IsArray ) {
                return true;
            }

            // value types (ie, structs) always have a default constructor
            if ( reflectedType.IsValueType ) {
                return true;
            }

            // consider private constructors as well
            return reflectedType.RTGetDefaultConstructor() != null;
        }

        ///----------------------------------------------------------------------------------------------


        /// Creates a new instance of the type that this metadata points back to. If this type has a
        /// default constructor, then Activator.CreateInstance will be used to construct the type
        /// (or Array.CreateInstance if it an array). Otherwise, an uninitialized object created via
        /// FormatterServices.GetSafeUninitializedObject is used to construct the instance.
        public object CreateInstance() {

            if ( generator != null ) {

                try { return generator(); }

                catch ( MissingMethodException e ) {
                    throw new InvalidOperationException("Unable to create instance of " + reflectedType + "; there is no default constructor", e);
                }

                catch ( TargetInvocationException e ) {
                    throw new InvalidOperationException("Constructor of " + reflectedType + " threw an exception when creating an instance", e);
                }

                catch ( MemberAccessException e ) {
                    throw new InvalidOperationException("Unable to access constructor of " + reflectedType, e);
                }
            }

            throw new Exception("Cant create instance generator for " + reflectedType);
        }

    }
}