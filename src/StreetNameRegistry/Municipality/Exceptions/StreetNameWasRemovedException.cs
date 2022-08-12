namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameWasRemovedException : StreetNameRegistryException
    {
        public PersistentLocalId PersistentLocalId { get; }

        public StreetNameWasRemovedException() { }

        public StreetNameWasRemovedException(PersistentLocalId persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
