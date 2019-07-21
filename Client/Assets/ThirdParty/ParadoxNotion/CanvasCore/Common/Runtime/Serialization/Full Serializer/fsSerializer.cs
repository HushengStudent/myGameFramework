using System;
using System.Collections.Generic;
using ParadoxNotion.Serialization.FullSerializer.Internal;
using ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters;

namespace ParadoxNotion.Serialization.FullSerializer
{

    public class fsSerializer
    {

        public const string KEY_OBJECT_REFERENCE = "$ref";
        public const string KEY_OBJECT_DEFINITION = "$id";
        public const string KEY_INSTANCE_TYPE = "$type";
        public const string KEY_VERSION = "$version";
        public const string KEY_CONTENT = "$content";

        /// Returns true if the given key is a special keyword that full serializer uses to
        /// add additional metadata on top of the emitted JSON.
        public static bool IsReservedKeyword(string key) {
            switch ( key ) {
                case ( KEY_OBJECT_REFERENCE ): return true;
                case ( KEY_OBJECT_DEFINITION ): return true;
                case ( KEY_INSTANCE_TYPE ): return true;
                case ( KEY_VERSION ): return true;
                case ( KEY_CONTENT ): return true;
            }
            return false;
        }

        private static bool IsObjectReference(fsData data) {
            if ( data.IsDictionary == false ) return false;
            return data.AsDictionary.ContainsKey(KEY_OBJECT_REFERENCE);
        }
        private static bool IsObjectDefinition(fsData data) {
            if ( data.IsDictionary == false ) return false;
            return data.AsDictionary.ContainsKey(KEY_OBJECT_DEFINITION);
        }
        private static bool IsVersioned(fsData data) {
            if ( data.IsDictionary == false ) return false;
            return data.AsDictionary.ContainsKey(KEY_VERSION);
        }
        private static bool IsTypeSpecified(fsData data) {
            if ( data.IsDictionary == false ) return false;
            return data.AsDictionary.ContainsKey(KEY_INSTANCE_TYPE);
        }
        private static bool IsWrappedData(fsData data) {
            if ( data.IsDictionary == false ) return false;
            return data.AsDictionary.ContainsKey(KEY_CONTENT);
        }

        ///----------------------------------------------------------------------------------------------

        private static void Invoke_OnBeforeSerialize(List<fsObjectProcessor> processors, Type storageType, object instance) {
            for ( int i = 0; i < processors.Count; ++i ) {
                processors[i].OnBeforeSerialize(storageType, instance);
            }

            //!!Call only on non-Unity objects, since they are called back anyways by Unity!!
            if ( instance is UnityEngine.ISerializationCallbackReceiver && !( instance is UnityEngine.Object ) ) {
                ( (UnityEngine.ISerializationCallbackReceiver)instance ).OnBeforeSerialize();
            }
        }
        private static void Invoke_OnAfterSerialize(List<fsObjectProcessor> processors, Type storageType, object instance, ref fsData data) {
            // We run the after calls in reverse order; this significantly reduces the interaction burden between
            // multiple processors - it makes each one much more independent and ignorant of the other ones.
            for ( int i = processors.Count - 1; i >= 0; --i ) {
                processors[i].OnAfterSerialize(storageType, instance, ref data);
            }
        }
        private static void Invoke_OnBeforeDeserialize(List<fsObjectProcessor> processors, Type storageType, ref fsData data) {
            for ( int i = 0; i < processors.Count; ++i ) {
                processors[i].OnBeforeDeserialize(storageType, ref data);
            }
        }
        private static void Invoke_OnBeforeDeserializeAfterInstanceCreation(List<fsObjectProcessor> processors, Type storageType, object instance, ref fsData data) {
            for ( int i = 0; i < processors.Count; ++i ) {
                processors[i].OnBeforeDeserializeAfterInstanceCreation(storageType, instance, ref data);
            }
        }
        private static void Invoke_OnAfterDeserialize(List<fsObjectProcessor> processors, Type storageType, object instance) {
            for ( int i = processors.Count - 1; i >= 0; --i ) {
                processors[i].OnAfterDeserialize(storageType, instance);
            }

            //!!Call only on non-Unity objects, since they are called back anyways by Unity!!
            if ( instance is UnityEngine.ISerializationCallbackReceiver && !( instance is UnityEngine.Object ) ) {
                ( (UnityEngine.ISerializationCallbackReceiver)instance ).OnAfterDeserialize();
            }
        }

