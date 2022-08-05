namespace StreetNameRegistry.Tests.AggregateTests.WhenRetiringStreetName
{
    using StreetNameRegistry.Municipality;
    using StreetNameRegistry.Municipality.Commands;

    public static class RetireStreetNameExtensions
    {
        public static RetireStreetName WithMunicipalityId(this RetireStreetName command, MunicipalityId municipalityId)
        {
            return new RetireStreetName(municipalityId, command.PersistentLocalId, command.Provenance);
        }
    }
}
