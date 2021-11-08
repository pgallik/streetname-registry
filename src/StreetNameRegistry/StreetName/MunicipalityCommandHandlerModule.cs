namespace StreetNameRegistry.StreetName
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
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetOptionalAsync(message.Command.MunicipalityId, ct);

                    if (municipality.HasValue)
                        throw new AggregateSourceException($"Municipality with id {message.Command.MunicipalityId} already exists");

                    var newMunicipality = Municipality.Register(message.Command.MunicipalityId, message.Command.NisCode);

                    getMunicipalities().Add(newMunicipality.MunicipalityId, newMunicipality);
                });

            For<ChangeMunicipalityNisCode>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(message.Command.MunicipalityId, ct);
                    municipality.ChangeNisCode(message.Command.NisCode);
                });

            For<SetMunicipalityToCurrent>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(message.Command.MunicipalityId, ct);
                    municipality.BecomeCurrent();
                });

            For<RetireMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(message.Command.MunicipalityId, ct);
                    municipality.Retire();
                });
        }
    }
}
