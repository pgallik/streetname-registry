namespace StreetNameRegistry.Exceptions
{
    using System;

    public class StreetNameNameLanguageNotSupportedException : StreetNameRegistryException
    {
        public StreetNameNameLanguageNotSupportedException() { }

        public StreetNameNameLanguageNotSupportedException(string message) : base(message) { }

        public StreetNameNameLanguageNotSupportedException(string message, Exception inner) : base(message, inner) { }
    }
}