        /// Ensures that the data is a dictionary. If it is not, then it is wrapped inside of one.
        private static void EnsureDictionary(fsData data) {
            if ( data.IsDictionary == false ) {
                var existingData = data.Clone();
                data.BecomeDictionary();
                data.AsDictionary[KEY_CONTENT] = existingData;
            }
        }

        /// This manages instance writing so that we do not write unnecessary $id fields. We
        /// only need to write out an $id field when there is a corresponding $ref field. This is able
        /// to write $id references lazily because the fsData instance is not actually written out to text
        /// until we have entirely finished serializing it.
        internal class fsLazyCycleDefinitionWriter
        {
            private Dictionary<int, fsData> _pendingDefinitions = new Dictionary<int, fsData>();
            private HashSet<int> _references = new HashSet<int>();

            public void WriteDefinition(int id, fsData data) {
                if ( _references.Contains(id) ) {
                    EnsureDictionary(data);
                    data.AsDictionary[KEY_OBJECT_DEFINITION] = new fsData(id.ToString());
                } else {
                    _pendingDefinitions[id] = data;
                }
            }

            public void WriteReference(int id, Dictionary<string, fsData> dict) {
                // Write the actual definition if necessary
                if ( _pendingDefinitions.ContainsKey(id) ) {
                    var data = _pendingDefinitions[id];
                    EnsureDictionary(data);
                    data.AsDictionary[KEY_OBJECT_DEFINITION] = new fsData(id.ToString());
                    _pendingDefinitions.Remove(id);
                } else {
                    _references.Add(id);
                }

                // Write the reference
                dict[KEY_OBJECT_REFERENCE] = new fsData(id.ToString());
            }

            public void Clear() {
                _pendingDefinitions.Clear();
                _references.Clear();
            }
        }

        /// Converter type to converter instance lookup table. This could likely be stored inside
        // of _cachedConverters, but there is a semantic difference because _cachedConverters goes
        /// from serialized type to converter.
        private Dictionary<Type, fsBaseConverter> _cachedOverrideConverterInstances;
        /// A cache from type to it's converter.
        private Dictionary<Type, fsBaseConverter> _cachedConverters;
        /// Converters that can be used for type registration.
        private readonly List<fsConverter> _availableConverters;
        /// Direct converters (optimized _converters). We use these so we don't have to
        /// perform a scan through every item in _converters and can instead just do an O(1)
        /// lookup. This is potentially important to perf when there are a ton of direct
        /// converters.
        private readonly Dictionary<Type, fsDirectConverter> _availableDirectConverters;

        /// Processors that are available.
        private readonly List<fsObjectProcessor> _processors;
        /// A cache from type to the set of processors that are interested in it.
        private Dictionary<Type, List<fsObjectProcessor>> _cachedProcessors;

        /// Reference manager for cycle detection.
        private readonly fsCyclicReferenceManager _references;
        private readonly fsLazyCycleDefinitionWriter _lazyReferenceWriter;


        public fsSerializer() {
            _cachedOverrideConverterInstances = new Dictionary<Type, fsBaseConverter>();
            _cachedConverters = new Dictionary<Type, fsBaseConverter>();
            _cachedProcessors = new Dictionary<Type, List<fsObjectProcessor>>();

            _references = new fsCyclicReferenceManager();
            _lazyReferenceWriter = new fsLazyCycleDefinitionWriter();

            // note: The order here is important. Items at the beginning of this
            //       list will be used before converters at the end. Converters
            //       added via AddConverter() are added to the front of the list.
            _availableConverters = new List<fsConverter>
            {
                new fsUnityObjectConverter { Serializer = this },
                new fsTypeConverter { Serializer = this },
                new fsEnumConverter { Serializer = this },
                new fsPrimitiveConverter { Serializer = this },
                new fsArrayConverter { Serializer = this },
                new fsDictionaryConverter { Serializer = this },
                new fsListConverter { Serializer = this },
                new fsReflectedConverter { Serializer = this }
            };
            _availableDirectConverters = new Dictionary<Type, fsDirectConverter>();

            _processors = new List<fsObjectProcessor>();

            Context = new fsContext();
            Config = new fsConfig();

            //DirectConverters. Add manually for performance
            AddConverter(new AnimationCurve_DirectConverter());
            AddConverter(new Bounds_DirectConverter());
            AddConverter(new GUIStyleState_DirectConverter());
            AddConverter(new GUIStyle_DirectConverter());
            AddConverter(new Gradient_DirectConverter());
            AddConverter(new Keyframe_DirectConverter());
            AddConverter(new LayerMask_DirectConverter());
            AddConverter(new RectOffset_DirectConverter());
            AddConverter(new Rect_DirectConverter());
        }

