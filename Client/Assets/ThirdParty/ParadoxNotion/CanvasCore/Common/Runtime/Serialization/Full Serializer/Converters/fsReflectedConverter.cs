using System;
using System.Collections;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
    public class fsReflectedConverter : fsConverter
    {

        public override bool CanProcess(Type type) {
            if ( type.IsArray || typeof(ICollection).IsAssignableFrom(type) ) {
                return false;
            }
            return true;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType) {
            serialized = fsData.CreateDictionary();
            var result = fsResult.Success;

            fsMetaType metaType = fsMetaType.Get(Serializer.Config, instance.GetType());

            //Dont do this for UnityObject. While there is fsUnityObjectConverter, this converter is also used as override,
            //when serializing a UnityObject directly.
            object defaultInstance = null;
            if ( fsGlobalConfig.SerializeDefaultValues == false && !( instance is UnityEngine.Object ) ) {
                defaultInstance = metaType.CreateInstance();
            }

            for ( int i = 0; i < metaType.Properties.Length; ++i ) {
                fsMetaProperty property = metaType.Properties[i];

                if ( fsGlobalConfig.SerializeDefaultValues == false && defaultInstance != null ) {
                    if ( Equals(property.Read(instance), property.Read(defaultInstance)) ) {
                        continue;
                    }
                }

                fsData serializedData;

                var itemResult = Serializer.TrySerialize(property.StorageType, null, property.Read(instance), out serializedData);
                result.AddMessages(itemResult);
                if ( itemResult.Failed ) {
                    continue;
                }

                serialized.AsDictionary[property.JsonName] = serializedData;
            }

            return result;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType) {
            var result = fsResult.Success;

            // Verify that we actually have an Object
            if ( ( result += CheckType(data, fsDataType.Object) ).Failed ) {
                return result;
            }

            if ( data.AsDictionary.Count == 0 ) {
                return fsResult.Success;
            }

            fsMetaType metaType = fsMetaType.Get(Serializer.Config, storageType);

            for ( int i = 0; i < metaType.Properties.Length; ++i ) {
                fsMetaProperty property = metaType.Properties[i];

                fsData propertyData;
                if ( data.AsDictionary.TryGetValue(property.JsonName, out propertyData) ) {
                    object deserializedValue = null;

                    var itemResult = Serializer.TryDeserialize(propertyData, property.StorageType, null, ref deserializedValue);
                    result.AddMessages(itemResult);
                    if ( itemResult.Failed ) continue;

                    property.Write(instance, deserializedValue);
                }
            }

            return result;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            fsMetaType metaType = fsMetaType.Get(Serializer.Config, storageType);
            return metaType.CreateInstance();
        }
    }
}