namespace StreetNameRegistry.StreetName
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Municipality.Exceptions;
    using Newtonsoft.Json;

    public sealed class NisCode : StringValueObject<NisCode>
    {
        public NisCode([JsonProperty("value")] string nisCode) : base(nisCode)
        {
            if (string.IsNullOrWhiteSpace(nisCode))
                throw new NoNisCodeHasNoValueException("NisCode of a municipality cannot be empty.");
        }
    }
}
