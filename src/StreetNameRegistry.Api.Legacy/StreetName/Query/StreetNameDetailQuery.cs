namespace StreetNameRegistry.Api.Legacy.StreetName.Query
{
    using System;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Gemeente;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Straatnaam;
    using Convertors;
    using Infrastructure.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Projections.Legacy;
    using Projections.Syndication;
    using Projections.Syndication.Municipality;
    using Responses;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Projections.Legacy.Interfaces;

    public class StreetNameDetailQuery
    {
        private readonly LegacyContext _legacyContext;
        private readonly SyndicationContext _syndicationContext;
        private readonly ResponseOptions _responseOptions;

        public StreetNameDetailQuery(
            LegacyContext legacyContext,
            SyndicationContext syndicationContext,
            IOptions<ResponseOptions> responseOptionsProvider)
        {
            _legacyContext = legacyContext;
            _syndicationContext = syndicationContext;
            _responseOptions = responseOptionsProvider.Value;
        }

        public async Task<StreetNameResponse> FilterAsync<T>(Expression<Func<T, object>> property, Expression<Func<T, bool>> filter, CancellationToken ct = default) where T : class, IStreetNameDetail, new()
        {
            var streetName = await _legacyContext
                .Get<T>()
                .AsNoTracking()
                .SingleOrDefaultAsync(filter, ct);

            if (streetName == null)
                throw new ApiException("Onbestaande straatnaam.", StatusCodes.Status404NotFound);

            if (streetName.Removed)
                throw new ApiException("Straatnaam verwijderd.", StatusCodes.Status410Gone);

            var gemeente = await GetStraatnaamDetailGemeente(streetName.NisCode, ct);

            // it's magiiiicccc!!!!
            // get property of obj with reflection
            var persistentLocalId = (int?)streetName.GetType().GetProperties()
                .FirstOrDefault(i => i.Name == GetMemberName(property))?
                .GetValue(streetName, null);

            if (persistentLocalId == null)
                throw new InvalidOperationException("persistentLocalId cannot be null");

            return new StreetNameResponse(
                _responseOptions.Naamruimte,
                persistentLocalId.Value,
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
                streetName.HomonymAdditionEnglish);
        }

        private async Task<StraatnaamDetailGemeente> GetStraatnaamDetailGemeente(string nisCode, CancellationToken ct)
        {
            var municipality = await _syndicationContext
                .MunicipalityLatestItems
                .AsNoTracking()
                .OrderByDescending(m => m.Position)
                .FirstOrDefaultAsync(m => m.NisCode == nisCode, ct);

            var municipalityDefaultName = GetDefaultMunicipalityName(municipality);
            var gemeente = new StraatnaamDetailGemeente
            {
                ObjectId = nisCode,
                Detail = string.Format(_responseOptions.GemeenteDetailUrl, nisCode),
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

        private string GetMemberName<T>(Expression<T> expression)
        {
            switch (expression.Body)
            {
                case MemberExpression m:
                    return m.Member.Name;
                case UnaryExpression u when u.Operand is MemberExpression m:
                    return m.Member.Name;
                default:
                    throw new NotImplementedException(expression.GetType().ToString());
            }
        }
    }
}
