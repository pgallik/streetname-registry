namespace StreetNameRegistry.Tests.Testing
{
    using System.Collections.Generic;
    using Api.BackOffice.Handlers.Sqs.Lambda;
    using Api.BackOffice.Handlers.Sqs.Lambda.Handlers;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Snapshotting;
    using Be.Vlaanderen.Basisregisters.AggregateSource.SqlStreamStore.Autofac;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.EventHandling.Autofac;
    using global::AutoFixture;
    using Infrastructure.Modules;
    using Microsoft.Extensions.Configuration;
    using Municipality;
    using Newtonsoft.Json;
    using Xunit.Abstractions;

    public class StreetNameRegistryTest : AutofacBasedTest
    {
        protected Fixture Fixture { get; }

        protected JsonSerializerSettings EventSerializerSettings { get; } = EventsJsonSerializerSettingsProvider.CreateSerializerSettings();
        public StreetNameRegistryTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            Fixture = new Fixture();
            Fixture.Register(() => (ISnapshotStrategy)NoSnapshotStrategy.Instance);
        }

        protected override void ConfigureCommandHandling(ContainerBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "ConnectionStrings:Events", "x" } })
                .AddInMemoryCollection(new Dictionary<string, string> { { "ConnectionStrings:Snapshots", "x" } })
                .Build();

            builder
                .RegisterModule(new CommandHandlingModule(configuration))
                .RegisterModule(new SqlStreamStoreModule())
                .RegisterModule(new SqsLambdaHandlersModule());

            builder.RegisterModule(new SqlSnapshotStoreModule());

            builder
                .Register(c => new MunicipalityFactory(Fixture.Create<ISnapshotStrategy>()))
                .As<IMunicipalityFactory>();
        }

        protected override void ConfigureEventHandling(ContainerBuilder builder)
        {
            var eventSerializerSettings = EventsJsonSerializerSettingsProvider.CreateSerializerSettings();
            builder.RegisterModule(new EventHandlingModule(typeof(DomainAssemblyMarker).Assembly, eventSerializerSettings));
        }
        public string GetSnapshotIdentifier(string identifier) => $"{identifier}-snapshots";
    }
}
