using System;


namespace NodeCanvas.Framework
{
    //An attribute to help with URLs in the welcome window. Thats all.
    [AttributeUsage(AttributeTargets.Class)]
    public class GraphInfoAttribute : Attribute
    {
        public string packageName;
        public string docsURL;
        public string resourcesURL;
        public string forumsURL;
    }

    //Appends custom menu items to the GraphEditor toolbar. Use on Graph methods.
    [AttributeUsage(AttributeTargets.Method)]
    public class ToolbarMenuItemAttribute : Attribute
    {
        readonly public string path;
        public ToolbarMenuItemAttribute(string path) {
            this.path = path;
        }
    }

    ///Marks the BBParameter possible to only pick values from a blackboard.
    [AttributeUsage(AttributeTargets.Field)]
    public class BlackboardOnlyAttribute : Attribute { }
}