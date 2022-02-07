namespace StreetNameRegistry.Tests.ProjectionTests
{
    using System;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Testing;
    using Consumer;
    using Microsoft.EntityFrameworkCore;

    public class StreetNameConsumerKafkaProjectionTest<TProjection>
        where TProjection : ConnectedProjection<ConsumerContext>, new()
    {
        protected ConnectedProjectionTest<ConsumerContext, TProjection> Sut { get; }

        public StreetNameConsumerKafkaProjectionTest()
        {
            Sut = new ConnectedProjectionTest<ConsumerContext, TProjection>(CreateContext);
        }

        protected virtual ConsumerContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ConsumerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ConsumerContext(options);
        }
    }
}
