﻿namespace YAB.Api.Contracts.Models.Plugins.OptionDescriptions
{
    public class PropertyDescriptionDto
    {
        public string PropertyDescription { get; set; }

        public string PropertyName { get; set; }

        public PropertyValueTypeDto ValueType { get; set; }
    }
}