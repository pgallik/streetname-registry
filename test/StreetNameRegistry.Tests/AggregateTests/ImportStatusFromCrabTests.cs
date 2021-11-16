namespace StreetNameRegistry.Tests.AggregateTests
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Assert;
    using FluentAssertions;
    using StreetName;
    using StreetName.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;
    using Generate;

    public class ImportStatusFromCrabStatusChangedTests : ImportStatusFromCrabTest
    {
        public ImportStatusFromCrabStatusChangedTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void AppliesStreetNameStatusWasRemovedAndBecameIncomplete()
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
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection)
            );

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameStatusWasRemoved>()
                .Which.Should().HaveStreetNameId(streetNameId);

            sut.Should().HaveSingleChange<StreetNameBecameIncomplete>()
                .Which.Should().HaveStreetNameId(streetNameId);
        }

        [Fact]
        public void AppliesStreetNameStatusWasRemovedEventWhenModificationIsDeleted()
        {
            //Arrange
            var givenModification = CrabModification.Delete;
            var streetNameId = Arrange(Generate.StreetNameId);
            var streetNameStatusId = Arrange(Generate.CrabStreetNameStatusId);
            var sut = RegisterWithId(streetNameId);

            sut = Act(sut, Arrange(Generate.EnumExcept<CrabStreetNameStatus>(CrabStreetNameStatus.Unknown)), Arrange(Generate.CrabLifetime),
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection), streetNameStatusId);

            sut.ClearChanges();

            //Act
            sut = Act(sut, CrabStreetNameStatus.InUse,
                Arrange(Generate.CrabLifetime),
                givenModification,
                streetNameStatusId);

            //Assert
            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameStatusWasRemoved>();
            sut.Should().NotHaveAnyChange<StreetNameBecameCurrent>();
        }

        [Fact]
        public void DoesNotApplyStreetNameStatusEventWhenNIsRetired()
        {
            //Arrange
            var givenModification = Arrange(Generate.CrabModificationNullableExceptDeleteCorrection);
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

        [Fact]
        public void ThrowsWhenStreetNameAlreadyRemovedAndModificationNotDelete()
        {
            //Arrange
            var givenModification = Arrange(Generate.NullableEnumExcept<CrabModification>(CrabModification.Delete));
            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);
            sut = EnsureStreetNameRemoved(sut);

            //Act
            Action act = () => sut = Act(sut, Arrange(Generate.CrabStreetNameStatus), Arrange(Generate.CrabLifetimeWithEndDate(null)), givenModification);

            //Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [InlineData(CrabStreetNameStatus.Proposed)]
        [InlineData(CrabStreetNameStatus.Reserved)]
        public void AppliesStreetNameWasProposed(CrabStreetNameStatus givenStatus)
        {
            //Arrange
            var lifetime = Arrange(Generate.CrabLifetimeWithEndDate(null));

            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, givenStatus, lifetime, Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameWasProposed>()
                .Which.Should().HaveStreetNameId(streetNameId);
            AssertBecameComplete(sut, streetNameId);
        }

        [Theory]
        [InlineData(CrabStreetNameStatus.InUse)]
        [InlineData(CrabStreetNameStatus.OutOfUse)]
        public void AppliesStreetNameBecameCurrent(CrabStreetNameStatus givenStatus)
        {
            //Arrange
            var lifetime = Arrange(Generate.CrabLifetimeWithEndDate(null));

            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);

            //Act
            sut = Act(sut, givenStatus, lifetime, Arrange(Generate.CrabModificationNullableExceptDeleteCorrection));

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange<StreetNameBecameCurrent>()
                .Which.Should().HaveStreetNameId(streetNameId);
            AssertBecameComplete(sut, streetNameId);
        }

        [Theory]
        [InlineData(CrabStreetNameStatus.Reserved, typeof(StreetNameWasProposed))]
        [InlineData(CrabStreetNameStatus.Proposed, typeof(StreetNameWasProposed))]
        [InlineData(CrabStreetNameStatus.InUse, typeof(StreetNameBecameCurrent))]
        [InlineData(CrabStreetNameStatus.OutOfUse, typeof(StreetNameBecameCurrent))]
        public void AppliesStreetNameStatusEventOnlyWhenChanged(CrabStreetNameStatus givenStatus, Type type)
        {
            //Arrange
            var lifetime = Arrange(Generate.CrabLifetimeWithEndDate(null));

            var streetNameId = Arrange(Generate.StreetNameId);
            var sut = RegisterWithId(streetNameId);


            //Act
            sut = Act(sut, givenStatus,
                lifetime,
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection)
            );

            sut = Act(sut, givenStatus,
                lifetime,
                Arrange(Generate.CrabModificationNullableExceptDeleteCorrection)
            );

            sut.Should().HaveChanges();
            sut.Should().HaveSingleChange(type);
            sut.Should().HaveSingleChange<StreetNameBecameComplete>();
            sut.Should().NotHaveAnyChange<StreetNameBecameIncomplete>();
        }

        [Fact]
        public void AppliesCorrectStatusChangedWhenOlderStatusWasCorrected()
        {
            //1: GivenEvents set status
            //2: => import with new status newer lifetime
            //3: => import with status from step 1, with same begin, but different end

            var register = Arrange(Generate.StreetNameWasRegistered);

            var sut = StreetName.Factory();

            sut.Initialize(new List<object> { register });

            var firstDate = new DateTime(1830, 1, 1).ToCrabLocalDateTime();
            var secondDate = new DateTime(2015, 1, 1).ToCrabLocalDateTime();
            var firstStatusId = Arrange(Generate.CrabStreetNameStatusId);

            sut = Act(sut, CrabStreetNameStatus.InUse, new CrabLifetime(firstDate, null), CrabModification.Historize, firstStatusId);
            sut = Act(sut, CrabStreetNameStatus.Proposed, new CrabLifetime(secondDate, null), CrabModification.Insert);
            sut = Act(sut, CrabStreetNameStatus.InUse, new CrabLifetime(firstDate, secondDate), CrabModification.Correction, firstStatusId);

            sut.Should().HaveSingleChange<StreetNameBecameCurrent>();
            sut.Should().HaveSingleChange<StreetNameWasProposed>();
            sut.Should().NotHaveAnyChange<StreetNameWasCorrectedToCurrent>();
            sut.Should().NotHaveAnyChange<StreetNameWasCorrectedToProposed>();
        }

        [Fact]
        public void AppliesCorrectStatusChangedWhenOlderStatusWasInserted()
        {
            //1: GivenEvents set status
            //2: => import with new status older lifetime
            //3: => import with status from step 1, with same begin, but different end

            var register = Arrange(Generate.StreetNameWasRegistered);

            var sut = StreetName.Factory();

            sut.Initialize(new List<object> { register });

            var firstDate = new DateTime(1830, 1, 1).ToCrabLocalDateTime();
            var secondDate = new DateTime(2015, 1, 1).ToCrabLocalDateTime();
            var firstStatusId = Arrange(Generate.CrabStreetNameStatusId);

            sut = Act(sut, CrabStreetNameStatus.InUse, new CrabLifetime(secondDate, null), CrabModification.Historize, firstStatusId);
            sut = Act(sut, CrabStreetNameStatus.Proposed, new CrabLifetime(firstDate, null), CrabModification.Insert);
            sut = Act(sut, CrabStreetNameStatus.InUse, new CrabLifetime(firstDate, secondDate), CrabModification.Correction, firstStatusId);

            sut.Should().HaveSingleChange<StreetNameBecameCurrent>();
            sut.Should().NotHaveAnyChange<StreetNameWasProposed>();
            sut.Should().NotHaveAnyChange<StreetNameWasCorrectedToCurrent>();
            sut.Should().NotHaveAnyChange<StreetNameWasCorrectedToProposed>();
        }

        [Fact]
        public void AppliesStatusChangedWhenStatusWasInsertedAfterAnotherWasRemoved()
        {
            //1: GivenEvents set status
            //2: => delete status from step 1
            //3: => import new status with same lifetime
            //

            var register = Arrange(Generate.StreetNameWasRegistered);

            var sut = StreetName.Factory();

            sut.Initialize(new List<object> { register });

            var crabLifetime = new CrabLifetime(new DateTime(1830, 1, 1).ToCrabLocalDateTime(), null);
            var firstStatusId = Arrange(Generate.CrabStreetNameStatusId);

            sut = Act(sut, CrabStreetNameStatus.InUse, crabLifetime, CrabModification.Historize, firstStatusId);
            sut = Act(sut, CrabStreetNameStatus.InUse, crabLifetime, CrabModification.Delete, firstStatusId);

            sut.Should().HaveSingleChange<StreetNameStatusWasRemoved>();
            sut.Should().HaveSingleChange<StreetNameBecameIncomplete>();

            sut.ClearChanges();

            sut = Act(sut, CrabStreetNameStatus.Proposed, crabLifetime, CrabModification.Insert);

            sut.Should().HaveSingleChange<StreetNameWasProposed>();
            sut.Should().HaveSingleChange<StreetNameBecameComplete>();
            sut.Should().NotHaveAnyChange<StreetNameBecameCurrent>();
            sut.Should().NotHaveAnyChange<StreetNameWasCorrectedToCurrent>();
            sut.Should().NotHaveAnyChange<StreetNameWasCorrectedToProposed>();
        }

        [Fact]
        public void NoChangesWhenStatusWasDeletedOfAnNonCurrentStatus()
        {
            //1: GivenEvents set status
            //2: => import new status (new id)
            //3: => import with status from step 1 with modification delete
            var register = Arrange(Generate.StreetNameWasRegistered);
            var sut = StreetName.Factory();
            sut.Initialize(new List<object> { register });

            var firstStatusId = Arrange(Generate.CrabStreetNameStatusId);
            var secondStatusId = Arrange(Generate.CrabStreetNameStatusId);

            var lifetime = new CrabLifetime(DateTime.MinValue.ToCrabLocalDateTime(), null);

            Act(sut, CrabStreetNameStatus.Proposed, lifetime, CrabModification.Insert, firstStatusId);
            Act(sut, CrabStreetNameStatus.InUse, lifetime, CrabModification.Insert, secondStatusId);
            Act(sut, CrabStreetNameStatus.Proposed, lifetime, CrabModification.Delete, firstStatusId);

            sut.Should().NotHaveAnyChange<StreetNameStatusWasRemoved>();
        }

        private AndConstraint<StreetNameBecameCompleteAssertions> AssertBecameComplete(StreetName sut, StreetNameId id)
        {
            return sut.Should().HaveSingleChange<StreetNameBecameComplete>()
                .Which.Should().HaveStreetNameId(id);
        }
    }
}
