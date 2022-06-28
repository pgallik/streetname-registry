namespace StreetNameRegistry.Infrastructure.Repositories
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.SqlStreamStore;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Municipality;
    using SqlStreamStore;

    public class Municipalities : Repository<Municipality, MunicipalityStreamId>, IMunicipalities
    {
        public Municipalities(IMunicipalityFactory municipalityFactory, ConcurrentUnitOfWork unitOfWork, IStreamStore eventStore, EventMapping eventMapping, EventDeserializer eventDeserializer)
            : base(municipalityFactory.Create, unitOfWork, eventStore, eventMapping, eventDeserializer) { }
    }
}
