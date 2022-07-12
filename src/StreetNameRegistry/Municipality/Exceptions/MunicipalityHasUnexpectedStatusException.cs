namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;

    public class MunicipalityHasUnexpectedStatusException : StreetNameRegistryException
    {
        public MunicipalityHasUnexpectedStatusException()
        { }

        public MunicipalityHasUnexpectedStatusException(string message) : base(message) { }

        public MunicipalityHasUnexpectedStatusException(string message, Exception inner) : base(message, inner) { }
    }
}
