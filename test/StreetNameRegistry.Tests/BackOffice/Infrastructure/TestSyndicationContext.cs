using System;

namespace StreetNameRegistry.Tests.BackOffice.Infrastructure
{
    using global::AutoFixture;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Projections.Syndication;
    using Projections.Syndication.Municipality;
    
    public class TestSyndicationContext : SyndicationContext
    {
        // This needs to be here to please EF
        public TestSyndicationContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public TestSyndicationContext(DbContextOptions<SyndicationContext> options)
            : base(options) { }

        public MunicipalityLatestItem AddMunicipalityLatestItemFixture()
        {
            var municipalityLatestItem = new Fixture().Create<MunicipalityLatestItem>();
            MunicipalityLatestItems.Add(municipalityLatestItem);
            SaveChanges();
            return municipalityLatestItem;
        }
    }

    public class FakeSyndicationContextFactory : IDesignTimeDbContextFactory<TestSyndicationContext>
    {
        public TestSyndicationContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SyndicationContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            return new TestSyndicationContext(builder.Options);
        }
    }
}
