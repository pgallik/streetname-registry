namespace StreetNameRegistry.Tests.AggregateTests.WhenProposingStreetName
{
    using System.Collections.Generic;
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using global::AutoFixture;
    using StreetName;
    using StreetName.Commands;
    using StreetName.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipality : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;

        public GivenMunicipality(ITestOutputHelper output) : base(output)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = Fixture.Create<MunicipalityId>();
        }

        [Fact]
        public void ThenStreetNameWasProposed()
        {
            var command = Fixture.Create<ProposeStreetName>()
                .WithMunicipalityId(_municipalityId)
                .WithRandomStreetName(Fixture);

            Assert(new Scenario()
                .Given(_municipalityId,
                    Fixture.Create<MunicipalityWasImported>())
                .When(command)
                .Then(new []
                {
                    new Fact(_municipalityId, new StreetNameWasProposedV2(_municipalityId,command.StreetNameNames, command.PersistentLocalId))
                }));

        }
    }

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
    }
}
