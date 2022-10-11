namespace StreetNameRegistry.Tests.AggregateTests.Extensions
{
    using Municipality;
    using Municipality.Commands;

    public static class CorrectStreetNameRetirementExtensions
    {
        public static CorrectStreetNameRetirement WithMunicipalityId(this CorrectStreetNameRetirement command, MunicipalityId municipalityId)
        {
            return new CorrectStreetNameRetirement(municipalityId, command.PersistentLocalId, command.Provenance);
        }
    }
}
