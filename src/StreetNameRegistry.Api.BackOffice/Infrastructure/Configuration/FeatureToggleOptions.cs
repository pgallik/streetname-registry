namespace StreetNameRegistry.Api.BackOffice.Infrastructure.Configuration
{
    public class FeatureToggleOptions
    {
        public const string ConfigurationKey = "FeatureToggles";
        public bool UseSqs { get; set; }
    }
}
