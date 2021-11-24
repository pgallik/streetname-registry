namespace StreetNameRegistry.Api.Oslo.StreetName.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Straatnaam;
    using Convertors;
    using Microsoft.EntityFrameworkCore;
    using Projections.Legacy;
    using Projections.Legacy.Interfaces;
    using Projections.Legacy.StreetNameList;
    using Projections.Legacy.StreetNameListV2;
    using Projections.Syndication;

    public class StreetNameListOsloQuery<T> : Query<T, StreetNameFilter>
        where T : class, IStreetNameListItem
    {
        private readonly LegacyContext _legacyContext;
        private readonly SyndicationContext _syndicationContext;

        protected override ISorting Sorting => new StreetNameSorting();

        public StreetNameListOsloQuery(LegacyContext legacyContext, SyndicationContext syndicationContext)
        {
            _legacyContext = legacyContext;
            _syndicationContext = syndicationContext;
        }

        protected override IQueryable<T> Filter(FilteringHeader<StreetNameFilter> filtering)
        {
            IQueryable<T>? streetNames = default;
            if (typeof(T) == typeof(StreetNameListItem))
            {
                streetNames = (_legacyContext
                    .StreetNameList
                    .AsNoTracking()
                    .OrderBy(x => x.PersistentLocalId)
                    .Where(s => !s.Removed && s.Complete && s.PersistentLocalId != null) as IQueryable<T>)!;
            }

            if (typeof(T) == typeof(StreetNameListItemV2))
            {
                streetNames = (_legacyContext
                    .StreetNameListV2
                    .AsNoTracking()
                    .OrderBy(x => x.PersistentLocalId)
                    .Where(s => !s.Removed) as IQueryable<T>)!;
            }

            if (streetNames == null)
            {
                throw new NotImplementedException();
            }

            if (!filtering.ShouldFilter)
            {
                return streetNames;
            }

            if (!string.IsNullOrEmpty(filtering.Filter.NisCode))
            {
                if ((streetNames is IQueryable<StreetNameListItemV2> streetNamesV2))
                {
                    streetNamesV2 = streetNamesV2.Where(m => m.NisCode == filtering.Filter.NisCode);
                    streetNames = streetNamesV2 as IQueryable<T>;
                }

                if ((streetNames is IQueryable<StreetNameListItem> streetNamesV1))
                {
                    streetNamesV1 = streetNamesV1.Where(m => m.NisCode == filtering.Filter.NisCode);
                    streetNames = streetNamesV1 as IQueryable<T>;
                }
            }

            if (!string.IsNullOrEmpty(filtering.Filter.NameDutch))
            {
                streetNames = streetNames.Where(s => s.NameDutch.Contains(filtering.Filter.NameDutch));
            }

            if (!string.IsNullOrEmpty(filtering.Filter.NameEnglish))
            {
                streetNames = streetNames.Where(s => s.NameEnglish.Contains(filtering.Filter.NameEnglish));
            }

            if (!string.IsNullOrEmpty(filtering.Filter.NameFrench))
            {
                streetNames = streetNames.Where(s => s.NameFrench.Contains(filtering.Filter.NameFrench));
            }

            if (!string.IsNullOrEmpty(filtering.Filter.NameGerman))
            {
                streetNames = streetNames.Where(s => s.NameGerman.Contains(filtering.Filter.NameGerman));
            }

            var filterMunicipalityName = filtering.Filter.MunicipalityName.RemoveDiacritics();
            if (!string.IsNullOrEmpty(filtering.Filter.MunicipalityName))
            {
                var municipalityNisCodes = _syndicationContext
                    .MunicipalityLatestItems
                    .AsNoTracking()
                    .Where(x => x.NameDutchSearch == filterMunicipalityName ||
                                x.NameFrenchSearch == filterMunicipalityName ||
                                x.NameEnglishSearch == filterMunicipalityName ||
                                x.NameGermanSearch == filterMunicipalityName)
                    .Select(x => x.NisCode)
                    .ToList();


                if ((streetNames is IQueryable<StreetNameListItemV2> streetNamesV2))
                {
                    streetNamesV2 = streetNamesV2.Where(m => municipalityNisCodes.Contains(m.NisCode));
                    streetNames = streetNamesV2 as IQueryable<T>;
                }

                if ((streetNames is IQueryable<StreetNameListItem> streetNamesV1))
                {
                    streetNamesV1 = streetNamesV1.Where(m => municipalityNisCodes.Contains(m.NisCode));
                    streetNames = streetNamesV1 as IQueryable<T>;
                }
            }

            var filterStreetName = filtering.Filter.StreetNameName.RemoveDiacritics();
            if (!string.IsNullOrEmpty(filtering.Filter.StreetNameName))
            {
                streetNames = streetNames
                    .Where(x => x.NameDutchSearch == filterStreetName ||
                                x.NameFrenchSearch == filterStreetName ||
                                x.NameEnglishSearch == filterStreetName ||
                                x.NameGermanSearch == filterStreetName);
            }

            if (!string.IsNullOrEmpty(filtering.Filter.Status))
            {
                if (Enum.TryParse(typeof(StraatnaamStatus), filtering.Filter.Status, true, out var status))
                {
                    var streetNameStatus = ((StraatnaamStatus) status).ConvertFromStraatnaamStatus();
                    streetNames = streetNames.Where(m => m.Status.HasValue && m.Status.Value == streetNameStatus);
                }
                else
                    //have to filter on EF cannot return new List<>().AsQueryable() cause non-EF provider does not support .CountAsync()
                {
                    streetNames = streetNames.Where(m => m.Status.HasValue && (int) m.Status.Value == -1);
                }
            }

            return streetNames;
        }
    }

    public class StreetNameSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(StreetNameListItem.NameDutch),
            nameof(StreetNameListItem.NameEnglish),
            nameof(StreetNameListItem.NameFrench),
            nameof(StreetNameListItem.NameGerman),
            nameof(StreetNameListItem.PersistentLocalId)
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(StreetNameListItem.PersistentLocalId), SortOrder.Ascending);
    }

    public class StreetNameFilter
    {
        public string StreetNameName { get; set; }
        public string MunicipalityName { get; set; }
        public string NameDutch { get; set; }
        public string NameFrench { get; set; }
        public string NameGerman { get; set; }
        public string NameEnglish { get; set; }
        public string Status { get; set; }
        public string? NisCode { get; set; }
    }
}
