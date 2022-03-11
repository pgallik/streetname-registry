namespace StreetNameRegistry.Tests.BackOffice.Infrastructure
{
    using System;
    using global::AutoFixture;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using StreetNameRegistry.Api.BackOffice;

    public class TestBackOfficeContext : BackOfficeContext
    {
        // This needs to be here to please EF
        public TestBackOfficeContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public TestBackOfficeContext(DbContextOptions<BackOfficeContext> options)
            : base(options) { }

        public MunicipalityIdByPersistentLocalId AddMunicipalityIdByPersistentLocalIdToFixture()
        {
            var item = new Fixture().Create<MunicipalityIdByPersistentLocalId>();
            MunicipalityIdByPersistentLocalId.Add(item);
            SaveChanges();
            return item;
        }
    }

    public class FakeBackOfficeContextFactory : IDesignTimeDbContextFactory<TestBackOfficeContext>
    {
        public TestBackOfficeContext CreateDbContext(params string[] args)
        {
            var builder = new DbContextOptionsBuilder<BackOfficeContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
            return new TestBackOfficeContext(builder.Options);
        }
    }
}
