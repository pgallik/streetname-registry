namespace StreetNameRegistry.StreetName
{
    using System;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.Crab;
    using Events;
    using Events.Crab;

    public partial class Municipality : AggregateRootEntity
    {
        public static readonly Func<Municipality> Factory = () => new Municipality();

        // public static Municipality Register(MunicipalityId municipalityId, NisCode nisCode)
        // {
        //     var municipality = Factory();
        //     municipality.ApplyChange(new StreetNameWasRegistered(municipalityId, nisCode));
        //     return streetName;
        // }

        public void ProposeStreetName(Names commandStreetNameNames)
        {
            throw new NotImplementedException();
        }
    }

    public partial class Municipality
    {
        // private Municipality()
        // {
        //     Register<MunicipalityWasRegistered>(When);
        // }
        //
        // private void When(MunicipalityWasRegistered @event)
        // {
        //
        // }
    }
}
