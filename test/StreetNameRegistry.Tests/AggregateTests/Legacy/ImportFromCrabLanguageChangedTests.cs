namespace StreetNameRegistry.Tests.AggregateTests.Legacy
{
    using Assert;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Generate;
    using StreetName.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class ImportFromCrabLanguageChangedTests : ImportFromCrabTest
    {
        public ImportFromCrabLanguageChangedTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void AppliesStreetNamePrimaryLanguageWasDefined()
        {
            //Arrange
            var expectedPrimaryLanguage = Arrange(Generate.CrabLanguage);
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, expectedPrimaryLanguage, Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNamePrimaryLanguageWasDefined>()
                .Which.Should().HavePrimaryLanguage(expectedPrimaryLanguage.ToLanguage())
                .And.HaveStreetNameId(streetNameId);
        }

        [Fact]
        public void AppliesStreetNamePrimaryLanguageWasCleared()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut = Act(sut, null, Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNamePrimaryLanguageWasCleared>()
                .Which.Should().HaveStreetNameId(streetNameId);
        }

        [Fact]
        public void AppliesStreetNamePrimaryLanguageWasCorrected()
        {
            //Arrange
            var expectedPrimaryLanguage = Arrange(Generate.CrabLanguage);
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, expectedPrimaryLanguage, Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNamePrimaryLanguageWasCorrected>()
                .Which.Should().HavePrimaryLanguage(expectedPrimaryLanguage.ToLanguage())
                .And.HaveStreetNameId(streetNameId);
        }

        [Fact]
        public void AppliesStreetNamePrimaryLanguageWasCorrectedToCleared()
        {
            //Arrange
            var expectedPrimaryLanguage = Arrange(Generate.CrabLanguage);
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, expectedPrimaryLanguage, Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLifetime), CrabModification.Correction);
            sut = Act(sut, null, Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNamePrimaryLanguageWasCorrectedToCleared>()
                .Which.Should().HaveStreetNameId(streetNameId);
        }

        [Fact]
        public void AppliesStreetNameSecondaryLanguageWasDefined()
        {
            //Arrange
            var expectedSecondaryLanguage = Arrange(Generate.CrabLanguage);
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabLanguage), expectedSecondaryLanguage, Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameSecondaryLanguageWasDefined>()
                .Which.Should().HaveSecondaryLanguage(expectedSecondaryLanguage.ToLanguage())
                .And.HaveStreetNameId(streetNameId);
        }

        [Fact]
        public void AppliesStreetNameSecondaryLanguageWasCleared()
        {
            //Arrange
            var expectedSecondaryLanguage = Arrange(Generate.CrabLanguage);
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabLanguage), expectedSecondaryLanguage, Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut = Act(sut, Arrange(Generate.CrabLanguage), null, Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameSecondaryLanguageWasCleared>()
                .Which.Should().HaveStreetNameId(streetNameId);
        }

        [Fact]
        public void AppliesStreetNameSecondaryLanguageWasCorrected()
        {
            //Arrange
            var expectedSecondaryLanguage = Arrange(Generate.CrabLanguage);
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabLanguage), expectedSecondaryLanguage, Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameSecondaryLanguageWasCorrected>()
                .Which.Should().HaveSecondaryLanguage(expectedSecondaryLanguage.ToLanguage())
                .And.HaveStreetNameId(streetNameId);
        }

        [Fact]
        public void AppliesStreetNameSecondaryLanguageWasCorrectedToCleared()
        {
            //Arrange
            var expectedSecondaryLanguage = Arrange(Generate.CrabLanguage);
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabLanguage), expectedSecondaryLanguage, Arrange(Generate.CrabLifetime), CrabModification.Correction);
            sut = Act(sut, Arrange(Generate.CrabLanguage), null, Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameSecondaryLanguageWasCorrectedToCleared>()
                .Which.Should().HaveStreetNameId(streetNameId);
        }

        [Fact]
        public void NoStreetNamePrimaryLanguageWasDefinedWhenLanguageIsNotChanged()
        {
            //Arrange
            var primaryLanguage = Arrange(Generate.CrabLanguage);
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            sut = Act(sut, primaryLanguage, Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut.ClearChanges();

            //Act
            sut = Act(sut, primaryLanguage, Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().NotHaveAnyChange<StreetNamePrimaryLanguageWasDefined>();
        }

        [Fact]
        public void NoStreetNamePrimaryLanguageWasClearedWhenLanguageIsNotChanged()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, null, Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().NotHaveAnyChange<StreetNamePrimaryLanguageWasCleared>();
        }

        [Fact]
        public void NoStreetNamePrimaryLanguageWasCorrectedWhenLanguageIsNotChanged()
        {
            //Arrange
            var primaryLanguage = Arrange(Generate.CrabLanguage);
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            sut = Act(sut, primaryLanguage, Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut.ClearChanges();

            //Act
            sut = Act(sut, primaryLanguage, Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().NotHaveAnyChange<StreetNamePrimaryLanguageWasCorrected>();
        }

        [Fact]
        public void NoStreetNamePrimaryLanguageWasCorrectedToClearedWhenLanguageIsNotChanged()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, null, Arrange(Generate.CrabLanguage), Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().NotHaveAnyChange<StreetNamePrimaryLanguageWasCorrectedToCleared>();
        }

        [Fact]
        public void NoStreetNameSecondaryLanguageWasDefinedWhenLanguageIsNotChanged()
        {
            //Arrange
            var secondaryLanguage = Arrange(Generate.CrabLanguage);
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            sut = Act(sut, Arrange(Generate.CrabLanguage), secondaryLanguage, Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut.ClearChanges();

            //Act
            sut = Act(sut, Arrange(Generate.CrabLanguage), secondaryLanguage, Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().NotHaveAnyChange<StreetNameSecondaryLanguageWasDefined>();
        }

        [Fact]
        public void NoStreetNameSecondaryLanguageWasClearedWhenLanguageIsNotChanged()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabLanguage), null, Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().NotHaveAnyChange<StreetNameSecondaryLanguageWasCleared>();
        }

        [Fact]
        public void NoStreetNameSecondaryLanguageWasCorrectedWhenLanguageIsNotChanged()
        {
            //Arrange
            var secondaryLanguage = Arrange(Generate.CrabLanguage);
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            sut = Act(sut, Arrange(Generate.CrabLanguage), secondaryLanguage, Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut.ClearChanges();

            //Act
            sut = Act(sut, Arrange(Generate.CrabLanguage), secondaryLanguage, Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().NotHaveAnyChange<StreetNameSecondaryLanguageWasCorrected>();
        }

        [Fact]
        public void NoStreetNameSecondaryLanguageWasCorrectedToClearedWhenLanguageIsNotChanged()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabLanguage), null, Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().NotHaveAnyChange<StreetNameSecondaryLanguageWasCorrectedToCleared>();
        }
    }
}
