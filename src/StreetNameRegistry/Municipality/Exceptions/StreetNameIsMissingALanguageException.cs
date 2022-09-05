namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class StreetNameIsMissingALanguageException : StreetNameRegistryException
    {
        public StreetNameIsMissingALanguageException()
        { }
        
        private StreetNameIsMissingALanguageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public StreetNameIsMissingALanguageException(string message)
            : base(message)
        { }

        public StreetNameIsMissingALanguageException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
