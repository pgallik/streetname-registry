namespace StreetNameRegistry.Projections.Legacy.StreetNameDetailV2
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality;
    using Municipality.Events;
    using NodaTime;
    using StreetNameName = Municipality.StreetNameName;

    [ConnectedProjectionName("API endpoint detail straatnamen")]
    [ConnectedProjectionDescription("Projectie die de straatnamen data voor het straatnamen detail voorziet.")]
    public class StreetNameDetailProjectionsV2 : ConnectedProjection<LegacyContext>
    {
        public StreetNameDetailProjectionsV2()
        {
            When<Envelope<StreetNameWasMigratedToMunicipality>>(async (context, message, ct) =>
            {
                var streetNameDetailV2 = new StreetNameDetailV2
                {
                    MunicipalityId = message.Message.MunicipalityId,
                    PersistentLocalId = message.Message.PersistentLocalId,
                    NisCode = message.Message.NisCode,
                    VersionTimestamp = message.Message.Provenance.Timestamp,
                    Removed = message.Message.IsRemoved,
                    Status = message.Message.Status
                };

                UpdateNameByLanguage(streetNameDetailV2, new Names(message.Message.Names));
                UpdateHomonymAdditionByLanguage(streetNameDetailV2, new HomonymAdditions(message.Message.HomonymAdditions));
                UpdateHash(streetNameDetailV2, message);

                await context
                    .StreetNameDetailV2
                    .AddAsync(streetNameDetailV2, ct);
            });

            When<Envelope<StreetNameWasProposedV2>>(async (context, message, ct) =>
            {
                var streetNameDetailV2 = new StreetNameDetailV2
                {
                    MunicipalityId = message.Message.MunicipalityId,
                    PersistentLocalId = message.Message.PersistentLocalId,
                    NisCode = message.Message.NisCode,
                    VersionTimestamp = message.Message.Provenance.Timestamp,
                    Removed = false,
                    Status = StreetNameStatus.Proposed
                };

                UpdateNameByLanguage(streetNameDetailV2, message.Message.StreetNameNames);
                UpdateHash(streetNameDetailV2, message);

                await context
                    .StreetNameDetailV2
                    .AddAsync(streetNameDetailV2, ct);
            });
        }

        private static void UpdateHash<T>(StreetNameDetailV2 entity, Envelope<T> wrappedEvent) where T : IHaveHash
        {
            if (!wrappedEvent.Metadata.ContainsKey(AddHashPipe.HashMetadataKey))
            {
                throw new InvalidOperationException($"Cannot find hash in metadata for event at position {wrappedEvent.Position}");
            }

            entity.LastEventHash = wrappedEvent.Metadata[AddHashPipe.HashMetadataKey].ToString()!;
        }

        private static void UpdateNameByLanguage(StreetNameDetailV2 entity, List<StreetNameName> streetNameNames)
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
                        throw new ArgumentOutOfRangeException(nameof(streetNameName.Language), streetNameName.Language, null);
                }
            }
        }

        private static void UpdateHomonymAdditionByLanguage(StreetNameDetailV2 entity, List<StreetNameHomonymAddition> homonymAdditions)
        {
            foreach (var homonymAddition in homonymAdditions)
            {
                switch (homonymAddition.Language)
                {
                    case Language.Dutch:
                        entity.HomonymAdditionDutch = homonymAddition.HomonymAddition;
                        break;

                    case Language.French:
                        entity.HomonymAdditionFrench = homonymAddition.HomonymAddition;
                        break;

                    case Language.German:
                        entity.HomonymAdditionGerman = homonymAddition.HomonymAddition;
                        break;

                    case Language.English:
                        entity.HomonymAdditionEnglish = homonymAddition.HomonymAddition;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(homonymAddition.Language), homonymAddition.Language, null);
                }
            }
        }

        private static void UpdateVersionTimestamp(StreetNameDetailV2 streetName, Instant versionTimestamp)
            => streetName.VersionTimestamp = versionTimestamp;
    }
}
