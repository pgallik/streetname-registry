namespace StreetNameRegistry.StreetName
{
    using Events;

    public partial class Municipality
    {
        private MunicipalityId _municipalityId;
        private Names _streetNameNames;
        private NisCode _nisCode;

        internal MunicipalityId MunicipalityId => _municipalityId;

        public MunicipalityStatus MunicipalityStatus { get; private set; }

        private Municipality()
        {
            Register<MunicipalityWasImported>(When);
            Register<MunicipalityWasRetired>(When);
            Register<MunicipalityNisCodeWasChanged>(When);
            Register<MunicipalityBecameCurrent>(When);

            Register<StreetNameWasProposedV2>(When);
        }

        #region Municipality
        private void When(MunicipalityWasImported @event)
        {
            _municipalityId = new MunicipalityId(@event.MunicipalityId);
            _nisCode = new NisCode(@event.NisCode);
        }

        private void When(MunicipalityWasRetired @event)
        {
            MunicipalityStatus = MunicipalityStatus.Retired;
        }

        private void When(MunicipalityBecameCurrent @event)
        {
            MunicipalityStatus = MunicipalityStatus.Current;
        }

        private void When(MunicipalityNisCodeWasChanged @event)
        {
            _nisCode = new NisCode(@event.NisCode);
        }
        #endregion Municipality

        private void When(StreetNameWasProposedV2 @event)
        {
            _streetNameNames = new Names(@event.StreetNameNames);
        }

    }
}
