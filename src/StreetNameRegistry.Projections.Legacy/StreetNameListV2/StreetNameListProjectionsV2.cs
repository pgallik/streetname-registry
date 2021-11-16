namespace StreetNameRegistry.Projections.Legacy.StreetNameListV2
{
    using System.Collections.Generic;
    using System.Threading;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using NodaTime;
    using StreetName.Events;
    using StreetNameList;
    using StreetNameName = StreetNameRegistry.StreetNameName;

    [ConnectedProjectionName("API endpoint lijst straatnamen")]
    [ConnectedProjectionDescription("Projectie die de straatnamen data voor de straatnamen lijst voorziet.")]
    public class StreetNameListProjectionsV2 : ConnectedProjection<LegacyContext>
    {
        public StreetNameListProjectionsV2()
        {
            When<Envelope<StreetNameWasProposedV2>>(async (context, message, ct) =>
            {
                var municipality =
                    await context.StreetNameListMunicipality.FindAsync(message.Message.MunicipalityId,
                        cancellationToken: ct);

                var streetNameListItemV2 = new StreetNameListItemV2
                {
                    PersistentLocalId = message.Message.PersistentLocalId,
                    MunicipalityId = message.Message.MunicipalityId,
                    NisCode = message.Message.NisCode,
                    VersionTimestamp = message.Message.Provenance.Timestamp,
                    Removed = false,
                    PrimaryLanguage = municipality.PrimaryLanguage
                };
                UpdateNameByLanguage(streetNameListItemV2, message.Message.StreetNameNames);
                await context
                    .StreetNameListV2
                    .AddAsync(streetNameListItemV2, ct);
            });

            When<Envelope<MunicipalityWasImported>>(async (context, message, ct) =>
            {
                var streetNameListMunicipality = new StreetNameListMunicipality()
                {
                    MunicipalityId = message.Message.MunicipalityId,
                    NisCode = message.Message.NisCode
                };

                await context
                    .StreetNameListMunicipality
                    .AddAsync(streetNameListMunicipality, ct);
            });
        }

        private static void UpdateNameByLanguage(StreetNameListItemV2 entity, List<StreetNameName> streetNameNames)
        {
            foreach (var streetNameName in streetNameNames)
            {
                switch (streetNameName.Language)
                {
                    case Language.Dutch:
                        entity.NameDutch = streetNameName.Name;
                        entity.NameDutchSearch = streetNameName.Name.RemoveDiacritics();
                        break;

                    case Language.French:
                        entity.NameFrench = streetNameName.Name;
                        entity.NameFrenchSearch = streetNameName.Name.RemoveDiacritics();
                        break;

                    case Language.German:
                        entity.NameGerman = streetNameName.Name;
                        entity.NameGermanSearch = streetNameName.Name.RemoveDiacritics();
                        break;

                    case Language.English:
                        entity.NameEnglish = streetNameName.Name;
                        entity.NameEnglishSearch = streetNameName.Name.RemoveDiacritics();
                        break;
                }
            }
        }

        private static void UpdateHomonymAdditionByLanguage(StreetNameListItem entity, Language? language, string homonymAddition)
        {
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

        private static void UpdateVersionTimestamp(StreetNameListItem streetNameListItem, Instant timestamp)
            => streetNameListItem.VersionTimestamp = timestamp;
    }
}
