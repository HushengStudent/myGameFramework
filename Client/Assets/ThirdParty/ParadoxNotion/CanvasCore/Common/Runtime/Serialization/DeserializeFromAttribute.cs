using System;

namespace ParadoxNotion.Serialization
{

    public class DeserializeFromAttribute : Attribute
    {
        readonly public string previousTypeFullName;
        public DeserializeFromAttribute(string previousTypeFullName) {
            this.previousTypeFullName = previousTypeFullName;
        }
    }
}