namespace StreetNameRegistry.Consumer
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts.MunicipalityRegistry;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Infrastructure;

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

        public Task Start()
        {
            var connectedProjection = new ConnectedProjection<MunicipalityBecameCurrent>(_container, _options, _topic);

            connectedProjection.When<MunicipalityBecameCurrent>((scope, message) =>
            {
                Console.WriteLine(message.GetType().FullName);
            });

            return Task.CompletedTask;
        }
    }
}
