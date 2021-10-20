namespace StreetNameRegistry.Tests.AutoFixture
{
    using System;
    using global::AutoFixture;
    using global::AutoFixture.Kernel;

    public class WithFixedMunicipalityId : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var municipalityId = fixture.Create<MunicipalityId>();
            
            fixture.Register(() => municipalityId);

            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder(municipalityId),
                    new ParameterSpecification(
                        typeof(Guid),
                        "municipalityId")));
        }
    }
}
