namespace StreetNameRegistry.Api.Oslo.Convertors
{
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Straatnaam;

    public static class StreetNameStatusExtensions
    {
        public static StraatnaamStatus ConvertFromStreetNameStatus(this StreetNameStatus? status)
            => ConvertFromStreetNameStatus(status ?? StreetNameStatus.Current);

        public static StraatnaamStatus ConvertFromStreetNameStatus(this StreetNameStatus status)
        {
            switch (status)
            {
                case StreetNameStatus.Retired:
                    return StraatnaamStatus.Gehistoreerd;

                case StreetNameStatus.Proposed:
                    return StraatnaamStatus.Voorgesteld;

                default:
                case StreetNameStatus.Current:
                    return StraatnaamStatus.InGebruik;
            }
        }
    }
}
