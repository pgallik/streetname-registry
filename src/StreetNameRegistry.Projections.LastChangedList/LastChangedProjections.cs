namespace StreetNameRegistry.Projections.LastChangedList
{
    using System;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using StreetName.Events;
    using StreetName.Events.Crab;

    [ConnectedProjectionName("Cache markering straatnamen")]
    [ConnectedProjectionDescription("Projectie die markeert voor hoeveel straatnamen de gecachte data nog geüpdated moeten worden.")]
    public class LastChangedProjections : LastChangedListConnectedProjection
    {
        private static readonly AcceptType[] SupportedAcceptTypes = { AcceptType.Json, AcceptType.Xml, AcceptType.JsonLd };

        public LastChangedProjections()
            : base(SupportedAcceptTypes)
        {
            #region Legacy Events
            When<Envelope<StreetNamePersistentLocalIdWasAssigned>>(async (context, message, ct) =>
            {
                var attachedRecords = await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);

                foreach (var record in attachedRecords)
                {
                    record.CacheKey = string.Format(record.CacheKey, message.Message.PersistentLocalId);
                    record.Uri = string.Format(record.Uri, message.Message.PersistentLocalId);
                }
            });

            When<Envelope<StreetNameWasRemoved>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameWasRegistered>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameWasNamed>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameNameWasCorrected>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameNameWasCleared>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameHomonymAdditionWasDefined>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameHomonymAdditionWasCorrected>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameHomonymAdditionWasCleared>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameHomonymAdditionWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameBecameComplete>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameBecameIncomplete>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameBecameCurrent>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameWasProposed>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameWasRetired>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameWasCorrectedToProposed>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameStatusWasRemoved>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameStatusWasCorrectedToRemoved>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNamePrimaryLanguageWasCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<StreetNamePrimaryLanguageWasCorrected>>(async (context, message, ct) => DoNothing());
            When<Envelope<StreetNamePrimaryLanguageWasCorrectedToCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<StreetNamePrimaryLanguageWasDefined>>(async (context, message, ct) => DoNothing());
            When<Envelope<StreetNameSecondaryLanguageWasCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<StreetNameSecondaryLanguageWasCorrected>>(async (context, message, ct) => DoNothing());
            When<Envelope<StreetNameSecondaryLanguageWasCorrectedToCleared>>(async (context, message, ct) => DoNothing());
            When<Envelope<StreetNameSecondaryLanguageWasDefined>>(async (context, message, ct) => DoNothing());

            When<Envelope<StreetNameWasImportedFromCrab>>(async (context, message, ct) => DoNothing());
            When<Envelope<StreetNameStatusWasImportedFromCrab>>(async (context, message, ct) => DoNothing());

            #endregion

            When<Envelope<StreetNameWasMigrated>>(async (context, message, ct) =>
            {
                var attachedRecords = await GetLastChangedRecordsAndUpdatePosition(message.Message.StreetNameId.ToString(), message.Position, context, ct);

                context.LastChangedList.RemoveRange(attachedRecords);
            });

            When<Envelope<StreetNameWasMigratedToMunicipality>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.PersistentLocalId.ToString(), message.Position, context, ct);
            });

            When<Envelope<StreetNameWasProposedV2>>(async (context, message, ct) =>
            {
                await GetLastChangedRecordsAndUpdatePosition(message.Message.PersistentLocalId.ToString(), message.Position, context, ct);
            });
        }

        protected override string BuildCacheKey(AcceptType acceptType, string identifier)
        {
            var shortenedAcceptType = acceptType.ToString().ToLowerInvariant();
            return acceptType switch
            {
                AcceptType.Json => string.Format("legacy/streetname:{{0}}.{1}", identifier, shortenedAcceptType),
                AcceptType.Xml => string.Format("legacy/streetname:{{0}}.{1}", identifier, shortenedAcceptType),
                AcceptType.JsonLd => string.Format("oslo/streetname:{{0}}.{1}", identifier, shortenedAcceptType),
                _ => throw new NotImplementedException($"Cannot build CacheKey for type {typeof(AcceptType)}")
            };
        }

        protected override string BuildUri(AcceptType acceptType, string identifier)
        {
            return acceptType switch
            {
                AcceptType.Json => string.Format("/v1/straatnamen/{{0}}", identifier),
                AcceptType.Xml => string.Format("/v1/straatnamen/{{0}}", identifier),
                AcceptType.JsonLd => string.Format("/v2/straatnamen/{{0}}", identifier),
                _ => throw new NotImplementedException($"Cannot build Uri for type {typeof(AcceptType)}")
            };
        }

        private static void DoNothing() { }
    }
}
