namespace StreetNameRegistry.Api.BackOffice
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using StreetNameRegistry.Infrastructure;

    public class BackOfficeContext : DbContext
    {
        public BackOfficeContext() { }

        public BackOfficeContext(DbContextOptions<BackOfficeContext> options)
            : base(options) { }

        public DbSet<MunicipalityIdByPersistentLocalId> MunicipalityIdByPersistentLocalId { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MunicipalityIdByPersistentLocalId>()
                .ToTable(nameof(MunicipalityIdByPersistentLocalId), Schema.BackOffice)
                .HasKey(x => x.PersistentLocalId)
                .IsClustered();

            modelBuilder.Entity<MunicipalityIdByPersistentLocalId>()
                .Property(x => x.PersistentLocalId)
                .ValueGeneratedNever();

            modelBuilder.Entity<MunicipalityIdByPersistentLocalId>()
                .Property(x => x.MunicipalityId);
        }
    }

    public class MunicipalityIdByPersistentLocalId
    {
        public int PersistentLocalId { get; set; }
        public Guid MunicipalityId { get; set; }

        private MunicipalityIdByPersistentLocalId()
        { }

        public MunicipalityIdByPersistentLocalId(int persistentLocalId, Guid municipalityId)
        {
            PersistentLocalId = persistentLocalId;
            MunicipalityId = municipalityId;
        }
    }

    public class ConfigBasedSequenceContextFactory : IDesignTimeDbContextFactory<BackOfficeContext>
    {
        public BackOfficeContext CreateDbContext(string[] args)
        {
            var migrationConnectionStringName = "BackOffice";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<BackOfficeContext>();

            var connectionString = configuration.GetConnectionString(migrationConnectionStringName);
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException(
                    $"Could not find a connection string with name '{migrationConnectionStringName}'");

            builder
                .UseSqlServer(connectionString, sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure();
                    sqlServerOptions.MigrationsHistoryTable(MigrationTables.BackOffice, Schema.BackOffice);
                });

            return new BackOfficeContext(builder.Options);
        }
    }
}
