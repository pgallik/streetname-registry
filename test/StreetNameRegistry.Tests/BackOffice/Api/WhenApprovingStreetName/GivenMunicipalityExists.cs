namespace StreetNameRegistry.Tests.BackOffice.Api.WhenApprovingStreetName
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
    using NodaTime;
    using StreetNameRegistry.Api.BackOffice;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Exceptions;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Requests;
    using StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Requests;
    using Xunit;
    using Xunit.Abstractions;

    public sealed class GivenMunicipalityExists : BackOfficeApiTest<StreetNameController>
    {
        public GivenMunicipalityExists(ITestOutputHelper testOutputHelper) : base(testOutputHelper, useSqs: true)
        {
        }

        [Fact]
        public async Task ThenAcceptedWithLocationIsReturned()
        {
            var expectedLocationResult = new LocationResult(Fixture.Create<Uri>());
            var expectedIfMatchHeader = Fixture.Create<string>();

            MockMediatorResponse<SqsStreetNameApproveRequest, LocationResult>(expectedLocationResult);
            var request = new StreetNameApproveRequest { PersistentLocalId = 123 };


            var result = (AcceptedResult)await Controller.Approve(
                MockValidIfMatchValidator(),
                MockPassingRequestValidator<StreetNameApproveRequest>(),
                request,
                ifMatchHeaderValue: expectedIfMatchHeader,
                CancellationToken.None);

            // Assert
            MockMediator.Verify(x =>
                x.Send(
                    It.Is<SqsStreetNameApproveRequest>(sqsRequest =>
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
                .Setup(x => x.Send(It.IsAny<SqsStreetNameApproveRequest>(), CancellationToken.None))
                .Throws(new AggregateIdIsNotFoundException());

            var request = new StreetNameApproveRequest { PersistentLocalId = 123 };
            Func<Task> act = async () =>
            {
                await Controller.Approve(
                    MockValidIfMatchValidator(),
                    MockPassingRequestValidator<StreetNameApproveRequest>(),
                    request,
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
                        && e.ErrorMessage.Contains($"De waarde '{request.PersistentLocalId}' is ongeldig.")));
        }

        [Fact]
        public async Task WithIfMatchHeaderValueMismatch_ThenReturnsPreconditionFailedResult()
        {
            var result = await Controller.Approve(
                MockValidIfMatchValidator(false),
                MockPassingRequestValidator<StreetNameApproveRequest>(),
                new StreetNameApproveRequest { PersistentLocalId = 123 },
                string.Empty,
                CancellationToken.None);

            //Assert
            result.Should().BeOfType<PreconditionFailedResult>();
        }
    }
}
