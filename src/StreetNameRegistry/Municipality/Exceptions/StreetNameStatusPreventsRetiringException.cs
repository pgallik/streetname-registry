namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameStatusPreventsRetiringException : StreetNameRegistryException
    {
        public PersistentLocalId PersistentLocalId { get; }

        public StreetNameStatusPreventsRetiringException() { }

        public StreetNameStatusPreventsRetiringException(PersistentLocalId persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
