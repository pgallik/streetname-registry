namespace StreetNameRegistry.Projector.Infrastructure.Modules
{
    using System;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.EventHandling.Autofac;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Autofac;
    using Be.Vlaanderen.Basisregisters.Projector;
    using Be.Vlaanderen.Basisregisters.Projector.ConnectedProjections;
    using Be.Vlaanderen.Basisregisters.Projector.Modules;
    using Be.Vlaanderen.Basisregisters.Shaperon;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using StreetNameRegistry.Infrastructure;
    using StreetNameRegistry.Projections.Extract;
    using StreetNameRegistry.Projections.Extract.StreetNameExtract;
    using StreetNameRegistry.Projections.LastChangedList;
    using StreetNameRegistry.Projections.Legacy;
    using StreetNameRegistry.Projections.Legacy.StreetNameDetail;
    using StreetNameRegistry.Projections.Legacy.StreetNameDetailV2;
    using StreetNameRegistry.Projections.Legacy.StreetNameList;
    using StreetNameRegistry.Projections.Legacy.StreetNameListV2;
    using StreetNameRegistry.Projections.Legacy.StreetNameName;
    using StreetNameRegistry.Projections.Legacy.StreetNameNameV2;
    using StreetNameRegistry.Projections.Wfs;
    using StreetNameRegistry.Projections.Wms;
    using StreetNameRegistry.Projections.Legacy.StreetNameSyndication;

    public class ApiModule : Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;
        private readonly bool _useProjectionsV2;

        public ApiModule(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _services = services;
            _loggerFactory = loggerFactory;
            _useProjectionsV2 = Convert.ToBoolean(_configuration.GetSection(FeatureToggleOptions.ConfigurationKey)[nameof(FeatureToggleOptions.UseProjectionsV2)]);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new DataDogModule(_configuration));
            RegisterProjectionSetup(builder);

            builder
                .RegisterType<ProblemDetailsHelper>()
                .AsSelf();

            builder.Populate(_services);
        }

        private void RegisterProjectionSetup(ContainerBuilder builder)
        {
            builder.RegisterModule(
                new EventHandlingModule(
                    typeof(DomainAssemblyMarker).Assembly,
                    EventsJsonSerializerSettingsProvider.CreateSerializerSettings()
                )
            );

            builder.RegisterModule<EnvelopeModule>();
            builder.RegisterEventstreamModule(_configuration);
            builder.RegisterModule(new ProjectorModule(_configuration));

            RegisterLastChangedProjections(builder);

            if (_useProjectionsV2)
            {
                RegisterExtractProjectionsV2(builder);
                RegisterLegacyProjectionsV2(builder);
                RegisterWfsProjectionsV2(builder);
                RegisterWmsProjectionsV2(builder);
                RegisterWfsProjections(builder); //TODO: Remove when Wfs has been filled in staging
                RegisterWmsProjections(builder); //TODO: Remove when Wms has been filled in staging
            }
            else
            {
                RegisterExtractProjections(builder);
                RegisterLegacyProjections(builder);
                RegisterWfsProjections(builder);
                RegisterWmsProjections(builder);
            }
        }

        private void RegisterExtractProjections(ContainerBuilder builder)
        {
            builder.RegisterModule(
                new ExtractModule(
                    _configuration,
                    _services,
                    _loggerFactory));

            builder
                .RegisterProjectionMigrator<ExtractContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<StreetNameExtractProjections, ExtractContext>(
                    context => new StreetNameExtractProjections(context.Resolve<IOptions<ExtractConfig>>(), DbaseCodePage.Western_European_ANSI.ToEncoding()),
                    ConnectedProjectionSettings.Default);
        }

        private void RegisterExtractProjectionsV2(ContainerBuilder builder)
        {
            builder.RegisterModule(
                new ExtractModule(
                    _configuration,
                    _services,
                    _loggerFactory));

            builder
                .RegisterProjectionMigrator<ExtractContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<StreetNameExtractProjectionsV2, ExtractContext>(
                    context => new StreetNameExtractProjectionsV2(context.Resolve<IOptions<ExtractConfig>>(), DbaseCodePage.Western_European_ANSI.ToEncoding()),
                    ConnectedProjectionSettings.Default);
        }

        private void RegisterLastChangedProjections(ContainerBuilder builder)
        {
            builder.RegisterModule(
                new StreetNameLastChangedListModule(
                    _configuration.GetConnectionString("LastChangedList"),
                    _configuration["DataDog:ServiceName"],
                    _services,
                    _loggerFactory));

            builder
                .RegisterProjectionMigrator<StreetNameRegistry.Projections.LastChangedList.LastChangedListContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjectionMigrator<DataMigrationContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<LastChangedProjections, LastChangedListContext>(ConnectedProjectionSettings.Default);
        }

        private void RegisterLegacyProjections(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new LegacyModule(
                        _configuration,
                        _services,
                        _loggerFactory));
            builder
                .RegisterProjectionMigrator<LegacyContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<StreetNameDetailProjections, LegacyContext>(ConnectedProjectionSettings.Default)
                .RegisterProjections<StreetNameListProjections, LegacyContext>(ConnectedProjectionSettings.Default)
                .RegisterProjections<StreetNameNameProjections, LegacyContext>(ConnectedProjectionSettings.Default)
                .RegisterProjections<StreetNameSyndicationProjections, LegacyContext>(ConnectedProjectionSettings.Default);
        }

        private void RegisterLegacyProjectionsV2(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new LegacyModule(
                        _configuration,
                        _services,
                        _loggerFactory));
            builder
                .RegisterProjectionMigrator<LegacyContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)
                .RegisterProjections<StreetNameDetailProjectionsV2, LegacyContext>(ConnectedProjectionSettings.Default)
                .RegisterProjections<StreetNameListProjectionsV2, LegacyContext>(ConnectedProjectionSettings.Default)
                .RegisterProjections<StreetNameNameProjectionsV2, LegacyContext>(ConnectedProjectionSettings.Default)
                .RegisterProjections<StreetNameSyndicationProjections, LegacyContext>(ConnectedProjectionSettings.Default);
        }

        private void RegisterWfsProjections(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new WfsModule(
                        _configuration,
                        _services,
                        _loggerFactory));

            var wfsProjectionSettings = ConnectedProjectionSettings
                .Configure(settings =>
                    settings.ConfigureLinearBackoff<SqlException>(_configuration, "Wfs"));

            builder
                .RegisterProjectionMigrator<WfsContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)

                .RegisterProjections<StreetNameRegistry.Projections.Wfs.StreetName.StreetNameHelperProjections, WfsContext>(() =>
                        new StreetNameRegistry.Projections.Wfs.StreetName.StreetNameHelperProjections(),
                    wfsProjectionSettings);
        }

        private void RegisterWfsProjectionsV2(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new WfsModule(
                        _configuration,
                        _services,
                        _loggerFactory));

            var wfsProjectionSettings = ConnectedProjectionSettings
                .Configure(settings =>
                    settings.ConfigureLinearBackoff<SqlException>(_configuration, "Wfs"));

            builder
                .RegisterProjectionMigrator<WfsContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)

                .RegisterProjections<StreetNameRegistry.Projections.Wfs.StreetNameHelperV2.StreetNameHelperV2Projections, WfsContext>(() =>
                        new StreetNameRegistry.Projections.Wfs.StreetNameHelperV2.StreetNameHelperV2Projections(),
                    wfsProjectionSettings);
        }

        private void RegisterWmsProjections(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new WmsModule(
                        _configuration,
                        _services,
                        _loggerFactory));

            var wmsProjectionSettings = ConnectedProjectionSettings
                .Configure(settings =>
                    settings.ConfigureLinearBackoff<SqlException>(_configuration, "Wms"));

            builder
                .RegisterProjectionMigrator<WmsContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)

                .RegisterProjections<StreetNameRegistry.Projections.Wms.StreetName.StreetNameHelperProjections, WmsContext>(() =>
                        new StreetNameRegistry.Projections.Wms.StreetName.StreetNameHelperProjections(),
                    wmsProjectionSettings);
        }

        private void RegisterWmsProjectionsV2(ContainerBuilder builder)
        {
            builder
                .RegisterModule(
                    new WmsModule(
                        _configuration,
                        _services,
                        _loggerFactory));

            var wmsProjectionSettings = ConnectedProjectionSettings
                .Configure(settings =>
                    settings.ConfigureLinearBackoff<SqlException>(_configuration, "Wms"));

            builder
                .RegisterProjectionMigrator<WmsContextMigrationFactory>(
                    _configuration,
                    _loggerFactory)

                .RegisterProjections<StreetNameRegistry.Projections.Wms.StreetNameHelperV2.StreetNameHelperV2Projections, WmsContext>(() =>
                        new StreetNameRegistry.Projections.Wms.StreetNameHelperV2.StreetNameHelperV2Projections(),
                    wmsProjectionSettings);
        }
    }
}
