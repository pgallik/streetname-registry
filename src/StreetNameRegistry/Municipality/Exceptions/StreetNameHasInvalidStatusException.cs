namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameHasInvalidStatusException : StreetNameRegistryException
    {
        public PersistentLocalId PersistentLocalId { get; }

        public StreetNameHasInvalidStatusException() { }

        public StreetNameHasInvalidStatusException(PersistentLocalId persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
