using System;

namespace YAB.Core.Annotations
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