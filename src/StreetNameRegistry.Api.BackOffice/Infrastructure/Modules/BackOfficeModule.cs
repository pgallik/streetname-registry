namespace StreetNameRegistry.Api.BackOffice.Infrastructure.Modules
{
    using Abstractions;
    using Autofac;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using StreetNameRegistry.Infrastructure;

    public class BackOfficeModule : Module
    {
        public BackOfficeModule(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            var projectionsConnectionString = configuration.GetConnectionString("BackOffice");

            services
                .AddDbContext<BackOfficeContext>(options => options
                    .UseLoggerFactory(loggerFactory)
                    .UseSqlServer(projectionsConnectionString, sqlServerOptions => sqlServerOptions
                            .EnableRetryOnFailure()
                            .MigrationsHistoryTable(MigrationTables.BackOffice, Schema.BackOffice)
                    ));
        }
    }
}
