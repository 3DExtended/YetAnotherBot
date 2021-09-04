using System;

namespace YAB.Plugins.Injectables.Options
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class OptionPropertyDescriptionAttribute : Attribute
    {
        public OptionPropertyDescriptionAttribute(bool isSecret, string description)
        {
            IsSecret = isSecret;
            Description = description;
        }

        public bool IsSecret { get; set; }
        public string Description { get; set; }
    }
}