using System;

namespace ParadoxNotion.Design
{

    ///Marker attribute to include generic type or a type's generic methods in the AOT spoof generation
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Delegate)]
    public class SpoofAOTAttribute : Attribute { }

    ///To exclude a type from being listed. Abstract classes are not listed anyway.
    [AttributeUsage(AttributeTargets.Class)]
    public class DoNotListAttribute : Attribute { }

    ///When a type should for some reason be marked as protected so to always have one instance active.
    [AttributeUsage(AttributeTargets.Class)]
    public class ProtectedSingletonAttribute : Attribute { }

    ///Use for execution prioratizing when it matters.
    [AttributeUsage(AttributeTargets.Class)]
    public class ExecutionPriorityAttribute : Attribute
    {
        readonly public int priority;
        public ExecutionPriorityAttribute(int priority) {
            this.priority = priority;
        }
    }

    ///Marks a generic type to be exposed at it's base definition rather than wrapping all preferred types around it.
    [AttributeUsage(AttributeTargets.Class)]
    public class ExposeAsDefinitionAttribute : Attribute { }

    ///Marks a field to be exposed for inspection even if private (within the context of custom inspector).
    ///In custom inspector, private fields even if with [SerializedField] are not exposed by default.
    [AttributeUsage(AttributeTargets.Field)]
    public class ExposeFieldAttribute : Attribute { }


    ///----------------------------------------------------------------------------------------------

    ///Use for friendly names and optional priority in relation to naming only
    [AttributeUsage(AttributeTargets.All)]
    public class NameAttribute : Attribute
    {
        readonly public string name;
        readonly public int priority;
        public NameAttribute(string name, int priority = 0) {
            this.name = name;
            this.priority = priority;
        }
    }

    ///Use for categorization
    [AttributeUsage(AttributeTargets.All)]
    public class CategoryAttribute : Attribute
    {
        readonly public string category;
        public CategoryAttribute(string category) {
            this.category = category;
        }
    }

    ///Use to give a description
    [AttributeUsage(AttributeTargets.All)]
    public class DescriptionAttribute : Attribute
    {
        readonly public string description;
        public DescriptionAttribute(string description) {
            this.description = description;
        }
    }

    ///When a type is associated with an icon
    [AttributeUsage(AttributeTargets.Class)]
    public class IconAttribute : Attribute
    {
        readonly public string iconName;
        readonly public bool fixedColor;
        readonly public string runtimeIconTypeCallback;
        readonly public Type fromType;
        public IconAttribute(string iconName = "", bool fixedColor = false, string runtimeIconTypeCallback = "") {
            this.iconName = iconName;
            this.fixedColor = fixedColor;
            this.runtimeIconTypeCallback = runtimeIconTypeCallback;
        }
        public IconAttribute(Type fromType) {
            this.fromType = fromType;
        }
    }

    ///When a type is associated with a color (provide in hex string without "#")
    [AttributeUsage(AttributeTargets.Class)]
    public class ColorAttribute : Attribute
    {
        readonly public string hexColor;
        public ColorAttribute(string hexColor) {
            this.hexColor = hexColor;
        }
    }

}