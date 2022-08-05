namespace StreetNameRegistry.Tests.BackOffice.Api
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using FluentValidation.Results;
    using global::AutoFixture;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using Moq;
    using Municipality;
    using StreetNameRegistry.Api.BackOffice;
    using StreetNameRegistry.Api.BackOffice.Infrastructure;
    using StreetNameRegistry.Api.BackOffice.Infrastructure.Options;
    using Testing;
    using Xunit.Abstractions;

    public class BackOfficeApiTest<TController> : StreetNameRegistryTest
        where TController: BackOfficeApiController
    {
        protected readonly TController Controller;
        protected const string DetailUrl = "https://www.registry.com/streetname/voorgesteld/{0}";
        protected IOptions<ResponseOptions> ResponseOptions { get; }
        protected Mock<IMediator> MockMediator { get; }

        public BackOfficeApiTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            ResponseOptions = Options.Create(Fixture.Create<ResponseOptions>());
            ResponseOptions.Value.DetailUrl = DetailUrl;
            MockMediator = new Mock<IMediator>();
            Controller = CreateApiBusControllerWithUser();
        }

        protected void MockMediatorResponse<TRequest, TResponse>(TResponse response)
            where TRequest : IRequest<TResponse>
        {
            MockMediator
                .Setup(x => x.Send(It.IsAny<TRequest>(), CancellationToken.None))
                .Returns(Task.FromResult(response));
        }

        protected IIfMatchHeaderValidator MockValidIfMatchValidator()
        {
            var mockIfMatchHeaderValidator = new Mock<IIfMatchHeaderValidator>();
            mockIfMatchHeaderValidator.Setup(x =>
                    x.IsValid(It.IsAny<string>(), It.IsAny<PersistentLocalId>(), CancellationToken.None))
                .Returns(Task.FromResult(true));
            return mockIfMatchHeaderValidator.Object;
        }

        protected IValidator<TRequest> MockPassingRequestValidator<TRequest>()
        {
            var mockRequestValidator = new Mock<IValidator<TRequest>>();
            mockRequestValidator.Setup(x => x.ValidateAsync(It.IsAny<TRequest>(), CancellationToken.None))
                .Returns(Task.FromResult(new ValidationResult()));
            return mockRequestValidator.Object;
        }

        protected IValidator<TRequest> MockPassingRetireValidator<TRequest>()
        {
            var mockRequestValidator = new Mock<IValidator<TRequest>>();
            mockRequestValidator.Setup(x => x.ValidateAsync(It.IsAny<TRequest>(), CancellationToken.None))
                .Returns(Task.FromResult(new ValidationResult()));
            return mockRequestValidator.Object;
        }

        protected string GetStreetNamePuri(int persistentLocalId)
            => $"https://data.vlaanderen.be/id/gemeente/{persistentLocalId}";

        public TController CreateApiBusControllerWithUser(string username="John Doe")
        {
            var controller = Activator.CreateInstance(typeof(TController), MockMediator.Object) as TController;

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.NameIdentifier, "userId"),
                new Claim("name", username),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            if (controller != null)
            {
                controller.ControllerContext.HttpContext = new DefaultHttpContext { User = claimsPrincipal };

                return controller;
            }

            throw new Exception("Could not find controller type");
        }
    }
}
