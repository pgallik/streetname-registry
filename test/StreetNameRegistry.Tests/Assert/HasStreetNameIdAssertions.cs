namespace StreetNameRegistry.Tests.Assert
{
    using System;
    using FluentAssertions;
    using FluentAssertions.Primitives;
    using StreetName;
    using StreetName.Events;

    public abstract class HasStreetNameIdAssertions<T, TAssertions> :
        Assertions<T, TAssertions>
        where TAssertions : ReferenceTypeAssertions<T, TAssertions>
        where T : IHasStreetNameId
    {
        protected HasStreetNameIdAssertions(T subject) : base(subject)
        {
        }

        public AndConstraint<TAssertions> HaveStreetNameId(StreetNameId streetNameId)
        {
            AssertingThat($"the StreetNameId is {streetNameId}");

            Subject.StreetNameId.Should().Be((Guid)streetNameId);

            return And();
        }
    }
}
