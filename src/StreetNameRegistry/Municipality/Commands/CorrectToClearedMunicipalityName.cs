namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class CorrectToClearedMunicipalityName : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("503d66c1-6f54-4860-8e78-a0d07abbaa53");

        public MunicipalityId MunicipalityId { get; }
        public Language Language { get; }
        public Provenance Provenance { get; }

        public CorrectToClearedMunicipalityName(MunicipalityId municipalityId, Language language, Provenance provenance)
        {
            MunicipalityId = municipalityId;
            Language = language;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(CorrectToClearedMunicipalityName)}-{ToString()}");
        
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
