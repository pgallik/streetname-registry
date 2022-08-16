namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;

    public class NoNisCodeHasNoValueException : StreetNameRegistryException
    {
        public NoNisCodeHasNoValueException() { }

        public NoNisCodeHasNoValueException(string message) : base(message) { }

        public NoNisCodeHasNoValueException(string message, Exception inner) : base(message, inner) { }
    }
}
