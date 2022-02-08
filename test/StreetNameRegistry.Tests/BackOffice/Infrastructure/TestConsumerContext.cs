namespace StreetNameRegistry.Tests.BackOffice.Infrastructure
{
    using System;
    using global::AutoFixture;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Consumer;
    using Consumer.Municipality;
    
    public class TestConsumerContext : ConsumerContext
    {
        // This needs to be here to please EF
        public TestConsumerContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public TestConsumerContext(DbContextOptions<ConsumerContext> options)
            : base(options) { }

        public MunicipalityConsumerItem AddMunicipalityLatestItemFixture()
        {
            var municipalityLatestItem = new Fixture().Create<MunicipalityConsumerItem>();
            MunicipalityConsumerItems.Add(municipalityLatestItem);
            SaveChanges();
            return municipalityLatestItem;
        }
    }

    public class FakeConsumerContextFactory : IDesignTimeDbContextFactory<TestConsumerContext>
    {
        public TestConsumerContext CreateDbContext(params string[] args)
        {
            var builder = new DbContextOptionsBuilder<ConsumerContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            return new TestConsumerContext(builder.Options);
        }
    }
}
