namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameStatusPreventsCorrectingStreetNameNameException : StreetNameRegistryException
    {
        public int PersistentLocalId { get; }

        public StreetNameStatusPreventsCorrectingStreetNameNameException() { }

        public StreetNameStatusPreventsCorrectingStreetNameNameException(int persistentLocalId)
        {
            PersistentLocalId = persistentLocalId;
        }
    }
}
