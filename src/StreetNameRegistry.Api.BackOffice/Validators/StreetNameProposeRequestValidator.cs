namespace StreetNameRegistry.Api.BackOffice.Validators
{
    using Consumer;
    using FluentValidation;
    using StreetName.Requests;

    public class StreetNameProposeRequestValidator : AbstractValidator<StreetNameProposeRequest>
    {
        public StreetNameProposeRequestValidator(ConsumerContext consumerContext)
        {
            RuleFor(x => x.GemeenteId)
                .SetAsyncValidator(new StreetNameExistingNisCodeValidator(consumerContext))
                .WithMessage(request => $"The municipality '{request.GemeenteId}' is not known in the Municipality registry.")
                .WithErrorCode("StreetNameExisitingMunicipalityValidator");

            RuleFor(x => x.GemeenteId)
                .SetAsyncValidator(new StreetNameFlemishRegionValidator(consumerContext))
                .WithMessage(request => $"The municipality '{request.GemeenteId}' is not a Flemish municipality.")
                .WithErrorCode("StreetNameFlemishRegionValidator");

            RuleForEach(x => x.Straatnamen)
                .SetValidator(new StreetNameNotEmptyValidator())
                .WithMessage((_, streetName) => $"The streetname in '{streetName.Key.ToString().ToLowerInvariant()}' can not be empty.")
                .WithErrorCode("StreetNameNotEmptyValidator");

            RuleForEach(x => x.Straatnamen)
                .SetValidator(new StreetNameMaxLengthValidator())
                .WithMessage((_, streetName) => $"The max length of a streetname in '{streetName.Key.ToString().ToLowerInvariant()}' is 60 characters. You currently have {streetName.Value.Length} characters.")
                .WithErrorCode("StreetNameMaxLengthValidator");
        }
    }
}
