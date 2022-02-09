namespace StreetNameRegistry.Tests.BackOffice.Api.WhenProposingStreetName
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using FluentAssertions;
    using FluentValidation;
    using global::AutoFixture;
    using Infrastructure;
    using Moq;
    using StreetName;
    using StreetName.Commands;
    using StreetName.Commands.Municipality;
    using StreetNameRegistry.Api.BackOffice.StreetName;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using StreetNameRegistry.Api.BackOffice.Validators;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityExists : StreetNameRegistryBackOfficeTest
    {
        private readonly TestConsumerContext _consumerContext;
        private readonly StreetNameController _controller;
        private readonly Fixture _fixture;
        private readonly IdempotencyContext _idempotencyContext;

        public GivenMunicipalityExists(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _controller = CreateApiBusControllerWithUser<StreetNameController>("John Doe");
            _idempotencyContext = new FakeIdempotencyContextFactory().CreateDbContext(Array.Empty<string>());
            _consumerContext = new FakeConsumerContextFactory().CreateDbContext(Array.Empty<string>());
        }

        [Fact]
        public async Task ThenTheStreetNameIsProposed()
        {
            const int expectedLocation = 5;

            //Arrange
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixtureWithNisCode("23002");
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(new PersistentLocalId(expectedLocation));

            var municipalityId = new MunicipalityId(municipalityLatestItem.MunicipalityId);
            var importMunicipality = new ImportMunicipality(
                municipalityId,
                new NisCode("23002"),
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(importMunicipality);

            var addOfficialLanguageDutch = new AddOfficialLanguageToMunicipality(
                municipalityId,
                Language.Dutch,
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(addOfficialLanguageDutch);

            var addOfficialLanguageFrench = new AddOfficialLanguageToMunicipality(
                municipalityId,
                Language.French,
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(addOfficialLanguageFrench);

            var body = new StreetNameProposeRequest
            {
                GemeenteId = $"https://data.vlaanderen.be/id/gemeente/{municipalityLatestItem.NisCode}",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, "Rodekruisstraat" },
                    { Taal.FR, "Rue de la Croix-Rouge" }
                }
            };

            //Act
            var result = (CreatedWithLastObservedPositionAsETagResult)await _controller.Propose(ResponseOptions,
                _idempotencyContext, _consumerContext, mockPersistentLocalIdGenerator.Object,
                new StreetNameProposeRequestValidator(_consumerContext), body);

            //Assert
            var expectedPosition = 3;
            result.Location.Should().Be(string.Format(DetailUrl, expectedLocation));
            result.LastObservedPositionAsETag.Should().Be(expectedPosition.ToString());
        }

        [Fact]
        public void WhenStraatnamenIsNull_ThenBadRequestIsExpected()
        {
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(new PersistentLocalId(1));

            var body = new StreetNameProposeRequest
            {
                GemeenteId = "https://data.vlaanderen.be/id/gemeente/11001",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, "" }
                }
            };

            //Act
            Func<Task> act = async () => await _controller.Propose(ResponseOptions, _idempotencyContext,
                _consumerContext, mockPersistentLocalIdGenerator.Object, new StreetNameProposeRequestValidator(_consumerContext), body);

            //Assert
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x => x.Message.Contains("The streetname in 'nl' can not be empty."));
        }

        [Fact]
        public void WhenOneOfStraatnamenIsNull_ThenBadRequestIsExpected()
        {
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(new PersistentLocalId(1));

            var body = new StreetNameProposeRequest
            {
                GemeenteId = "https://data.vlaanderen.be/id/gemeente/11001",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, "" },
                    { Taal.EN, "abc" }
                }
            };

            //Act
            Func<Task> act = async () => await _controller.Propose(ResponseOptions, _idempotencyContext,
                _consumerContext, mockPersistentLocalIdGenerator.Object, new StreetNameProposeRequestValidator(_consumerContext), body);

            //Assert
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x => x.Message.Contains("The streetname in 'nl' can not be empty."));
        }

        [Fact]
        public void WhenOneOfStraatnamenHasExceededMaxLength_ThenBadRequestIsExpected()
        {
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(new PersistentLocalId(1));

            var name = "Boulevard Louis Edelhart Lodewijk van Groothertogdom Luxemburg";
            var body = new StreetNameProposeRequest
            {
                GemeenteId = "https://data.vlaanderen.be/id/gemeente/11001",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, name },
                    { Taal.EN, "abc" }
                }
            };

            //Act
            Func<Task> act = async () => await _controller.Propose(ResponseOptions, _idempotencyContext,
                _consumerContext, mockPersistentLocalIdGenerator.Object, new StreetNameProposeRequestValidator(_consumerContext), body);

            //Assert
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x => x.Message.Contains($"The max length of a streetname in 'nl' is 60 characters. You currently have {name.Length} characters."));
        }

        [Fact]
        public void WithOneOfStraatnamenAlreadyExists_ThenBadRequestIsExpected()
        {
            //Arrange
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixtureWithNisCode("23002");
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            var persistentLocalId = new PersistentLocalId(1);
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(persistentLocalId);

            var municipalityId = new MunicipalityId(municipalityLatestItem.MunicipalityId);

            var importMunicipality = new ImportMunicipality(
                municipalityId,
                new NisCode(municipalityLatestItem.NisCode),
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(importMunicipality);

            var addOfficialLanguage = new AddOfficialLanguageToMunicipality(
                municipalityId,
                Language.Dutch,
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(addOfficialLanguage);

            var streetNameName = new StreetNameName("teststraat", Language.Dutch);
            var proposeStreetName = new ProposeStreetName(
                municipalityId,
                new Names { streetNameName },
                persistentLocalId,
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(proposeStreetName);

            var body = new StreetNameProposeRequest
            {
                GemeenteId = $"https://data.vlaanderen.be/id/gemeente/{municipalityLatestItem.NisCode}",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, streetNameName.Name },
                    { Taal.EN, "abc" }
                }
            };

            //Act
            Func<Task> act = async () => await _controller.Propose(ResponseOptions, _idempotencyContext,
                _consumerContext, mockPersistentLocalIdGenerator.Object, new StreetNameProposeRequestValidator(_consumerContext), body);

            //Assert
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x=> x.Message.Contains("Streetname 'teststraat' already exists within the municipality."));
        }

        [Fact]
        public void WithMunicipalityRetired_ThenBadRequestIsExpected()
        {
            //Arrange
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixtureWithNisCode("23002");
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(new PersistentLocalId(1));

            var municipalityId = new MunicipalityId(municipalityLatestItem.MunicipalityId);

            var importMunicipality = new ImportMunicipality(
                municipalityId,
                new NisCode("23002"),
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(importMunicipality);

            var retireMunicipality = new RetireMunicipality(
                municipalityId,
                Fixture.Create<RetirementDate>(),
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(retireMunicipality);

            var body = new StreetNameProposeRequest
            {
                GemeenteId = $"https://data.vlaanderen.be/id/gemeente/{municipalityLatestItem.NisCode}",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, "teststraat" },
                    { Taal.EN, "abc" }
                }
            };

            //Act
            Func<Task> act = async () => await _controller.Propose(ResponseOptions, _idempotencyContext,
                _consumerContext, mockPersistentLocalIdGenerator.Object, new StreetNameProposeRequestValidator(_consumerContext), body);

            //Assert
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x => x.Message.Contains("This municipality was retired."));
        }

        [Fact]
        public async Task WithNotSupportedLanguage_ThenBadRequestIsExpected()
        {
            //Arrange
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixture();
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(new PersistentLocalId(1));

            var municipalityId = new MunicipalityId(municipalityLatestItem.MunicipalityId);

            var importMunicipality = new ImportMunicipality(
                municipalityId,
                new NisCode(municipalityLatestItem.NisCode),
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(importMunicipality);

            var body = new StreetNameProposeRequest
            {
                GemeenteId = $"https://data.vlaanderen.be/id/gemeente/{municipalityLatestItem.NisCode}",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, "teststraat" },
                    { Taal.EN, "abc" }
                }
            };

            //Act
            Func<Task> act = async () => await _controller.Propose(ResponseOptions, _idempotencyContext,
                _consumerContext, mockPersistentLocalIdGenerator.Object, new StreetNameProposeRequestValidator(_consumerContext), body);

            //Assert
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x => x.Message.Contains("Straatnamen can only be in the official or facility language of the municipality."));
        }
    }
}
