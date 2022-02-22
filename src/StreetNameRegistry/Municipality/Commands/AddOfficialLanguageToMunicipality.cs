namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class AddOfficialLanguageToMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("a69fa75e-4cea-4660-a7c6-204eb7d85f1d");

        public MunicipalityId MunicipalityId { get; }
        public Language Language { get; }
        public Provenance Provenance { get; }

        public AddOfficialLanguageToMunicipality(MunicipalityId municipalityId, Language language, Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Language = language;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(AddOfficialLanguageToMunicipality)}-{ToString()}");

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return Language;
            yield return Provenance;
        }
    }
}
