namespace StreetNameRegistry.Tests.BackOffice.Api.WhenRejectingStreetName
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
        {
        }

        [Fact]
        public async Task ThenAcceptedWithLocationIsReturned()
        {
            var expectedLocationResult = new LocationResult(Fixture.Create<Uri>());
            var expectedIfMatchHeader = Fixture.Create<string>();
            MockMediatorResponse<SqsStreetNameRejectRequest, LocationResult>(expectedLocationResult);

            var request = new StreetNameRejectRequest { PersistentLocalId = 123 };

            var result = (AcceptedResult)await Controller.Reject(
                MockValidIfMatchValidator(),
                MockPassingRequestValidator<StreetNameRejectRequest>(),
                ResponseOptions,
                request,
                ifMatchHeaderValue: expectedIfMatchHeader,
                CancellationToken.None);

            // Assert
            MockMediator.Verify(x =>
                x.Send(
                    It.Is<SqsStreetNameRejectRequest>(sqsRequest =>
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
                .Setup(x => x.Send(It.IsAny<SqsStreetNameRejectRequest>(), CancellationToken.None))
                .Throws(new AggregateIdIsNotFoundException());

            Func<Task> act = async () => await Controller.Reject(
                MockValidIfMatchValidator(),
                MockPassingRequestValidator<StreetNameRejectRequest>(),
                ResponseOptions,
                new StreetNameRejectRequest { PersistentLocalId = 123 },
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
            var result = await Controller.Reject(
                MockValidIfMatchValidator(false),
                MockPassingRequestValidator<StreetNameRejectRequest>(),
                ResponseOptions,
                new StreetNameRejectRequest { PersistentLocalId = 123 },
                string.Empty,
                CancellationToken.None);

            //Assert
            result.Should().BeOfType<PreconditionFailedResult>();
        }
    }
}
