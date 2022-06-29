namespace StreetNameRegistry.Tests.AutoFixture
{
    using System;
    using System.Collections.Generic;
    using global::AutoFixture;
    using Municipality;

    public class WithMunicipalityStatus : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var statuses = new List<MunicipalityStatus>()
            {
                MunicipalityStatus.Current,
                MunicipalityStatus.Retired
            };

            fixture.Register(() => statuses[new Random(fixture.Create<int>()).Next(statuses.Count - 1)]);
        }
    }
}
