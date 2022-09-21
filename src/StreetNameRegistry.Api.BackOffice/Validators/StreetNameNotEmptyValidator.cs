namespace StreetNameRegistry.Api.BackOffice.Validators
{
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;

    public static class StreetNameNotEmptyValidator
    {
        public const string Code = "StraatnaamNietLeegValidatie";

        public static bool IsValid(KeyValuePair<Taal, string> streetName) => !string.IsNullOrWhiteSpace(streetName.Value);
    }
}
