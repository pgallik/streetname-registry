namespace StreetNameRegistry.Api.BackOffice.Validators
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentValidation;
    using FluentValidation.Validators;
    using StreetName.Requests;

    public class StreetNameMaxLengthValidator : PropertyValidator<StreetNameProposeRequest, KeyValuePair<Taal, string>>
    {
        public const string Code = "StreetNameMaxLengthValidator";

        public override bool IsValid(ValidationContext<StreetNameProposeRequest> context, KeyValuePair<Taal, string> value) => value.Value.Length <= 60;

        public override string Name => nameof(StreetNameMaxLengthValidator);
    }
}
