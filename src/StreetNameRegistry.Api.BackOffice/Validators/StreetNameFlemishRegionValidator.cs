namespace StreetNameRegistry.Api.BackOffice.Validators
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Oslo.Extensions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Consumer;
    using Convertors;
    using FluentValidation;
    using FluentValidation.Validators;
    using Microsoft.EntityFrameworkCore;
    using StreetName.Requests;

    public class StreetNameFlemishRegionValidator : AsyncPropertyValidator<StreetNameProposeRequest, string>
    {
        public const string Code = "StraatnaamVlaamsGewestValidatie";

        private readonly ConsumerContext _consumerContext;

        public StreetNameFlemishRegionValidator(ConsumerContext consumerContext)
        {
            _consumerContext = consumerContext;
        }

        public override async Task<bool> IsValidAsync(ValidationContext<StreetNameProposeRequest> context, string value, CancellationToken cancellation)
        {
            try
            {
                var identifier = value
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

        public override string Name => nameof(StreetNameFlemishRegionValidator);
    }
}
