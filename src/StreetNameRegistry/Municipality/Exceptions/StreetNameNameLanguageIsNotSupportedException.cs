namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class StreetNameNameLanguageIsNotSupportedException : StreetNameRegistryException
    {
        public StreetNameNameLanguageIsNotSupportedException() { }

        private StreetNameNameLanguageIsNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        public StreetNameNameLanguageIsNotSupportedException(string message)
            : base(message)
        { }

        public StreetNameNameLanguageIsNotSupportedException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
