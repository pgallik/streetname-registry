namespace StreetNameRegistry.Consumer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts.MunicipalityRegistry;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using NodaTime.Extensions;
    using StreetName.Commands;

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

    public class MunicipalityKafkaProjection : ConnectedProjection<CommandHandler>
    {
        public MunicipalityKafkaProjection()
        {
            When<MunicipalityWasRegistered>(async (commandHandler, message, ct) =>
            {
                var importMunicipality = new ImportMunicipality(
                    new MunicipalityId(Guid.Parse(message.MunicipalityId)),
                    new NisCode(message.NisCode),
                    new Provenance(
                        DateTimeOffset.Parse(message.Provenance.Timestamp).ToInstant(), //TODO: Check other conversions
                        Enum.Parse<Application>(message.Provenance.Application),
                        new Reason(message.Provenance.Reason),
                        new Operator(message.Provenance.Operator),
                        Enum.Parse<Modification>(message.Provenance.Modification),
                        Enum.Parse<Organisation>(message.Provenance.Organisation)));

                await commandHandler.Handle(importMunicipality);
            });
        }
    }

    public class CommandHandler
    {
        private readonly ILifetimeScope _container;

        public CommandHandler(ILifetimeScope container)
        {
            _container = container;
        }

        public async Task Handle<T>(T command)
        {
            await Task.Delay(5000);
            Console.WriteLine("bla bla");
        }
    }
}
