namespace StreetNameRegistry.Api.BackOffice.Validators
{
    using System.Collections.Generic;
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentValidation;
    using FluentValidation.Validators;

    public static class StreetNameMaxLengthValidator
    {
        public const string Code = "StraatnaamMaxlengteValidatie";

        public static bool IsValid(KeyValuePair<Taal, string> streetName) => streetName.Value.Length <= 60;
    }
}
