using System;

namespace YAB.Plugins.Injectables.Options
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OptionPropertyDescriptionAttribute : Attribute
    {
        public OptionPropertyDescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}