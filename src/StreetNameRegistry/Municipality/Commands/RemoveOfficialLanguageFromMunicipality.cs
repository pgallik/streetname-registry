namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public sealed class RemoveOfficialLanguageFromMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("4e2a6dff-1f00-4d6a-a4ec-69a01bf3c2b9");

        public MunicipalityId MunicipalityId { get; }
        public Language Language { get; }
        public Provenance Provenance { get; }

        public RemoveOfficialLanguageFromMunicipality(MunicipalityId municipalityId, Language language, Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Language = language;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(RemoveOfficialLanguageFromMunicipality)}-{ToString()}");
        
        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return Language;

            foreach (var field in Provenance.GetIdentityFields())
            {
                yield return field;
            }
        }
    }
}
