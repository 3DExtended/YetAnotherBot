using System;

namespace YAB.Plugins.Injectables.Options
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReactorConfigurationDescriptionAttribute : Attribute
    {
        public ReactorConfigurationDescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}