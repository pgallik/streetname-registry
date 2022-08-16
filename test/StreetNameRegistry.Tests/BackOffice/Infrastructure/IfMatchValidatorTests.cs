namespace StreetNameRegistry.Tests.BackOffice.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using FluentAssertions;
    using global::AutoFixture;
    using Municipality;
    using Municipality.Events;
    using Municipality.Exceptions;
    using StreetNameRegistry.Api.BackOffice.Infrastructure;
    using Xunit;
    using Xunit.Abstractions;

    public class IfMatchValidatorTests : BackOfficeTest
    {
        private readonly TestBackOfficeContext _backOfficeContext;

        public IfMatchValidatorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _backOfficeContext = new FakeBackOfficeContextFactory().CreateDbContext(Array.Empty<string>());
        }

        [Fact]
        public async Task IfMatchHeaderValid_ShouldReturnTrue()
        {
            var municipalityId = new MunicipalityId(Guid.NewGuid());
            var streetNamePersistentLocalId = new PersistentLocalId(456);
            var niscode = new NisCode("23002");
            var streetNames = new Names();
            var provenance = Fixture.Create<Provenance>();

            ImportMunicipality(municipalityId, niscode);
            ProposeStreetName(municipalityId, streetNames, streetNamePersistentLocalId, provenance);

            _backOfficeContext.AddMunicipalityIdByPersistentLocalIdToFixture(streetNamePersistentLocalId, municipalityId);

            var lastEvent = new StreetNameWasProposedV2(
                municipalityId,
                niscode,
                streetNames,
                streetNamePersistentLocalId);
            ((ISetProvenance)lastEvent).SetProvenance(provenance);

            var expectedEtag = new ETag(ETagType.Strong, lastEvent.GetHash());

            var sut = new IfMatchHeaderValidator(_backOfficeContext, Container.Resolve<IMunicipalities>());

            // Act
            var result = await sut.IsValid(expectedEtag.ToString(), streetNamePersistentLocalId, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IfMatchHeaderNotValid_ShouldReturnFalse()
        {
            var municipalityId = new MunicipalityId(Guid.NewGuid());
            var streetNamePersistentLocalId = new PersistentLocalId(456);
            var niscode = new NisCode("23002");
            var streetNames = new Names();
            var provenance = Fixture.Create<Provenance>();

            ImportMunicipality(municipalityId, niscode);
            ProposeStreetName(municipalityId, streetNames, streetNamePersistentLocalId, provenance);

            _backOfficeContext.AddMunicipalityIdByPersistentLocalIdToFixture(streetNamePersistentLocalId, municipalityId);

            var sut = new IfMatchHeaderValidator(_backOfficeContext, Container.Resolve<IMunicipalities>());

            // Act
            var result = await sut.IsValid("NON MATCHING ETAG", streetNamePersistentLocalId, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task WhenMunicipalityIdNotFoundForStreetName_ShouldThrowStreetNameNotFoundException()
        {
            var municipalityId = new MunicipalityId(Guid.NewGuid());
            var streetNamePersistentLocalId = new PersistentLocalId(456);
            var niscode = new NisCode("23002");
            var streetNames = new Names();
            var provenance = Fixture.Create<Provenance>();

            ImportMunicipality(municipalityId, niscode);
            ProposeStreetName(municipalityId, streetNames, streetNamePersistentLocalId, provenance);

            var sut = new IfMatchHeaderValidator(_backOfficeContext, Container.Resolve<IMunicipalities>());

            // Act
            Func<Task> act = async() => await sut.IsValid(string.Empty, streetNamePersistentLocalId, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<StreetNameIsNotFoundException>();
        }
    }
}
