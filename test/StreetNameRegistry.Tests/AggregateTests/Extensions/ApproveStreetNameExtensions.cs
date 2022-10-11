namespace StreetNameRegistry.Tests.AggregateTests.Extensions
{
    using Municipality;
    using Municipality.Commands;

    public static class ApproveStreetNameExtensions
    {
        public static ApproveStreetName WithMunicipalityId(this ApproveStreetName command, MunicipalityId municipalityId)
        {
            return new ApproveStreetName(municipalityId, command.PersistentLocalId, command.Provenance);
        }
    }
}
