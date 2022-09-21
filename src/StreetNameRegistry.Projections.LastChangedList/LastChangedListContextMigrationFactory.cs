namespace StreetNameRegistry.Projections.LastChangedList
{
    public sealed class LastChangedListContextMigrationFactory
        : Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList.LastChangedListContextMigrationFactory
    {
        public LastChangedListContextMigrationFactory()
            : base("LastChangedListAdmin")
        { }
    }
}
