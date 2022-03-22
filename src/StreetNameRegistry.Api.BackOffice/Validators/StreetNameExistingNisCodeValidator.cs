namespace StreetNameRegistry.Api.BackOffice.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Oslo.Extensions;
    using Consumer;
    using Convertors;
    using FluentValidation;
    using FluentValidation.Validators;
    using Microsoft.EntityFrameworkCore;
    using StreetName.Requests;

    public class StreetNameExistingNisCodeValidator : AsyncPropertyValidator<StreetNameProposeRequest, string>
    {
        public const string Code = "StraatnaamGemeenteNietGekendValidatie";

        private readonly ConsumerContext _consumerContext;

        public StreetNameExistingNisCodeValidator(ConsumerContext consumerContext)
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
                if (item == null)
                {
                    return false;
                }

                return Comparer<Guid>.Default.Compare(Guid.Empty, item.MunicipalityId) != 0;
            }
            catch (UriFormatException)
            {
                return false;
            }
        }

        public override string Name => nameof(StreetNameExistingNisCodeValidator);
    }
}
