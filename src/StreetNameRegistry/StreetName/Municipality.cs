namespace StreetNameRegistry.StreetName
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Events;

    public partial class Municipality : AggregateRootEntity
    {
        public static readonly Func<Municipality> Factory = () => new Municipality();

        // public static Municipality Register(MunicipalityId municipalityId, NisCode nisCode)
        // {
        //     var municipality = Factory();
        //     municipality.ApplyChange(new StreetNameWasRegistered(municipalityId, nisCode));
        //     return streetName;
        // }


        public void ProposeStreetName(Names streetNameNames, IPersistentLocalIdGenerator persistentLocalIdGenerator)
        {
            ApplyChange(new StreetNameWasProposedV2(_municipalityId,streetNameNames, persistentLocalIdGenerator.GenerateNextPersistentLocalId()));
        }
    }
}
