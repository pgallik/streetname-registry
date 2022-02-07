namespace StreetNameRegistry.Tests.BackOffice.Validators
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentValidation.TestHelper;
    using StreetNameRegistry.Api.BackOffice.StreetName.Requests;
    using Xunit;

    public class StreetNameProposeRequestValidatorTests
    {
        private readonly StreetNameProposeRequestValidator _validator = new StreetNameProposeRequestValidator();

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
                .WithErrorMessage($"The streetname in 'fr' can not be empty.");
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
    }
}
