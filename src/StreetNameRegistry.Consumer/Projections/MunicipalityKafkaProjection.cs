namespace StreetNameRegistry.Consumer.Projections
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts.MunicipalityRegistry;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using NodaTime.Text;
    using StreetName.Commands.Municipality;
    using Contracts = Be.Vlaanderen.Basisregisters.GrAr.Contracts.Common;
    using Provenance = Be.Vlaanderen.Basisregisters.GrAr.Provenance.Provenance;

    public class MunicipalityKafkaProjection : ConnectedProjection<CommandHandler>
    {
        private Provenance FromProvenance(Contracts.Provenance provenance) =>
            new Provenance(
                InstantPattern.General.Parse(provenance.Timestamp).GetValueOrThrow(),
                Enum.Parse<Application>(provenance.Application),
                new Reason(provenance.Reason),
                new Operator(provenance.Operator),
                Enum.Parse<Modification>(provenance.Modification),
                Enum.Parse<Organisation>(provenance.Organisation));

        public MunicipalityKafkaProjection()
        {
            When<MunicipalityWasRegistered>(async (commandHandler, message, ct) =>
            {
                var municipalityId = MunicipalityId.CreateFor(message.MunicipalityId);
                var command = new ImportMunicipality(
                    municipalityId,
                    new NisCode(message.NisCode),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityNisCodeWasDefined>(async (commandHandler, message, ct) =>
            {
                var municipalityId = MunicipalityId.CreateFor(message.MunicipalityId);
                var command = new DefineMunicipalityNisCode(
                    municipalityId,
                    new NisCode(message.NisCode),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityNisCodeWasCorrected>(async (commandHandler, message, ct) =>
            {
                var municipalityId = MunicipalityId.CreateFor(message.MunicipalityId);
                var command = new CorrectMunicipalityNisCode(
                    municipalityId,
                    new NisCode(message.NisCode),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityWasNamed>(async (commandHandler, message, ct) =>
            {
                var command = new NameMunicipality(
                    MunicipalityId.CreateFor(message.MunicipalityId),
                    new MunicipalityName(message.Name, Enum.Parse<Language>(message.Language)),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityNameWasCorrected>(async (commandHandler, message, ct) =>
            {
                var command = new CorrectMunicipalityName(
                    MunicipalityId.CreateFor(message.MunicipalityId),
                    new MunicipalityName(message.Name, Enum.Parse<Language>(message.Language)),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityNameWasCorrectedToCleared>(async (commandHandler, message, ct) =>
            {
                var command = new CorrectToClearedMunicipalityName(
                    MunicipalityId.CreateFor(message.MunicipalityId),
                    Enum.Parse<Language>(message.Language),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityOfficialLanguageWasAdded>(async (commandHandler, message, ct) =>
            {
                var command = new AddOfficialLanguageToMunicipality(
                    MunicipalityId.CreateFor(message.MunicipalityId),
                    Enum.Parse<Language>(message.Language),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityOfficialLanguageWasRemoved>(async (commandHandler, message, ct) =>
            {
                var command = new RemoveOfficialLanguageFromMunicipality(
                    MunicipalityId.CreateFor(message.MunicipalityId),
                    Enum.Parse<Language>(message.Language),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityFacilityLanguageWasAdded>(async (commandHandler, message, ct) =>
            {
                var command = new AddFacilityLanguageToMunicipality(
                    MunicipalityId.CreateFor(message.MunicipalityId),
                    Enum.Parse<Language>(message.Language),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityFacilitiesLanguageWasRemoved>(async (commandHandler, message, ct) =>
            {
                var command = new RemoveFacilityLanguageFromMunicipality(
                    MunicipalityId.CreateFor(message.MunicipalityId),
                    Enum.Parse<Language>(message.Language),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityBecameCurrent>(async (commandHandler, message, ct) =>
            {
                var command = new SetMunicipalityToCurrent(
                    MunicipalityId.CreateFor(message.MunicipalityId),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityWasCorrectedToCurrent>(async (commandHandler, message, ct) =>
            {
                var command = new CorrectToCurrentMunicipality(
                    MunicipalityId.CreateFor(message.MunicipalityId),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityWasRetired>(async (commandHandler, message, ct) =>
            {
                var command = new RetireMunicipality(
                    MunicipalityId.CreateFor(message.MunicipalityId),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });

            When<MunicipalityWasCorrectedToRetired>(async (commandHandler, message, ct) =>
            {
                var command = new CorrectToRetiredMunicipality(
                    MunicipalityId.CreateFor(message.MunicipalityId),
                    new RetirementDate(InstantPattern.General.Parse(message.RetirementDate).GetValueOrThrow()),
                    FromProvenance(message.Provenance));

                await commandHandler.Handle(command, ct);
            });
        }
    }
}
