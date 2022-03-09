namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class AddFacilityLanguageToMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("1afc9880-b6c9-40be-ab40-f66dd3813aee");

        public MunicipalityId MunicipalityId { get; }
        public Language Language { get; }
        public Provenance Provenance { get; }

        public AddFacilityLanguageToMunicipality(MunicipalityId municipalityId, Language language, Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Language = language;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(AddFacilityLanguageToMunicipality)}-{ToString()}");

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
