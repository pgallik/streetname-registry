namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    [Serializable]
    public abstract class StreetNameRegistryException : DomainException
    {
        protected StreetNameRegistryException() { }

        protected StreetNameRegistryException(string message) : base(message) { }

        protected StreetNameRegistryException(string message, Exception inner) : base(message, inner) { }

        protected StreetNameRegistryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
