namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameIsRemovedException : StreetNameRegistryException
    {
        public PersistentLocalId PersistentLocalId { get; }

        public StreetNameIsRemovedException() { }

        public StreetNameIsRemovedException(PersistentLocalId persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
