namespace StreetNameRegistry.Api.Oslo.StreetName
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Api.Search;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Straatnaam;
    using Convertors;
    using Infrastructure.FeatureToggles;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Projections.Legacy;
    using Projections.Legacy.StreetNameList;
    using Projections.Legacy.StreetNameListV2;
    using Projections.Syndication;
    using Projections.Syndication.Municipality;
    using Query;
    using Responses;
    using StreetNameRegistry.StreetName;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    [ApiVersion("2.0")]
    [AdvertiseApiVersions("2.0")]
    [ApiRoute("straatnamen")]
    [ApiExplorerSettings(GroupName = "Straatnamen")]
    public class StreetNameController : ApiController
    {
        private readonly UseProjectionsV2Toggle _useProjectionsV2Toggle;

        public StreetNameController(UseProjectionsV2Toggle useProjectionsV2Toggle)
        {
            _useProjectionsV2Toggle = useProjectionsV2Toggle;
        }

        /// <summary>
        /// Vraag een straatnaam op.
        /// </summary>
        /// <param name="legacyContext"></param>
        /// <param name="syndicationContext"></param>
        /// <param name="responseOptions"></param>
        /// <param name="persistentLocalId">De persistente lokale identificator van de straatnaam.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de straatnaam gevonden is.</response>
        /// <response code="404">Als de straatnaam niet gevonden kan worden.</response>
        /// <response code="410">Als de straatnaam verwijderd is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("{persistentLocalId}")]
        [Produces(AcceptTypes.JsonLd)]
        [ProducesResponseType(typeof(StreetNameOsloResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(StreetNameNotFoundResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status410Gone, typeof(StreetNameGoneResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Get(
            [FromServices] LegacyContext legacyContext,
            [FromServices] SyndicationContext syndicationContext,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            [FromRoute] int persistentLocalId,
            CancellationToken cancellationToken = default)
        {
            if (_useProjectionsV2Toggle.FeatureEnabled)
            {
                var streetNameV2 = await legacyContext
                    .StreetNameDetailV2
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.PersistentLocalId == persistentLocalId, cancellationToken);

                if (streetNameV2 == null)
                    throw new ApiException("Onbestaande straatnaam.", StatusCodes.Status404NotFound);

                if (streetNameV2.Removed)
                    throw new ApiException("Straatnaam verwijderd.", StatusCodes.Status410Gone);

                var gemeenteV2 = await GetStraatnaamDetailGemeente(syndicationContext, streetNameV2.NisCode, responseOptions.Value.GemeenteDetailUrl, cancellationToken);


                return Ok(new StreetNameOsloResponse(
                    responseOptions.Value.Naamruimte,
                    responseOptions.Value.ContextUrlDetail,
                    persistentLocalId,
                    streetNameV2.Status.ConvertFromMunicipalityStreetNameStatus(),
                    gemeenteV2,
                    streetNameV2.VersionTimestamp.ToBelgianDateTimeOffset(),
                    streetNameV2.NameDutch,
                    streetNameV2.NameFrench,
                    streetNameV2.NameGerman,
                    streetNameV2.NameEnglish,
                    streetNameV2.HomonymAdditionDutch,
                    streetNameV2.HomonymAdditionFrench,
                    streetNameV2.HomonymAdditionGerman,
                    streetNameV2.HomonymAdditionEnglish));
            }

            var streetName = await legacyContext
                .StreetNameDetail
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.PersistentLocalId == persistentLocalId, cancellationToken);

            if (streetName == null)
                throw new ApiException("Onbestaande straatnaam.", StatusCodes.Status404NotFound);

            if (streetName.Removed)
                throw new ApiException("Straatnaam verwijderd.", StatusCodes.Status410Gone);

            var gemeente = await GetStraatnaamDetailGemeente(syndicationContext, streetName.NisCode, responseOptions.Value.GemeenteDetailUrl, cancellationToken);

            return Ok(new StreetNameOsloResponse(
                responseOptions.Value.Naamruimte,
                responseOptions.Value.ContextUrlDetail,
                persistentLocalId,
                streetName.Status.ConvertFromStreetNameStatus(),
                gemeente,
                streetName.VersionTimestamp.ToBelgianDateTimeOffset(),
                streetName.NameDutch,
                streetName.NameFrench,
                streetName.NameGerman,
                streetName.NameEnglish,
                streetName.HomonymAdditionDutch,
                streetName.HomonymAdditionFrench,
                streetName.HomonymAdditionGerman,
                streetName.HomonymAdditionEnglish));
        }

        /// <summary>
        /// Vraag een lijst met straatnamen op.
        /// </summary>
        /// <param name="legacyContext"></param>
        /// <param name="syndicationContext"></param>
        /// <param name="taal">Gewenste taal van de respons.</param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van een lijst met straatnamen gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet]
        [Produces(AcceptTypes.JsonLd)]
        [ProducesResponseType(typeof(StreetNameListOsloResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameListOsloResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> List(
            [FromServices] SyndicationContext syndicationContext,
            [FromServices] LegacyContext legacyContext,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            Taal? taal,
            CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<StreetNameFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            if (_useProjectionsV2Toggle.FeatureEnabled)
            {
                var pagedStreetNamesV2 =
                    new StreetNameListOsloQueryV2(legacyContext, syndicationContext)
                        .Fetch<StreetNameListItemV2, StreetNameListItemV2>(filtering, sorting, pagination);

                Response.AddPagedQueryResultHeaders(pagedStreetNamesV2);

                return Ok(
                    new StreetNameListOsloResponse
                    {
                        Straatnamen = await pagedStreetNamesV2
                            .Items
                            .Select(m => new StreetNameListOsloItemResponse(
                                m.PersistentLocalId,
                                responseOptions.Value.Naamruimte,
                                responseOptions.Value.DetailUrl,
                                GetGeografischeNaamByTaal(m, m.PrimaryLanguage),
                                GetHomoniemToevoegingByTaal(m, m.PrimaryLanguage),
                                m.Status.ConvertFromMunicipalityStreetNameStatus(),
                                m.VersionTimestamp.ToBelgianDateTimeOffset()))
                            .ToListAsync(cancellationToken),
                        Volgende = BuildNextUri(pagedStreetNamesV2.PaginationInfo, responseOptions.Value.VolgendeUrl),
                        Context = responseOptions.Value.ContextUrlList
                    });
            }

            var pagedStreetNames = new StreetNameListOsloQuery(legacyContext, syndicationContext)
                .Fetch<StreetNameListItem, StreetNameListItem>(filtering, sorting, pagination);

            Response.AddPagedQueryResultHeaders(pagedStreetNames);

            return Ok(
                new StreetNameListOsloResponse
                {
                    Straatnamen = await pagedStreetNames
                        .Items
                        .Select(m => new StreetNameListOsloItemResponse(
                            m.PersistentLocalId,
                            responseOptions.Value.Naamruimte,
                            responseOptions.Value.DetailUrl,
                            GetGeografischeNaamByTaal(m, m.PrimaryLanguage),
                            GetHomoniemToevoegingByTaal(m, m.PrimaryLanguage),
                            m.Status.ConvertFromStreetNameStatus(),
                            m.VersionTimestamp.ToBelgianDateTimeOffset()))
                        .ToListAsync(cancellationToken),
                    Volgende = BuildNextUri(pagedStreetNames.PaginationInfo, responseOptions.Value.VolgendeUrl),
                    Context = responseOptions.Value.ContextUrlList
                });
        }

        /// <summary>
        /// Vraag het totaal aantal van straatnamen op.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="syndicationContext"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Als de opvraging van het totaal aantal gelukt is.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("totaal-aantal")]
        [Produces(AcceptTypes.JsonLd)]
        [ProducesResponseType(typeof(TotaalAantalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TotalCountResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Count(
            [FromServices] LegacyContext context,
            [FromServices] SyndicationContext syndicationContext,
            CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<StreetNameFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = new NoPaginationRequest();

            if (_useProjectionsV2Toggle.FeatureEnabled)
            {
                return Ok(
                    new TotaalAantalResponse
                    {
                        Aantal = await new StreetNameListOsloQueryV2(context, syndicationContext)
                            .Fetch<StreetNameListItemV2, StreetNameListItemV2>(filtering, sorting, pagination)
                            .Items
                            .CountAsync(cancellationToken)
                    });
            }

            return Ok(
                new TotaalAantalResponse
                {
                    Aantal = filtering.ShouldFilter
                        ? await new StreetNameListOsloQuery(context, syndicationContext)
                            .Fetch<StreetNameListItem, StreetNameListItem>(filtering, sorting, pagination)
                            .Items
                            .CountAsync(cancellationToken)
                        : Convert.ToInt32((await context
                                .StreetNameListViewCount
                                .FirstAsync(cancellationToken: cancellationToken))
                            .Count)
                });
        }

        private static GeografischeNaam GetGeografischeNaamByTaal(StreetNameListItemV2 item, Municipality.Language? taal)
        {
            switch (taal)
            {
                case null when !string.IsNullOrEmpty(item.NameDutch):
                case Municipality.Language.Dutch when !string.IsNullOrEmpty(item.NameDutch):
                    return new GeografischeNaam(
                        item.NameDutch,
                        Taal.NL);

                case Municipality.Language.French when !string.IsNullOrEmpty(item.NameFrench):
                    return new GeografischeNaam(
                        item.NameFrench,
                        Taal.FR);

                case Municipality.Language.German when !string.IsNullOrEmpty(item.NameGerman):
                    return new GeografischeNaam(
                        item.NameGerman,
                        Taal.DE);

                case Municipality.Language.English when !string.IsNullOrEmpty(item.NameEnglish):
                    return new GeografischeNaam(
                        item.NameEnglish,
                        Taal.EN);

                default:
                    return null;
            }
        }

        private static GeografischeNaam GetGeografischeNaamByTaal(StreetNameListItem item, Language? taal)
        {
            switch (taal)
            {
                case null when !string.IsNullOrEmpty(item.NameDutch):
                case Language.Dutch when !string.IsNullOrEmpty(item.NameDutch):
                    return new GeografischeNaam(
                        item.NameDutch,
                        Taal.NL);

                case Language.French when !string.IsNullOrEmpty(item.NameFrench):
                    return new GeografischeNaam(
                        item.NameFrench,
                        Taal.FR);

                case Language.German when !string.IsNullOrEmpty(item.NameGerman):
                    return new GeografischeNaam(
                        item.NameGerman,
                        Taal.DE);

                case Language.English when !string.IsNullOrEmpty(item.NameEnglish):
                    return new GeografischeNaam(
                        item.NameEnglish,
                        Taal.EN);

                default:
                    return null;
            }
        }

        private static GeografischeNaam GetHomoniemToevoegingByTaal(StreetNameListItem item, Language? taal)
        {
            switch (taal)
            {
                case null when !string.IsNullOrEmpty(item.HomonymAdditionDutch):
                case Language.Dutch when !string.IsNullOrEmpty(item.HomonymAdditionDutch):
                    return new GeografischeNaam(
                        item.HomonymAdditionDutch,
                        Taal.NL);

                case Language.French when !string.IsNullOrEmpty(item.HomonymAdditionFrench):
                    return new GeografischeNaam(
                        item.HomonymAdditionFrench,
                        Taal.FR);

                case Language.German when !string.IsNullOrEmpty(item.HomonymAdditionGerman):
                    return new GeografischeNaam(
                        item.HomonymAdditionGerman,
                        Taal.DE);

                case Language.English when !string.IsNullOrEmpty(item.HomonymAdditionEnglish):
                    return new GeografischeNaam(
                        item.HomonymAdditionEnglish,
                        Taal.EN);

                default:
                    return null;
            }
        }

        private static Uri? BuildNextUri(PaginationInfo paginationInfo, string nextUrlBase)
        {
            var offset = paginationInfo.Offset;
            var limit = paginationInfo.Limit;

            return paginationInfo.HasNextPage
                ? new Uri(string.Format(nextUrlBase, offset + limit, limit))
                : null;
        }

        private static GeografischeNaam GetHomoniemToevoegingByTaal(StreetNameListItemV2 item, Municipality.Language? taal)
        {
            switch (taal)
            {
                case null when !string.IsNullOrEmpty(item.HomonymAdditionDutch):
                case Municipality.Language.Dutch when !string.IsNullOrEmpty(item.HomonymAdditionDutch):
                    return new GeografischeNaam(
                        item.HomonymAdditionDutch,
                        Taal.NL);

                case Municipality.Language.French when !string.IsNullOrEmpty(item.HomonymAdditionFrench):
                    return new GeografischeNaam(
                        item.HomonymAdditionFrench,
                        Taal.FR);

                case Municipality.Language.German when !string.IsNullOrEmpty(item.HomonymAdditionGerman):
                    return new GeografischeNaam(
                        item.HomonymAdditionGerman,
                        Taal.DE);

                case Municipality.Language.English when !string.IsNullOrEmpty(item.HomonymAdditionEnglish):
                    return new GeografischeNaam(
                        item.HomonymAdditionEnglish,
                        Taal.EN);

                default:
                    return null;
            }
        }

        private async Task<StraatnaamDetailGemeente> GetStraatnaamDetailGemeente(SyndicationContext syndicationContext, string nisCode, string gemeenteDetailUrl, CancellationToken ct)
        {
            var municipality = await syndicationContext
                .MunicipalityLatestItems
                .AsNoTracking()
                .OrderByDescending(m => m.Position)
                .FirstOrDefaultAsync(m => m.NisCode == nisCode, ct);

            var municipalityDefaultName = GetDefaultMunicipalityName(municipality);
            var gemeente = new StraatnaamDetailGemeente
            {
                ObjectId = nisCode,
                Detail = string.Format(gemeenteDetailUrl, nisCode),
                Gemeentenaam = new Gemeentenaam(new GeografischeNaam(municipalityDefaultName.Value, municipalityDefaultName.Key))
            };
            return gemeente;
        }

        private static KeyValuePair<Taal, string> GetDefaultMunicipalityName(MunicipalityLatestItem municipality)
        {
            switch (municipality.PrimaryLanguage)
            {
                default:
                case null:
                case Taal.NL:
                    return new KeyValuePair<Taal, string>(Taal.NL, municipality.NameDutch);
                case Taal.FR:
                    return new KeyValuePair<Taal, string>(Taal.FR, municipality.NameFrench);
                case Taal.DE:
                    return new KeyValuePair<Taal, string>(Taal.DE, municipality.NameGerman);
                case Taal.EN:
                    return new KeyValuePair<Taal, string>(Taal.EN, municipality.NameEnglish);
            }
        }
    }
}
