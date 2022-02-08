namespace StreetNameRegistry.Tests.BackOffice.Validators
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using Consumer;
    using FluentValidation.TestHelper;
    using Microsoft.EntityFrameworkCore;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using StreetNameRegistry.Api.BackOffice.Validators;
    using Xunit;

    public class StreetNameProposeRequestValidatorTests
    {
        private readonly StreetNameProposeRequestValidator _validator;

        public StreetNameProposeRequestValidatorTests()
        {
            var factory = new ConsumerContextFactory("ConsumerAdmin");
            var options = new DbContextOptionsBuilder<ConsumerContext>()
                .UseInMemoryDatabase(nameof(StreetNameProposeRequestValidatorTests))
                .Options;
            var consumerContext = factory.Create(options);
            _validator = new StreetNameProposeRequestValidator(consumerContext);
        }

        [Fact]
        public void GivenEmptyStreetName_ThenReturnsExpectedMessage()
        {
            var result = _validator.TestValidate(new StreetNameProposeRequest
            {
                GemeenteId = "bla",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, "" }
                }
            });

            result.ShouldHaveValidationErrorFor($"{nameof(StreetNameProposeRequest.Straatnamen)}[0]")
                .WithErrorMessage($"The streetname in 'nl' can not be empty.");
        }

        [Fact]
        public void GivenOneEmptyStreetName_ThenReturnsExpectedMessage()
        {
            var result = _validator.TestValidate(new StreetNameProposeRequest
            {
                GemeenteId = "bla",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, "teststraat"},
                    { Taal.FR, "" }
                }
            });

            result.ShouldHaveValidationErrorFor($"{nameof(StreetNameProposeRequest.Straatnamen)}[1]")
                .WithErrorMessage("The streetname in 'fr' can not be empty.");
        }

        [Fact]
        public void GivenStreetNameExceededMaxLength_ThenReturnsExpectedMessage()
        {
            const string streetName = "Boulevard Louis Edelhart Lodewijk van Groothertogdom Luxemburg";

            var result = _validator.TestValidate(new StreetNameProposeRequest
            {
                GemeenteId = "bla",
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, streetName }
                }
            });

            result.ShouldHaveValidationErrorFor($"{nameof(StreetNameProposeRequest.Straatnamen)}[0]")
                .WithErrorMessage($"The max length of a streetname in 'nl' is 60 characters. You currently have {streetName.Length} characters.");
        }

        [Fact]
        public void GivenNisCodeIsInvalidPuri_ThenReturnsExpectedMessage()
        {
            const string gemeenteId = "bla";

            var result = _validator.TestValidate(new StreetNameProposeRequest
            {
                GemeenteId = gemeenteId,
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, "Sint-Niklaasstraat" }
                }
            });

            result.ShouldHaveValidationErrorFor($"{nameof(StreetNameProposeRequest.GemeenteId)}")
                .WithErrorMessage($"The municipality '{gemeenteId}' is not known in the Municipality registry.");
        }

        [Fact]
        public void GivenNonExistantNisCode_ThenReturnsExpectedMessage()
        {
            const string gemeenteId = "https://data.vlaanderen.be/id/gemeente/bla";

            var result = _validator.TestValidate(new StreetNameProposeRequest
            {
                GemeenteId = gemeenteId,
                Straatnamen = new Dictionary<Taal, string>
                {
                    { Taal.NL, "Sint-Niklaasstraat" }
                }
            });

            result.ShouldHaveValidationErrorFor($"{nameof(StreetNameProposeRequest.GemeenteId)}")
                .WithErrorMessage($"The municipality '{gemeenteId}' is not known in the Municipality registry.");
        }
    }
}
