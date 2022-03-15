namespace StreetNameRegistry.Tests.BackOffice.Api.WhenApprovingStreetName
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using FluentAssertions;
    using global::AutoFixture;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Municipality;
    using StreetNameRegistry.Api.BackOffice.StreetName;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using StreetNameRegistry.Api.BackOffice.Validators;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityDoesNotExist : StreetNameRegistryBackOfficeTest
    {
        private readonly Fixture _fixture;
        private readonly StreetNameController _controller;
        private readonly TestBackOfficeContext _backOfficeContext;
        private readonly IdempotencyContext _idempotencyContext;

        public GivenMunicipalityDoesNotExist(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _controller = CreateApiBusControllerWithUser<StreetNameController>("John Doe");
            _idempotencyContext = new FakeIdempotencyContextFactory().CreateDbContext(Array.Empty<string>());
            _backOfficeContext = new FakeBackOfficeContextFactory().CreateDbContext(Array.Empty<string>());
        }

        [Fact]
        public void ThenAggregateIsNotFound()
        {
            //Arrange
            var persistentLocalId = _fixture.Create<int>();
            _ = _backOfficeContext.AddMunicipalityIdByPersistentLocalIdToFixture(persistentLocalId);

            var body = new StreetNameApproveRequest
            {
                PersistentLocalId = persistentLocalId
            };

            //Act
            Func<Task> act = async () => await _controller.Approve(_idempotencyContext, _backOfficeContext, new StreetNameApproveRequestValidator(), Container.Resolve<IMunicipalities>(), body, null);

            //Assert
            act.Should().Throw<AggregateNotFoundException>();
        }

        [Fact]
        public async Task ThenNotFoundResult()
        {
            //Arrange
            var persistentLocalId = _fixture.Create<int>();

            var body = new StreetNameApproveRequest
            {
                PersistentLocalId = persistentLocalId
            };

            //Act
            var result = await _controller.Approve(_idempotencyContext, _backOfficeContext, new StreetNameApproveRequestValidator(), Container.Resolve<IMunicipalities>(), body, null);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
