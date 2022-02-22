namespace StreetNameRegistry.Projections.Syndication.Municipality
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication;

    public class MunicipalitySyndiciationProjections : AtomEntryProjectionHandlerModule<MunicipalityEvent, SyndicationContent<Gemeente>, SyndicationContext>
    {
        public MunicipalitySyndiciationProjections()
        {
            When(MunicipalityEvent.MunicipalityWasRegistered, AddSyndicationItemEntry);
            When(MunicipalityEvent.MunicipalityNisCodeWasDefined, AddSyndicationItemEntry);
            When(MunicipalityEvent.MunicipalityNisCodeWasCorrected, AddSyndicationItemEntry);
            When(MunicipalityEvent.MunicipalityWasNamed, AddSyndicationItemEntry);
            When(MunicipalityEvent.MunicipalityNameWasCleared, AddSyndicationItemEntry);
            When(MunicipalityEvent.MunicipalityNameWasCorrected, AddSyndicationItemEntry);
            When(MunicipalityEvent.MunicipalityNameWasCorrectedToCleared, AddSyndicationItemEntry);

            //these events only update version timestamp & position
            When(MunicipalityEvent.MunicipalityOfficialLanguageWasAdded, AddSyndicationItemEntry);
            When(MunicipalityEvent.MunicipalityOfficialLanguageWasRemoved, AddSyndicationItemEntry);
            When(MunicipalityEvent.MunicipalityFacilitiesLanguageWasAdded, AddSyndicationItemEntry);
            When(MunicipalityEvent.MunicipalityFacilitiesLanguageWasRemoved, AddSyndicationItemEntry);

            When(MunicipalityEvent.MunicipalityBecameCurrent, DoNothing);
            When(MunicipalityEvent.MunicipalityWasCorrectedToCurrent, DoNothing);
            When(MunicipalityEvent.MunicipalityWasRetired, DoNothing);
            When(MunicipalityEvent.MunicipalityWasCorrectedToRetired, DoNothing);
        }

        private static async Task AddSyndicationItemEntry(AtomEntry<SyndicationContent<Gemeente>> entry, SyndicationContext context, CancellationToken ct)
        {
            var municipalitySyndicationItem = new MunicipalitySyndicationItem
            {
                MunicipalityId = entry.Content.Object.Id,
                NisCode = entry.Content.Object.Identificator?.ObjectId,
                Version = entry.Content.Object.Identificator?.Versie,
                Position = long.Parse(entry.FeedEntry.Id)
            };

            UpdateNamesByGemeentenamen(municipalitySyndicationItem, entry.Content.Object.Gemeentenamen);

            await context
                .MunicipalitySyndicationItems
                .AddAsync(municipalitySyndicationItem, ct);
        }

        private static void UpdateNamesByGemeentenamen(MunicipalitySyndicationItem syndicationItem, IReadOnlyCollection<GeografischeNaam> gemeentenamen)
        {
            if (gemeentenamen == null || !gemeentenamen.Any())
                return;

            foreach (var naam in gemeentenamen)
            {
                switch (naam.Taal)
                {
                    default:
                    case Taal.NL:
                        syndicationItem.NameDutch = naam.Spelling;
                        break;

                    case Taal.FR:
                        syndicationItem.NameFrench = naam.Spelling;
                        break;

                    case Taal.DE:
                        syndicationItem.NameGerman = naam.Spelling;
                        break;

                    case Taal.EN:
                        syndicationItem.NameEnglish = naam.Spelling;
                        break;
                }
            }
        }

        private static Task DoNothing(AtomEntry<SyndicationContent<Gemeente>> entry, SyndicationContext context, CancellationToken ct) => Task.CompletedTask;
    }
}
