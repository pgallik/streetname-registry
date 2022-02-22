namespace StreetNameRegistry.Api.Legacy.Convertors
{
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;

    public static class TaalExtensions
    {
        public static StreetNameRegistry.StreetName.Language ConvertToStreetNameLanguage(this Taal? taal)
            => ConvertToStreetNameLanguage(taal ?? Taal.NL);

        public static StreetNameRegistry.StreetName.Language ConvertToStreetNameLanguage(this Taal taal)
        {
            switch (taal)
            {
                default:
                case Taal.NL:
                    return StreetNameRegistry.StreetName.Language.Dutch;

                case Taal.FR:
                    return StreetNameRegistry.StreetName.Language.French;

                case Taal.DE:
                    return StreetNameRegistry.StreetName.Language.German;

                case Taal.EN:
                    return StreetNameRegistry.StreetName.Language.English;
            }
        }

        public static StreetNameRegistry.Municipality.Language ConvertToMunicipalityLanguage(this Taal? taal)
            => ConvertToMunicipalityLanguage(taal ?? Taal.NL);

        public static StreetNameRegistry.Municipality.Language ConvertToMunicipalityLanguage(this Taal taal)
        {
            switch (taal)
            {
                default:
                case Taal.NL:
                    return StreetNameRegistry.Municipality.Language.Dutch;

                case Taal.FR:
                    return StreetNameRegistry.Municipality.Language.French;

                case Taal.DE:
                    return StreetNameRegistry.Municipality.Language.German;

                case Taal.EN:
                    return StreetNameRegistry.Municipality.Language.English;
            }
        }
    }
}
