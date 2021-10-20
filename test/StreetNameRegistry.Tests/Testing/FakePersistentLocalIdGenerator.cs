namespace StreetNameRegistry.Tests.Testing
{
    using StreetName;

    public class FakePersistentLocalIdGenerator : IPersistentLocalIdGenerator
    {
        public PersistentLocalId GenerateNextPersistentLocalId()
        {
            return new PersistentLocalId(1);
        }
    }
}
