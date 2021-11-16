namespace StreetNameRegistry.Projections.Legacy.StreetNameNameV2
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using NodaTime;
    using StreetName.Events;
    using StreetNameName = StreetNameRegistry.StreetNameName;

    [ConnectedProjectionName("API endpoint straatnamen ifv BOSA DT")]
    [ConnectedProjectionDescription("Projectie die de straatnamen data voor straatnamen ifv BOSA DT voorziet.")]
    public class StreetNameNameProjectionsV2 : ConnectedProjection<LegacyContext>
    {
        public StreetNameNameProjectionsV2()
        {
            When<Envelope<StreetNameWasProposedV2>>(async (context, message, ct) =>
            {
                var streetNameNameV2 = new StreetNameNameV2
                {
                    PersistentLocalId = message.Message.PersistentLocalId,
                    MunicipalityId = message.Message.MunicipalityId,
                    NisCode = message.Message.NisCode,
                    VersionTimestamp = message.Message.Provenance.Timestamp,
                    IsFlemishRegion = RegionFilter.IsFlemishRegion(message.Message.NisCode),
                    Removed = false
                };
                UpdateNameByLanguage(streetNameNameV2, message.Message.StreetNameNames);
                await context
                    .StreetNameNamesV2
                    .AddAsync(streetNameNameV2, ct);
            });
        }

        private static void UpdateNameByLanguage(StreetNameNameV2 entity, List<StreetNameName> streetNameNames)
        {
            foreach (var streetNameName in streetNameNames)
            {
                switch (streetNameName.Language)
                {
                    case Language.Dutch:
                        entity.NameDutch = streetNameName.Name;
                        entity.NameDutchSearch = streetNameName.Name.SanitizeForBosaSearch();
                        break;

                    case Language.French:
                        entity.NameFrench = streetNameName.Name;
                        entity.NameFrenchSearch = streetNameName.Name.SanitizeForBosaSearch();
                        break;

                    case Language.German:
                        entity.NameGerman = streetNameName.Name;
                        entity.NameGermanSearch = streetNameName.Name.SanitizeForBosaSearch();
                        break;

                    case Language.English:
                        entity.NameEnglish = streetNameName.Name;
                        entity.NameEnglishSearch = streetNameName.Name.SanitizeForBosaSearch();
                        break;
                }
            }
        }

        private static void UpdateVersionTimestamp(StreetNameNameV2 streetNameName, Instant versionTimestamp)
            => streetNameName.VersionTimestamp = versionTimestamp;
    }
}
