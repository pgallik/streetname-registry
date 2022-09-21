namespace StreetNameRegistry.Tests.AggregateTests.Legacy
{
    using Assert;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Generate;
    using StreetName;
    using StreetName.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public sealed class ImportFromCrabNameChangedTests : ImportFromCrabTest
    {
        public ImportFromCrabNameChangedTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void AppliesStreetNameHomonymAdditionWasClearedForPrimary()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = CrabLanguage.Dutch;

            //Act
            sut = Act(sut, new CrabStreetName(Arrange(Generate.StreetNameWithHomonymString("UK")), language), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut.ClearChanges();
            sut = Act(sut, new CrabStreetName(Arrange(Generate.StreetNameWithHomonymString("")), language), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameHomonymAdditionWasCleared>()
                .Which.Should().HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameHomonymAdditionWasClearedForSecondary()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = CrabLanguage.English;

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(CrabLanguage.English)), new CrabStreetName(Arrange(Generate.StreetNameWithHomonymString("UK")), language),
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut.ClearChanges();
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(CrabLanguage.English)), new CrabStreetName(Arrange(Generate.StreetNameWithHomonymString(null)), language),
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameHomonymAdditionWasCleared>()
                .Which.Should().HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameHomonymAdditionWasCorrectedForPrimary()
        {
            //Arrange
            var homonymAddition = "UK";
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = CrabLanguage.Dutch;

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameHomonymAdditionWasCorrected>()
                .Which.Should().HaveHomonymAddition(homonymAddition)
                .And.HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameHomonymAdditionWasCorrectedForSecondary()
        {
            //Arrange
            var homonymAddition = "UK";
            CrabLanguage? language = null;
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameHomonymAdditionWasCorrected>()
                .Which.Should().HaveHomonymAddition(homonymAddition)
                .And.HaveStreetNameId(streetNameId)
                .And.HaveLanguage(null);
        }

        [Fact]
        public void AppliesStreetNameHomonymAdditionWasCorrectedToClearedForPrimary()
        {
            //Arrange
            var homonymAddition = "UK";
            var language = CrabLanguage.Dutch;
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, new CrabStreetName(streetName, language), Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabLifetime),
                CrabModification.Correction);
            sut.ClearChanges();
            sut = Act(sut, new CrabStreetName(Arrange(Generate.StreetNameWithHomonymString("")), language), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameHomonymAdditionWasCorrectedToCleared>()
                .Which.Should().HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameHomonymAdditionWasCorrectedToClearedForSecondary()
        {
            //Arrange
            var homonymAddition = "UK";
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            CrabLanguage? language = null;

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);
            sut.ClearChanges();
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabStreetNameWithNameAndLanguage(Arrange(Generate.StreetNameWithHomonymString("")), language)), Arrange(Generate.CrabLifetime),
                CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameHomonymAdditionWasCorrectedToCleared>()
                .Which.Should().HaveStreetNameId(streetNameId)
                .And.HaveLanguage(null);
        }

        [Fact]
        public void AppliesStreetNameHomonymAdditionWasDefinedForPrimary()
        {
            //Arrange
            var homonymAddition = "UK";
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = CrabLanguage.Dutch;

            //Act
            sut = Act(sut, new CrabStreetName(streetName, language), Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameHomonymAdditionWasDefined>()
                .Which.Should().HaveHomonymAddition(homonymAddition)
                .And.HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameHomonymAdditionWasDefinedForSecondary()
        {
            //Arrange
            var homonymAddition = "UK";
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = CrabLanguage.English;

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), new CrabStreetName(streetName, language), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameHomonymAdditionWasDefined>()
                .Which.Should().HaveHomonymAddition(homonymAddition)
                .And.HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameHomonymAdditionWasDefinedTwice()
        {
            //Arrange
            var homonymAddition = "UK";
            var homonymAddition2 = "EK";
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetName2 = Arrange(Generate.StreetNameWithHomonymString(homonymAddition2));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = CrabLanguage.Dutch;
            var language2 = CrabLanguage.French;

            //Act
            sut = Act(sut, new CrabStreetName(streetName, language), new CrabStreetName(streetName2, language2), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveCountOfChanges<StreetNameHomonymAdditionWasDefined>(2);
        }

        [Fact]
        public void AppliesStreetNameNameWasClearedForPrimary()
        {
            //Arrange
            var primaryname = Arrange(Generate.StreetNameString);
            var streetNameId = Arrange(Generate.StreetNameId);
            var language = Arrange(Generate.CrabLanguage);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(primaryname, language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut.GetChanges();
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage("", language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameNameWasCleared>()
                .Which.Should().HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameNameWasClearedForSecondary()
        {
            //Arrange
            var name = Arrange(Generate.StreetNameString);
            var streetNameId = Arrange(Generate.StreetNameId);
            var language = Arrange(Generate.CrabLanguage);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(name, language)),
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut.GetChanges();
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage("", language)),
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameNameWasCleared>()
                .Which.Should().HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameNameWasCorrectedForPrimary()
        {
            //Arrange
            var expectedPrimaryname = Arrange(Generate.StreetNameString);
            var streetNameId = Arrange(Generate.StreetNameId);
            var language = Arrange(Generate.CrabLanguageNullable);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(expectedPrimaryname, language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameNameWasCorrected>(x => x.Language == language?.ToLanguage())
                .Which.Should().HaveName(expectedPrimaryname)
                .And.HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language?.ToLanguage());
        }

        [Fact]
        public void NoAppliesStreetNameNameWasCorrectedForPrimaryWhenNull()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            CrabLanguage? language = null;
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, new CrabStreetName(null, null), Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabLifetime),
                CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().NotHaveAnyChange<StreetNameNameWasCorrected>(x => x.Language == language?.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameNameWasCorrectedForSecondary()
        {
            //Arrange
            var expectedName = Arrange(Generate.StreetNameString);
            var streetNameId = Arrange(Generate.StreetNameId);
            var language = Arrange(Generate.CrabLanguageNullable);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(expectedName, language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameNameWasCorrected>(x => x.Language == language?.ToLanguage())
                .Which.Should().HaveName(expectedName)
                .And.HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language?.ToLanguage());
        }

        [Fact]
        public void NoAppliesStreetNameNameWasCorrectedForSecondaryWhenNull()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            CrabLanguage? language = null;
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), new CrabStreetName(null, null), Arrange(Generate.CrabLifetime),
                CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().NotHaveAnyChange<StreetNameNameWasCorrected>(x => x.Language == language?.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameNameWasCorrectedToClearedForPrimary()
        {
            //Arrange
            var primaryname = Arrange(Generate.StreetNameString);
            var streetNameId = Arrange(Generate.StreetNameId);
            var language = Arrange(Generate.CrabLanguage);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(primaryname, language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);
            sut.ClearChanges();
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage("", language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameNameWasCorrectedToCleared>()
                .Which.Should().HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameNameWasCorrectedToClearedForSecondary()
        {
            //Arrange
            var streetName = Arrange(Generate.StreetNameString);
            var streetNameId = Arrange(Generate.StreetNameId);
            var language = Arrange(Generate.CrabLanguage);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);
            sut.ClearChanges();
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage("", language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameNameWasCorrectedToCleared>()
                .Which.Should().HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameNameWasNamedForPrimary()
        {
            //Arrange
            var expectedPrimaryname = Arrange(Generate.StreetNameString);
            var streetNameId = Arrange(Generate.StreetNameId);
            var language = Arrange(Generate.CrabLanguageNullable);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, new CrabStreetName(expectedPrimaryname, language), Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameWasNamed>(x => x.Language == language?.ToLanguage())
                .Which.Should().HaveName(expectedPrimaryname)
                .And.HaveStreetNameId(streetNameId)
                .And.HaveLanguage(language?.ToLanguage());
        }

        [Fact]
        public void NoAppliesStreetNameNameWasNamedForPrimaryWhenNull()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            CrabLanguage? language = null;
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, new CrabStreetName(null, null), Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().NotHaveAnyChange<StreetNameWasNamed>(x => x.Language == language?.ToLanguage());
        }

        [Fact]
        public void AppliesStreetNameNameWasNamedForSecondary()
        {
            //Arrange
            var expectedName = Arrange(Generate.StreetNameString);
            var streetNameId = Arrange(Generate.StreetNameId);
            var language = Arrange(Generate.CrabLanguageNullable);
            var sut = RegisterWithId(streetNameId);
            var secondaryStreetName = Arrange(Generate.CrabStreetNameExceptLanguage(language));

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(expectedName, language)), secondaryStreetName,
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameWasNamed>(x => x.Language == secondaryStreetName.Language?.ToLanguage())
                .Which.Should().HaveName(secondaryStreetName.Name)
                .And.HaveStreetNameId(streetNameId)
                .And.HaveLanguage(secondaryStreetName.Language?.ToLanguage());
        }

        [Fact]
        public void NoAppliesStreetNameNameWasNamedForSecondaryWhenNull()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            CrabLanguage? language = null;
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), new CrabStreetName(null, null), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().NotHaveAnyChange<StreetNameWasNamed>(x => x.Language == language?.ToLanguage());
        }

        [Fact]
        public void NoStreetNameHomonymAdditionWasClearedWhenAdditionIsNotChangedWhenPrimaryName()
        {
            //Arrange
            var streetName = Arrange(Generate.StreetNameWithHomonymString(""));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = CrabLanguage.Dutch;

            //Act
            sut = Act(sut, new CrabStreetName(streetName, language), Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().NotHaveAnyChange<StreetNameHomonymAdditionWasCleared>();
        }

        [Fact]
        public void NoStreetNameHomonymAdditionWasClearedWhenAdditionIsNotChangedWhenSecondaryName()
        {
            //Arrange
            var streetName = Arrange(Generate.StreetNameWithHomonymString(null));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = CrabLanguage.English;

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), new CrabStreetName(streetName, language), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().NotHaveAnyChange<StreetNameHomonymAdditionWasCleared>();
        }

        [Fact]
        public void NoStreetNameHomonymAdditionWasCorrectedToClearedWhenAdditionIsNotChangedForPrimary()
        {
            //Arrange
            var homonymAddition = "";
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = CrabLanguage.Dutch;

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);
            sut.ClearChanges();
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().NotHaveAnyChange<StreetNameHomonymAdditionWasCorrectedToCleared>();
        }

        [Fact]
        public void NoStreetNameHomonymAdditionWasCorrectedToClearedWhenAdditionIsNotChangedForSecondary()
        {
            //Arrange
            var homonymAddition = "";
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            CrabLanguage? language = null;

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);
            sut.ClearChanges();
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().NotHaveAnyChange<StreetNameHomonymAdditionWasCorrectedToCleared>();
        }

        [Fact]
        public void NoStreetNameHomonymAdditionWasCorrectedWhenAdditionIsNotChangedForPrimary()
        {
            //Arrange
            var homonymAddition = "UK";
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = CrabLanguage.Dutch;

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);
            sut.ClearChanges();
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().NotHaveAnyChange<StreetNameHomonymAdditionWasCorrected>();
            sut.Should().NotHaveAnyChange<StreetNameHomonymAdditionWasCorrectedToCleared>();
        }

        [Fact]
        public void NoStreetNameHomonymAdditionWasCorrectedWhenAdditionIsNotChangedForSecondary()
        {
            //Arrange
            var homonymAddition = "UK";
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            CrabLanguage? language = null;

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);
            sut.ClearChanges();
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().NotHaveAnyChange<StreetNameHomonymAdditionWasCorrected>();
            sut.Should().NotHaveAnyChange<StreetNameHomonymAdditionWasCorrectedToCleared>();
        }

        [Fact]
        public void NoStreetNameHomonymAdditionWasDefinedWhenAdditionIsNotChangedAndPrimaryStreetName()
        {
            //Arrange
            var homonymAddition = "UK";
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = CrabLanguage.Dutch;

            //Act
            sut = Act(sut, new CrabStreetName(streetName, language), Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.ClearChanges();

            sut = Act(sut, new CrabStreetName(streetName, language), Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().NotHaveAnyChange<StreetNameHomonymAdditionWasDefined>();
            sut.Should().NotHaveAnyChange<StreetNameHomonymAdditionWasCleared>();
        }

        [Fact]
        public void NoStreetNameHomonymAdditionWasDefinedWhenAdditionIsNotChangedAndSecondaryStreetName()
        {
            //Arrange
            var homonymAddition = "UK";
            var streetName = Arrange(Generate.StreetNameWithHomonymString(homonymAddition));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = CrabLanguage.Dutch;

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), new CrabStreetName(streetName, language), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut.ClearChanges();
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), new CrabStreetName(streetName, language), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().NotHaveAnyChange<StreetNameHomonymAdditionWasDefined>();
            sut.Should().NotHaveAnyChange<StreetNameHomonymAdditionWasCleared>();
        }

        [Fact]
        public void NoStreetNameNameWasClearedWhenNameIsNotChangedForPrimary()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = Arrange(Generate.CrabLanguageNullable);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(null, language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().NotHaveAnyChange<StreetNameNameWasCleared>();
        }

        [Fact]
        public void NoStreetNameNameWasClearedWhenNameIsNotChangedForSecondary()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = Arrange(Generate.CrabLanguageNullable);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(null, language)),
                Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().NotHaveAnyChange<StreetNameNameWasCleared>();
        }

        [Fact]
        public void NoStreetNameNameWasCorrectedToClearedWhenNameIsNotChangedForPrimary()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = Arrange(Generate.CrabLanguageNullable);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(null, language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().NotHaveAnyChange<StreetNameNameWasCorrectedToCleared>();
        }

        [Fact]
        public void NoStreetNameNameWasCorrectedToClearedWhenNameIsNotChangedForSecondary()
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            var language = Arrange(Generate.CrabLanguageNullable);

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(null, language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().NotHaveAnyChange<StreetNameNameWasCorrectedToCleared>();
        }

        [Fact]
        public void NoStreetNameNameWasCorrectedWhenNameIsNotChangedForPrimary()
        {
            //Arrange
            var streetName = Arrange(Generate.StreetNameString);
            var streetNameId = Arrange(Generate.StreetNameId);
            var language = Arrange(Generate.CrabLanguageNullable);
            var secondaryLanguage = Arrange(Generate.NullableEnumExcept(language));
            var sut = RegisterWithId(streetNameId);

            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)), Arrange(Generate.CrabStreetNameWithLanguage(secondaryLanguage)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);
            sut.ClearChanges();

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)), Arrange(Generate.CrabStreetNameWithLanguage(secondaryLanguage)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().NotHaveAnyChange<StreetNameNameWasCorrected>(x => x.Language == language?.ToLanguage());
        }

        [Fact]
        public void NoStreetNameNameWasCorrectedWhenNameIsNotChangedForSecondary()
        {
            //Arrange
            var streetName = Arrange(Generate.StreetNameString);
            var streetNameId = Arrange(Generate.StreetNameId);
            var language = Arrange(Generate.CrabLanguageNullable);
            var primaryLanguage = Arrange(Generate.NullableEnumExcept(language));
            var sut = RegisterWithId(streetNameId);

            sut = Act(sut, Arrange(Generate.CrabStreetNameWithLanguage(primaryLanguage)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);
            sut.ClearChanges();

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithLanguage(primaryLanguage)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)),
                Arrange(Generate.CrabLifetime), CrabModification.Correction);

            sut.Should().NotHaveAnyChange<StreetNameNameWasCorrected>(x => x.Language == language?.ToLanguage());
        }

        [Fact]
        public void NoStreetNameNameWasNamedWhenNameIsNotChangedForPrimary()
        {
            //Arrange
            var streetName = Arrange(Generate.StreetNameString);
            var streetNameId = Arrange(Generate.StreetNameId);
            var language = Arrange(Generate.CrabLanguageNullable);

            var sut = RegisterWithId(streetNameId);

            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut.ClearChanges();

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)), Arrange(Generate.CrabStreetNameExceptLanguage(language)),
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().NotHaveAnyChange<StreetNameWasNamed>(x => x.Language == language?.ToLanguage());
        }

        [Fact]
        public void NoStreetNameNameWasNamedWhenNameIsNotChangedForSecondary()
        {
            //Arrange
            var streetName = Arrange(Generate.StreetNameString);
            var streetNameId = Arrange(Generate.StreetNameId);
            var language = Arrange(Generate.CrabLanguageNullable);
            var sut = RegisterWithId(streetNameId);

            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)),
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));
            sut.ClearChanges();

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameExceptLanguage(language)), Arrange(Generate.CrabStreetNameWithNameAndLanguage(streetName, language)),
                Arrange(Generate.CrabLifetime), Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().NotHaveAnyChange<StreetNameWasNamed>(x => x.Language == language?.ToLanguage());
        }
    }
}
