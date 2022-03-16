namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;

    public class MunicipalityHasUnexpectedStatusException : StreetNameRegistryException
    {
        public MunicipalityStatus Actual { get; }
        public MunicipalityStatus Expected { get; }

        public MunicipalityHasUnexpectedStatusException(MunicipalityStatus actual, MunicipalityStatus expected)
        {
            Actual = actual;
            Expected = expected;
        }

        public MunicipalityHasUnexpectedStatusException(string message) : base(message) { }

        public MunicipalityHasUnexpectedStatusException(string message, Exception inner) : base(message, inner) { }
    }
}
