namespace StreetNameRegistry.Exceptions
{
    using System;

    public class MunicipalityWasRetiredException : StreetNameRegistryException
    {
        public MunicipalityWasRetiredException() { }

        public MunicipalityWasRetiredException(string message) : base(message) { }

        public MunicipalityWasRetiredException(string message, Exception inner) : base(message, inner) { }
    }
}
