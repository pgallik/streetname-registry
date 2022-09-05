namespace StreetNameRegistry.Tests.BackOffice.Lambda.WhenRejectingStreetName
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
    using Moq;
    using Municipality;
    using Municipality.Exceptions;
    using SqlStreamStore;
    using SqlStreamStore.Streams;
    using StreetNameRegistry.Api.BackOffice.Abstractions;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Requests;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Response;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda.Handlers;
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
        public async Task ThenStreetNameWasRejected()
        {
            var municipalityId = new MunicipalityId(Guid.NewGuid());
            var streetNamePersistentLocalId = new PersistentLocalId(456);
            var provenance = Fixture.Create<Provenance>();

            ImportMunicipality(municipalityId, new NisCode("23002"));
            SetMunicipalityToCurrent(municipalityId);
            AddOfficialLanguageDutch(municipalityId);
            ProposeStreetName(
                municipalityId,
                new Names(new Dictionary<Language, string>{{Language.Dutch, "Bremt"}}),
                streetNamePersistentLocalId,
                provenance);

            await _backOfficeContext.MunicipalityIdByPersistentLocalId.AddAsync(
                new MunicipalityIdByPersistentLocalId(streetNamePersistentLocalId, municipalityId));
            _backOfficeContext.SaveChanges();

            ETagResponse? etag = null;

            var handler = new SqsStreetNameRejectHandler(
                MockTicketing(result =>
                {
                    etag = result;
                }).Object,
                Container.Resolve<IMunicipalities>(),
                new IdempotentCommandHandler(Container.Resolve<ICommandHandlerResolver>(), _idempotencyContext));

            //Act
            await handler.Handle(new SqsLambdaStreetNameRejectRequest
            {
                Request = new StreetNameBackOfficeRejectRequest { PersistentLocalId = streetNamePersistentLocalId },
                MessageGroupId = municipalityId
            }, CancellationToken.None);

            //Assert
            var stream = await Container.Resolve<IStreamStore>().ReadStreamBackwards(new StreamId(new MunicipalityStreamId(municipalityId)), 4, 1); //3 = version of stream (zero based)
            stream.Messages.First().JsonMetadata.Should().Contain(etag.LastEventHash);
        }

        [Fact]
        public async Task WhenStreetNameHasInvalidStatus_ThenTicketingErrorIsExpected()
        {
            var ticketing = new Mock<ITicketing>();

            var sut = new SqsStreetNameRejectHandler(
                ticketing.Object,
                Mock.Of<IMunicipalities>(),
                MockExceptionIdempotentCommandHandler<StreetNameHasInvalidStatusException>().Object);

            // Act
            await sut.Handle(new SqsLambdaStreetNameRejectRequest
            {
                Request = new StreetNameBackOfficeRejectRequest(),
                MessageGroupId = Guid.NewGuid().ToString(),
                TicketId = Guid.NewGuid()
            }, CancellationToken.None);

            //Assert
            ticketing.Verify(x =>
                x.Error(It.IsAny<Guid>(),
                    new TicketError("Deze actie is enkel toegestaan op straatnamen met status 'voorgesteld'.", "StraatnaamInGebruikOfGehistoreerd"), CancellationToken.None));
        }

        [Fact]
        public async Task WhenMunicipalityHasInvalidStatus_ThenTicketingErrorIsExpected()
        {
            var ticketing = new Mock<ITicketing>();

            var sut = new SqsStreetNameRejectHandler(
                ticketing.Object,
                Mock.Of<IMunicipalities>(),
                MockExceptionIdempotentCommandHandler<MunicipalityHasInvalidStatusException>().Object);

            // Act
            await sut.Handle(new SqsLambdaStreetNameRejectRequest
            {
                Request = new StreetNameBackOfficeRejectRequest(),
                MessageGroupId = Guid.NewGuid().ToString(),
                TicketId = Guid.NewGuid()
            }, CancellationToken.None);

            //Assert
            ticketing.Verify(x =>
                x.Error(It.IsAny<Guid>(),
                    new TicketError("Deze actie is enkel toegestaan binnen gemeenten met status 'inGebruik'.", "StraatnaamGemeenteInGebruik"), CancellationToken.None));
        }
    }
}
