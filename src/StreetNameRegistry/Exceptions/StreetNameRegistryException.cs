namespace StreetNameRegistry.Exceptions
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public abstract class StreetNameRegistryException : DomainException
    {
        protected StreetNameRegistryException() { }

        protected StreetNameRegistryException(string message) : base(message) { }

        protected StreetNameRegistryException(string message, Exception inner) : base(message, inner) { }
    }
}
