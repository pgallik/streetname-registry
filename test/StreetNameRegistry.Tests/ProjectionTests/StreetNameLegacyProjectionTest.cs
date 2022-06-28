namespace StreetNameRegistry.Tests.ProjectionTests
{
    using System;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Testing;
    using Microsoft.EntityFrameworkCore;
    using Projections.Legacy;

    public class StreetNameLegacyProjectionTest<TProjection>
        where TProjection : ConnectedProjection<LegacyContext>, new()
    {
        protected ConnectedProjectionTest<LegacyContext, TProjection> Sut { get; }

        public StreetNameLegacyProjectionTest()
        {
            Sut = new ConnectedProjectionTest<LegacyContext, TProjection>(CreateContext, () => new TProjection());
        }

        protected virtual LegacyContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<LegacyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new LegacyContext(options);
        }
    }
}
