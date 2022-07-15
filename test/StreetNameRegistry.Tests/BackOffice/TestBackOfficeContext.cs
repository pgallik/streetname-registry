namespace StreetNameRegistry.Tests.BackOffice
{
    using System;
    using global::AutoFixture;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using StreetNameRegistry.Api.BackOffice.Abstractions;

    public class TestBackOfficeContext : BackOfficeContext
    {
        // This needs to be here to please EF
        public TestBackOfficeContext() { }

        // This needs to be DbContextOptions<T> for Autofac!
        public TestBackOfficeContext(DbContextOptions<BackOfficeContext> options)
            : base(options) { }

        public MunicipalityIdByPersistentLocalId AddMunicipalityIdByPersistentLocalIdToFixture(int? persistentLocalId = null, Guid? municipalityId = null)
        {
            var item = new Fixture().Create<MunicipalityIdByPersistentLocalId>();
            if (persistentLocalId is not null)
            {
                item.PersistentLocalId = persistentLocalId.Value;
            }

            if (municipalityId is not null)
            {
                item.MunicipalityId = municipalityId.Value;
            }
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
