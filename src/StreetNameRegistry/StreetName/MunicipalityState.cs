namespace StreetNameRegistry.StreetName
{
    using Events;

    public partial class Municipality
    {
        private MunicipalityId _municipalityId;
        private Names _streetNameNames;
        private Municipality()
        {
            Register<MunicipalityWasImported>(When);
            Register<StreetNameWasProposedV2>(When);
        }
        private void When(MunicipalityWasImported @event)
        {
            _municipalityId = new MunicipalityId(@event.MunicipalityId);
        }

        private void When(StreetNameWasProposedV2 @event)
        {
            _streetNameNames = new Names(@event.StreetNameNames);
        }
    }
}
