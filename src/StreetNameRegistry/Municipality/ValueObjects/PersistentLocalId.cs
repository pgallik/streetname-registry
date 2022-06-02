namespace StreetNameRegistry.Municipality
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public class PersistentLocalId : IntegerValueObject<PersistentLocalId>
    {
        public PersistentLocalId(int persistentLocalId) : base(persistentLocalId) {}
    }
}
