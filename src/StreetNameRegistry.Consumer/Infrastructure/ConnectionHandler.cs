namespace StreetNameRegistry.Consumer.Infrastructure
{
    using System;
    using System.Threading;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;

    public class ConnectionHandler<TMessage>
        where TMessage : class
    {
        private readonly KafkaOptions _options;
        private readonly string _topic;
        private Action<ILifetimeScope, TMessage> _handler;

        public ConnectionHandler(ILifetimeScope container, KafkaOptions options, string topic, Action<ILifetimeScope, TMessage> handler, CancellationToken cancellationToken)
        {
            _options = options;
            _topic = topic;
            _handler = handler;

            _ = KafkaConsumer.Consume<TMessage>(_options, $"{nameof(StreetNameRegistry)}.{nameof(Consumer)}.{_topic}", _topic, message =>
            {
                using var scope = container.BeginLifetimeScope();

                _handler.Invoke(scope, message);
            }, cancellationToken);
        }
    }
}
