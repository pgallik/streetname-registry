namespace StreetNameRegistry.Tests.BackOffice.Api.WhenProposingStreetName
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using FluentAssertions;
    using FluentValidation;
    using global::AutoFixture;
    using Infrastructure;
    using Moq;
    using Municipality;
    using Municipality.Commands;
    using StreetNameRegistry.Api.BackOffice.StreetName;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using StreetNameRegistry.Api.BackOffice.Validators;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityLatestItemDoesNotExist : StreetNameRegistryBackOfficeTest
    {
        private readonly Fixture _fixture;
        private readonly StreetNameController _controller;
        private readonly TestConsumerContext _consumerContext;
        private readonly IdempotencyContext _idempotencyContext;

        public GivenMunicipalityLatestItemDoesNotExist(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _controller = CreateApiBusControllerWithUser<StreetNameController>("John Doe");
            _idempotencyContext = new FakeIdempotencyContextFactory().CreateDbContext(Array.Empty<string>());
            _consumerContext = new FakeConsumerContextFactory().CreateDbContext(Array.Empty<string>());
        }

        [Fact]
        public void ThenResultIsNotFound()
        {
            var mockPersistentLocalIdGenerator = new Mock<IPersistentLocalIdGenerator>();

            //Arrange
            var importMunicipality = new ImportMunicipality(
                new MunicipalityId(Guid.NewGuid()),
                new NisCode("123"),
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(importMunicipality);

            var body = new StreetNameProposeRequest()
            {
                GemeenteId = $"https://data.vlaanderen.be/id/gemeente/{importMunicipality.NisCode}",
                Straatnamen = new Dictionary<Taal, string>()
                {
                    {Taal.NL, "Rodekruisstraat"},
                    {Taal.FR, "Rue de la Croix-Rouge"}
                }
            };

            //Act
            Func<Task> act = async () => await _controller.Propose(
                ResponseOptions,
                _idempotencyContext,
                _consumerContext,
                mockPersistentLocalIdGenerator.Object,
                new StreetNameProposeRequestValidator(_consumerContext),
                Container.Resolve<IMunicipalities>(),
                body);

            // Assert
            act.Should().ThrowAsync<ValidationException>()
                .Result.Where(ex => ex.Message.Contains($"The municipality '{body.GemeenteId}' is not known in the Municipality registry."));
        }
    }
}
