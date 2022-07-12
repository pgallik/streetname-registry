namespace StreetNameRegistry.Api.BackOffice.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Convertors;
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Oslo.Extensions;
    using Consumer;
    using FluentValidation;
    using FluentValidation.Validators;
    using Microsoft.EntityFrameworkCore;

    public class StreetNameExistingNisCodeValidator
    {
        public const string Code = "StraatnaamGemeenteNietGekendValidatie";

        private readonly ConsumerContext _consumerContext;

        public StreetNameExistingNisCodeValidator(ConsumerContext consumerContext)
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
    }
}
