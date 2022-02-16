namespace StreetNameRegistry.Infrastructure.Repositories
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.SqlStreamStore;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using SqlStreamStore;
    using StreetName;

    public class Municipalities : Repository<Municipality, MunicipalityStreamId>, IMunicipalities
    {
        public Municipalities(ConcurrentUnitOfWork unitOfWork, IStreamStore eventStore, EventMapping eventMapping, EventDeserializer eventDeserializer)
            : base(Municipality.Factory, unitOfWork, eventStore, eventMapping, eventDeserializer) { }
    }
}
