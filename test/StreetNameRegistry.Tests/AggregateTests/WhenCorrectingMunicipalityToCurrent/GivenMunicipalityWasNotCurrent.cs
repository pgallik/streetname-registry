using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetNameRegistry.Tests.AggregateTests.WhenCorrectingMunicipalityToCurrent
{
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using global::AutoFixture;
    using StreetName.Commands.Municipality;
    using StreetName.Events;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;

    public class GivenMunicipalityWasNotCurrent : StreetNameRegistryTest
    {
        private readonly MunicipalityId _municipalityId;

        public GivenMunicipalityWasNotCurrent(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture.Customize(new InfrastructureCustomization());
            Fixture.Customize(new WithFixedMunicipalityId());
            _municipalityId = Fixture.Create<MunicipalityId>();
        }

        [Fact]
        public void ThenMunicipalityGetsCorrectedToCurrent()
        {
            var commandCorrectMunicipality = Fixture.Create<CorrectToCurrentMunicipality>();
            Assert(new Scenario()
                .Given(_municipalityId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                })
                .When(commandCorrectMunicipality)
                .Then(new[]
                {
                    new Fact(_municipalityId, new MunicipalityWasCorrectedToCurrent(_municipalityId))
                }));
        }

        [Fact]
        public void AndCorrectedToCurrent_ThenNone()
        {
            var commandCorrectMunicipality = Fixture.Create<CorrectToCurrentMunicipality>();
            Assert(new Scenario()
                .Given(_municipalityId, new object[]
                {
                    Fixture.Create<MunicipalityWasImported>(),
                    Fixture.Create<MunicipalityWasCorrectedToCurrent>(),
                })
                .When(commandCorrectMunicipality)
                .ThenNone());
        }
    }
}
