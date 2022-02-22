namespace StreetNameRegistry.Api.Oslo.Convertors
{
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Straatnaam;

    public static class StreetNameStatusExtensions
    {
        public static StraatnaamStatus ConvertFromStreetNameStatus(this StreetNameRegistry.StreetName.StreetNameStatus? status)
            => ConvertFromStreetNameStatus(status ?? StreetNameRegistry.StreetName.StreetNameStatus.Current);

        public static StraatnaamStatus ConvertFromStreetNameStatus(this StreetNameRegistry.StreetName.StreetNameStatus status)
        {
            switch (status)
            {
                case StreetNameRegistry.StreetName.StreetNameStatus.Retired:
                    return StraatnaamStatus.Gehistoreerd;

                case StreetNameRegistry.StreetName.StreetNameStatus.Proposed:
                    return StraatnaamStatus.Voorgesteld;

                default:
                case StreetNameRegistry.StreetName.StreetNameStatus.Current:
                    return StraatnaamStatus.InGebruik;
            }
        }

        public static StraatnaamStatus ConvertFromMunicipalityStreetNameStatus(this StreetNameRegistry.Municipality.StreetNameStatus? status)
            => ConvertFromMunicipalityStreetNameStatus(status ?? StreetNameRegistry.Municipality.StreetNameStatus.Current);

        public static StraatnaamStatus ConvertFromMunicipalityStreetNameStatus(this StreetNameRegistry.Municipality.StreetNameStatus status)
        {
            switch (status)
            {
                case StreetNameRegistry.Municipality.StreetNameStatus.Retired:
                    return StraatnaamStatus.Gehistoreerd;

                case StreetNameRegistry.Municipality.StreetNameStatus.Proposed:
                    return StraatnaamStatus.Voorgesteld;

                default:
                case StreetNameRegistry.Municipality.StreetNameStatus.Current:
                    return StraatnaamStatus.InGebruik;
            }
        }
    }
}
