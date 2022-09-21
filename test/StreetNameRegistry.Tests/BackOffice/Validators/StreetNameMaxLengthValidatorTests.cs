namespace StreetNameRegistry.Tests.BackOffice.Validators
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentAssertions;
    using StreetNameRegistry.Api.BackOffice.Validators;
    using Xunit;

    public sealed class StreetNameMaxLengthValidatorTests
    {
        [Theory]
        [InlineData("long enough", true)]
        [InlineData("Exactly60CharactersFillerFromHereOnaaaaaaaaaaaaaaaaaaaaaaaaa", true)]
        [InlineData("Exactly61CharactersFillerFromHereOnaaaaaaaaaaaaaaaaaaaaaaaaaa", false)]
        [InlineData("MoreThan60CharactersMoreThan60CharactersMoreThan60CharactersMoreThan60CharactersMoreThan60CharactersMoreThan60Characters", false)]
        public void GivenStreetName_ThenReturnsExpectedResult(string name, bool expectedResult)
        {
            StreetNameMaxLengthValidator
                .IsValid(new KeyValuePair<Taal, string>(Taal.NL, name))
                .Should()
                .Be(expectedResult);
        }
    }
}
