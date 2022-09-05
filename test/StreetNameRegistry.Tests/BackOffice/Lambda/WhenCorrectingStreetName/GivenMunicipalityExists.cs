namespace StreetNameRegistry.Tests.BackOffice.Lambda.WhenCorrectingStreetName
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using FluentAssertions;
    using global::AutoFixture;
    using Moq;
    using SqlStreamStore;
    using SqlStreamStore.Streams;
    using StreetNameRegistry.Api.BackOffice.Abstractions;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Requests;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Response;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Handlers;
    using Municipality;
    using Municipality.Exceptions;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests;
    using TicketingService.Abstractions;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityExists : BackOfficeTest
    {
        private readonly BackOfficeContext _backOfficeContext;
        private readonly IdempotencyContext _idempotencyContext;

        public GivenMunicipalityExists(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _idempotencyContext = new FakeIdempotencyContextFactory().CreateDbContext(Array.Empty<string>());
            _backOfficeContext = new FakeBackOfficeContextFactory().CreateDbContext(Array.Empty<string>());
        }

        [Fact]
        public async Task ThenStreetNameNamesWereCorrected()
        {
            var municipalityId = new MunicipalityId(Guid.NewGuid());
            var streetNamePersistentLocalId = new PersistentLocalId(456);
            var provenance = Fixture.Create<Provenance>();

            ImportMunicipality(municipalityId, new NisCode("23002"));
            SetMunicipalityToCurrent(municipalityId);
            AddOfficialLanguageDutch(municipalityId);
            AddOfficialLanguageFrench(municipalityId);
            ProposeStreetName(
                municipalityId,
                new Names(new Dictionary<Language, string>{{Language.Dutch, "Bremt"}, { Language.French, "Rue de la Croix-Rouge" } }),
                streetNamePersistentLocalId,
                provenance);

            await _backOfficeContext.MunicipalityIdByPersistentLocalId.AddAsync(
                new MunicipalityIdByPersistentLocalId(streetNamePersistentLocalId, municipalityId));
            _backOfficeContext.SaveChanges();

            ETagResponse? etag = null;

            var handler = new SqsStreetNameCorrectNamesHandler(
                MockTicketing(result =>
                {
                    etag = result;
                }).Object,
                Container.Resolve<IMunicipalities>(),
                new IdempotentCommandHandler(Container.Resolve<ICommandHandlerResolver>(), _idempotencyContext));

            //Act
            await handler.Handle(new SqsLambdaStreetNameCorrectNamesRequest()
            {
                Request = new StreetNameBackOfficeCorrectNamesRequest
                {
                    PersistentLocalId = streetNamePersistentLocalId,
                    Straatnamen = new Dictionary<Taal, string>
                    {
                        { Taal.NL, "Rodekruisstraat" },
                        { Taal.FR, "Rue de la Croix-Rouge" }
                    }
                },
                MessageGroupId = municipalityId
            }, CancellationToken.None);

            //Assert
            var stream = await Container.Resolve<IStreamStore>().ReadStreamBackwards(new StreamId(new MunicipalityStreamId(municipalityId)), 5, 1); //3 = version of stream (zero based)
            stream.Messages.First().JsonMetadata.Should().Contain(etag.LastEventHash);
        }

        [Fact]
        public async Task WhenStreetNameNameAlreadyExistsException_ThenTicketingErrorIsExpected()
        {
            var ticketing = new Mock<ITicketing>();

            var streetname = "Bremt";

            var sut = new SqsStreetNameCorrectNamesHandler(
                ticketing.Object,
                Mock.Of<IMunicipalities>(),
                MockExceptionIdempotentCommandHandler(() => new StreetNameNameAlreadyExistsException(streetname)).Object);

            // Act
            await sut.Handle(new SqsLambdaStreetNameCorrectNamesRequest
            {
                Request = new StreetNameBackOfficeCorrectNamesRequest{ Straatnamen = new Dictionary<Taal, string>() },
                MessageGroupId = Guid.NewGuid().ToString(),
                TicketId = Guid.NewGuid()
            }, CancellationToken.None);

            //Assert
            ticketing.Verify(x =>
                x.Error(It.IsAny<Guid>(),
                    new TicketError($"Straatnaam '{streetname}' bestaat reeds in de gemeente.", "StraatnaamBestaatReedsInGemeente"), CancellationToken.None));
        }

        [Fact]
        public async Task WhenStreetNameHasInvalidStatusException_ThenTicketingErrorIsExpected()
        {
            var ticketing = new Mock<ITicketing>();

            var sut = new SqsStreetNameCorrectNamesHandler(
                ticketing.Object,
                Mock.Of<IMunicipalities>(),
                MockExceptionIdempotentCommandHandler<StreetNameHasInvalidStatusException>().Object);

            // Act
            await sut.Handle(new SqsLambdaStreetNameCorrectNamesRequest
            {
                Request = new StreetNameBackOfficeCorrectNamesRequest { Straatnamen = new Dictionary<Taal, string>() },
                MessageGroupId = Guid.NewGuid().ToString(),
                TicketId = Guid.NewGuid()
            }, CancellationToken.None);

            //Assert
            ticketing.Verify(x =>
                x.Error(It.IsAny<Guid>(),
                    new TicketError("Deze actie is enkel toegestaan op straatnamen met status 'voorgesteld' of 'inGebruik'.", "StraatnaamGehistoreerdOfAfgekeurd"), CancellationToken.None));
        }

        [Fact]
        public async Task WhenStreetNameNameLanguageIsNotSupportedException_ThenTicketingErrorIsExpected()
        {
            var ticketing = new Mock<ITicketing>();

            var sut = new SqsStreetNameCorrectNamesHandler(
                ticketing.Object,
                Mock.Of<IMunicipalities>(),
                MockExceptionIdempotentCommandHandler<StreetNameNameLanguageIsNotSupportedException>().Object);

            // Act
            await sut.Handle(new SqsLambdaStreetNameCorrectNamesRequest
            {
                Request = new StreetNameBackOfficeCorrectNamesRequest { Straatnamen = new Dictionary<Taal, string>() },
                MessageGroupId = Guid.NewGuid().ToString(),
                TicketId = Guid.NewGuid()
            }, CancellationToken.None);

            //Assert
            ticketing.Verify(x =>
                x.Error(It.IsAny<Guid>(),
                    new TicketError(
                        "'Straatnamen' kunnen enkel voorkomen in de officiële of faciliteitentaal van de gemeente.",
                        "StraatnaamTaalNietInOfficieleOfFaciliteitenTaal"), CancellationToken.None));
        }
    }
}
