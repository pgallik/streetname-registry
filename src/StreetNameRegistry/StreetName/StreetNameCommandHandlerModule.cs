namespace StreetNameRegistry.StreetName
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.CommandHandling.SqlStreamStore;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Commands;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
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
                .AddProvenance(getUnitOfWork, provenanceFactory)
                .Handle(async (message, ct) =>
                {
                    var municipality = await getMunicipalities().GetAsync(message.Command.MunicipalityId, ct);
                    municipality.ProposeStreetName(message.Command.StreetNameNames);
                });
        }

    }
}
