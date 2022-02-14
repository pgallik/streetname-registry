namespace StreetNameRegistry.StreetName.Commands
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;

    public class MigrateStreetNameToMunicipality
    {
        public StreetNameId StreetNameId { get; }
        public bool IsCompleted { get; }
        public StreetNameStatus? Status { get; }
        public Language? PrimaryLanguage { get; }
        public Language? SecondaryLanguage { get; }
        public PersistentLocalId PersistentLocalId { get; }
        public HomonymAdditions HomonymAdditions { get; }
        public Names Names { get; }
        public bool IsRemoved { get; }
        public StreetNameMunicipality StreetNameMunicipality { get; set; }

        public MigrateStreetNameToMunicipality(StreetNameId streetNameId, bool isCompleted, StreetNameStatus? status, Language? primaryLanguage, Language? secondaryLanguage,
            PersistentLocalId persistentLocalId, HomonymAdditions homonymAdditions, Names names, bool isRemoved, StreetNameMunicipality streetNameMunicipality)
        {
            StreetNameId = streetNameId;
            IsCompleted = isCompleted;
            Status = status;
            PrimaryLanguage = primaryLanguage;
            SecondaryLanguage = secondaryLanguage;
            PersistentLocalId = persistentLocalId;
            HomonymAdditions = homonymAdditions;
            Names = names;
            IsRemoved = isRemoved;
            StreetNameMunicipality = streetNameMunicipality;
        }
    }

    public class StreetNameMunicipality : Entity
    {
        public StreetNameMunicipality(Action<object> applier)
            : base(applier)
        { }
    }
}
