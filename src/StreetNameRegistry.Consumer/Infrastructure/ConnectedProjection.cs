namespace StreetNameRegistry.Consumer.Infrastructure
{
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Autofac;

    public class ConnectedProjection<TMsg>
        where TMsg : class
    {
        private readonly ILifetimeScope _container;
        private readonly KafkaOptions _options;
        private readonly string _topic;
        private readonly Dictionary<Type, ConnectionHandler<TMsg>> _handlers = new Dictionary<Type, ConnectionHandler<TMsg>>();

        public ConnectedProjection(ILifetimeScope container, KafkaOptions options, string topic)
        {
            _container = container;
            _options = options;
            _topic = topic;
        }

        public void When<TMessage>(Action<ILifetimeScope, TMsg> handler)
            where TMessage : class
        {
            var cancellationToken = CancellationToken.None;

            var connectionHandler = new ConnectionHandler<TMsg>(_container, _options, _topic, handler, cancellationToken);
            _handlers.Add(typeof(TMessage), connectionHandler);
        }
    }
}
