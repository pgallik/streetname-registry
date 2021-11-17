namespace StreetNameRegistry.Tests.BackOffice.Api.WhenProposingStreetName
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using FluentAssertions;
    using global::AutoFixture;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using StreetName.Commands;
    using StreetNameRegistry.Api.BackOffice.StreetName;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using StreetName;
    using Testing;
    using Xunit.Abstractions;

    public class GivenMunicipalityLatestItemDoesNotExist : StreetNameRegistryBackOfficeTest
    {
        private readonly Fixture _fixture;
        private readonly StreetNameController _controller;
        private readonly TestSyndicationContext _syndicationContext;
        private readonly IdempotencyContext _idempotencyContext;

        public GivenMunicipalityLatestItemDoesNotExist(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _controller = CreateApiBusControllerWithUser<StreetNameController>("John Doe");
            _idempotencyContext = new FakeIdempotencyContextFactory().CreateDbContext(Array.Empty<string>());
            _syndicationContext = new FakeSyndicationContextFactory().CreateDbContext(Array.Empty<string>());
        }

        //TODO: change with validation story
        //[Fact]
        public async Task ThenResultIsNotFound()
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
                GemeenteId = $"https://data.vlaanderen.be/id/gemeente/{123}",
                Straatnamen = new Dictionary<Taal, string>()
                {
                    {Taal.NL, "Rodekruisstraat"},
                    {Taal.FR, "Rue de la Croix-Rouge"}
                }
            };

            //Act
            var result = await _controller.Propose(ResponseOptions, _idempotencyContext, _syndicationContext, mockPersistentLocalIdGenerator.Object, body);
            result.Should().BeOfType<NotFoundResult>();

        }
    }
}
