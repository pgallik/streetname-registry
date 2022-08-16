namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;

    public class StreetNameIsMissingALanguageException : StreetNameRegistryException
    {
        public StreetNameIsMissingALanguageException() { }

        public StreetNameIsMissingALanguageException(string message) : base(message) { }

        public StreetNameIsMissingALanguageException(string message, Exception inner) : base(message, inner) { }
    }
}
