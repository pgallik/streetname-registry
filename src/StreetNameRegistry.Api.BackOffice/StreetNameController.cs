namespace StreetNameRegistry.Api.BackOffice
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Api;
    using FluentValidation;
    using FluentValidation.Results;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersion("2.0")]
    [AdvertiseApiVersions("2.0")]
    [ApiRoute("straatnamen")]
    [ApiExplorerSettings(GroupName = "Straatnamen")]
    public partial class StreetNameController : BackOfficeApiController
    {
        private readonly IMediator _mediator;
        public StreetNameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private ValidationException CreateValidationException(string errorCode, string propertyName, string message)
        {
            var failure = new ValidationFailure(propertyName, message)
            {
                ErrorCode = errorCode
            };

            return new ValidationException(new List<ValidationFailure>
            {
                failure
            });
        }
    }
}
