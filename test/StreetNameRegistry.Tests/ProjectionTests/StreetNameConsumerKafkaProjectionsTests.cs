namespace StreetNameRegistry.Tests.ProjectionTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts.MunicipalityRegistry;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Consumer.Projections;
    using Generate;
    using global::AutoFixture;
    using Moq;
    using Municipality;
    using NodaTime;
    using Projections.Legacy.StreetNameDetail;
    using Testing;
    using Xunit;
    using Xunit.Abstractions;
    using Provenance = Be.Vlaanderen.Basisregisters.GrAr.Contracts.Common.Provenance;

    public class StreetNameConsumerKafkaProjectionsTests : StreetNameRegistryProjectionTest<StreetNameDetailProjections>
    {
        public StreetNameConsumerKafkaProjectionsTests(ITestOutputHelper output)
            : base(output)
        { }

        private class MunicipalityEventsGenerator : IEnumerable<object[]>
        {
            private readonly Fixture? _fixture;

            public MunicipalityEventsGenerator()
            {
                _fixture = new Fixture();
                _fixture.Customize(new InfrastructureCustomization());
                _fixture.Customize(new WithFixedMunicipalityId());
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                var id = _fixture.Create<MunicipalityId>().ToString();
                var nisCode = _fixture.Create<NisCode>();
                var name = _fixture.Create<string>();
                var language = Language.Dutch.ToString();
                var retirementDate = _fixture.Create<Instant>().ToString();
                var provenance = new Provenance(
                    Instant.FromDateTimeOffset(DateTimeOffset.Now).ToString(),
                    Application.StreetNameRegistry.ToString(),
                    Modification.Unknown.ToString(),
                    Organisation.DigitaalVlaanderen.ToString(),
                    new Reason("")
                    );

                var result = new List<object[]>
                {
                    new object[] { new MunicipalityWasRegistered(id, nisCode, provenance) },
                    new object[] { new MunicipalityNisCodeWasDefined(id, nisCode, provenance) },
                    new object[] { new MunicipalityNisCodeWasCorrected(id, nisCode, provenance) },
                    new object[] { new MunicipalityWasNamed(id, name, language, provenance) },
                    new object[] { new MunicipalityNameWasCorrected(id, name, language, provenance) },
                    new object[] { new MunicipalityNameWasCorrectedToCleared(id, language, provenance) },
                    new object[] { new MunicipalityOfficialLanguageWasAdded(id, language, provenance) },
                    new object[] { new MunicipalityOfficialLanguageWasRemoved(id, language, provenance) },
                    new object[] { new MunicipalityFacilityLanguageWasAdded(id, language, provenance) },
                    new object[] { new MunicipalityFacilityLanguageWasRemoved(id, language, provenance) },
                    new object[] { new MunicipalityBecameCurrent(id, provenance) },
                    new object[] { new MunicipalityWasCorrectedToCurrent(id, provenance) },
                    new object[] { new MunicipalityWasRetired(id, retirementDate, provenance) },
                    new object[] { new MunicipalityWasCorrectedToRetired(id, retirementDate, provenance) } // test fails on date format
                };
                return result.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(MunicipalityEventsGenerator))]
        public async Task HandleMessage(object obj)
        {
            if (obj is not IQueueMessage queueMessage)
            {
                throw new InvalidOperationException("Parameter is not an IQueueMessage");
            }

            var command = MunicipalityKafkaProjection.GetCommand(queueMessage);

            var mock = new Mock<CommandHandler>();
            mock.Setup(commandHandler => commandHandler.Handle(command, default))
                .Returns(Task.CompletedTask);

            var given = GivenEvents(Generate.EventsFor.Objects(command));
            var project = given
                .Project(Generate.This(command));
            await project
                .Then(ct =>
                {
                    mock.Verify(commandHandler => commandHandler.Handle(It.IsAny<IHasCommandProvenance>(), default), Times.AtMostOnce());
                    return Task.CompletedTask;
                });
        }
    }
}
