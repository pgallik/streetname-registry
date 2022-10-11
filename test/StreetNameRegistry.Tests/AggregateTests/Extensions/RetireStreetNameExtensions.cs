namespace StreetNameRegistry.Tests.AggregateTests.Extensions
{
    using Municipality;
    using Municipality.Commands;

    public static class RetireStreetNameExtensions
    {
        public static RetireStreetName WithMunicipalityId(this RetireStreetName command, MunicipalityId municipalityId)
        {
            return new RetireStreetName(municipalityId, command.PersistentLocalId, command.Provenance);
        }
    }
}
