using System;
using System.Linq;
using System.Reflection;
using ParadoxNotion.Serialization.FullSerializer.Internal;
using UnityEngine;

namespace ParadoxNotion.Serialization
{
    [Serializable]
    public class SerializedConstructorInfo : SerializedMethodBaseInfo
    {

        [SerializeField]
        private string _baseInfo;
        [SerializeField]
        private string _paramsInfo;

        [NonSerialized]
        private ConstructorInfo _constructor;
        [NonSerialized]
        private bool _hasChanged;

        public override void OnBeforeSerialize() {
            _hasChanged = false;
            if ( _constructor != null ) {
                _baseInfo = _constructor.RTReflectedOrDeclaredType().FullName + "|" + "$Constructor";
                _paramsInfo = string.Join("|", _constructor.GetParameters().Select(p => p.ParameterType.FullName).ToArray());
            }
        }

        public override void OnAfterDeserialize() {
            _hasChanged = false;
            var split = _baseInfo.Split('|');
            var type = ReflectionTools.GetType(split[0], true);
            if ( type == null ) {
                _constructor = null;
                return;
            }
            var paramTypeNames = string.IsNullOrEmpty(_paramsInfo) ? null : _paramsInfo.Split('|');
            var parameterTypes = paramTypeNames == null ? new Type[0] : paramTypeNames.Select(n => ReflectionTools.GetType(n, true)).ToArray();
            if ( parameterTypes.All(t => t != null) ) {
                _constructor = type.RTGetConstructor(parameterTypes);
            }

            if ( _constructor == null ) {
                _hasChanged = true;
                _constructor = type.RTGetConstructors().FirstOrDefault();
            }
        }

        //required
        public SerializedConstructorInfo() { }
        ///Serialize a new ConstructorInfo
        public SerializedConstructorInfo(ConstructorInfo constructor) {
            _hasChanged = false;
            _constructor = constructor;
        }

        ///Deserialize and return target ConstructorInfo.
        public ConstructorInfo Get() {
            return _constructor;
        }

        //MethodBase info
        public override MethodBase GetBase() {
            return Get();
        }

        ///Are the original and finaly resolve methods different?
        public override bool HasChanged() {
            return _hasChanged;
        }

        ///Returns the serialized constructor information.
        public override string GetMethodString() {
            return string.Format("{0} ({1})", _baseInfo.Replace("|", "."), _paramsInfo.Replace("|", ", "));
        }
    }
}