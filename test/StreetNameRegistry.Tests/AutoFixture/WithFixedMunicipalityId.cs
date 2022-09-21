namespace StreetNameRegistry.Tests.AutoFixture
{
    using System;
    using global::AutoFixture;
    using global::AutoFixture.Kernel;
    using Municipality;

    public sealed class WithFixedMunicipalityId : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var municipalityId = fixture.Create<MunicipalityId>();
            
            fixture.Register(() => municipalityId);
            fixture.Register(() => new MunicipalityStreamId(municipalityId));
            fixture.Register(() => new StreetName.MunicipalityId(municipalityId));

            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new FixedBuilder(municipalityId),
                    new ParameterSpecification(
                        typeof(Guid),
                        "municipalityId")));
        }
    }
}
