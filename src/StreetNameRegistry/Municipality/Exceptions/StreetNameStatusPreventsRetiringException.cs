namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameStatusPreventsRetiringException : StreetNameRegistryException
    {
        public int PersistentLocalId { get; }

        public StreetNameStatusPreventsRetiringException() { }

        public StreetNameStatusPreventsRetiringException(int persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
