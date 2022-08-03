namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameStatusPreventsRejectionException : StreetNameRegistryException
    {
        public int PersistentLocalId { get; }

        public StreetNameStatusPreventsRejectionException() { }

        public StreetNameStatusPreventsRejectionException(int persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
