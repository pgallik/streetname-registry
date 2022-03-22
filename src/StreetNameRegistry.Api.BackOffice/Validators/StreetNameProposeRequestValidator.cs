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
                .WithMessage(request => $"De gemeente '{request.GemeenteId}' is niet gekend in het gemeenteregister.")
                .WithErrorCode(StreetNameExistingNisCodeValidator.Code);

            RuleFor(x => x.GemeenteId)
                .SetAsyncValidator(new StreetNameFlemishRegionValidator(consumerContext))
                .WithMessage(request => $"De gemeente '{request.GemeenteId}' is geen Vlaamse gemeente.")
                .WithErrorCode(StreetNameFlemishRegionValidator.Code);

            RuleForEach(x => x.Straatnamen)
                .SetValidator(new StreetNameNotEmptyValidator())
                .WithMessage((_, streetName) => $"Straatnaam in '{streetName.Key.ToString().ToLowerInvariant()}' kan niet leeg zijn.")
                .WithErrorCode(StreetNameNotEmptyValidator.Code);

            RuleForEach(x => x.Straatnamen)
                .SetValidator(new StreetNameMaxLengthValidator())
                .WithMessage((_, streetName) => $"Maximum lengte van een straatnaam in '{streetName.Key.ToString().ToLowerInvariant()}' is 60 tekens. U heeft momenteel {streetName.Value.Length} tekens.")
                .WithErrorCode(StreetNameMaxLengthValidator.Code);
        }
    }
}
