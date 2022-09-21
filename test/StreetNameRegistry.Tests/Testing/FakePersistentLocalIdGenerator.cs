namespace StreetNameRegistry.Tests.Testing
{
    using Municipality;

    public sealed class FakePersistentLocalIdGenerator : IPersistentLocalIdGenerator
    {
        public PersistentLocalId GenerateNextPersistentLocalId()
        {
            return new PersistentLocalId(1);
        }
    }
}
