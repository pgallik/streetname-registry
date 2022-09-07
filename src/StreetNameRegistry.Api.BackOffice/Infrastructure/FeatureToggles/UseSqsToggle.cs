namespace StreetNameRegistry.Api.Legacy.Infrastructure.FeatureToggles
{
    using FeatureToggle;

    public class UseSqsToggle : IFeatureToggle
    {
        public bool FeatureEnabled { get; }

        public UseSqsToggle(bool featureEnabled)
        {
            FeatureEnabled = featureEnabled;
        }
    }
}
