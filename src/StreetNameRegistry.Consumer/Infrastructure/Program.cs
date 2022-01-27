namespace StreetNameRegistry.Consumer.Infrastructure
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Aws.DistributedMutex;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Modules;
    using Serilog;

    public class Program
    {
        private static readonly AutoResetEvent Closing = new AutoResetEvent(false);
        private static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public static async Task Main(string[] args)
        {
            var ct = CancellationTokenSource.Token;

            ct.Register(() => Closing.Set());
            Console.CancelKeyPress += (sender, eventArgs) => CancellationTokenSource.Cancel();

            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
                Log.Debug(
                    eventArgs.Exception,
                    "FirstChanceException event raised in {AppDomain}.",
                    AppDomain.CurrentDomain.FriendlyName);

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                Log.Fatal((Exception) eventArgs.ExceptionObject, "Encountered a fatal exception, exiting program.");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .AddCommandLine(args ?? new string[0])
                .Build();

            var container = ConfigureServices(configuration);

            Log.Information("Starting StreetNameRegistry.Projections.Syndication");

            try
            {
                await DistributedLock<Program>.RunAsync(
                    async () =>
                    {
                        try
                        {
                            var bootstrapServers = configuration["Kafka:BootstrapServers"];
                            var schemaRegistryUrl = configuration["Kafka:SchemaRegistryUrl"];
                            var kafkaOptions = new KafkaOptions(bootstrapServers, schemaRegistryUrl);

                            var topic = $"{configuration["MunicipalityTopic"]}" ?? throw new ArgumentException("Configuration has no MunicipalityTopic.");

                            var actualContainer = container.GetRequiredService<ILifetimeScope>();
                            var consumer = new Consumer(actualContainer, kafkaOptions, topic);
                            await consumer.Start();
                        }
                        catch (Exception e)
                        {
                            Log.Fatal(e, "Encountered a fatal exception, exiting program.");
                            throw;
                        }
                    },
                    DistributedLockOptions.LoadFromConfiguration(configuration) ?? DistributedLockOptions.Defaults,
                    container.GetService<ILogger<Program>>()!);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Encountered a fatal exception, exiting program.");
                Log.CloseAndFlush();

                // Allow some time for flushing before shutdown.
                Task.Delay(1000, default).GetAwaiter().GetResult();
                throw;
            }

            Log.Information("Stopping...");
            Closing.Close();
        }

        private static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();
            var builder = new ContainerBuilder();

            builder.RegisterModule(new LoggingModule(configuration, services));

            builder.Populate(services);

            return new AutofacServiceProvider(builder.Build());
        }
    }
}
