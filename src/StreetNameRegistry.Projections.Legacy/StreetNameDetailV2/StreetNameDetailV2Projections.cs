namespace StreetNameRegistry.Projections.Legacy.StreetNameDetailV2
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using NodaTime;
    using StreetName.Events;
    using StreetNameName = StreetNameRegistry.StreetNameName;

    [ConnectedProjectionName("API endpoint detail straatnamen")]
    [ConnectedProjectionDescription("Projectie die de straatnamen data voor het straatnamen detail voorziet.")]
    public class StreetNameDetailProjectionsV2 : ConnectedProjection<LegacyContext>
    {
        public StreetNameDetailProjectionsV2()
        {
            When<Envelope<StreetNameWasProposedV2>>(async (context, message, ct) =>
            {
                var streetNameDetailV2 = new StreetNameDetailV2
                {
                    MunicipalityId = message.Message.MunicipalityId,
                    PersistentLocalId = message.Message.PersistentLocalId,
                    NisCode = message.Message.NisCode,
                    VersionTimestamp = message.Message.Provenance.Timestamp,
                    Complete = false,
                    Removed = false
                };
                UpdateNameByLanguage(streetNameDetailV2, message.Message.StreetNameNames);
                await context
                    .StreetNameDetailV2
                    .AddAsync(streetNameDetailV2, ct);
            });

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
                        throw new ArgumentOutOfRangeException(nameof(streetNameName), streetNameName, null);
                }
            }
        }

        private static void UpdateHomonymAdditionByLanguage(StreetNameDetailV2 entity, Language? language, string homonymAddition)
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, null);
            }
        }

        private static void UpdateVersionTimestamp(StreetNameDetailV2 streetName, Instant versionTimestamp)
            => streetName.VersionTimestamp = versionTimestamp;
    }
}
