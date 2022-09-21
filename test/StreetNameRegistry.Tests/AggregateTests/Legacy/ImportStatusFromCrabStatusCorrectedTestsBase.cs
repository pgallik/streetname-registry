namespace StreetNameRegistry.Tests.AggregateTests.Legacy
{
    using System;
    using Assert;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Generate;
    using StreetName.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public sealed class ImportStatusFromCrabStatusCorrectedTestsBase : ImportStatusFromCrabTest
    {
        public ImportStatusFromCrabStatusCorrectedTestsBase(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void AppliesStreetNameStatusWasCorrectedToRemoved()
        {
            //Arrange
            var givenStatus = CrabStreetNameStatus.Unknown;

            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut,
                Arrange(Generate.EnumExcept<CrabStreetNameStatus>(CrabStreetNameStatus.Unknown)),
                new CrabLifetime(DateTime.Now.AddDays(-1).ToCrabLocalDateTime(), null),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection)
            );

            sut = Act(sut, givenStatus,
                new CrabLifetime(DateTime.Now.ToCrabLocalDateTime(), null),
                CrabModification.Correction
            );

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameStatusWasCorrectedToRemoved>()
                .Which.Should().HaveStreetNameId(streetNameId);
        }

        [Theory]
        [InlineData(CrabStreetNameStatus.Proposed)]
        [InlineData(CrabStreetNameStatus.Reserved)]
        public void AppliesStreetNameStatusWasCorrectedToProposed(CrabStreetNameStatus givenStatus)
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            var lifetime = Arrange(Generate.CrabLifetimeWithEndDate(null));
            var statusId = Arrange(Generate.CrabStreetNameStatusId);

            //Act
            sut = Act(sut,
                Arrange(Generate.EnumExcept<CrabStreetNameStatus>(CrabStreetNameStatus.Proposed, CrabStreetNameStatus.Reserved)),
                lifetime,
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection),
                statusId);

            sut = Act(sut, givenStatus,
                lifetime,
                CrabModification.Correction,
                statusId);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameWasCorrectedToProposed>()
                .Which.Should().HaveStreetNameId(streetNameId);
        }

        [Theory]
        [InlineData(CrabStreetNameStatus.InUse)]
        [InlineData(CrabStreetNameStatus.OutOfUse)]
        public void AppliesStreetNameStatusWasCorrectedToCurrent(CrabStreetNameStatus givenStatus)
        {
            //Arrange
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            var lifetime = Arrange(Generate.CrabLifetimeWithEndDate(null));
            var statusId = Arrange(Generate.CrabStreetNameStatusId);

            //Act
            sut = Act(sut,
                Arrange(Generate.EnumExcept<CrabStreetNameStatus>(CrabStreetNameStatus.InUse, CrabStreetNameStatus.OutOfUse)),
                lifetime,
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection),
                statusId);

            sut = Act(sut, givenStatus,
                lifetime,
                CrabModification.Correction,
                statusId);

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameWasCorrectedToCurrent>()
                .Which.Should().HaveStreetNameId(streetNameId);
        }

        [Fact]
        public void DoesNotApplyStreetNameStatusEventWhenStreetNameIsRetired()
        {
            //Arrange
            var givenModification = CrabModification.Correction;
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            sut.ImportFromCrab(Arrange(Generate.CrabStreetNameId),
                Arrange(Generate.CrabMunicipalityId),
                Arrange(Generate.CrabStreetName),
                Arrange(Generate.CrabStreetName),
                Arrange(Generate.CrabTransStreetName),
                Arrange(Generate.CrabTransStreetName),
                Arrange(Generate.CrabLanguageNullable),
                Arrange(Generate.CrabLanguageNullable),
                Arrange(Generate.CrabLifetimeWithEndDate(DateTime.Now)),
                Arrange(Generate.CrabTimestamp),
                Arrange(Generate.CrabOperator),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection),
                Arrange(Generate.CrabOrganisationNullable));

            sut.ClearChanges();

            //Act
            sut = Act(sut, Arrange(Generate.CrabStreetNameStatus),
                Arrange(Generate.CrabLifetime), givenModification);

            //Assert
            sut.Should().HaveChanges();
            sut.Should().NotHaveAnyChange<StreetNameBecameCurrent>();
            sut.Should().NotHaveAnyChange<StreetNameWasProposed>();
            sut.Should().NotHaveAnyChange<StreetNameStatusWasCorrectedToRemoved>();
            sut.Should().NotHaveAnyChange<StreetNameWasCorrectedToCurrent>();
            sut.Should().NotHaveAnyChange<StreetNameWasCorrectedToProposed>();
            sut.Should().NotHaveAnyChange<StreetNameStatusWasRemoved>();
            sut.Should().NotHaveAnyChange<StreetNameWasRetired>();
        }
    }
}
