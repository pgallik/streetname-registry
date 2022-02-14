namespace StreetNameRegistry.StreetName.Commands
{
    using System;
    using System.Collections.Generic;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;

    public class MigrateStreetNameToMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("48ec913b-d7ba-48ba-9705-a98e4533dd30");

        public MunicipalityId MunicipalityId { get; }
        public StreetNameId StreetNameId { get; }
        public PersistentLocalId PersistentLocalId { get; }
        public StreetNameStatus? Status { get; }
        public Language? PrimaryLanguage { get; }
        public Language? SecondaryLanguage { get; }
        public Names Names { get; }
        public HomonymAdditions HomonymAdditions { get; }
        public bool IsCompleted { get; }
        public bool IsRemoved { get; }
        public Provenance Provenance { get; }

        public MigrateStreetNameToMunicipality(MunicipalityId municipalityId,
            StreetNameId streetNameId,
            PersistentLocalId persistentLocalId,
            StreetNameStatus? status,
            Language? primaryLanguage,
            Language? secondaryLanguage,
            Names names,
            HomonymAdditions homonymAdditions,
            bool isCompleted,
            bool isRemoved,
            Provenance provenance)
        {
            MunicipalityId = municipalityId;
            StreetNameId = streetNameId;
            PersistentLocalId = persistentLocalId;
            Status = status;
            PrimaryLanguage = primaryLanguage;
            SecondaryLanguage = secondaryLanguage;
            Names = names;
            HomonymAdditions = homonymAdditions;
            IsCompleted = isCompleted;
            IsRemoved = isRemoved;
            Provenance = provenance;
        }

        public Guid CreateCommandId()
            => Deterministic.Create(Namespace, $"MigrateStreetNameToMunicipality-{ToString()}");

        public override string? ToString()
            => ToStringBuilder.ToString(IdentityFields());

        private IEnumerable<object> IdentityFields()
        {
            yield return StreetNameId;
            yield return IsCompleted;
            yield return Status ?? StreetNameStatus.Current;
            yield return PrimaryLanguage ?? Language.Dutch;
            yield return SecondaryLanguage ?? Language.Dutch;
            yield return PersistentLocalId;

            foreach (var homonymAddition in HomonymAdditions)
            {
                yield return homonymAddition;
            }

            foreach (var name in Names)
            {
                yield return name;
            }

            yield return IsRemoved;
            yield return MunicipalityId;
        }
    }
}
