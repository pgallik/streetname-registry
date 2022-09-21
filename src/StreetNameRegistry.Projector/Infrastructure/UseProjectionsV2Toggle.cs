namespace StreetNameRegistry.Projector.Infrastructure
{
    using FeatureToggle;
    public sealed class FeatureToggleOptions
    {
        public const string ConfigurationKey = "FeatureToggles";
        public bool UseProjectionsV2 { get; set; }
    }

    public sealed class UseProjectionsV2Toggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public UseProjectionsV2Toggle(bool featureEnabled)
        {
            FeatureEnabled = featureEnabled;
        }
    }
}
