using System.Collections.Generic;

namespace YAB.Api.Contracts.Models.Plugins.OptionDescriptions
{
    public class OptionsDescriptionDto
    {
        public string OptionFullName { get; set; }

        public IReadOnlyList<PropertyDescriptionDto> Properties { get; set; }
    }
}