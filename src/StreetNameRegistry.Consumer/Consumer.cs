namespace StreetNameRegistry.Consumer
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Infrastructure.Projections;

    public class Consumer
    {
        private readonly ILifetimeScope _container;
        private readonly KafkaOptions _options;
        private readonly string _topic;

        public Consumer(ILifetimeScope container, KafkaOptions options, string topic)
        {
            _container = container;
            _options = options;
            _topic = topic;
        }

        public async Task Start(CancellationToken cancellationToken = default)
        {
            var commandHandler = new CommandHandler(_container);
            var projector = new ConnectedProjector<CommandHandler>(Resolve.WhenEqualToHandlerMessageType(new MunicipalityKafkaProjection().Handlers));

            await KafkaConsumer.Consume(
                _options,
                $"{nameof(StreetNameRegistry)}.{nameof(Consumer)}.{_topic}",
                _topic,
                async message =>
                {
                    await projector.ProjectAsync(commandHandler, message, cancellationToken);
                },
                cancellationToken);
        }
    }
}