        /// A context object that fsConverters can use to customize how they operate.
        public fsContext Context;
        /// Configuration options. Also see fsGlobalConfig.
        public fsConfig Config;

        /// Fetches all of the processors for the given type.
        private List<fsObjectProcessor> GetProcessors(Type type) {
            List<fsObjectProcessor> processors;
            if ( _cachedProcessors.TryGetValue(type, out processors) ) {
                return processors;
            }

            // Check to see if the user has defined a custom processor for the type. If they
            // have, then we don't need to scan through all of the processor to check which
            // one can process the type; instead, we directly use the specified processor.
            var attr = type.RTGetAttribute<fsObjectAttribute>(true);
            if ( attr != null && attr.Processor != null ) {
                var processor = (fsObjectProcessor)Activator.CreateInstance(attr.Processor);
                processors = new List<fsObjectProcessor>();
                processors.Add(processor);
                _cachedProcessors[type] = processors;
            } else if ( _cachedProcessors.TryGetValue(type, out processors) == false ) {
                processors = new List<fsObjectProcessor>();

                for ( int i = 0; i < _processors.Count; ++i ) {
                    var processor = _processors[i];
                    if ( processor.CanProcess(type) ) {
                        processors.Add(processor);
                    }
                }

                _cachedProcessors[type] = processors;
            }

            return processors;
        }


        /// Adds a new converter that can be used to customize how an object is serialized and
        /// deserialized.
        public void AddConverter(fsBaseConverter converter) {
            if ( converter.Serializer != null ) {
                throw new InvalidOperationException("Cannot add a single converter instance to " +
                    "multiple fsConverters -- please construct a new instance for " + converter);
            }

            if ( converter is fsDirectConverter ) {
                var directConverter = (fsDirectConverter)converter;
                _availableDirectConverters[directConverter.ModelType] = directConverter;
            } else if ( converter is fsConverter ) {
                _availableConverters.Insert(0, (fsConverter)converter);
            } else {
                throw new InvalidOperationException("Unable to add converter " + converter +
                    "; the type association strategy is unknown. Please use either " +
                    "fsDirectConverter or fsConverter as your base type.");
            }

            converter.Serializer = this;
            _cachedConverters = new Dictionary<Type, fsBaseConverter>();
        }

        /// Fetches a converter that can serialize/deserialize the given type.
        private fsBaseConverter GetConverter(Type type, Type overrideConverterType) {

            // Use an override converter type instead if that's what the user has requested.
            if ( overrideConverterType != null ) {
                fsBaseConverter overrideConverter;
                if ( _cachedOverrideConverterInstances.TryGetValue(overrideConverterType, out overrideConverter) == false ) {
                    overrideConverter = (fsBaseConverter)Activator.CreateInstance(overrideConverterType);
                    overrideConverter.Serializer = this;
                    _cachedOverrideConverterInstances[overrideConverterType] = overrideConverter;
                }

                return overrideConverter;
            }

            // Try to lookup an existing converter.
            fsBaseConverter converter;
            if ( _cachedConverters.TryGetValue(type, out converter) ) {
                return converter;
            }

            // Check to see if the user has defined a custom converter for the type. If they
            // have, then we don't need to scan through all of the converters to check which
            // one can process the type; instead, we directly use the specified converter.
            {
                var attr = type.RTGetAttribute<fsObjectAttribute>(true);
                if ( attr != null && attr.Converter != null ) {
                    converter = (fsBaseConverter)Activator.CreateInstance(attr.Converter);
                    converter.Serializer = this;
                    return _cachedConverters[type] = converter;
                }
            }

            // Check for a [fsForward] attribute.
            {
                var attr = type.RTGetAttribute<fsForwardAttribute>(true);
                if ( attr != null ) {
                    converter = new fsForwardConverter(attr);
                    converter.Serializer = this;
                    return _cachedConverters[type] = converter;
                }
            }

            // No converter specified. Find match from general ones.
            {
                fsDirectConverter directConverter;
                if ( _availableDirectConverters.TryGetValue(type, out directConverter) ) {
                    return _cachedConverters[type] = directConverter;
                }

                for ( var i = 0; i < _availableConverters.Count; i++ ) {
                    if ( _availableConverters[i].CanProcess(type) ) {
                        return _cachedConverters[type] = _availableConverters[i];
                    }
                }
            }

            // No converter available
            return _cachedConverters[type] = null;
        }

