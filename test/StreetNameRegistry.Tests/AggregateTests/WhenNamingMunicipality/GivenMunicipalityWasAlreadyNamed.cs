namespace StreetNameRegistry.Tests.AggregateTests.WhenNamingMunicipality
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using global::AutoFixture;
    using StreetName;
    using StreetName.Commands.Municipality;
    using StreetName.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityWasAlreadyNamed : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;
        private readonly MunicipalityStreamId _streamId;

        public GivenMunicipalityWasAlreadyNamed(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = Fixture.Create<MunicipalityId>();
            _streamId = Fixture.Create<MunicipalityStreamId>();
        }

        [Theory]
        [InlineData(Language.Dutch)]
        [InlineData(Language.French)]
        [InlineData(Language.English)]
        [InlineData(Language.German)]
        public void ThenNamedAgain(Language language)
        {
            var commandNameMunicipality = Fixture.Create<NameMunicipality>().WithName("GreatName", language);
            Assert(new Scenario()
                .Given(_streamId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityWasNamed>()
                })
                .When(commandNameMunicipality)
                .Then(new[]
                {
                    new Fact(_streamId, new MunicipalityWasNamed(_municipalityId, new MunicipalityName("GreatName", language)))
                }));
        }

    }
    public static class NameExtensions
    {
        public static NameMunicipality WithName(this NameMunicipality command, string name, Language language)
        {
            return new NameMunicipality(command.MunicipalityId, new MunicipalityName(name, language), command.Provenance);
        }
    }

}
