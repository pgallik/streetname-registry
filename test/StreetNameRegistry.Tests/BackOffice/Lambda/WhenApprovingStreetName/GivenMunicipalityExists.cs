namespace StreetNameRegistry.Tests.BackOffice.Lambda.WhenApprovingStreetName
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using FluentAssertions;
    using global::AutoFixture;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using Municipality;
    using Municipality.Exceptions;
    using SqlStreamStore;
    using SqlStreamStore.Streams;
    using StreetNameRegistry.Api.BackOffice.Abstractions;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Exceptions;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Requests;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Response;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Handlers;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Requests;
    using TicketingService.Abstractions;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityExists : BackOfficeLambdaTest
    {
        private readonly BackOfficeContext _backOfficeContext;
        private readonly IdempotencyContext _idempotencyContext;

        public GivenMunicipalityExists(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _idempotencyContext = new FakeIdempotencyContextFactory().CreateDbContext(Array.Empty<string>());
            _backOfficeContext = new FakeBackOfficeContextFactory().CreateDbContext(Array.Empty<string>());
        }

        [Fact]
        public async Task ThenStreetNameWasApproved()
        {
            // Arrange
            var municipalityId = new MunicipalityId(Guid.NewGuid());
            var streetNamePersistentLocalId = new PersistentLocalId(456);
            var provenance = Fixture.Create<Provenance>();

            ImportMunicipality(municipalityId, new NisCode("23002"));
            SetMunicipalityToCurrent(municipalityId);
            AddOfficialLanguageDutch(municipalityId);
            ProposeStreetName(
                municipalityId,
                new Names(new Dictionary<Language, string> { { Language.Dutch, "Bremt" } }),
                streetNamePersistentLocalId,
                provenance);

            await _backOfficeContext.MunicipalityIdByPersistentLocalId.AddAsync(
                new MunicipalityIdByPersistentLocalId(streetNamePersistentLocalId, municipalityId));
            await _backOfficeContext.SaveChangesAsync();

            var etag = new ETagResponse(string.Empty, Fixture.Create<string>());
            var handler = new SqsStreetNameApproveLambdaHandler(
                Container.Resolve<IConfiguration>(),
                new FakeRetryPolicy(),
                MockTicketing(result => { etag = result; }).Object,
                Container.Resolve<IMunicipalities>(),
                new IdempotentCommandHandler(Container.Resolve<ICommandHandlerResolver>(), _idempotencyContext));

            //Act
            await handler.Handle(new SqsLambdaStreetNameApproveRequest
            {
                Request = new StreetNameBackOfficeApproveRequest { PersistentLocalId = streetNamePersistentLocalId },
                MessageGroupId = municipalityId,
                TicketId = Guid.NewGuid(),
                Metadata = new Dictionary<string, object>(),
                Provenance = Fixture.Create<Provenance>()
            }, CancellationToken.None);

            //Assert
            var stream = await Container.Resolve<IStreamStore>()
                .ReadStreamBackwards(new StreamId(new MunicipalityStreamId(municipalityId)), 4, 1);
            stream.Messages.First().JsonMetadata.Should().Contain(etag.ETag);
        }

        [Fact]
        public async Task WhenStreetNameHasInvalidStatus_ThenTicketingErrorIsExpected()
        {
            // Arrange
            var ticketing = new Mock<ITicketing>();

            var sut = new SqsStreetNameApproveLambdaHandler(
                Container.Resolve<IConfiguration>(),
                new FakeRetryPolicy(),
                ticketing.Object,
                Mock.Of<IMunicipalities>(),
                MockExceptionIdempotentCommandHandler<StreetNameHasInvalidStatusException>().Object);

            // Act
            await sut.Handle(new SqsLambdaStreetNameApproveRequest
            {
                Request = new StreetNameBackOfficeApproveRequest(),
                MessageGroupId = Guid.NewGuid().ToString(),
                TicketId = Guid.NewGuid(),
                Metadata = new Dictionary<string, object>(),
                Provenance = Fixture.Create<Provenance>()
            }, CancellationToken.None);

            //Assert
            ticketing.Verify(x =>
                x.Error(
                    It.IsAny<Guid>(),
                    new TicketError(
                        "Deze actie is enkel toegestaan op straatnamen met status 'voorgesteld'.",
                        "StraatnaamGehistoreerdOfAfgekeurd"),
                    CancellationToken.None));
        }

        [Fact]
        public async Task WhenMunicipalityHasInvalidStatus_ThenTicketingErrorIsExpected()
        {
            // Arrange
            var ticketing = new Mock<ITicketing>();

            var sut = new SqsStreetNameApproveLambdaHandler(
                Container.Resolve<IConfiguration>(),
                new FakeRetryPolicy(),
                ticketing.Object,
                Mock.Of<IMunicipalities>(),
                MockExceptionIdempotentCommandHandler<MunicipalityHasInvalidStatusException>().Object);

            // Act
            await sut.Handle(new SqsLambdaStreetNameApproveRequest
            {
                Request = new StreetNameBackOfficeApproveRequest(),
                MessageGroupId = Guid.NewGuid().ToString(),
                TicketId = Guid.NewGuid(),
                Metadata = new Dictionary<string, object>(),
                Provenance = Fixture.Create<Provenance>()
            }, CancellationToken.None);

            //Assert
            ticketing.Verify(x =>
                x.Error(
                    It.IsAny<Guid>(),
                    new TicketError(
                        "Deze actie is enkel toegestaan binnen gemeenten met status 'inGebruik'.",
                        "StraatnaamGemeenteInGebruik"),
                    CancellationToken.None));
        }

        [Fact]
        public async Task WhenIdempotencyException_ThenTicketingCompleteIsExpected()
        {
            // Arrange
            var ticketing = new Mock<ITicketing>();
            var municipalityId = new MunicipalityId(Guid.NewGuid());
            var streetNamePersistentLocalId = new PersistentLocalId(456);
            var provenance = Fixture.Create<Provenance>();

            ImportMunicipality(municipalityId, new NisCode("23002"));
            SetMunicipalityToCurrent(municipalityId);
            AddOfficialLanguageDutch(municipalityId);
            ProposeStreetName(
                municipalityId,
                new Names(new Dictionary<Language, string> { { Language.Dutch, "Bremt" } }),
                streetNamePersistentLocalId,
                provenance);

            var municipalities = Container.Resolve<IMunicipalities>();
            var sut = new SqsStreetNameApproveLambdaHandler(
                Container.Resolve<IConfiguration>(),
                new FakeRetryPolicy(),
                ticketing.Object,
                municipalities,
                MockExceptionIdempotentCommandHandler(() => new IdempotencyException(string.Empty)).Object);

            var municipality =
                await municipalities.GetAsync(new MunicipalityStreamId(municipalityId), CancellationToken.None);

            // Act
            await sut.Handle(new SqsLambdaStreetNameApproveRequest
            {
                Request = new StreetNameBackOfficeApproveRequest
                {
                    PersistentLocalId = streetNamePersistentLocalId
                },
                MessageGroupId = municipalityId.ToString(),
                TicketId = Guid.NewGuid(),
                Metadata = new Dictionary<string, object>(),
                Provenance = Fixture.Create<Provenance>()
            }, CancellationToken.None);

            //Assert
            ticketing.Verify(x =>
                x.Complete(
                    It.IsAny<Guid>(),
                    new TicketResult(
                        new ETagResponse(
                            string.Format(ConfigDetailUrl, streetNamePersistentLocalId),
                            municipality.GetStreetNameHash(streetNamePersistentLocalId))),
                    CancellationToken.None));
        }
    }
}
