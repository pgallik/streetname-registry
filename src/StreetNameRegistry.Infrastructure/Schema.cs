namespace StreetNameRegistry.Infrastructure
{
    public class Schema
    {
        public const string Default = "StreetNameRegistry";
        public const string Import = "StreetNameRegistryImport";
        public const string Extract = "StreetNameRegistryExtract";
        public const string Legacy = "StreetNameRegistryLegacy";
        public const string Syndication = "StreetNameRegistrySyndication";
        public const string Sequence = "StreetNameRegistrySequence";
        public const string Consumer = "StreetNameRegistryConsumer";
        public const string Wfs = "wfs.streetname";
        public const string MigrateStreetName = "StreetNameRegistryMigration";
    }

    public class MigrationTables
    {
        public const string Legacy = "__EFMigrationsHistoryLegacy";
        public const string Extract = "__EFMigrationsHistoryExtract";
        public const string Syndication = "__EFMigrationsHistorySyndication";
        public const string RedisDataMigration = "__EFMigrationsHistoryDataMigration";
        public const string Sequence = "__EFMigrationsHistorySequence";
        public const string Consumer = "__EFMigrationsHistoryConsumer";
        public const string Wfs = "__EFMigrationsHistoryWfsStreetName";
    }
}
