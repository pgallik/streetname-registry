namespace StreetNameRegistry.Tests.Assert
{
    using StreetName.Events;

    public sealed class StreetNameBecameCurrentAssertions :
        HasStreetNameIdAssertions<StreetNameBecameCurrent, StreetNameBecameCurrentAssertions>
    {
        public StreetNameBecameCurrentAssertions(StreetNameBecameCurrent subject) : base(subject)
        {
        }
    }
}
