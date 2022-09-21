namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public sealed class CorrectMunicipalityNisCode : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("cec655f2-ff8d-46d1-ae5d-ed7c5757f196");

        public MunicipalityId MunicipalityId { get; }
        public NisCode NisCode { get; }
        public Provenance Provenance { get; }

        public CorrectMunicipalityNisCode(MunicipalityId municipalityId, NisCode nisCode, Provenance provenance)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(CorrectMunicipalityNisCode)}-{ToString()}");
        
        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return MunicipalityId;
            yield return NisCode;

            foreach (var field in Provenance.GetIdentityFields())
            {
                yield return field;
            }
        }
    }
}
