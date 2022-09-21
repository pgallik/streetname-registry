namespace StreetNameRegistry.Municipality
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public sealed class StreetNameId : GuidValueObject<StreetNameId>
    {
        public StreetNameId(Guid id) : base(id) { }
    }
}
