namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class DefineMunicipalityNisCode : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("7932ba11-aef2-49bd-9378-efed993967e8");

        public MunicipalityId MunicipalityId { get; }
        public NisCode NisCode { get; }
        public Provenance Provenance { get; }

        public DefineMunicipalityNisCode(MunicipalityId municipalityId, NisCode nisCode, Provenance provenance)
        {
            MunicipalityId = municipalityId;
            NisCode = nisCode;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"{nameof(DefineMunicipalityNisCode)}-{ToString()}");

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
