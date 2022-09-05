namespace StreetNameRegistry.Municipality.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class StreetNameNameAlreadyExistsException : StreetNameRegistryException
    {
        /// <summary>
        /// The name which already exists
        /// </summary>
        public string Name { get; }

        public StreetNameNameAlreadyExistsException()
        {
            Name = string.Empty;
        }

        public StreetNameNameAlreadyExistsException(string name)
        {
            Name = name;
        }

        private StreetNameNameAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Name = string.Empty;
        }
    }
}
