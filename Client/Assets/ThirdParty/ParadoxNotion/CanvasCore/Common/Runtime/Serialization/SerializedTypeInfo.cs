using System;
using UnityEngine;

namespace ParadoxNotion.Serialization
{

    [Serializable]
    public class SerializedTypeInfo : ISerializationCallbackReceiver
    {

        [SerializeField]
        private string _baseInfo;

        [NonSerialized]
        private Type _type;

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            if ( _type != null ) {
                _baseInfo = _type.FullName;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if ( _baseInfo == null ) {
                return;
            }
            _type = ReflectionTools.GetType(_baseInfo, true);
        }

        public SerializedTypeInfo() { }
        public SerializedTypeInfo(Type info) {
            _type = info;
        }

        public Type Get() {
            return _type;
        }
    }
}