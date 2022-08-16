namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;

    public class StreetNameNameLanguageIsNotSupportedException : StreetNameRegistryException
    {
        public StreetNameNameLanguageIsNotSupportedException() { }

        public StreetNameNameLanguageIsNotSupportedException(string message) : base(message) { }

        public StreetNameNameLanguageIsNotSupportedException(string message, Exception inner) : base(message, inner) { }
    }
}
