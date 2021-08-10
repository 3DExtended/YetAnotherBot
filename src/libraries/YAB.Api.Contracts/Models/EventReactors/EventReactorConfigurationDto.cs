namespace YAB.Api.Contracts.Models.EventReactors
{
    public class EventReactorConfigurationDto
    {
        public string EventTypeName { get; set; }

        public string SeralizedEventReactorConfiguration { get; set; }
    }
}