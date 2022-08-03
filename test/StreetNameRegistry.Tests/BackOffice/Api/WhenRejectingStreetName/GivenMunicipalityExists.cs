namespace StreetNameRegistry.Tests.BackOffice.Api.WhenRejectingStreetName
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.ETag;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using FluentAssertions;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using StreetNameRegistry.Api.BackOffice;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Requests;
    using StreetNameRegistry.Api.BackOffice.Abstractions.Response;
    using Municipality.Exceptions;
    using Xunit;
    using Xunit.Abstractions;
    using MunicipalityId = Municipality.MunicipalityId;
    using PersistentLocalId = Municipality.PersistentLocalId;

    public class GivenMunicipalityExists : BackOfficeApiTest<StreetNameController>
    {
        private readonly TestBackOfficeContext _backOfficeContext;

        public GivenMunicipalityExists(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _backOfficeContext = new FakeBackOfficeContextFactory().CreateDbContext(Array.Empty<string>());
        }

        [Fact]
        public async Task ThenMediatorSends_StreetNameRejectRequest()
        {
            var municipalityId = new MunicipalityId(Guid.NewGuid());
            var persistentLocalId = new PersistentLocalId(456);

            _backOfficeContext.AddMunicipalityIdByPersistentLocalIdToFixture(persistentLocalId, municipalityId);

            MockMediatorResponse<StreetNameRejectRequest, ETagResponse>(new ETagResponse("hash"));

            // Act
            var result = (NoContentWithETagResult)await Controller.Reject(
                MockValidIfMatchValidator(),
                MockPassingRequestValidator<StreetNameRejectRequest>(),
                new StreetNameRejectRequest
                {
                    PersistentLocalId = persistentLocalId,
                },
                ifMatchHeaderValue: null,
                CancellationToken.None);

            // Assert
            MockMediator.Verify(x => x.Send(It.IsAny<StreetNameRejectRequest>(), CancellationToken.None));
            result.ETag.Should().Be("hash");
        }

        [Fact]
        public void WhenStreetNameIsNotFound_ThenBadRequestIsExpected()
        {
            MockMediator
                .Setup(x => x.Send(It.IsAny<StreetNameRejectRequest>(), CancellationToken.None))
                .Throws(new StreetNameNotFoundException());

            //Act
            Func<Task> act = async () => await Controller.Reject(
                MockValidIfMatchValidator(),
                MockPassingRequestValidator<StreetNameRejectRequest>(),
                new StreetNameRejectRequest
                {
                    PersistentLocalId = new PersistentLocalId(456)
                },
                null);

            //Assert
            act
                .Should()
                .ThrowAsync<ApiException>()
                .Result
                .Where(x => x.Message.Contains("Onbestaande straatnaam")
                            && x.StatusCode == StatusCodes.Status404NotFound);
        }

        [Fact]
        public void WhenStreetNameIsRemoved_ThenBadRequestIsExpected()
        {
            MockMediator
                .Setup(x => x.Send(It.IsAny<StreetNameRejectRequest>(), CancellationToken.None))
                .Throws(new StreetNameWasRemovedException());

            //Act
            Func<Task> act = async () => await Controller.Reject(
                MockValidIfMatchValidator(),
                MockPassingRequestValidator<StreetNameRejectRequest>(),
                new StreetNameRejectRequest
                {
                    PersistentLocalId = new PersistentLocalId(456)
                },
                null);

            //Assert
            act
                .Should()
                .ThrowAsync<ApiException>()
                .Result
                .Where(x => x.Message.Contains("Verwijderde straatnaam")
                            && x.StatusCode == StatusCodes.Status410Gone);
        }

        [Fact]
        public void WhenStreetNameIsNotInStatusCurrentOrRetired_ThenBadRequestIsExpected()
        {
            MockMediator
                .Setup(x => x.Send(It.IsAny<StreetNameRejectRequest>(), CancellationToken.None))
                .Throws(new StreetNameStatusPreventsRejectionException());

            //Act
            Func<Task> act = async () => await Controller.Reject(
                MockValidIfMatchValidator(),
                MockPassingRequestValidator<StreetNameRejectRequest>(),
                new StreetNameRejectRequest
                {
                    PersistentLocalId = new PersistentLocalId(456)
                },
                null);

            //Assert
            act
                .Should()
                .ThrowAsync<ApiException>()
                .Result
                .Where(x => x.Message.Contains("Deze actie is enkel toegestaan op straatnaam met status 'voorgesteld'.")
                            && x.StatusCode == StatusCodes.Status409Conflict);
        }

        [Fact]
        public async Task WhenMunicipalityIsRetired_ThenBadRequestIsReturned()
        {
            MockMediator
                .Setup(x => x.Send(It.IsAny<StreetNameRejectRequest>(), CancellationToken.None))
                .Throws(new MunicipalityHasUnexpectedStatusException());

            //Act
            Func<Task> act = async () => await Controller.Reject(
                MockValidIfMatchValidator(),
                MockPassingRequestValidator<StreetNameRejectRequest>(),
                new StreetNameRejectRequest
                {
                    PersistentLocalId = new PersistentLocalId(456)
                },
                null);

            //Assert
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x => x.Message.Contains("Deze actie is enkel toegestaan binnen gemeenten met status 'inGebruik'."))
                .Where(x => x.Errors.Single().ErrorCode == "StraatnaamGemeenteInGebruik");
        }
    }
}
