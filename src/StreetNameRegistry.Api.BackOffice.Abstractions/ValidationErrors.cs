namespace StreetNameRegistry.Api.BackOffice.Abstractions
{
    public static class ValidationErrorCodes
    {
        public static class Municipality
        {
            public const string MunicipalityStatusNotCurrent = "StraatnaamGemeenteInGebruik";
            public const string MunicipalityHasInvalidStatus = "StraatnaamGemeenteGehistoreerd";
        }

        public static class StreetName
        {
            public const string StreetNameNotFound = "OnbestaandeStraatnaam"; // ToBeReviewed
            public const string StreetNameIsRemoved = "VerwijderdeStraatnaam"; // ToBeReviewed
            public const string StreetNameAlreadyExists = "StraatnaamBestaatReedsInGemeente";
            public const string StreetNameCannotBeApproved = "StraatnaamGehistoreerdOfAfgekeurd"; // ToBeReviewed
            public const string StreetNameCannotBeCorrected = "StraatnaamGehistoreerdOfAfgekeurd";
            public const string StreetNameCannotBeRejected = "StraatnaamInGebruikOfGehistoreerd";
            public const string StreetNameCannotBeRetired = "StraatnaamVoorgesteldOfAfgekeurd";
            public const string StreetNameNameLanguageIsNotSupported = "StraatnaamTaalNietInOfficieleOfFaciliteitenTaal";
            public const string StreetNameIsMissingALanguage = "StraatnaamOntbreektOfficieleOfFaciliteitenTaal";
        }
    }
    public static class ValidationErrorMessages
    {
        public static class Municipality
        {
            public const string MunicipalityStatusNotCurrent = "Deze actie is enkel toegestaan binnen gemeenten met status 'inGebruik'.";
            public const string MunicipalityHasInvalidStatus = "De gemeente is gehistoreerd.";
        }

        public static class StreetName
        {
            public const string StreetNameNotFound = "Onbestaande straatnaam.";
            public const string StreetNameIsRemoved = "Verwijderde straatnaam.";
            public const string StreetNameCannotBeApproved = "Deze actie is enkel toegestaan op straatnamen met status 'voorgesteld'."; // ToBeReviewed
            public const string StreetNameCannotBeCorrected = "Deze actie is enkel toegestaan op straatnamen met status 'voorgesteld' of 'inGebruik'."; // ToBeReviewed
            public const string StreetNameCannotBeRetired = "Deze actie is enkel toegestaan op straatnamen met status 'inGebruik'.";
            public const string StreetNameCannotBeRejected = "Deze actie is enkel toegestaan op straatnamen met status 'voorgesteld'.";
            public static string StreetNameAlreadyExists(string name) => $"Straatnaam '{name}' bestaat reeds in de gemeente.";
            public const string StreetNameHasInvalidStatus = "Deze actie is enkel toegestaan op straatnamen met status 'voorgesteld' of 'inGebruik'.";
            public const string StreetNameNameLanguageIsNotSupported = "'Straatnamen' kunnen enkel voorkomen in de officiële of faciliteitentaal van de gemeente.";
            public const string StreetNameIsMissingALanguage = "In 'Straatnamen' ontbreekt een officiële of faciliteitentaal.";
        }
    }
}
