namespace StreetNameRegistry.Producer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Microsoft.Extensions.Configuration;
    using Domain = StreetNameRegistry.Municipality.Events;

    [ConnectedProjectionName("Kafka producer")]
    [ConnectedProjectionDescription("Projectie die berichten naar de kafka broker stuurt.")]
    public class ProducerProjections : ConnectedProjection<ProducerContext>
    {
        private readonly KafkaOptions _kafkaOptions;
        private readonly string _topic;
        private string _streetnameTopicKey = "StreetNameTopic";

        public ProducerProjections(IConfiguration configuration)
        {
            var bootstrapServers = configuration["Kafka:BootstrapServers"];
            _kafkaOptions = new KafkaOptions(bootstrapServers, EventsJsonSerializerSettingsProvider.CreateSerializerSettings());
            _topic = $"{configuration[_streetnameTopicKey]}" ?? throw new ArgumentException($"Configuration has no value for {_streetnameTopicKey}");


            //When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.MunicipalityWasDrawn>>(async (context, message, ct) =>
            //{
            //    await Produce(message.Message.MunicipalityId, message.Message.ToContract(), ct);
            //});
        }

        private async Task Produce<T>(Guid municipalityId, T message, CancellationToken cancellationToken = default)
            where T : class, IQueueMessage
        {
            var result = await KafkaProducer.Produce(_kafkaOptions, _topic, municipalityId.ToString("D"), message, cancellationToken);
            if (!result.IsSuccess)
                throw new ApplicationException(result.Error + Environment.NewLine + result.ErrorReason); //TODO: create custom exception
        }
    }
}
