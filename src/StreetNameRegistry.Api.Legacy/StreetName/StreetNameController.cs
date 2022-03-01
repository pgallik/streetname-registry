namespace StreetNameRegistry.Api.Legacy.StreetName
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Api.Search;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
    using Be.Vlaanderen.Basisregisters.Api.Syndication;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Syndication;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Straatnaam;
    using Convertors;
    using Infrastructure;
    using Infrastructure.FeatureToggles;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Microsoft.SyndicationFeed;
    using Microsoft.SyndicationFeed.Atom;
    using Projections.Legacy;
    using Projections.Legacy.StreetNameList;
    using Projections.Legacy.StreetNameListV2;
    using Projections.Syndication;
    using Projections.Syndication.Municipality;
    using Query;
    using Requests;
    using Responses;
    using StreetNameRegistry.StreetName;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("straatnamen")]
    [ApiExplorerSettings(GroupName = "Straatnamen")]
    public class StreetNameController : ApiController
    {
        private UseProjectionsV2Toggle _useProjectionsV2Toggle;

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
        [ProducesResponseType(typeof(StreetNameResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status410Gone)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameResponseExamples))]
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

                var gemeenteV2 = await GetStraatnaamDetailGemeente(syndicationContext, streetNameV2.NisCode, responseOptions.Value.GemeenteDetailUrl,cancellationToken);
                var streetNameResponse = new StreetNameResponse(
                    responseOptions.Value.Naamruimte,
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
                    streetNameV2.HomonymAdditionEnglish);

                return string.IsNullOrWhiteSpace(streetNameV2.LastEventHash)
                    ? Ok(streetNameResponse)
                    : new OkWithLastObservedPositionAsETagResult(streetNameResponse, streetNameV2.LastEventHash);
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

            return Ok(new StreetNameResponse(
                responseOptions.Value.Naamruimte,
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
        [ProducesResponseType(typeof(StreetNameListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameListResponseExamples))]
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
                    new StreetNameListQueryV2(legacyContext, syndicationContext)
                        .Fetch<StreetNameListItemV2, StreetNameListItemV2>(filtering, sorting, pagination);

                Response.AddPagedQueryResultHeaders(pagedStreetNamesV2);

                return Ok(
                    new StreetNameListResponse
                    {
                        Straatnamen = await pagedStreetNamesV2
                            .Items
                            .Select(m => new StreetNameListItemResponse(
                                m.PersistentLocalId,
                                responseOptions.Value.Naamruimte,
                                responseOptions.Value.DetailUrl,
                                GetGeografischeNaamByTaal(m, m.PrimaryLanguage),
                                GetHomoniemToevoegingByTaal(m, m.PrimaryLanguage),
                                m.Status.ConvertFromMunicipalityStreetNameStatus(),
                                m.VersionTimestamp.ToBelgianDateTimeOffset()))
                            .ToListAsync(cancellationToken),
                        Volgende = BuildNextUri(pagedStreetNamesV2.PaginationInfo, responseOptions.Value.VolgendeUrl)
                    });
            }

            var pagedStreetNames = new StreetNameListQuery(legacyContext, syndicationContext)
                .Fetch<StreetNameListItem, StreetNameListItem>(filtering, sorting, pagination);

            Response.AddPagedQueryResultHeaders(pagedStreetNames);

            return Ok(
                new StreetNameListResponse
                {
                    Straatnamen = await pagedStreetNames
                        .Items
                        .Select(m => new StreetNameListItemResponse(
                            m.PersistentLocalId,
                            responseOptions.Value.Naamruimte,
                            responseOptions.Value.DetailUrl,
                            GetGeografischeNaamByTaal(m, m.PrimaryLanguage),
                            GetHomoniemToevoegingByTaal(m, m.PrimaryLanguage),
                            m.Status.ConvertFromStreetNameStatus(),
                            m.VersionTimestamp.ToBelgianDateTimeOffset()))
                        .ToListAsync(cancellationToken),
                    Volgende = BuildNextUri(pagedStreetNames.PaginationInfo, responseOptions.Value.VolgendeUrl)
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
                        Aantal = await new StreetNameListQueryV2(context, syndicationContext)
                            .Fetch<StreetNameListItemV2, StreetNameListItemV2>(filtering, sorting, pagination)
                            .Items
                            .CountAsync(cancellationToken)
                    });
            }

            return Ok(
                new TotaalAantalResponse
                {
                    Aantal = filtering.ShouldFilter
                        ? await new StreetNameListQuery(context, syndicationContext)
                            .Fetch<StreetNameListItem, StreetNameListItem>(filtering, sorting, pagination)
                            .Items
                            .CountAsync(cancellationToken)
                        : Convert.ToInt32(context
                            .StreetNameListViewCount
                            .First()
                            .Count)
                });
        }

        /// <summary>
        /// Vraag een lijst met wijzigingen van straatnamen op.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="context"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("sync")]
        [Produces("text/xml")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameSyndicationResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Sync(
            [FromServices] IConfiguration configuration,
            [FromServices] LegacyContext context,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            CancellationToken cancellationToken = default)
        {
            var filtering = Request.ExtractFilteringRequest<StreetNameSyndicationFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var lastFeedUpdate = await context
                .StreetNameSyndication
                .AsNoTracking()
                .OrderByDescending(item => item.Position)
                .Select(item => item.SyndicationItemCreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastFeedUpdate == default)
                lastFeedUpdate = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);

            var pagedStreetNames =
                new StreetNameSyndicationQuery(
                        context,
                        filtering.Filter?.Embed)
                    .Fetch(filtering, sorting, pagination);

            return new ContentResult
            {
                Content = await BuildAtomFeed(lastFeedUpdate, pagedStreetNames, responseOptions, configuration),
                ContentType = MediaTypeNames.Text.Xml,
                StatusCode = StatusCodes.Status200OK
            };
        }

        /// <summary>
        /// Zoek naar straatnamen in het Vlaams Gewest in het BOSA formaat.
        /// </summary>
        /// <param name="legacyContext"></param>
        /// <param name="syndicationContext"></param>
        /// <param name="responseOptions"></param>
        /// <param name="request">De request in BOSA formaat.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("bosa")]
        [ProducesResponseType(typeof(StreetNameBosaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(BosaStreetNameRequest), typeof(StreetNameBosaRequestExamples))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StreetNameBosaResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Post(
            [FromServices] LegacyContext legacyContext,
            [FromServices] SyndicationContext syndicationContext,
            [FromServices] IOptions<ResponseOptions> responseOptions,
            [FromBody] BosaStreetNameRequest request,
            CancellationToken cancellationToken = default)
        {
            if (Request.ContentLength.HasValue && Request.ContentLength > 0 && request == null)
                return Ok(new StreetNameBosaResponse());

            if (_useProjectionsV2Toggle.FeatureEnabled)
            {
                var filterV2 = new StreetNameNameFilterV2(request);

                var streetNameBosaResponseV2 = await
                    new StreetNameBosaQueryV2(
                            legacyContext,
                            syndicationContext,
                            responseOptions)
                        .FilterAsync(filterV2, cancellationToken);

                return Ok(streetNameBosaResponseV2);
            }

            var filter = new StreetNameNameFilter(request);
            var streetNameBosaResponse = await
                new StreetNameBosaQuery(
                        legacyContext,
                        syndicationContext,
                        responseOptions)
                    .FilterAsync(filter, cancellationToken);

            return Ok(streetNameBosaResponse);
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

        private static async Task<string> BuildAtomFeed(
            DateTimeOffset lastFeedUpdate,
            PagedQueryable<StreetNameSyndicationQueryResult> pagedStreetNames,
            IOptions<ResponseOptions> responseOptions,
            IConfiguration configuration)
        {
            var sw = new StringWriterWithEncoding(Encoding.UTF8);

            using (var xmlWriter = XmlWriter.Create(sw,
                new XmlWriterSettings {Async = true, Indent = true, Encoding = sw.Encoding}))
            {
                var formatter = new AtomFormatter(null, xmlWriter.Settings) {UseCDATA = true};
                var writer = new AtomFeedWriter(xmlWriter, null, formatter);
                var syndicationConfiguration = configuration.GetSection("Syndication");
                var atomFeedConfig = AtomFeedConfigurationBuilder.CreateFrom(syndicationConfiguration, lastFeedUpdate);

                await writer.WriteDefaultMetadata(atomFeedConfig);

                var streetNames = pagedStreetNames.Items.ToList();

                var nextFrom = streetNames.Any()
                    ? streetNames.Max(s => s.Position) + 1
                    : (long?) null;

                var nextUri = BuildNextSyncUri(pagedStreetNames.PaginationInfo.Limit, nextFrom,
                    syndicationConfiguration["NextUri"]);
                if (nextUri != null)
                    await writer.Write(new SyndicationLink(nextUri, GrArAtomLinkTypes.Next));

                foreach (var streetName in streetNames)
                    await writer.WriteStreetName(responseOptions, formatter, syndicationConfiguration["Category"],
                        streetName);

                xmlWriter.Flush();
            }

            return sw.ToString();
        }

        private static Uri BuildNextUri(PaginationInfo paginationInfo, string nextUrlBase)
        {
            var offset = paginationInfo.Offset;
            var limit = paginationInfo.Limit;

            return paginationInfo.HasNextPage
                ? new Uri(string.Format(nextUrlBase, offset + limit, limit))
                : null;
        }

        private static Uri BuildNextSyncUri(int limit, long? from, string nextUrlBase)
        {
            return from.HasValue
                ? new Uri(string.Format(nextUrlBase, from, limit))
                : null;
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
