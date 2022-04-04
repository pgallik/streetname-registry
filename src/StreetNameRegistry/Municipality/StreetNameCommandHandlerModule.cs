namespace StreetNameRegistry.Municipality
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.SqlStreamStore;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Pipes;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Commands;
    using SqlStreamStore;

    public sealed class StreetNameCommandHandlerModule : CommandHandlerModule
    {
        public StreetNameCommandHandlerModule(
            Func<IMunicipalities> getMunicipalities,
            Func<ConcurrentUnitOfWork> getUnitOfWork,
            Func<IStreamStore> getStreamStore,
            EventMapping eventMapping,
            EventSerializer eventSerializer,
            StreetNameProvenanceFactory provenanceFactory)
        {
            For<ProposeStreetName>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddEventHash<ProposeStreetName, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.ProposeStreetName(message.Command.StreetNameNames, message.Command.PersistentLocalId);
                });

            For<ApproveStreetName>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddEventHash<ApproveStreetName, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.ApproveStreetName(message.Command.PersistentLocalId);
                });

            For<MigrateStreetNameToMunicipality>()
                .AddSqlStreamStore(getStreamStore, getUnitOfWork, eventMapping, eventSerializer)
                .AddEventHash<MigrateStreetNameToMunicipality, Municipality>(getUnitOfWork)
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(new MunicipalityStreamId(message.Command.MunicipalityId), ct);
                    municipality.MigrateStreetName(
                        message.Command.StreetNameId,
                        message.Command.PersistentLocalId,
                        message.Command.Status,
                        message.Command.PrimaryLanguage,
                        message.Command.SecondaryLanguage,
                        message.Command.Names,
                        message.Command.HomonymAdditions,
                        message.Command.IsCompleted,
                        message.Command.IsRemoved);
                });
        }
    }
}
