namespace StreetNameRegistry.Exceptions
{
    using System;

    public class NoNisCodeException : StreetNameRegistryException
    {
        public NoNisCodeException() { }

        public NoNisCodeException(string message) : base(message) { }

        public NoNisCodeException(string message, Exception inner) : base(message, inner) { }
    }
}
