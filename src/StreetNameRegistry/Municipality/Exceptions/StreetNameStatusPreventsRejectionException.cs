namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameStatusPreventsRejectionException : StreetNameRegistryException
    {
        public PersistentLocalId PersistentLocalId { get; }

        public StreetNameStatusPreventsRejectionException() { }

        public StreetNameStatusPreventsRejectionException(PersistentLocalId persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
