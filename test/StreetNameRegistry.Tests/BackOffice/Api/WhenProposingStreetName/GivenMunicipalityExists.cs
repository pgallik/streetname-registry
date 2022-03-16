namespace StreetNameRegistry.Tests.BackOffice.Api.WhenProposingStreetName
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentAssertions;
    using FluentValidation;
    using Infrastructure;
    using Moq;
    using Municipality;
    using SqlStreamStore;
    using SqlStreamStore.Streams;
    using StreetNameRegistry.Api.BackOffice.StreetName;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using StreetNameRegistry.Api.BackOffice.Validators;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityExists : StreetNameRegistryBackOfficeTest
    {
        private readonly TestConsumerContext _consumerContext;
        private readonly TestBackOfficeContext _backOfficeContext;
        private readonly StreetNameController _controller;
        private readonly IdempotencyContext _idempotencyContext;

        public GivenMunicipalityExists(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _controller = CreateApiBusControllerWithUser<StreetNameController>("John Doe");
            _idempotencyContext = new FakeIdempotencyContextFactory().CreateDbContext(Array.Empty<string>());
            _consumerContext = new FakeConsumerContextFactory().CreateDbContext(Array.Empty<string>());
            _backOfficeContext = new FakeBackOfficeContextFactory().CreateDbContext(Array.Empty<string>());
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
            ImportMunicipality(municipalityId);

            AddOfficialLanguageDutch(municipalityId);
            AddOfficialLanguageFrench(municipalityId);

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
            var result = (CreatedWithLastObservedPositionAsETagResult)await _controller.Propose(
                ResponseOptions,
                _idempotencyContext,
                _consumerContext,
                _backOfficeContext,
                mockPersistentLocalIdGenerator.Object,
                new StreetNameProposeRequestValidator(_consumerContext),
                Container.Resolve<IMunicipalities>(),
                body);

            //Assert
            result.Location.Should().Be(string.Format(DetailUrl, expectedLocation));
            result.LastObservedPositionAsETag.Length.Should().Be(128);

            var stream = await Container.Resolve<IStreamStore>().ReadStreamBackwards(new StreamId(new MunicipalityStreamId(municipalityId)), 3, 1); //3 = version of stream (zero based)
            stream.Messages.First().JsonMetadata.Should().Contain(result.LastObservedPositionAsETag);

            var municipalityIdByPersistentLocalId = await _backOfficeContext.MunicipalityIdByPersistentLocalId.FindAsync(expectedLocation);
            municipalityIdByPersistentLocalId.Should().NotBeNull();
            municipalityIdByPersistentLocalId.MunicipalityId.Should().Be(municipalityLatestItem.MunicipalityId);
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
            Func<Task> act = async () => await _controller.Propose(
                ResponseOptions,
                _idempotencyContext,
                _consumerContext,
                _backOfficeContext,
                mockPersistentLocalIdGenerator.Object,
                new StreetNameProposeRequestValidator(_consumerContext),
                Container.Resolve<IMunicipalities>(),
                body);

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
            Func<Task> act = async () => await _controller.Propose(
                ResponseOptions,
                _idempotencyContext,
                _consumerContext,
                _backOfficeContext,
                mockPersistentLocalIdGenerator.Object,
                new StreetNameProposeRequestValidator(_consumerContext),
                Container.Resolve<IMunicipalities>(),
                body);

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
            Func<Task> act = async () => await _controller.Propose(
                ResponseOptions,
                _idempotencyContext,
                _consumerContext,
                _backOfficeContext,
                mockPersistentLocalIdGenerator.Object,
                new StreetNameProposeRequestValidator(_consumerContext),
                Container.Resolve<IMunicipalities>(), body);

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

            ImportMunicipality(municipalityId);
            AddOfficialLanguageDutch(municipalityId);

            var streetNameName = new StreetNameName("teststraat", Language.Dutch);
            ProposeStreetName(municipalityId, new Names
            {
                streetNameName
            }, persistentLocalId);

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
            Func<Task> act = async () => await _controller.Propose(
                ResponseOptions,
                _idempotencyContext,
                _consumerContext,
                _backOfficeContext,
                mockPersistentLocalIdGenerator.Object,
                new StreetNameProposeRequestValidator(_consumerContext),
                Container.Resolve<IMunicipalities>(),
                body);

            //Assert
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x => x.Message.Contains("Streetname 'teststraat' already exists within the municipality."));
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

            ImportMunicipality(municipalityId);
            RetireMunicipality(municipalityId);

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
            Func<Task> act = async () => await _controller.Propose(
                ResponseOptions,
                _idempotencyContext,
                _consumerContext,
                _backOfficeContext,
                mockPersistentLocalIdGenerator.Object,
                new StreetNameProposeRequestValidator(_consumerContext),
                Container.Resolve<IMunicipalities>(),
                body);

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
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixtureWithNisCode("23002");
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(new PersistentLocalId(1));

            var municipalityId = new MunicipalityId(municipalityLatestItem.MunicipalityId);

            ImportMunicipality(municipalityId);

            var body = new StreetNameProposeRequest
            {
                GemeenteId = $"https://data.vlaanderen.be/id/gemeente/{municipalityLatestItem.NisCode}",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, "teststraat" }
                }
            };

            //Act
            Func<Task> act = async () => await _controller.Propose(
                ResponseOptions,
                _idempotencyContext,
                _consumerContext,
                _backOfficeContext,
                mockPersistentLocalIdGenerator.Object,
                new StreetNameProposeRequestValidator(_consumerContext),
                Container.Resolve<IMunicipalities>(),
                body);

            //Assert
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x => x.Message.Contains("'Straatnamen' can only be in the official or facility language of the municipality."));
        }

        [Fact]
        public async Task WithAMissingLanguage_ThenBadRequestIsExpected()
        {
            //Arrange
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixtureWithNisCode("23002");
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(new PersistentLocalId(1));

            var municipalityId = new MunicipalityId(municipalityLatestItem.MunicipalityId);

            ImportMunicipality(municipalityId);
            AddOfficialLanguageDutch(municipalityId);
            AddFacilityLanguageToMunicipality(municipalityId, Language.English);

            var body = new StreetNameProposeRequest
            {
                GemeenteId = $"https://data.vlaanderen.be/id/gemeente/{municipalityLatestItem.NisCode}",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.EN, "test" }
                }
            };

            //Act
            Func<Task> act = async () => await _controller.Propose(
                ResponseOptions,
                _idempotencyContext,
                _consumerContext,
                _backOfficeContext,
                mockPersistentLocalIdGenerator.Object,
                new StreetNameProposeRequestValidator(_consumerContext),
                Container.Resolve<IMunicipalities>(),
                body);

            //Assert
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x => x.Message.Contains("'Straatnamen' is missing an official or facility language."));
        }
    }
}
