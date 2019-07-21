using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ParadoxNotion.Serialization.FullSerializer.Internal;

namespace ParadoxNotion.Serialization
{

    [Serializable]
    public class SerializedFieldInfo : ISerializationCallbackReceiver
    {

        [SerializeField]
        private string _baseInfo;

        [NonSerialized]
        private FieldInfo _field;

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            if ( _field != null ) {
                _baseInfo = string.Format("{0}|{1}", _field.RTReflectedOrDeclaredType().FullName, _field.Name);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if ( _baseInfo == null ) {
                return;
            }
            var split = _baseInfo.Split('|');
            var type = ReflectionTools.GetType(split[0], true);
            if ( type == null ) {
                _field = null;
                return;
            }
            var name = split[1];
            _field = type.RTGetField(name);
        }

        public SerializedFieldInfo() { }
        public SerializedFieldInfo(FieldInfo info) {
            _field = info;
        }

        public FieldInfo Get() {
            return _field;
        }
    }
}