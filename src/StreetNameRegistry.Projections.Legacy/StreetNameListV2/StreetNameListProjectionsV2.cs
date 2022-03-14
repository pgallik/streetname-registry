namespace StreetNameRegistry.Projections.Legacy.StreetNameListV2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality;
    using Municipality.Events;
    using NodaTime;
    using StreetNameList;
    using StreetNameName = Municipality.StreetNameName;

    [ConnectedProjectionName("API endpoint lijst straatnamen")]
    [ConnectedProjectionDescription("Projectie die de straatnamen data voor de straatnamen lijst voorziet.")]
    public class StreetNameListProjectionsV2 : ConnectedProjection<LegacyContext>
    {
        public StreetNameListProjectionsV2()
        {
            When<Envelope<StreetNameWasMigratedToMunicipality>>(async (context, message, ct) =>
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
                    Removed = message.Message.IsRemoved,
                    PrimaryLanguage = municipality.PrimaryLanguage,
                    Status = message.Message.Status
                };

                UpdateNameByLanguage(streetNameListItemV2, new Names(message.Message.Names));
                UpdateHomonymAdditionByLanguage(streetNameListItemV2, new HomonymAdditions(message.Message.HomonymAdditions));

                await context
                    .StreetNameListV2
                    .AddAsync(streetNameListItemV2, ct);
            });

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
                    PrimaryLanguage = municipality.PrimaryLanguage,
                    Status = StreetNameStatus.Proposed
                };

                UpdateNameByLanguage(streetNameListItemV2, message.Message.StreetNameNames);

                await context
                    .StreetNameListV2
                    .AddAsync(streetNameListItemV2, ct);
            });

            When<Envelope<StreetNameWasApproved>>(async (context, message, ct) =>
            {
                await context.FindAndUpdateStreetNameListItem(
                    message.Message.PersistentLocalId, streetNameListItemV2 =>
                    {
                        UpdateStatus(streetNameListItemV2, StreetNameStatus.Current);
                        UpdateVersionTimestamp(streetNameListItemV2, message.Message.Provenance.Timestamp);
                    }, ct);
            });

            When<Envelope<MunicipalityWasImported>>(async (context, message, ct) =>
            {
                var streetNameListMunicipality = new StreetNameListMunicipality
                {
                    MunicipalityId = message.Message.MunicipalityId,
                    NisCode = message.Message.NisCode
                };

                await context
                    .StreetNameListMunicipality
                    .AddAsync(streetNameListMunicipality, ct);
            });

            When<Envelope<MunicipalityNisCodeWasChanged>>(async (context, message, ct) =>
            {
                var municipality =
                    await context.StreetNameListMunicipality.FindAsync(message.Message.MunicipalityId,
                        cancellationToken: ct);

                municipality.NisCode = message.Message.NisCode;

                var streetNames = context
                    .StreetNameListV2
                    .Local
                    .Where(s => s.MunicipalityId == message.Message.MunicipalityId)
                    .Union(context.StreetNameListV2.Where(s => s.MunicipalityId == message.Message.MunicipalityId));

                foreach (var streetName in streetNames)
                    streetName.NisCode = message.Message.NisCode;
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

        private static void UpdateHomonymAdditionByLanguage(StreetNameListItemV2 entity, List<StreetNameHomonymAddition> homonymAdditions)
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

        private static void UpdateVersionTimestamp(StreetNameListItemV2 streetNameListItemV2, Instant timestamp)
            => streetNameListItemV2.VersionTimestamp = timestamp;

        private static void UpdateStatus(StreetNameListItemV2 streetNameListItemV2, StreetNameStatus status)
            => streetNameListItemV2.Status = status;
    }
}
