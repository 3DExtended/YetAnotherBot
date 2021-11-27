using System;

namespace YAB.Plugins.Injectables.Options
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClassDescriptionAttribute : Attribute
    {
        public ClassDescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}