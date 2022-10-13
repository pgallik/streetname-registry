using System;
using System.Threading.Tasks;

namespace StreetNameRegistry.Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using FluentAssertions;
    using Moq;
    using NodaTime;
    using Producer.Snapshot.Oslo.Infrastructure;
    using Xunit;

    public class SnapshotManagerTests
    {
        [Fact(Skip = "Tool to test SnapshotManager, requires bastion tunnel to staging.")]
        public async Task T()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.basisregisters.staging-vlaanderen.be/v2/straatnamen");
            var proxy = new PublicApiHttpProxy(httpClient);
            var snapshotManager = new SnapshotManager(proxy, 1, 1);
            var result = await snapshotManager.FindMatchingSnapshot(
                "50083",
                Instant.FromDateTimeOffset(DateTimeOffset.Parse("2022-03-23T14:24:04+01:00")),
                throwStaleWhenGone: false,
                CancellationToken.None);

            result.Should().NotBeNull();
        }

        [Fact]
        public void WhenVersionsMatch_ThenReturnOsloResult()
        {
            var eventVersion = "2022-03-23T14:24:04.801+01:00";
            var snapshotVersion = "2022-03-23T14:24:04+01:00";

            var ct = new CancellationTokenSource(5000);

            var mockProxy = new Mock<IPublicApiHttpProxy>();
            mockProxy.Setup(x => x.GetSnapshot(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new OsloResult
                {
                    Identificator = new OsloIdentificator
                    {
                        Versie = snapshotVersion
                    }
                });

            var snapshotManager = new SnapshotManager(mockProxy.Object, 1, 1);
            var result =  snapshotManager.FindMatchingSnapshot(
                "50083",
                Instant.FromDateTimeOffset(DateTimeOffset.Parse(eventVersion)),
                throwStaleWhenGone: false,
                CancellationToken.None);

            Task.WaitAny(new Task[]{result}, ct.Token);

            result.Result.Should().NotBeNull();
        }

        [Fact]
        public void WhenEventVersionOlderThanSnapshot_ThenReturnNull()
        {
            var eventVersion = $"202{1}-03-23T14:24:04.801+01:00";
            var snapshotVersion = "2022-03-23T14:24:04+01:00";

            var ct = new CancellationTokenSource(5000);

            var mockProxy = new Mock<IPublicApiHttpProxy>();
            mockProxy.Setup(x => x.GetSnapshot(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new OsloResult
                {
                    Identificator = new OsloIdentificator
                    {
                        Versie = snapshotVersion
                    }
                });

            var snapshotManager = new SnapshotManager(mockProxy.Object, 1, 1);
            var result = snapshotManager.FindMatchingSnapshot(
                "50083",
                Instant.FromDateTimeOffset(DateTimeOffset.Parse(eventVersion)),
                throwStaleWhenGone: false,
                CancellationToken.None);

            Task.WaitAny(new Task[] { result }, ct.Token);

            result.Result.Should().BeNull();
        }

        [Fact]
        public async Task WhenStaleSnapshot_ThenRetry()
        {
            var maxRetryWaitIntervalSeconds = 2;
            var retryBackoffFactor = 1;

            var eventVersion = $"2022-03-23T1{4}:24:04+01:00";
            var staleSnapshotVersion = $"2022-03-23T1{1}:24:04+01:00";

            var count = 0;

            var mockProxy = new Mock<IPublicApiHttpProxy>();

            mockProxy.Setup(x => x.GetSnapshot(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(() =>
                {
                    if (count == maxRetryWaitIntervalSeconds)
                    {
                        // Return snapshot with correct version
                        return new OsloResult
                        {
                            Identificator = new OsloIdentificator
                            {
                                Versie = eventVersion
                            }
                        };
                    }

                    count++;

                    // Return stale snapshot
                    return new OsloResult
                    {
                        Identificator = new OsloIdentificator
                        {
                            Versie = staleSnapshotVersion
                        }
                    };
                });

            var snapshotManager = new SnapshotManager(mockProxy.Object, maxRetryWaitIntervalSeconds, retryBackoffFactor);
            var result = await snapshotManager.FindMatchingSnapshot(
                "50083",
                Instant.FromDateTimeOffset(DateTimeOffset.Parse(eventVersion)),
                throwStaleWhenGone: false,
                CancellationToken.None);

            result.Should().NotBeNull();
            mockProxy.Verify(x => x.GetSnapshot(It.IsAny<string>(), CancellationToken.None), () => Times.AtMost(maxRetryWaitIntervalSeconds + 1));
        }

        [Fact]
        public async Task WhenGetSnapshotResponse410AndThrowStaleWhenGone_ThenRetry()
        {
            var maxRetryWaitIntervalSeconds = 2;
            var retryBackoffFactor = 1;

            var throwStaleWhenGone = true;

            var eventVersion = "2022-03-23T14:24:04+01:00";

            var count = 0;

            var mockProxy = new Mock<IPublicApiHttpProxy>();

            mockProxy.Setup(x => x.GetSnapshot(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(() =>
                {
                    // Circuit breaker
                    if (count == maxRetryWaitIntervalSeconds)
                    {
                        return new OsloResult
                        {
                            Identificator = new OsloIdentificator
                            {
                                Versie = eventVersion
                            }
                        };
                    }

                    count++;

                    throw new HttpRequestException(string.Empty, null, HttpStatusCode.Gone);
                });

            var snapshotManager = new SnapshotManager(mockProxy.Object, maxRetryWaitIntervalSeconds, retryBackoffFactor);
            var result = await snapshotManager.FindMatchingSnapshot(
                "50083",
                Instant.FromDateTimeOffset(DateTimeOffset.Parse(eventVersion)),
                throwStaleWhenGone,
                CancellationToken.None);

            result.Should().NotBeNull();
            mockProxy.Verify(x => x.GetSnapshot(It.IsAny<string>(), CancellationToken.None), () => Times.AtMost(maxRetryWaitIntervalSeconds + 1));
        }

        [Fact]
        public async Task WhenGetSnapshotResponse410_ThenReturnNull()
        {
            var maxRetryWaitIntervalSeconds = 2;
            var retryBackoffFactor = 1;

            var doNotThrowStaleWhenGone = false;

            var mockProxy = new Mock<IPublicApiHttpProxy>();

            mockProxy.Setup(x => x.GetSnapshot(It.IsAny<string>(), CancellationToken.None))
                .Throws(new HttpRequestException(string.Empty, null, HttpStatusCode.Gone));

            var snapshotManager = new SnapshotManager(mockProxy.Object, maxRetryWaitIntervalSeconds, retryBackoffFactor);
            var result = await snapshotManager.FindMatchingSnapshot(
                "50083",
                Instant.FromDateTimeOffset(DateTimeOffset.Parse("2022-03-23T14:24:04+01:00")),
                doNotThrowStaleWhenGone,
                CancellationToken.None);

            result.Should().BeNull();
            mockProxy.Verify(x => x.GetSnapshot(It.IsAny<string>(), CancellationToken.None), Times.AtMostOnce);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.MethodNotAllowed)]
        [InlineData(HttpStatusCode.NotAcceptable)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData(HttpStatusCode.Conflict)]
        [InlineData(HttpStatusCode.PreconditionFailed)]
        [InlineData(HttpStatusCode.TooManyRequests)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.NotImplemented)]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.GatewayTimeout)]
        public async Task WhenGetSnapshotResponseInList_ThenRetry(HttpStatusCode httpStatusCode)
        {
            var maxRetryWaitIntervalSeconds = 1;
            var retryBackoffFactor = 0;

            var eventVersion = "2022-03-23T14:24:04+01:00";

            var count = 0;

            var mockProxy = new Mock<IPublicApiHttpProxy>();

            mockProxy.Setup(x => x.GetSnapshot(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(() =>
                {
                    if (count == maxRetryWaitIntervalSeconds)
                    {
                        // Circuit breaker
                        return new OsloResult
                        {
                            Identificator = new OsloIdentificator
                            {
                                Versie = eventVersion
                            }
                        };
                    }

                    count++;

                    throw new HttpRequestException(string.Empty, null, httpStatusCode);
                });

            var snapshotManager = new SnapshotManager(mockProxy.Object, maxRetryWaitIntervalSeconds, retryBackoffFactor);
            await snapshotManager.FindMatchingSnapshot(
                "50083",
                Instant.FromDateTimeOffset(DateTimeOffset.Parse(eventVersion)),
                throwStaleWhenGone: false,
                CancellationToken.None);

            mockProxy.Verify(x => x.GetSnapshot(It.IsAny<string>(), CancellationToken.None), () => Times.Exactly(maxRetryWaitIntervalSeconds + 1));
        }
    }
}
