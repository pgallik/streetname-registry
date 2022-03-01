namespace StreetNameRegistry.Tests.BackOffice.Api.WhenProposingStreetName
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentAssertions;
    using global::AutoFixture;
    using Infrastructure;
    using Moq;
    using Municipality;
    using StreetNameRegistry.Api.BackOffice.StreetName;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using StreetNameRegistry.Api.BackOffice.Validators;
    using StreetNameRegistry.Infrastructure.Repositories;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityDoesNotExist : StreetNameRegistryBackOfficeTest
    {
        private readonly Fixture _fixture;
        private readonly StreetNameController _controller;
        private readonly TestConsumerContext _consumerContext;
        private readonly IdempotencyContext _idempotencyContext;

        public GivenMunicipalityDoesNotExist(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _controller = CreateApiBusControllerWithUser<StreetNameController>("John Doe");
            _idempotencyContext = new FakeIdempotencyContextFactory().CreateDbContext(Array.Empty<string>());
            _consumerContext = new FakeConsumerContextFactory().CreateDbContext(Array.Empty<string>());
        }

        [Fact]
        public void ThenAggregateIsNotFound()
        {
            //Arrange
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixtureWithNisCode("23002");
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();
            mockPersistentLocalIdGenerator
                .Setup(x => x.GenerateNextPersistentLocalId())
                .Returns(new PersistentLocalId(5));

            var body = new StreetNameProposeRequest
            {
                GemeenteId = $"https://data.vlaanderen.be/id/gemeente/{municipalityLatestItem.NisCode}",
                Straatnamen = new Dictionary<Taal, string>
                {
                    {Taal.NL, "Rodekruisstraat"},
                    {Taal.FR, "Rue de la Croix-Rouge"}
                }
            };

            //Act
            Func<Task> act = async () => await _controller.Propose(ResponseOptions, _idempotencyContext, _consumerContext, mockPersistentLocalIdGenerator.Object, new StreetNameProposeRequestValidator(_consumerContext), Container.Resolve<IMunicipalities>(), body);

            //Assert
            act.Should().Throw<AggregateNotFoundException>();
        }
    }
}
