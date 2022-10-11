namespace StreetNameRegistry.Tests.AggregateTests.Extensions
{
    using Municipality;
    using Municipality.Commands;

    public static class RejectStreetNameExtensions
    {
        public static RejectStreetName WithMunicipalityId(this RejectStreetName command, MunicipalityId municipalityId)
        {
            return new RejectStreetName(municipalityId, command.PersistentLocalId, command.Provenance);
        }
    }
}
