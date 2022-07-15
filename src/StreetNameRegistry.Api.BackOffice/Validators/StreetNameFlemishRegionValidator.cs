namespace StreetNameRegistry.Api.BackOffice.Validators
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Convertors;
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Oslo.Extensions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Consumer;
    using FluentValidation;
    using FluentValidation.Validators;
    using Microsoft.EntityFrameworkCore;

    public class StreetNameFlemishRegionValidator
    {
        public const string Code = "StraatnaamVlaamsGewestValidatie";

        private readonly ConsumerContext _consumerContext;

        public StreetNameFlemishRegionValidator(ConsumerContext consumerContext)
        {
            _consumerContext = consumerContext;
        }

        public async Task<bool> IsValidAsync(string municipalityPuri, CancellationToken cancellation)
        {
            try
            {
                var identifier = municipalityPuri
                    .AsIdentifier()
                    .Map(IdentifierMappings.MunicipalityNisCode);

                var item = await _consumerContext.MunicipalityConsumerItems
                    .AsNoTracking()
                    .SingleOrDefaultAsync(item => item.NisCode == identifier.Value, cancellation);

                return item != null
                    && RegionFilter.IsFlemishRegion(item.NisCode!);
            }
            catch (UriFormatException)
            {
                return false;
            }
        }
    }
}
