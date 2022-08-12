namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameStatusPreventsApprovalException : StreetNameRegistryException
    {
        public PersistentLocalId PersistentLocalId { get; }

        public StreetNameStatusPreventsApprovalException() { }

        public StreetNameStatusPreventsApprovalException(PersistentLocalId persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
