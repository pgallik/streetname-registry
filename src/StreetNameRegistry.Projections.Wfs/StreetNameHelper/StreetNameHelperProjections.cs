namespace StreetNameRegistry.Projections.Wfs.StreetName
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using StreetNameRegistry.StreetName.Events;
    using NodaTime;

    [ConnectedProjectionName("WFS straatnaam")]
    [ConnectedProjectionDescription("Projectie die de straatnaam data voor het WFS straatnaamregister voorziet.")]
    public class StreetNameHelperProjections: ConnectedProjection<WfsContext>
    {

        public StreetNameHelperProjections()
        {
            When<Envelope<StreetNameWasRegistered>>(async (context, message, ct) =>
            {
                await context
                    .StreetNameHelper
                    .AddAsync(
                        new StreetNameHelper
                        {
                            StreetNameId = message.Message.StreetNameId,
                            NisCode = message.Message.NisCode,
                            Version = message.Message.Provenance.Timestamp,
                            Complete = false,
                            Removed = false
                        }, ct);
            });

            When<Envelope<StreetNameWasNamed>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        UpdateNameByLanguage(entity, message.Message.Language, message.Message.Name);
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameNameWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        UpdateNameByLanguage(entity, message.Message.Language, message.Message.Name);
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameNameWasCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        UpdateNameByLanguage(entity, message.Message.Language, string.Empty);
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameNameWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        UpdateNameByLanguage(entity, message.Message.Language, string.Empty);
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameHomonymAdditionWasDefined>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        UpdateHomonymAdditionByLanguage(entity, message.Message.Language, message.Message.HomonymAddition);
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameHomonymAdditionWasCorrected>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        UpdateHomonymAdditionByLanguage(entity, message.Message.Language, message.Message.HomonymAddition);
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameHomonymAdditionWasCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        UpdateHomonymAdditionByLanguage(entity, message.Message.Language, string.Empty);
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameHomonymAdditionWasCorrectedToCleared>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        UpdateHomonymAdditionByLanguage(entity, message.Message.Language, string.Empty);
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameBecameComplete>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        entity.Complete = true;
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameBecameIncomplete>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        entity.Complete = false;
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        entity.Removed = true;
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNamePersistentLocalIdWasAssigned>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        entity.PersistentLocalId = message.Message.PersistentLocalId;
                    },
                    ct);
            });

            When<Envelope<StreetNameBecameCurrent>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        entity.Status = StreetNameStatus.Current;
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameWasProposed>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        entity.Status = StreetNameStatus.Proposed;
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameWasRetired>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        entity.Status = StreetNameStatus.Retired;
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameWasCorrectedToCurrent>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        entity.Status = StreetNameStatus.Current;
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameWasCorrectedToProposed>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        entity.Status = StreetNameStatus.Proposed;
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameWasCorrectedToRetired>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        entity.Status = StreetNameStatus.Retired;
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameStatusWasRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        entity.Status = null;
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });

            When<Envelope<StreetNameStatusWasCorrectedToRemoved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameHelper(
                    message.Message.StreetNameId,
                    entity =>
                    {
                        entity.Status = null;
                        UpdateVersionTimestamp(entity, message.Message.Provenance.Timestamp);
                    },
                    ct);
            });
        }

        private static void UpdateNameByLanguage(StreetNameHelper entity, List<StreetNameName> streetNameNames)
        {
            foreach (var streetNameName in streetNameNames)
            {
                switch (streetNameName.Language)
                {
                    case Language.Dutch:
                        entity.NameDutch = streetNameName.Name;
                        break;

                    case Language.French:
                        entity.NameFrench = streetNameName.Name;
                        break;

                    case Language.German:
                        entity.NameGerman = streetNameName.Name;
                        break;

                    case Language.English:
                        entity.NameEnglish = streetNameName.Name;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(streetNameName), streetNameName, null);
                }
            }
        }

        private static void UpdateNameByLanguage(StreetNameHelper entity, Language? language, string name)
        {
            switch (language)
            {
                case Language.Dutch:
                    entity.NameDutch = name;
                    break;

                case Language.French:
                    entity.NameFrench = name;
                    break;

                case Language.German:
                    entity.NameGerman = name;
                    break;

                case Language.English:
                    entity.NameEnglish = name;
                    break;
            }
        }

        private static void UpdateHomonymAdditionByLanguage(StreetNameHelper entity, Language? language, string homonymAddition)
        {
            if (entity == null)
                return;

            switch (language)
            {
                case Language.Dutch:
                    entity.HomonymAdditionDutch = homonymAddition;
                    break;

                case Language.French:
                    entity.HomonymAdditionFrench = homonymAddition;
                    break;

                case Language.German:
                    entity.HomonymAdditionGerman = homonymAddition;
                    break;

                case Language.English:
                    entity.HomonymAdditionEnglish = homonymAddition;
                    break;
            }
        }

        private static void UpdateVersionTimestamp(StreetNameHelper streetName, Instant versionTimestamp)
            => streetName.Version = versionTimestamp;
    }
}
