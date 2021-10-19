namespace StreetNameRegistry.Api.BackOffice.Convertors
{
    using System;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Oslo.Extensions;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;

    public static class TaalMapper
    {
        public static Language ToLanguage(this Taal taal)
        {
            switch (taal)
            {
                case Taal.NL:
                    return Language.Dutch;

                case Taal.FR:
                    return Language.French;

                case Taal.DE:
                    return Language.German;

                case Taal.EN:
                    return Language.English;

                default:
                    throw new ArgumentOutOfRangeException(nameof(taal), taal, $"Non existing language '{taal}'.");
            }
        }
    }

    public static class IdentifierMappings
    {
        public static Func<string, string> MunicipalityNisCode = s => s;
    }
}
