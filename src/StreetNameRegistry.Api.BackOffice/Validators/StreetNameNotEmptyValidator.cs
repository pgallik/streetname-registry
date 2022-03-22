namespace StreetNameRegistry.Api.BackOffice.Validators
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentValidation;
    using FluentValidation.Validators;
    using StreetName.Requests;

    public class StreetNameNotEmptyValidator : PropertyValidator<StreetNameProposeRequest, KeyValuePair<Taal, string>>
    {
        public const string Code = "StraatnaamNietLeegValidatie";

        public override bool IsValid(ValidationContext<StreetNameProposeRequest> context, KeyValuePair<Taal, string> value) => !string.IsNullOrWhiteSpace(value.Value);

        public override string Name => nameof(StreetNameNotEmptyValidator);
    }
}
