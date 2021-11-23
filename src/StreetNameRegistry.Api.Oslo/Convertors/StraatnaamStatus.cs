namespace StreetNameRegistry.Api.Oslo.Convertors
{
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Straatnaam;

    public static class StraatnaamStatusExtensions
    {
        public static StreetNameStatus ConvertFromStraatnaamStatus(this StraatnaamStatus? status)
            => ConvertFromStraatnaamStatus(status ?? StraatnaamStatus.InGebruik);

        public static StreetNameStatus ConvertFromStraatnaamStatus(this StraatnaamStatus status)
        {
            switch (status)
            {
                default:
                case StraatnaamStatus.InGebruik:
                    return StreetNameStatus.Current;

                case StraatnaamStatus.Gehistoreerd:
                    return StreetNameStatus.Retired;

                case StraatnaamStatus.Voorgesteld:
                    return StreetNameStatus.Proposed;
            }
        }
    }
}
