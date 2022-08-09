namespace StreetNameRegistry.Tests.BackOffice
{
    using System;
    using System.Threading;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using global::AutoFixture;
    using Moq;
    using Municipality;
    using Municipality.Commands;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Response;
    using Testing;
    using TicketingService.Abstractions;
    using Xunit.Abstractions;

    public class BackOfficeTest: StreetNameRegistryTest
    {
        public BackOfficeTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

       public void DispatchArrangeCommand<T> (T command) where T : IHasCommandProvenance
        {
            using var scope = Container.BeginLifetimeScope();
            var bus = scope.Resolve<ICommandHandlerResolver>();
            bus.Dispatch(command.CreateCommandId(), command);
        }

        protected string GetStreetNamePuri(int persistentLocalId)
            => $"https://data.vlaanderen.be/id/gemeente/{persistentLocalId}";

        protected void RetireMunicipality(MunicipalityId municipalityId)
        {
            var retireMunicipality = new RetireMunicipality(
                municipalityId,
                Fixture.Create<RetirementDate>(),
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(retireMunicipality);
        }

        protected void ProposeStreetName(
            MunicipalityId municipalityId,
            Names streetNameNames,
            PersistentLocalId persistentLocalId,
            Provenance provenance)
        {
            var proposeCommand = new ProposeStreetName(
                municipalityId,
                streetNameNames,
                persistentLocalId,
                provenance);
            DispatchArrangeCommand(proposeCommand);
        }

        protected Mock<ITicketingUrl> MockTicketingUrl()
        {
            var ticketingUrl = new Mock<ITicketingUrl>();
            ticketingUrl.Setup(x => x.For(It.IsAny<Guid>())).Returns("");
            return ticketingUrl;
        }

        protected Mock<ITicketing> MockTicketing(Action<ETagResponse> callback)
        {
            var ticketing = new Mock<ITicketing>();
            ticketing.Setup(x => x.Complete(It.IsAny<Guid>(), It.IsAny<TicketResult>(), CancellationToken.None))
                .Callback<Guid, TicketResult, CancellationToken>((_, ticketResult, _) =>
                {
                    callback((ETagResponse)ticketResult.Result);
                });

            return ticketing;
        }

        protected void AddOfficialLanguageFrench(MunicipalityId municipalityId)
        {
            var addOfficialLanguageFrench = new AddOfficialLanguageToMunicipality(
                municipalityId,
                Language.French,
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(addOfficialLanguageFrench);
        }

        protected void AddOfficialLanguageDutch(MunicipalityId municipalityId)
        {
            var addOfficialLanguageDutch = new AddOfficialLanguageToMunicipality(
                municipalityId,
                Language.Dutch,
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(addOfficialLanguageDutch);
        }

        protected void ImportMunicipality(MunicipalityId municipalityId, NisCode niscode)
        {
            var importMunicipality = new ImportMunicipality(
                municipalityId,
                niscode,
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(importMunicipality);
        }

        protected void ApproveStreetName(MunicipalityId municipalityId, PersistentLocalId persistentLocalId)
        {
            var approveCommand = new ApproveStreetName(
                municipalityId,
                persistentLocalId,
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(approveCommand);
        }

        protected void CorrectStreetNameName(
            MunicipalityId municipalityId,
            Names streetNameNames,
            PersistentLocalId persistentLocalId,
            Provenance provenance)
        {
            var correctStreetNameNameCommand = new CorrectStreetNameNames(
                municipalityId,
                persistentLocalId,
                streetNameNames, provenance);
            DispatchArrangeCommand(correctStreetNameNameCommand);
        }

        protected void SetMunicipalityToCurrent(MunicipalityId municipalityId)
        {
            var setMunicipalityToCurrent = new SetMunicipalityToCurrent(
                municipalityId,
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(setMunicipalityToCurrent);
        }

        protected void AddFacilityLanguageToMunicipality(MunicipalityId municipalityId, Language language)
        {
            var addFacilityLanguageToMunicipality = new AddFacilityLanguageToMunicipality(
                municipalityId,
                language,
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(addFacilityLanguageToMunicipality);
        }
    }
}
