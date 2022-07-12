namespace StreetNameRegistry.Api.BackOffice.Validators
{
    using System.Collections.Generic;
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentValidation;
    using FluentValidation.Validators;

    public static class StreetNameNotEmptyValidator
    {
        public const string Code = "StraatnaamNietLeegValidatie";

        public static bool IsValid(KeyValuePair<Taal, string> streetName) => !string.IsNullOrWhiteSpace(streetName.Value);
    }
}
