namespace StreetNameRegistry.Tests.BackOffice.Api.WhenApprovingStreetName
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.CommandHandling.Idempotency;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using FluentAssertions;
    using global::AutoFixture;
    using Infrastructure;
    using Municipality;
    using Municipality.Commands;
    using SqlStreamStore;
    using SqlStreamStore.Streams;
    using StreetNameRegistry.Api.BackOffice.StreetName;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using StreetNameRegistry.Api.BackOffice.Validators;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;
    using Language = Municipality.Language;
    using MunicipalityId = Municipality.MunicipalityId;
    using Names = Municipality.Names;
    using NisCode = Municipality.NisCode;
    using PersistentLocalId = Municipality.PersistentLocalId;
    using StreetNameName = Municipality.StreetNameName;
    using StreetNameStatus = StreetName.StreetNameStatus;

    public class GivenMunicipalityExists : StreetNameRegistryBackOfficeTest
    {
        private readonly TestConsumerContext _consumerContext;
        private readonly TestBackOfficeContext _backOfficeContext;
        private readonly StreetNameController _controller;
        private readonly Fixture _fixture;
        private readonly IdempotencyContext _idempotencyContext;

        public GivenMunicipalityExists(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _fixture = new Fixture();
            _controller = CreateApiBusControllerWithUser<StreetNameController>("John Doe");
            _idempotencyContext = new FakeIdempotencyContextFactory().CreateDbContext(Array.Empty<string>());
            _consumerContext = new FakeConsumerContextFactory().CreateDbContext(Array.Empty<string>());
            _backOfficeContext = new FakeBackOfficeContextFactory().CreateDbContext(Array.Empty<string>());
        }

        [Fact]
        public async Task WhenStreetNameIsProposed_ThenNoContentWithETagResultIsReturned()
        {
            const int expectedLocation = 5;

            var persistentLocalId = new PersistentLocalId(expectedLocation);

            //Arrange
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixtureWithNisCode("23002");
            _backOfficeContext.AddMunicipalityIdByPersistentLocalIdToFixture(persistentLocalId, municipalityLatestItem.MunicipalityId);

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

            var streetNameNames = new Names
            {
                new StreetNameName(Fixture.Create<string>(), Language.Dutch),
                new StreetNameName(Fixture.Create<string>(), Language.French)
            };
            var proposeCommand = new ProposeStreetName(
                municipalityId,
                streetNameNames,
                persistentLocalId,
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(proposeCommand);

            var body = new StreetNameApproveRequest
            {
                PersistentLocalId = persistentLocalId
            };

            //Act
            var result = (NoContentWithETagResult)await _controller.Approve(
                _idempotencyContext,
                _backOfficeContext,
                new StreetNameApproveRequestValidator(),
                Container.Resolve<IMunicipalities>(),
                body,
                null);

            //Assert
            result.ETag.Length.Should().Be(128);
            var stream = await Container.Resolve<IStreamStore>().ReadStreamBackwards(new StreamId(new MunicipalityStreamId(municipalityId)), 4, 1); //4 = version of stream (zero based)
            stream.Messages.First().JsonMetadata.Should().Contain(result.ETag);
        }

        [Fact]
        public async Task WhenStreetNameIsApproved_ThenNoContentWithETagResultIsReturned()
        {
            const int expectedLocation = 5;

            var persistentLocalId = new PersistentLocalId(expectedLocation);

            //Arrange
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixtureWithNisCode("23002");
            _backOfficeContext.AddMunicipalityIdByPersistentLocalIdToFixture(persistentLocalId, municipalityLatestItem.MunicipalityId);

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

            var streetNameNames = new Names
            {
                new StreetNameName(Fixture.Create<string>(), Language.Dutch),
                new StreetNameName(Fixture.Create<string>(), Language.French)
            };
            var proposeCommand = new ProposeStreetName(
                municipalityId,
                streetNameNames,
                persistentLocalId,
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(proposeCommand);

            var approveCommand = new ApproveStreetName(
                municipalityId,
                persistentLocalId,
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(approveCommand);

            var body = new StreetNameApproveRequest
            {
                PersistentLocalId = persistentLocalId
            };

            //Act
            var result = (NoContentWithETagResult)await _controller.Approve(
                _idempotencyContext,
                _backOfficeContext,
                new StreetNameApproveRequestValidator(),
                Container.Resolve<IMunicipalities>(),
                body,
                null);

            //Assert
            result.ETag.Length.Should().Be(128);
            var stream = await Container.Resolve<IStreamStore>().ReadStreamBackwards(new StreamId(new MunicipalityStreamId(municipalityId)), 4, 1); //4 = version of stream (zero based)
            stream.Messages.First().JsonMetadata.Should().Contain(result.ETag);
        }

        [Fact]
        public void WhenStreetNameIsNotFound_ThenBadRequestIsExpected()
        {
            //Arrange
            var persistentLocalId = new PersistentLocalId(1);
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixtureWithNisCode("23002");
            _backOfficeContext.AddMunicipalityIdByPersistentLocalIdToFixture(persistentLocalId, municipalityLatestItem.MunicipalityId);

            var municipalityId = new MunicipalityId(municipalityLatestItem.MunicipalityId);
            var importMunicipality = new ImportMunicipality(
                municipalityId,
                new NisCode("23002"),
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(importMunicipality);

            var body = new StreetNameApproveRequest
            {
                PersistentLocalId = persistentLocalId
            };

            //Act
            Func<Task> act = async () => await _controller.Approve(
                _idempotencyContext,
                _backOfficeContext,
                new StreetNameApproveRequestValidator(),
                Container.Resolve<IMunicipalities>(),
                body,
                null);

            //Assert
            act
                .Should()
                .ThrowAsync<ApiException>()
                .Result
                .Where(x => x.Message.Contains("Onbestaande straatnaam"));
        }

        [Fact]
        public void WhenStreetNameIsRemoved_ThenBadRequestIsExpected()
        {
            //Arrange
            var persistentLocalId = new PersistentLocalId(1);
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixtureWithNisCode("23002");
            _backOfficeContext.AddMunicipalityIdByPersistentLocalIdToFixture(persistentLocalId, municipalityLatestItem.MunicipalityId);

            var municipalityId = new MunicipalityId(municipalityLatestItem.MunicipalityId);
            var importMunicipality = new ImportMunicipality(
                municipalityId,
                new NisCode("23002"),
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(importMunicipality);

            var migrateCommand = new MigrateStreetNameToMunicipality(
                new StreetName.MunicipalityId(municipalityId),
                new StreetName.StreetNameId(Fixture.Create<Guid>()),
                new StreetName.PersistentLocalId(persistentLocalId),
                StreetNameStatus.Current,
                StreetName.Language.Dutch,
                StreetName.Language.French,
                new StreetName.Names(),
                new StreetName.HomonymAdditions(),
                true,
                true,
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(migrateCommand);

            var body = new StreetNameApproveRequest
            {
                PersistentLocalId = persistentLocalId
            };

            //Act
            Func<Task> act = async () => await _controller.Approve(
                _idempotencyContext,
                _backOfficeContext,
                new StreetNameApproveRequestValidator(),
                Container.Resolve<IMunicipalities>(),
                body,
                null);

            //Assert
            act
                .Should()
                .ThrowAsync<ApiException>()
                .Result
                .Where(x => x.Message.Contains("Straatnaam verwijderd"));
        }

        [Fact]
        public void WhenStreetNameIsNotInStatusProposedOrCurrent_ThenBadRequestIsExpected()
        {
            //Arrange
            var persistentLocalId = new PersistentLocalId(1);
            var municipalityLatestItem = _consumerContext.AddMunicipalityLatestItemFixtureWithNisCode("23002");
            _backOfficeContext.AddMunicipalityIdByPersistentLocalIdToFixture(persistentLocalId, municipalityLatestItem.MunicipalityId);

            var municipalityId = new MunicipalityId(municipalityLatestItem.MunicipalityId);
            var importMunicipality = new ImportMunicipality(
                municipalityId,
                new NisCode("23002"),
                _fixture.Create<Provenance>());
            DispatchArrangeCommand(importMunicipality);

            var migrateCommand = new MigrateStreetNameToMunicipality(
                new StreetName.MunicipalityId(municipalityId),
                new StreetName.StreetNameId(Fixture.Create<Guid>()),
                new StreetName.PersistentLocalId(persistentLocalId),
                StreetNameStatus.Retired,
                StreetName.Language.Dutch,
                StreetName.Language.French,
                new StreetName.Names(),
                new StreetName.HomonymAdditions(),
                true,
                isRemoved: false,
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(migrateCommand);

            var body = new StreetNameApproveRequest
            {
                PersistentLocalId = persistentLocalId
            };

            //Act
            Func<Task> act = async () => await _controller.Approve(
                _idempotencyContext,
                _backOfficeContext,
                new StreetNameApproveRequestValidator(),
                Container.Resolve<IMunicipalities>(),
                body,
                null);

            //Assert
            act
                .Should()
                .ThrowAsync<ApiException>()
                .Result
                .Where(x => x.Message.Contains("Straatnaam kan niet meer goedgekeurd worden."));
        }
    }
}
