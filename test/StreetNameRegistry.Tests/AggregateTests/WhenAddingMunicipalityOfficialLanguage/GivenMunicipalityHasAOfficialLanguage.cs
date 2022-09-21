namespace StreetNameRegistry.Tests.AggregateTests.WhenAddingMunicipalityOfficialLanguage
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Commands;
    using Municipality.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public sealed class GivenMunicipalityHasAOfficialLanguage : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;
        private readonly MunicipalityStreamId _streamId;

        public GivenMunicipalityHasAOfficialLanguage(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
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
        public void ThenNone(Language language)
        {
            Fixture.Register(() => language);
            var commandLanguageAdded = Fixture.Create<AddOfficialLanguageToMunicipality>();

            Assert(new Scenario()
                .Given(_streamId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityOfficialLanguageWasAdded>()
                })
                .When(commandLanguageAdded)
                .ThenNone());
        }

        [Theory]
        [InlineData(Language.Dutch)]
        [InlineData(Language.French)]
        [InlineData(Language.English)]
        [InlineData(Language.German)]
        public void AndWasRemoved_OfficialLanguageWasAdded(Language language)
        {
            Fixture.Register(() => language);
            var commandLanguageAdded = Fixture.Create<AddOfficialLanguageToMunicipality>();

            Assert(new Scenario()
                .Given(_streamId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                    Fixture.Create<MunicipalityOfficialLanguageWasRemoved>()
                })
                .When(commandLanguageAdded)
                .Then(new[]
                {
                    new Fact(_streamId, new MunicipalityOfficialLanguageWasAdded(_municipalityId, language))
                }));
        }

        [Fact]
        public void AndHasMultipleLanguages_TheCorrectOneWasAdded()
        {
            var languageAddGerman = Fixture.Create<AddOfficialLanguageToMunicipality>().WithLanguage(Language.German);
            var commandAddedEnglish = Fixture.Create<AddOfficialLanguageToMunicipality>().WithLanguage(Language.English);
            var commandAddedDutch = Fixture.Create<AddOfficialLanguageToMunicipality>().WithLanguage(Language.Dutch);
            Assert(new Scenario()
                .Given(_streamId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    commandAddedEnglish.ToEvent(),
                    commandAddedDutch.ToEvent()
                })
                .When(languageAddGerman)
                .Then(new[]
                {
                    new Fact(_streamId, new MunicipalityOfficialLanguageWasAdded(_municipalityId, Language.German))
                }));
        }
    }
}
