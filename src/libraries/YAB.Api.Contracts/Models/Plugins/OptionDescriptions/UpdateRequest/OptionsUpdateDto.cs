using System.Collections.Generic;

namespace YAB.Api.Contracts.Models.Plugins.OptionDescriptions.UpdateRequest
{
    public class OptionsUpdateDto
    {
        public string OptionFullName { get; set; }

        public IReadOnlyList<OptionsPropertyUpdateRequestDto> UpdatedProperties { get; set; }
    }
}