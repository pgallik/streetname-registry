namespace StreetNameRegistry.Tests.AggregateTests.WhenRemovingMunicipalityOfficialLanguage
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Commands;
    using Municipality.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityHasAOfficialLanguage : StreetNameRegistryTest
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
        public void ThenOfficialLanguageWasRemoved(Language language)
        {
            Fixture.Register(() => language);
            var commandLanguageRemoved = Fixture.Create<RemoveOfficialLanguageFromMunicipality>();
            Assert(new Scenario()
                .Given(_streamId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityOfficialLanguageWasAdded>()
                })
                .When(commandLanguageRemoved)
                .Then(new[]
                {
                    new Fact(_streamId, new MunicipalityOfficialLanguageWasRemoved(_municipalityId, language))
                }));
        }

        [Theory]
        [InlineData(Language.Dutch)]
        [InlineData(Language.French)]
        [InlineData(Language.English)]
        [InlineData(Language.German)]
        public void AndWasRemoved_ThenNone(Language language)
        {
            Fixture.Register(() => language);
            var commandLanguageRemoved = Fixture.Create<RemoveOfficialLanguageFromMunicipality>();
            Assert(new Scenario()
                .Given(_streamId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityOfficialLanguageWasAdded>(),
                    Fixture.Create<MunicipalityOfficialLanguageWasRemoved>()
                })
                .When(commandLanguageRemoved)
                .ThenNone());
        }

        [Fact]
        public void AndHasMultipleLanguages_TheCorrectOneWasRemoved()
        {
            var commandLanguageRemoved = Fixture.Create<RemoveOfficialLanguageFromMunicipality>().WithLanguage(Language.English);
            var commandAddedEnglish = Fixture.Create<AddOfficialLanguageToMunicipality>().WithLanguage(Language.English);
            var commandAddedDutch = Fixture.Create<AddOfficialLanguageToMunicipality>().WithLanguage(Language.Dutch);
            Assert(new Scenario()
                .Given(_streamId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    commandAddedEnglish.ToEvent(),
                    commandAddedDutch.ToEvent()
                })
                .When(commandLanguageRemoved)
                .Then(new[]
                {
                    new Fact(_streamId, new MunicipalityOfficialLanguageWasRemoved(_municipalityId, Language.English))
                }));
        }
    }

    public static class OfficialLanguageExtensions
    {
        public static AddOfficialLanguageToMunicipality WithLanguage(this AddOfficialLanguageToMunicipality command, Language language)
        {
            return new AddOfficialLanguageToMunicipality(command.MunicipalityId, language, command.Provenance);
        }

        public static RemoveOfficialLanguageFromMunicipality WithLanguage(this RemoveOfficialLanguageFromMunicipality command, Language language)
        {
            return new RemoveOfficialLanguageFromMunicipality(command.MunicipalityId, language, command.Provenance);
        }

        public static MunicipalityOfficialLanguageWasAdded ToEvent(this AddOfficialLanguageToMunicipality command)
        {
            var eventAdded = new MunicipalityOfficialLanguageWasAdded(command.MunicipalityId, command.Language);
            ((ISetProvenance)eventAdded).SetProvenance(command.Provenance);

            return eventAdded;
        }
    }
}
