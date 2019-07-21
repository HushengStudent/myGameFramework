using System;

namespace ParadoxNotion.Serialization.FullSerializer
{

    /// The given property or field annotated with [JsonIgnore] will not be serialized.
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class fsIgnoreAttribute : Attribute { }
}