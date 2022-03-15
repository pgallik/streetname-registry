namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameStatusPreventsApprovalException : StreetNameRegistryException
    {
        public int PersistentLocalId { get; }

        public StreetNameStatusPreventsApprovalException() { }

        public StreetNameStatusPreventsApprovalException(int persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
