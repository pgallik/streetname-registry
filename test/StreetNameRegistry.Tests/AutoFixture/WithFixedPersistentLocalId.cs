namespace StreetNameRegistry.Tests.AutoFixture
{
    using global::AutoFixture;
    using global::AutoFixture.Kernel;
    using StreetName;

    public class WithFixedPersistentLocalId : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var persistentLocalIdInt = fixture.Create<int>();

            fixture.Register(() => new PersistentLocalId(persistentLocalIdInt));

            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder(persistentLocalIdInt),
                    new ParameterSpecification(
                        typeof(int),
                        "persistentLocalId")));
        }
    }
}
