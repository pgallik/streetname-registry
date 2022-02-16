namespace StreetNameRegistry.StreetName
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Events;

    public class MunicipalityStreetName : Entity
    {
        private MunicipalityId _municipalityId;

        public StreetNameStatus? Status { get; private set; }
        public HomonymAdditions HomonymAdditions { get; private set; } = new HomonymAdditions();
        public Names Names { get; private set; } = new Names();
        public PersistentLocalId PersistentLocalId { get; private set; }
        public bool IsRemoved { get; private set; }

        public MunicipalityStreetName(Action<object> applier)
            : base(applier)
        {
            Register<StreetNameWasMigratedToMunicipality>(When);
            Register<StreetNameWasProposedV2>(When);
        }

        void When(StreetNameWasMigratedToMunicipality @event)
        {
            _municipalityId = new MunicipalityId(@event.MunicipalityId);
            Status = @event.Status;
            PersistentLocalId = new PersistentLocalId(@event.PersistentLocalId);
            HomonymAdditions = new HomonymAdditions(@event.HomonymAdditions);
            Names = new Names(@event.Names);
            IsRemoved = @event.IsRemoved;
        }

        void When(StreetNameWasProposedV2 @event)
        {
            _municipalityId = new MunicipalityId(@event.MunicipalityId);
            Status = StreetNameStatus.Proposed;
            PersistentLocalId = new PersistentLocalId(@event.PersistentLocalId);
            Names = new Names(@event.StreetNameNames);
            IsRemoved = false;
        }
    }
}
