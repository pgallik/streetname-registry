namespace StreetNameRegistry.Tests.AggregateTests.WhenAddingMunicipalityFacilityLanguage
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using global::AutoFixture;
    using StreetName;
    using StreetName.Commands.Municipality;
    using StreetName.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityHasAFacilityLanguage : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;
        private readonly MunicipalityStreamId _streamId;

        public GivenMunicipalityHasAFacilityLanguage(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
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
            var commandLanguageAdded = Fixture.Create<AddFacilityLanguageToMunicipality>();

            Assert(new Scenario()
                .Given(_municipalityId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityFacilityLanguageWasAdded>()
                })
                .When(commandLanguageAdded)
                .ThenNone());
        }

        [Theory]
        [InlineData(Language.Dutch)]
        [InlineData(Language.French)]
        [InlineData(Language.English)]
        [InlineData(Language.German)]
        public void AndWasRemoved_FacilityLanguageWasAdded(Language language)
        {
            Fixture.Register(() => language);
            var commandLanguageAdded = Fixture.Create<AddFacilityLanguageToMunicipality>();

            Assert(new Scenario()
                .Given(_municipalityId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityFacilityLanguageWasAdded>(),
                    Fixture.Create<MunicipalityFacilityLanguageWasRemoved>()
                })
                .When(commandLanguageAdded)
                .Then(new[]
                {
                    new Fact(_municipalityId, new MunicipalityFacilityLanguageWasAdded(_municipalityId, language))
                }));
        }

        [Fact]
        public void AndHasMultipleLanguages_TheCorrectOneWasAdded()
        {
            var languageAddGerman = Fixture.Create<AddFacilityLanguageToMunicipality>().WithLanguage(Language.German);
            var commandAddedEnglish = Fixture.Create<AddFacilityLanguageToMunicipality>().WithLanguage(Language.English);
            var commandAddedDutch = Fixture.Create<AddFacilityLanguageToMunicipality>().WithLanguage(Language.Dutch);
            Assert(new Scenario()
                .Given(_municipalityId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    commandAddedEnglish.ToEvent(),
                    commandAddedDutch.ToEvent()
                })
                .When(languageAddGerman)
                .Then(new[]
                {
                    new Fact(_municipalityId, new MunicipalityFacilityLanguageWasAdded(_municipalityId, Language.German))
                }));
        }
    }
    public static class FacilityLanguageExtensions
    {
        public static AddFacilityLanguageToMunicipality WithLanguage(this AddFacilityLanguageToMunicipality command, Language language)
        {
            return new AddFacilityLanguageToMunicipality(command.MunicipalityId, language, command.Provenance);
        }

        public static RemoveFacilityLanguageFromMunicipality WithLanguage(this RemoveFacilityLanguageFromMunicipality command, Language language)
        {
            return new RemoveFacilityLanguageFromMunicipality(command.MunicipalityId, language, command.Provenance);
        }

        public static MunicipalityFacilityLanguageWasAdded ToEvent(this AddFacilityLanguageToMunicipality command)
        {
            var eventAdded = new MunicipalityFacilityLanguageWasAdded(command.MunicipalityId, command.Language);
            ((ISetProvenance)eventAdded).SetProvenance(command.Provenance);

            return eventAdded;
        }
    }

}
