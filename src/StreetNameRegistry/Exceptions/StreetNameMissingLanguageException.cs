namespace StreetNameRegistry.Exceptions
{
    using System;

    public class StreetNameMissingLanguageException : StreetNameRegistryException
    {
        public StreetNameMissingLanguageException() { }

        public StreetNameMissingLanguageException(string message) : base(message) { }

        public StreetNameMissingLanguageException(string message, Exception inner) : base(message, inner) { }
    }
}
