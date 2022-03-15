namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameWasRemovedException : StreetNameRegistryException
    {
        public int PersistentLocalId { get; }

        public StreetNameWasRemovedException() { }

        public StreetNameWasRemovedException(int persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
