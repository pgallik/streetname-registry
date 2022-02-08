namespace StreetNameRegistry.Tests.AggregateTests.WhenProposingStreetName
{
    using System.Collections.Generic;
    using global::AutoFixture;
    using StreetName;
    using StreetName.Commands;

    public static class ProposeStreetNameExtensions
    {
        public static ProposeStreetName WithMunicipalityId(this ProposeStreetName command, MunicipalityId municipalityId)
        {
            return new ProposeStreetName(municipalityId, command.StreetNameNames, command.PersistentLocalId, command.Provenance);
        }

        public static ProposeStreetName WithRandomStreetName(this ProposeStreetName command, Fixture fixture)
        {
            return new ProposeStreetName(command.MunicipalityId, new Names(new List<StreetNameName>{fixture.Create<StreetNameName>()}), command.PersistentLocalId, command.Provenance);
        }

        public static ProposeStreetName WithStreetNameNames(this ProposeStreetName command, Names names)
        {
            return new ProposeStreetName(command.MunicipalityId, names, command.PersistentLocalId, command.Provenance);
        }
    }
}