        ///----------------------------------------------------------------------------------------------

        /// Helper method that simply forwards the call to TrySerialize(typeof(T), instance, out data);
        public fsResult TrySerialize<T>(T instance, out fsData data) {
            return TrySerialize(typeof(T), instance, out data);
        }

        /// Serialize the given value.
        public fsResult TrySerialize(Type storageType, object instance, out fsData data) {
            return TrySerialize(storageType, null, instance, out data);
        }

        /// Serialize the given value.
        /// StorageType: field type.
        /// OverideConverter: optional override converter.
        /// Instance: the object instance.
        /// Data: the serialized state.
        public fsResult TrySerialize(Type storageType, Type overrideConverterType, object instance, out fsData data) {
            var processors = GetProcessors(instance == null ? storageType : instance.GetType());
            Invoke_OnBeforeSerialize(processors, storageType, instance);

            // We always serialize null directly as null
            if ( ReferenceEquals(instance, null) ) {
                data = new fsData();
                Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
                return fsResult.Success;
            }

            fsResult result;

            try {
                _references.Enter();
                result = Internal_Serialize(storageType, overrideConverterType, instance, out data);
            }

            finally {
                if ( _references.Exit() ) { _lazyReferenceWriter.Clear(); }
            }

            Invoke_OnAfterSerialize(processors, storageType, instance, ref data);
            return result;
        }

        //...
        fsResult Internal_Serialize(Type storageType, Type overrideConverterType, object instance, out fsData data) {

            var instanceType = instance.GetType();
            var instanceTypeConverter = GetConverter(instanceType, overrideConverterType);
            if ( instanceTypeConverter == null ) {
                data = new fsData();
                return fsResult.Warn(string.Format("No converter for {0}", instanceType));
            }

            var needsCycleSupport = instanceTypeConverter.RequestCycleSupport(instanceType);
            if ( needsCycleSupport ) {
                // We've already serialized this object instance (or it is pending higher up on the call stack).
                // Just serialize a reference to it to escape the cycle.
                if ( _references.IsReference(instance) ) {
                    data = fsData.CreateDictionary();
                    _lazyReferenceWriter.WriteReference(_references.GetReferenceId(instance), data.AsDictionary);
                    return fsResult.Success;
                }

                // Mark inside the object graph that we've serialized the instance. We do this *before*
                // serialization so that if we get back into this function recursively, it'll already
                // be marked and we can handle the cycle properly without going into an infinite loop.
                _references.MarkSerialized(instance);
            }


            // Serialize the instance with it's actual instance type, not storageType.
            var serializeResult = instanceTypeConverter.TrySerialize(instance, out data, instanceType);
            if ( serializeResult.Failed ) {
                return serializeResult;
            }

            // Do we need to add type information? If the field type and the instance type are different.
            if ( storageType != instanceType && GetConverter(storageType, overrideConverterType).RequestInheritanceSupport(storageType) ) {
                EnsureDictionary(data);
                data.AsDictionary[KEY_INSTANCE_TYPE] = new fsData(instanceType.FullName);
            }

            if ( needsCycleSupport ) {
                _lazyReferenceWriter.WriteDefinition(_references.GetReferenceId(instance), data);
            }

            return serializeResult;
        }


        ///----------------------------------------------------------------------------------------------

