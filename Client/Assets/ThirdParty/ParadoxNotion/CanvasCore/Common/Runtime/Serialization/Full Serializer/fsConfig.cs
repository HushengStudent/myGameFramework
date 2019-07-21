using System;

namespace ParadoxNotion.Serialization.FullSerializer
{

    // Global configuration options.
    public static class fsGlobalConfig
    {

        /// Serialize default values?
        public static bool SerializeDefaultValues = false;

        /// Should deserialization be case sensitive? If this is false and the JSON has multiple members with the
        /// same keys only separated by case, then this results in undefined behavior.
        public static bool IsCaseSensitive = false;
    }

    /// Enables some top-level customization of Full Serializer.
    public class fsConfig
    {

        /// The attributes that will force a field or property to be serialized.
        public Type[] SerializeAttributes =
        {
            typeof(UnityEngine.SerializeField),
            typeof(fsSerializeAsAttribute)
        };

        /// The attributes that will force a field or property to *not* be serialized.
        public Type[] IgnoreSerializeAttributes =
        {
            typeof(NonSerializedAttribute),
            typeof(fsIgnoreAttribute)
        };

        /// If not null, this string format will be used for DateTime instead of the default one.
        public string CustomDateTimeFormatString = null;

        /// Int64 and UInt64 will be serialized and deserialized as string for compatibility
        public bool Serialize64BitIntegerAsString = false;

        /// Enums are serialized using their names by default. Setting this to true will serialize them as integers instead.
        public bool SerializeEnumsAsInteger = false;
    }
}