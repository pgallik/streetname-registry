namespace StreetNameRegistry.Tests.AutoFixture
{
    using System;
    using global::AutoFixture;
    using global::AutoFixture.Kernel;
    using StreetName;

    public class WithFixedMunicipalityId : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var municipalityId = fixture.Create<MunicipalityId>();
            
            fixture.Register(() => municipalityId);
            fixture.Register(() => new MunicipalityStreamId(municipalityId));

            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder(municipalityId),
                    new ParameterSpecification(
                        typeof(Guid),
                        "municipalityId")));
        }
    }
}
