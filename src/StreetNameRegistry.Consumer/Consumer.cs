namespace StreetNameRegistry.Consumer
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Infrastructure.Projections;
    using Microsoft.Extensions.Logging;

    public class Consumer
    {
        private readonly ILifetimeScope _container;
        private readonly ILoggerFactory _loggerFactory;
        private readonly KafkaOptions _options;
        private readonly string _topic;

        public Consumer(ILifetimeScope container, ILoggerFactory loggerFactory, KafkaOptions options, string topic)
        {
            _container = container;
            _loggerFactory = loggerFactory;
            _options = options;
            _topic = topic;
        }

        public async Task Start(CancellationToken cancellationToken = default)
        {
            var commandHandler = new CommandHandler(_container, _loggerFactory.CreateLogger<CommandHandler>());
            var projector = new ConnectedProjector<CommandHandler>(Resolve.WhenEqualToHandlerMessageType(new MunicipalityKafkaProjection().Handlers));

            var consumerGroupId = $"{nameof(StreetNameRegistry)}.{nameof(Consumer)}.{_topic}_2";
            var result = await KafkaConsumer.Consume(
                _options,
                consumerGroupId,
                _topic,
                async message =>
                {
                    await projector.ProjectAsync(commandHandler, message, cancellationToken);
                },
                cancellationToken);

            if (!result.IsSuccess)
            {
                var logger = _loggerFactory.CreateLogger<Consumer>();
                logger.LogCritical($"Consumer group {consumerGroupId} could not consume from topic {_topic}");
            }
        }
    }
}
