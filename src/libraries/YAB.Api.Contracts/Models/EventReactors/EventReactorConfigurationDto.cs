using System.Collections.Generic;

using YAB.Api.Contracts.Models.Plugins.OptionDescriptions;

namespace YAB.Api.Contracts.Models.EventReactors
{
    public class EventReactorConfigurationDto
    {
        public string Description { get; set; }

        public string EventTypeName { get; set; }

        public List<PropertyDescriptionDto> Properties { get; set; }

        public string SeralizedEventReactorConfiguration { get; set; }
    }
}