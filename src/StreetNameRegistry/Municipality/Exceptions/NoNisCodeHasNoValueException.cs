namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class NoNisCodeHasNoValueException : StreetNameRegistryException
    {
        public NoNisCodeHasNoValueException()
        { }
        
        private NoNisCodeHasNoValueException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public NoNisCodeHasNoValueException(string message)
            : base(message)
        { }

        public NoNisCodeHasNoValueException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
