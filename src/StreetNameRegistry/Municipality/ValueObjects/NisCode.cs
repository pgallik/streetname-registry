namespace StreetNameRegistry.Municipality
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Exceptions;

    public class NisCode : StringValueObject<NisCode>
    {
        public NisCode(string nisCode) : base(nisCode)
        {
            if (string.IsNullOrWhiteSpace(nisCode))
                throw new NoNisCodeHasNoValueException("NisCode of a municipality cannot be empty.");
        }
    }
}
