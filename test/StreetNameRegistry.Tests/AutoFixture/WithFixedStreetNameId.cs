namespace StreetNameRegistry.Tests.AutoFixture
{
    using System;
    using Be.Vlaanderen.Basisregisters.Crab;
    using global::AutoFixture;
    using global::AutoFixture.Kernel;

    public class WithFixedStreetNameId : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var crabStreetNameId = fixture.Create<CrabStreetNameId>();
            var streetNameId = StreetNameId.CreateFor(crabStreetNameId);

            fixture.Register(() => crabStreetNameId);
            fixture.Register(() => streetNameId);
            fixture.Register(() => new StreetNameId(streetNameId));

            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder(streetNameId),
                    new ParameterSpecification(
                        typeof(Guid),
                        "streetNameId")));
        }
    }
}
