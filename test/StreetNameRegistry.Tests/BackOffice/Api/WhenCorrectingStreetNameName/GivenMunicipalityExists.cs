namespace StreetNameRegistry.Tests.BackOffice.Api.WhenCorrectingStreetNameName
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.GrAr.Legacy;
    using FluentAssertions;
    using FluentValidation;
    using global::AutoFixture;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using StreetNameRegistry.Api.BackOffice;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Requests;
    using NodaTime;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Exceptions;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests;
    using Xunit;
    using Xunit.Abstractions;

    public sealed class GivenMunicipalityExists : BackOfficeApiTest<StreetNameController>
    {
        public GivenMunicipalityExists(ITestOutputHelper testOutputHelper) : base(testOutputHelper, useSqs: true)
        { }

        [Fact]
        public async Task ThenAcceptedWithLocationIsReturned()
        {
            var expectedLocationResult = new LocationResult(Fixture.Create<Uri>());
            var expectedIfMatchHeader = Fixture.Create<string>();
            MockMediatorResponse<SqsStreetNameCorrectNamesRequest, LocationResult>(expectedLocationResult);

            var request = new StreetNameCorrectNamesRequest
            {
                Straatnamen = new Dictionary<Taal, string> { { Taal.NL, "Bosstraat" } }
            };

            var result = (AcceptedResult) await Controller.CorrectStreetNameNames(
                MockValidIfMatchValidator(),
                MockPassingRequestValidator<StreetNameCorrectNamesRequest>(),
                ResponseOptions,
                123,
                request,
                ifMatchHeaderValue: expectedIfMatchHeader,
                CancellationToken.None);

            // Assert
            MockMediator.Verify(x =>
                x.Send(
                    It.Is<SqsStreetNameCorrectNamesRequest>(sqsRequest =>
                        sqsRequest.Request == request &&
                        sqsRequest.ProvenanceData.Timestamp != Instant.MinValue && // Just to verify that ProvenanceData has been populated.
                        sqsRequest.IfMatchHeaderValue == expectedIfMatchHeader),
                    CancellationToken.None));
            result.Location.Should().Be(expectedLocationResult.Location.ToString());
        }

        [Fact]
        public void WithAggregateIdIsNotFound_ThenThrowsValidationException()
        {
            MockMediator
                .Setup(x => x.Send(It.IsAny<SqsStreetNameCorrectNamesRequest>(), CancellationToken.None))
                .Throws(new AggregateIdIsNotFoundException());

            var persistentLocalId = 123;
            Func<Task> act = async () =>
            {
                await Controller.CorrectStreetNameNames(
                    MockValidIfMatchValidator(),
                    MockPassingRequestValidator<StreetNameCorrectNamesRequest>(),
                    ResponseOptions,
                    persistentLocalId,
                    new StreetNameCorrectNamesRequest(),
                    string.Empty,
                    CancellationToken.None);
            };

            //Assert
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x =>
                    x.Errors.Any(e =>
                        e.ErrorCode == ""
                        && e.ErrorMessage.Contains($"De waarde '{persistentLocalId}' is ongeldig.")));
        }

        [Fact]
        public async Task WithIfMatchHeaderValueMismatch_ThenReturnsPreconditionFailedResult()
        {
            var result = await Controller.CorrectStreetNameNames(
                MockValidIfMatchValidator(false),
                MockPassingRequestValidator<StreetNameCorrectNamesRequest>(),
                ResponseOptions,
                123,
                new StreetNameCorrectNamesRequest(),
                string.Empty,
                CancellationToken.None);

            //Assert
            result.Should().BeOfType<PreconditionFailedResult>();
        }
    }
}
