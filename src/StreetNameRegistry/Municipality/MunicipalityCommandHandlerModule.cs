namespace StreetNameRegistry.Municipality
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.SqlStreamStore;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Commands;
    using SqlStreamStore;

    public sealed class MunicipalityCommandHandlerModule : CommandHandlerModule
    {
        public MunicipalityCommandHandlerModule(
            Func<IMunicipalities> getMunicipalities,
            Func<ConcurrentUnitOfWork> getUnitOfWork,
            Func<IStreamStore> getStreamStore,
            EventMapping eventMapping,
            EventSerializer eventSerializer,
            StreetNameProvenanceFactory provenanceFactory)
        {
            For<ImportMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<ImportMunicipality, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipalityStreamId = new MunicipalityStreamId(message.Command.MunicipalityId);
                    var municipality = await getMunicipalities().GetOptionalAsync(municipalityStreamId, ct);

                    if (municipality.HasValue)
                    {
                        throw new AggregateSourceException($"Municipality with id {message.Command.MunicipalityId} already exists");
                    }

                    var newMunicipality = Municipality.Register(message.Command.MunicipalityId, message.Command.NisCode);

                    getMunicipalities().Add(municipalityStreamId, newMunicipality);
                });

            For<ChangeMunicipalityNisCode>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<ChangeMunicipalityNisCode, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.DefineOrChangeNisCode(message.Command.NisCode);
                });

            For<SetMunicipalityToCurrent>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<SetMunicipalityToCurrent, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.BecomeCurrent();
                });

            For<RetireMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<RetireMunicipality, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.Retire();
                });

            For<DefineMunicipalityNisCode>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<DefineMunicipalityNisCode, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.DefineOrChangeNisCode(message.Command.NisCode);
                });

            For<CorrectMunicipalityNisCode>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<CorrectMunicipalityNisCode, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.DefineOrChangeNisCode(message.Command.NisCode);
                });

            For<NameMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<NameMunicipality, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.NameMunicipality(message.Command.Name);
                });

            For<CorrectMunicipalityName>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<CorrectMunicipalityName, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.NameMunicipality(message.Command.Name);
                });

            For<CorrectToClearedMunicipalityName>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<CorrectToClearedMunicipalityName, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.NameMunicipality(new MunicipalityName(string.Empty, message.Command.Language));
                });

            For<AddOfficialLanguageToMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<AddOfficialLanguageToMunicipality, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.AddOfficialLanguage(message.Command.Language);
                });

            For<RemoveOfficialLanguageFromMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<RemoveOfficialLanguageFromMunicipality, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.RemoveOfficialLanguage(message.Command.Language);
                });

            For<AddFacilityLanguageToMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<AddFacilityLanguageToMunicipality, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.AddFacilityLanguage(message.Command.Language);
                });

            For<RemoveFacilityLanguageFromMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<RemoveFacilityLanguageFromMunicipality, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.RemoveFacilityLanguage(message.Command.Language);
                });

            For<CorrectToCurrentMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<CorrectToCurrentMunicipality, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.CorrectToCurrent();
                });

            For<CorrectToRetiredMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddHash<CorrectToRetiredMunicipality, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.CorrectToRetired();
                });
        }
    }
}
