namespace StreetNameRegistry.Tests.AggregateTests.Extensions
{
    using Municipality;
    using Municipality.Commands;
    using Municipality.Events;

    public static class OfficialLanguageExtensions
    {
        public static AddOfficialLanguageToMunicipality WithLanguage(this AddOfficialLanguageToMunicipality command, Language language)
        {
            return new AddOfficialLanguageToMunicipality(command.MunicipalityId, language, command.Provenance);
        }

        public static RemoveOfficialLanguageFromMunicipality WithLanguage(this RemoveOfficialLanguageFromMunicipality command, Language language)
        {
            return new RemoveOfficialLanguageFromMunicipality(command.MunicipalityId, language, command.Provenance);
        }

        public static MunicipalityOfficialLanguageWasAdded ToEvent(this AddOfficialLanguageToMunicipality command)
        {
            var eventAdded = new MunicipalityOfficialLanguageWasAdded(command.MunicipalityId, command.Language);
            eventAdded.SetProvenance(command.Provenance);

            return eventAdded;
        }

        public static MunicipalityOfficialLanguageWasRemoved ToEvent(this RemoveOfficialLanguageFromMunicipality command)
        {
            var eventAdded = new MunicipalityOfficialLanguageWasRemoved(command.MunicipalityId, command.Language);
            eventAdded.SetProvenance(command.Provenance);

            return eventAdded;
        }
    }
}
