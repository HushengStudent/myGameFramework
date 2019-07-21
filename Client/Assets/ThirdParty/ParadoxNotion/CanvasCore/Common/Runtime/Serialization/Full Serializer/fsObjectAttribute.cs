using System;

namespace ParadoxNotion.Serialization.FullSerializer
{

    /// This attribute controls some serialization behavior for a type. See the comments
    /// on each of the fields for more information.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class fsObjectAttribute : Attribute
    {

        /// Specify a custom converter to use for serialization. The converter type needs
        /// to derive from fsBaseConverter. This defaults to null.
        public Type Converter;

        /// Specify a custom processor to use during serialization. The processor type needs
        /// to derive from fsObjectProcessor and the call to CanProcess is not invoked. This
        /// defaults to null.
        public Type Processor;

        public fsObjectAttribute() { }
    }
}