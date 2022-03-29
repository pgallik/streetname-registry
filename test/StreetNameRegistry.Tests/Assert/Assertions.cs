namespace StreetNameRegistry.Tests.Assert
{
    using System;
    using System.Diagnostics;
    using FluentAssertions;
    using FluentAssertions.Primitives;
    using Testing;

    // <summary>
    // Allows for changing the default (Trace.WriteLine) log behaviour of Assertions
    // </summary>
    public static class Assertions
    {
        public static Action<string> LogAction =>
            TestScope.LogAction ?? (message => Trace.WriteLine(message));
    }

    public abstract class Assertions<T, TAssertion> : ReferenceTypeAssertions<T, TAssertion>
        where TAssertion : ReferenceTypeAssertions<T, TAssertion>
    {
        private readonly Action<string> _logAction;

        protected Assertions(T subject) : base(subject)
        {
            var notNullLogaction = Assertions.LogAction ?? (message => Trace.WriteLine(message));
            _logAction = message => notNullLogaction($"{"ASSERT".PadRight(10)}{Identifier}: {message}");
        }

        protected override string Identifier => typeof(T).Name;

        protected void AssertingThat(string message)
        {
            _logAction(string.Format("Asserting that {0}.", message));
        }

        protected AndConstraint<TAssertion> And()
        {
            return new AndConstraint<TAssertion>(this as TAssertion);
        }

        protected AndWhichConstraint<TAssertion, TWhich> AndWhich<TWhich>(TWhich newSubject)
        {
            return new AndWhichConstraint<TAssertion, TWhich>(this as TAssertion, newSubject);
        }
    }
}
