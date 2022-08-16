namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;

    public class MunicipalityHasInvalidStatusException : StreetNameRegistryException
    {
        public MunicipalityHasInvalidStatusException()
        { }

        public MunicipalityHasInvalidStatusException(string message) : base(message) { }

        public MunicipalityHasInvalidStatusException(string message, Exception inner) : base(message, inner) { }
    }
}
