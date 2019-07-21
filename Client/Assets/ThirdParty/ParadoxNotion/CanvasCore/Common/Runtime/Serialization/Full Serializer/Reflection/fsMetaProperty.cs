using System;
using System.Reflection;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
    /// A field on a MetaType.
    public class fsMetaProperty
    {

        /// Internal handle to the reflected member.
        private FieldInfo _fieldInfo;

        /// The type of value that is stored inside of the property.
        public Type StorageType { get; private set; }
        /// The serialized name of the property, as it should appear in JSON.
        public string JsonName { get; private set; }
        /// The real name of the member info.
        public string MemberName { get { return _fieldInfo.Name; } }

        internal fsMetaProperty(fsConfig config, FieldInfo field) {
            this._fieldInfo = field;
            this.StorageType = field.FieldType;

            var attr = _fieldInfo.RTGetAttribute<fsSerializeAsAttribute>(true);
            this.JsonName = attr != null && !string.IsNullOrEmpty(attr.Name) ? attr.Name : field.Name;
        }

        /// Writes a value to the property that this MetaProperty represents, using given object
        /// instance as the context.
        public void Write(object context, object value) {
            _fieldInfo.SetValue(context, value);
        }

        /// Reads a value from the property that this MetaProperty represents, using the given
        /// object instance as the context.
        public object Read(object context) {
            return _fieldInfo.GetValue(context);
        }
    }
}