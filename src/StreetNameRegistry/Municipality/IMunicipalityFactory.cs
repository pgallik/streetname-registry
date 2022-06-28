namespace StreetNameRegistry.Municipality
{
    using Be.Vlaanderen.Basisregisters.AggregateSource.Snapshotting;

    public interface IMunicipalityFactory
    {
        public Municipality Create();
    }

    public class MunicipalityFactory : IMunicipalityFactory
    {
        private readonly ISnapshotStrategy _snapshotStrategy;

        public MunicipalityFactory(ISnapshotStrategy snapshotStrategy)
        {
            _snapshotStrategy = snapshotStrategy;
        }

        public Municipality Create()
        {
            return new Municipality(_snapshotStrategy);
        }
    }
}
