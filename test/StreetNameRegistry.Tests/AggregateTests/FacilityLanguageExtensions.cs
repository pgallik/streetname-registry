namespace StreetNameRegistry.Tests.AggregateTests
{
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Municipality;
    using Municipality.Commands;
    using Municipality.Events;

    public static class FacilityLanguageExtensions
    {
        public static AddFacilityLanguageToMunicipality WithLanguage(this AddFacilityLanguageToMunicipality command, Language language)
        {
            return new AddFacilityLanguageToMunicipality(command.MunicipalityId, language, command.Provenance);
        }

        public static RemoveFacilityLanguageFromMunicipality WithLanguage(this RemoveFacilityLanguageFromMunicipality command, Language language)
        {
            return new RemoveFacilityLanguageFromMunicipality(command.MunicipalityId, language, command.Provenance);
        }

        public static MunicipalityFacilityLanguageWasAdded ToEvent(this AddFacilityLanguageToMunicipality command)
        {
            var eventAdded = new MunicipalityFacilityLanguageWasAdded(command.MunicipalityId, command.Language);
            ((ISetProvenance)eventAdded).SetProvenance(command.Provenance);

            return eventAdded;
        }
        public static MunicipalityFacilityLanguageWasRemoved ToEvent(this RemoveFacilityLanguageFromMunicipality command)
        {
            var eventAdded = new MunicipalityFacilityLanguageWasRemoved(command.MunicipalityId, command.Language);
            ((ISetProvenance)eventAdded).SetProvenance(command.Provenance);

            return eventAdded;
        }
    }
}
