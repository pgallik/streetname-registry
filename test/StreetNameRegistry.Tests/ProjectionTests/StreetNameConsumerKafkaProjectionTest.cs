namespace StreetNameRegistry.Tests.ProjectionTests
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Testing;
    using Consumer;
    using Microsoft.EntityFrameworkCore;
    using Testing;
    using Xunit.Abstractions;

    public class StreetNameConsumerKafkaProjectionTest<TProjection> : ProjectionTest<ConsumerContext, TProjection>
        where TProjection : ConnectedProjection<ConsumerContext>, new()
    {
        protected ConnectedProjectionTest<ConsumerContext, TProjection> Sut { get; }

        public StreetNameConsumerKafkaProjectionTest(ITestOutputHelper output)
            : base(output)
        {
            Sut = new ConnectedProjectionTest<ConsumerContext, TProjection>(CreateContext);
        }

        protected override ConsumerContext CreateContext(DbContextOptions<ConsumerContext> options) => new ConsumerContext(options);
    }
}
