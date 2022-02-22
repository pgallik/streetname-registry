namespace StreetNameRegistry.Api.Legacy.Convertors
{
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy.Straatnaam;

    public static class StraatnaamStatusExtensions
    {
        public static StreetNameRegistry.StreetName.StreetNameStatus ConvertToStreetNameStatus(this StraatnaamStatus? status)
            => ConvertToStreetNameStatus(status ?? StraatnaamStatus.InGebruik);

        public static StreetNameRegistry.StreetName.StreetNameStatus ConvertToStreetNameStatus(this StraatnaamStatus status)
        {
            switch (status)
            {
                default:
                case StraatnaamStatus.InGebruik:
                    return StreetNameRegistry.StreetName.StreetNameStatus.Current;

                case StraatnaamStatus.Gehistoreerd:
                    return StreetNameRegistry.StreetName.StreetNameStatus.Retired;

                case StraatnaamStatus.Voorgesteld:
                    return StreetNameRegistry.StreetName.StreetNameStatus.Proposed;
            }
        }

        public static StreetNameRegistry.Municipality.StreetNameStatus ConvertToMunicipalityStreetNameStatus(this StraatnaamStatus? status)
            => ConvertToMunicipalityStreetNameStatus(status ?? StraatnaamStatus.InGebruik);

        public static StreetNameRegistry.Municipality.StreetNameStatus ConvertToMunicipalityStreetNameStatus(this StraatnaamStatus status)
        {
            switch (status)
            {
                default:
                case StraatnaamStatus.InGebruik:
                    return StreetNameRegistry.Municipality.StreetNameStatus.Current;

                case StraatnaamStatus.Gehistoreerd:
                    return StreetNameRegistry.Municipality.StreetNameStatus.Retired;

                case StraatnaamStatus.Voorgesteld:
                    return StreetNameRegistry.Municipality.StreetNameStatus.Proposed;
            }
        }
    }
}
