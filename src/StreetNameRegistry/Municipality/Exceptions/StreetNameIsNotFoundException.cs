namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameIsNotFoundException : StreetNameRegistryException
    {
        public PersistentLocalId PersistentLocalId { get; }

        public StreetNameIsNotFoundException() { }

        public StreetNameIsNotFoundException(PersistentLocalId persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
