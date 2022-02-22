namespace StreetNameRegistry.Municipality
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Newtonsoft.Json;

    public class StreetNameId : GuidValueObject<StreetNameId>
    {
        public static StreetNameId CreateFor(CrabStreetNameId crabStreetNameId)
            => new StreetNameId(crabStreetNameId.CreateDeterministicId());

        public StreetNameId([JsonProperty("value")] Guid id) : base(id) { }
    }
}
