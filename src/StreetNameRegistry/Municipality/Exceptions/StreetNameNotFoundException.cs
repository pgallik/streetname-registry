namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameNotFoundException : StreetNameRegistryException
    {
        public int PersistentLocalId { get; }

        public StreetNameNotFoundException() { }

        public StreetNameNotFoundException(int persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
