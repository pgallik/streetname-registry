namespace StreetNameRegistry.Projections.Wfs.StreetNameHelperV2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;

    public static class StreetNameHelperV2Extensions
    {
        public static async Task<StreetNameHelperV2> FindAndUpdateStreetNameHelper(
            this WfsContext context,
            int persistentLocalId,
            Action<StreetNameHelperV2> updateFunc,
            CancellationToken ct)
        {
            var streetName = await context
                .StreetNameHelperV2
                .FindAsync(persistentLocalId, cancellationToken: ct);

            if (streetName == null)
                throw DatabaseItemNotFound(persistentLocalId);

            updateFunc(streetName);

            return streetName;
        }

        private static ProjectionItemNotFoundException<StreetNameHelperV2Projections> DatabaseItemNotFound(int persistentLocalId)
            => new ProjectionItemNotFoundException<StreetNameHelperV2Projections>(persistentLocalId.ToString());
    }
}