        /// Generic wrapper around TryDeserialize that simply forwards the call.
        public fsResult TryDeserialize<T>(fsData data, ref T instance) {
            object boxed = instance;
            var result = TryDeserialize(data, typeof(T), ref boxed);
            if ( result.Succeeded ) {
                instance = (T)boxed;
            }
            return result;
        }

        /// Attempts to deserialize a value from a serialized state.
        public fsResult TryDeserialize(fsData data, Type storageType, ref object result) {
            return TryDeserialize(data, storageType, null, ref result);
        }

        /// Attempts to deserialize a value from a serialized state.
        public fsResult TryDeserialize(fsData data, Type storageType, Type overrideConverterType, ref object result) {
            if ( data.IsNull ) {
                result = null;
                var processors = GetProcessors(storageType);
                Invoke_OnBeforeDeserialize(processors, storageType, ref data);
                Invoke_OnAfterDeserialize(processors, storageType, null);
                return fsResult.Success;
            }

            try {
                _references.Enter();

                List<fsObjectProcessor> processors;
                var r = Internal_Deserialize(overrideConverterType, data, storageType, ref result, out processors);
                if ( r.Succeeded ) { Invoke_OnAfterDeserialize(processors, storageType, result); }
                return r;
            }

            catch ( Exception e ) {
                ParadoxNotion.Services.Logger.LogException(e, "Deserialization", result);
                return fsResult.Fail(e.Message);
            }

            finally {
                _references.Exit();
            }
        }

        //...
        fsResult Internal_Deserialize(Type overrideConverterType, fsData data, Type storageType, ref object result, out List<fsObjectProcessor> processors) {
            //$ref encountered. Do before inheritance.
            if ( IsObjectReference(data) ) {
                int refId = int.Parse(data.AsDictionary[KEY_OBJECT_REFERENCE].AsString);
                result = _references.GetReferenceObject(refId);
                processors = GetProcessors(result.GetType());
                return fsResult.Success;
            }

            var deserializeResult = fsResult.Success;

            // We wait until here to actually Invoke_OnBeforeDeserialize because we do not
            // have the correct set of processors to invoke until *after* we have resolved
            // the proper type to use for deserialization.
            processors = GetProcessors(storageType);
            Invoke_OnBeforeDeserialize(processors, storageType, ref data);

            var objectType = storageType;

            // If the serialized state contains type information, then we need to make sure to update our
            // objectType and data to the proper values so that when we construct an object instance later
            // and run deserialization we run it on the proper type.
            // $type
            if ( IsTypeSpecified(data) ) {
                var typeNameData = data.AsDictionary[KEY_INSTANCE_TYPE];

                do {
                    if ( !typeNameData.IsString ) {
                        deserializeResult.AddMessage(string.Format("{0} value must be a string", KEY_INSTANCE_TYPE));
                        break;
                    }

                    var typeName = typeNameData.AsString;
                    var type = ReflectionTools.GetType(typeName, storageType);

                    if ( type == null ) {
                        deserializeResult.AddMessage(string.Format("{0} type can not be resolved", typeName));
                        break;
                    }

                    if ( !storageType.IsAssignableFrom(type) ) {
                        deserializeResult.AddMessage(string.Format("Ignoring type specifier. Field or type {0} can't hold and instance of type {1}", storageType, type));
                        break;
                    }

                    objectType = type;

                } while ( false );
            }

            var converter = GetConverter(objectType, overrideConverterType);
            if ( converter == null ) {
                return fsResult.Warn(string.Format("No Converter available for {0}", objectType));
            }

            // Construct an object instance if we don't have one already using actual objectType
            if ( ReferenceEquals(result, null) || result.GetType() != objectType ) {
                result = converter.CreateInstance(data, objectType);
            }

            // invoke callback with storageType
            Invoke_OnBeforeDeserializeAfterInstanceCreation(processors, storageType, result, ref data);

            // $id
            if ( IsObjectDefinition(data) ) {
                var sourceId = int.Parse(data.AsDictionary[KEY_OBJECT_DEFINITION].AsString);
                _references.AddReferenceWithId(sourceId, result);
            }

            // $content
            if ( IsWrappedData(data) ) {
                data = data.AsDictionary[KEY_CONTENT];
            }

            // must pass actual objectType instead of storageType
            return deserializeResult += converter.TryDeserialize(data, ref result, objectType);
        }

    }
}