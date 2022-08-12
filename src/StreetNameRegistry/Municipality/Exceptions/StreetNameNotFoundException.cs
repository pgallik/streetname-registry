namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameNotFoundException : StreetNameRegistryException
    {
        public PersistentLocalId PersistentLocalId { get; }

        public StreetNameNotFoundException() { }

        public StreetNameNotFoundException(PersistentLocalId persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
