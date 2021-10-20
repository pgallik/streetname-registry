namespace StreetNameRegistry.StreetName
{
    public interface IPersistentLocalIdGenerator
    {
        PersistentLocalId GenerateNextPersistentLocalId();
    }
}
