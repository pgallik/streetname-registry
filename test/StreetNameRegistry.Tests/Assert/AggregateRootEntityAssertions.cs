namespace StreetNameRegistry.Tests.Assert
{
    using System;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using FluentAssertions;

    public sealed class AggregateRootEntityAssertions : Assertions<AggregateRootEntity, AggregateRootEntityAssertions>
    {
        public AggregateRootEntityAssertions(AggregateRootEntity subject) : base(subject)
        {
        }

        public AndConstraint<AggregateRootEntityAssertions> HaveChanges()
        {
            AssertingThat("changes were applied");

            Subject.GetChanges().Should().HaveCountGreaterThan(0);

            return And();
        }

        public AndWhichConstraint<AggregateRootEntityAssertions, T> HaveFirstChange<T>(Func<T, bool> condition = null)
        {
            AssertingThat($"a {typeof(T).Name} was applied as first change");

            Subject.GetChanges().OfType<T>().Where(condition ?? (x => true)).Should().HaveCount(1);

            return new AndWhichConstraint<AggregateRootEntityAssertions, T>(this,
                Subject.GetChanges().OfType<T>().Where(condition ?? (x => true)).Single());
        }

        public AndWhichConstraint<AggregateRootEntityAssertions, T> HaveSingleChange<T>(Func<T, bool> condition = null)
        {
            AssertingThat($"a {typeof(T).Name} was applied");

            Subject.GetChanges().OfType<T>().Where(condition??(x=>true)).Should().HaveCount(1);

            return new AndWhichConstraint<AggregateRootEntityAssertions, T>(this,
                Subject.GetChanges().OfType<T>().Where(condition ?? (x => true)).Single());
        }

        public AndConstraint<AggregateRootEntityAssertions> NotHaveAnyChange<T>(Func<T, bool> condition = null)
        {
            AssertingThat($"no {typeof(T).Name} was applied");

            Subject.GetChanges().OfType<T>().Where(condition??(x=>true)).Should().BeEmpty();

            return And();
        }

        internal AndConstraint<AggregateRootEntityAssertions> HaveSingleChange(Type type)
        {
            AssertingThat($"a {type.Name} was applied");

            Subject.GetChanges().Where(t => t.GetType() == type).Should().HaveCount(1);

            //var andWhich = typeof(AndWhichConstraint<,>)
            //    .MakeGenericType(typeof(AggregateRootEntityAssertions), type)
            //    .GetConstructor(new []{this.GetType(), type})
            //    .Invoke(new object[]{this, Subject.GetChanges().Single(t => t.GetType() == type)});

            return And();
        }

        public AndConstraint<AggregateRootEntityAssertions> HaveCountOfChanges<T>(int count)
        {
            AssertingThat($"{typeof(T).Name} was applied {count} times");

            Subject.GetChanges().OfType<T>().Should().HaveCount(count);

            return And();
        }
    }
}
