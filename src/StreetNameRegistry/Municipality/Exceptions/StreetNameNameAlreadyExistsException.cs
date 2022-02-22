namespace StreetNameRegistry.Municipality.Exceptions
{
    public class StreetNameNameAlreadyExistsException : StreetNameRegistryException
    {
        /// <summary>
        /// The name which already exists
        /// </summary>
        public string Name { get; }

        public StreetNameNameAlreadyExistsException() { }

        public StreetNameNameAlreadyExistsException(string name)
        {
            Name = name;
        }
    }
}
