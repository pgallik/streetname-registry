namespace StreetNameRegistry.Tests.ProjectionTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts.MunicipalityRegistry;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Consumer.Projections;
    using Generate;
    using Moq;
    using NodaTime;
    using NodaTime.Text;
    using Projections.Legacy.StreetNameDetail;
    using StreetName.Commands;
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
            public IEnumerator<object[]> GetEnumerator()
            {
                var random = new Random();
                var id = Produce.Guid().Generate(random).ToString("D");
                var nisCode = Produce.NumericString(5).Generate(random);
                var name = Produce.AlphaNumericString(10).Generate(random);
                var language = Language.Dutch.ToString();
                var retirementDate = Produce.LocalDateTime().Generate(random).PlusYears(50).ToString(InstantPattern.General.PatternText, CultureInfo.InvariantCulture);
                var provenance = new Provenance(
                    Instant.FromDateTimeOffset(DateTimeOffset.Now).ToString(),
                    Application.StreetNameRegistry.ToString(),
                    Modification.Unknown.ToString(),
                    new Operator(""),
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
                    new object[] { new MunicipalityFacilitiesLanguageWasRemoved(id, language, provenance) },
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
