namespace StreetNameRegistry.Municipality.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Generators.Guid;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.Utilities;
    using StreetName;

    public sealed class MigrateStreetNameToMunicipality : IHasCommandProvenance
    {
        private static readonly Guid Namespace = new Guid("48ec913b-d7ba-48ba-9705-a98e4533dd30");

        public StreetNameRegistry.Municipality.MunicipalityId MunicipalityId { get; }
        public StreetNameRegistry.Municipality.StreetNameId StreetNameId { get; }
        public StreetNameRegistry.Municipality.PersistentLocalId PersistentLocalId { get; }
        public StreetNameRegistry.Municipality.StreetNameStatus Status { get; }
        public StreetNameRegistry.Municipality.Language PrimaryLanguage { get; }
        public StreetNameRegistry.Municipality.Language? SecondaryLanguage { get; }
        public StreetNameRegistry.Municipality.Names Names { get; }
        public StreetNameRegistry.Municipality.HomonymAdditions HomonymAdditions { get; }
        public bool IsCompleted { get; }
        public bool IsRemoved { get; }
        public Provenance Provenance { get; }

        public MigrateStreetNameToMunicipality(
            MunicipalityId municipalityId,
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
            MunicipalityId = new StreetNameRegistry.Municipality.MunicipalityId(municipalityId);
            StreetNameId = new StreetNameRegistry.Municipality.StreetNameId(streetNameId);
            PersistentLocalId = new StreetNameRegistry.Municipality.PersistentLocalId(persistentLocalId);
            Status = status.ToMunicipalityStreetNameStatus();
            PrimaryLanguage = primaryLanguage.ToMunicipalityLanguage();
            SecondaryLanguage = secondaryLanguage == null ? null : secondaryLanguage.ToMunicipalityLanguage();

            Names = new StreetNameRegistry.Municipality.Names(names.Select(x =>
                new StreetNameRegistry.Municipality.StreetNameName(x.Name, x.Language.ToMunicipalityLanguage())));

            HomonymAdditions = new StreetNameRegistry.Municipality.HomonymAdditions(homonymAdditions.Select(x =>
                new StreetNameRegistry.Municipality.StreetNameHomonymAddition(x.HomonymAddition, x.Language.ToMunicipalityLanguage())));

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
            yield return Status;
            yield return PrimaryLanguage;
            yield return SecondaryLanguage ?? StreetNameRegistry.Municipality.Language.Dutch;
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
