namespace YAB.Api.Contracts.Models.Plugins.OptionDescriptions
{
    public class PropertyDescriptionDto
    {
        public string CurrentValue { get; set; }

        public bool IsSecret { get; set; }

        public string PropertyDescription { get; set; }

        public string PropertyName { get; set; }

        public PropertyValueTypeDto ValueType { get; set; }
    }
}