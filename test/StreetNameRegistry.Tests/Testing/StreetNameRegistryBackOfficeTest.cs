namespace StreetNameRegistry.Tests.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Api.BackOffice.Infrastructure.Options;
    using Api.BackOffice.StreetName;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using global::AutoFixture;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Municipality;
    using Municipality.Commands;
    using Xunit.Abstractions;

    public class StreetNameRegistryBackOfficeTest : StreetNameRegistryTest
    {
        internal const string DetailUrl = "https://www.registry.com/streetname/voorgesteld/{0}";
        protected IOptions<ResponseOptions> ResponseOptions { get; }

        public StreetNameRegistryBackOfficeTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            ResponseOptions = Options.Create<ResponseOptions>(Fixture.Create<ResponseOptions>());
            ResponseOptions.Value.DetailUrl = DetailUrl;
        }

        public void DispatchArrangeCommand<T> (T command) where T : IHasCommandProvenance
        {
            using var scope = Container.BeginLifetimeScope();
            var bus = scope.Resolve<ICommandHandlerResolver>();
            bus.Dispatch(command.CreateCommandId(), command);
        }

        public T CreateApiBusControllerWithUser<T>(string username) where T : ApiBusController
        {
            var bus = Container.Resolve<ICommandHandlerResolver>();
            var controller = Activator.CreateInstance(typeof(T), bus) as T;

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.NameIdentifier, "userId"),
                new Claim("name", username),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            if (controller != null)
            {
                controller.ControllerContext.HttpContext = new DefaultHttpContext { User = claimsPrincipal };

                return controller;
            }
            else
            {
                throw new Exception("Could not find controller type");
            }
        }

        protected void RetireMunicipality(MunicipalityId municipalityId)
        {
            var retireMunicipality = new RetireMunicipality(
                municipalityId,
                Fixture.Create<RetirementDate>(),
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(retireMunicipality);
        }

        protected void ProposeStreetName(MunicipalityId municipalityId, Names streetNameNames,
            PersistentLocalId persistentLocalId)
        {
            var proposeCommand = new ProposeStreetName(
                municipalityId,
                streetNameNames,
                persistentLocalId,
                Fixture.Create<Provenance>());
            DispatchArrangeCommand(proposeCommand);
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

        protected void ImportMunicipality(MunicipalityId municipalityId)
        {
            var importMunicipality = new ImportMunicipality(
                municipalityId,
                new NisCode("23002"),
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
