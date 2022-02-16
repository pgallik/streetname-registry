namespace StreetNameRegistry.Tests.AggregateTests.WhenRemovingMunicipalityFacilityLanguage
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
        public void ThenFacilityLanguageWasRemoved(Language language)
        {
            Fixture.Register(() => language);
            var commandLanguageRemoved = Fixture.Create<RemoveFacilityLanguageFromMunicipality>();
            Assert(new Scenario()
                .Given(_streamId, Fixture.Create<MunicipalityWasImported>(), Fixture.Create<MunicipalityFacilityLanguageWasAdded>())
                .When(commandLanguageRemoved)
                .Then(new Fact(_streamId, new MunicipalityFacilityLanguageWasRemoved(_municipalityId, language))));
        }

        [Theory]
        [InlineData(Language.Dutch)]
        [InlineData(Language.French)]
        [InlineData(Language.English)]
        [InlineData(Language.German)]
        public void AndWasRemoved_ThenNone(Language language)
        {
            Fixture.Register(() => language);
            var commandLanguageRemoved = Fixture.Create<RemoveFacilityLanguageFromMunicipality>();
            Assert(new Scenario()
                .Given(_streamId, Fixture.Create<MunicipalityWasImported>(), Fixture.Create<MunicipalityFacilityLanguageWasAdded>(), Fixture.Create<MunicipalityFacilityLanguageWasRemoved>())
                .When(commandLanguageRemoved)
                .ThenNone());
        }

        [Fact]
        public void AndHasMultipleLanguages_TheCorrectOneWasRemoved()
        {
            var commandLanguageRemoved = Fixture.Create<RemoveFacilityLanguageFromMunicipality>().WithLanguage(Language.English);
            var commandAddedEnglish = Fixture.Create<AddFacilityLanguageToMunicipality>().WithLanguage(Language.English);
            var commandAddedDutch = Fixture.Create<AddFacilityLanguageToMunicipality>().WithLanguage(Language.Dutch);
            Assert(new Scenario()
                .Given(_streamId, Fixture.Create<MunicipalityWasImported>(), commandAddedEnglish.ToEvent(), commandAddedDutch.ToEvent())
                .When(commandLanguageRemoved)
                .Then(new Fact(_streamId, new MunicipalityFacilityLanguageWasRemoved(_municipalityId, Language.English))));
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
