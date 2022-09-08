namespace StreetNameRegistry.Tests.BackOffice.Api.WhenRetiringStreetName
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
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

    public class GivenMunicipalityExists : BackOfficeApiTest<StreetNameController>
    {

        public GivenMunicipalityExists(ITestOutputHelper testOutputHelper) : base(testOutputHelper, useSqs: true)
        { }

        [Fact]
        public async Task ThenAcceptedWithLocationIsReturned()
        {
            var expectedLocationResult = new LocationResult(Fixture.Create<Uri>());
            MockMediatorResponse<SqsStreetNameRetireRequest, LocationResult>(expectedLocationResult);

            var request = new StreetNameRetireRequest
            {
                PersistentLocalId = 123
            };

            var result = (AcceptedResult) await Controller.Retire(
                MockValidIfMatchValidator(),
                MockPassingRequestValidator<StreetNameRetireRequest>(),
                ResponseOptions,
                request,
                ifMatchHeaderValue: string.Empty,
                CancellationToken.None);

            // Assert
            MockMediator.Verify(x =>
                x.Send(
                    It.Is<SqsStreetNameRetireRequest>(sqsRequest =>
                        sqsRequest.Request == request &&
                        sqsRequest.ProvenanceData.Timestamp != Instant.MinValue), // Just to verify that ProvenanceData has been populated.
                    CancellationToken.None));
            result.Location.Should().Be(expectedLocationResult.Location.ToString());
        }

        [Fact]
        public void WithAggregateIdIsNotFound_ThenThrowsValidationException()
        {
            MockMediator
                .Setup(x => x.Send(It.IsAny<SqsStreetNameRetireRequest>(), CancellationToken.None))
                .Throws(new AggregateIdIsNotFoundException());

            Func<Task> act = async () => await Controller.Retire(
                MockValidIfMatchValidator(),
                MockPassingRequestValidator<StreetNameRetireRequest>(),
                ResponseOptions,
                new StreetNameRetireRequest { PersistentLocalId = 123 },
                string.Empty,
                CancellationToken.None);

            //Assert
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x =>
                    x.Errors.Any(e => e.ErrorCode == "code"
                                      && e.ErrorMessage.Contains("message")));
        }

        [Fact]
        public async Task WithIfMatchHeaderValueMismatch_ThenReturnsPreconditionFailedResult()
        {
            var result = await Controller.Retire(
                MockValidIfMatchValidator(false),
                MockPassingRequestValidator<StreetNameRetireRequest>(),
                ResponseOptions,
                new StreetNameRetireRequest { PersistentLocalId = 123 },
                string.Empty,
                CancellationToken.None);

            //Assert
            result.Should().BeOfType<PreconditionFailedResult>();
        }
    }
}
