using System;

namespace YAB.Plugins.Injectables.Options
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyDescriptionAttribute : Attribute
    {
        public PropertyDescriptionAttribute(bool isSecret, string description)
        {
            IsSecret = isSecret;
            Description = description;
        }

        public string Description { get; set; }

        public bool IsSecret { get; set; }
    }
}