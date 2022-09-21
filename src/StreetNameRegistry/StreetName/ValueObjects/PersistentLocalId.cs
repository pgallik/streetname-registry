namespace StreetNameRegistry.StreetName
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public sealed class PersistentLocalId : IntegerValueObject<PersistentLocalId>
    {
        public PersistentLocalId([JsonProperty("value")] int persistentLocalId) : base(persistentLocalId) {}
    }
}
