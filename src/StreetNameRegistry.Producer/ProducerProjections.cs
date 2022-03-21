namespace StreetNameRegistry.Producer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Extensions;
    using Microsoft.Extensions.Configuration;
    using Domain = StreetNameRegistry.StreetName.Events;

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


            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameBecameComplete>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameBecameCurrent>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameBecameIncomplete>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameHomonymAdditionWasCleared>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameHomonymAdditionWasCorrected>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameHomonymAdditionWasCorrectedToCleared>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameHomonymAdditionWasDefined>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameNameWasCleared>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameNameWasCorrected>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameNameWasCorrectedToCleared>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNamePersistentLocalIdWasAssigned>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNamePrimaryLanguageWasCleared>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNamePrimaryLanguageWasCorrected>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNamePrimaryLanguageWasCorrectedToCleared>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNamePrimaryLanguageWasDefined>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameSecondaryLanguageWasCleared>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameSecondaryLanguageWasCorrected>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameSecondaryLanguageWasCorrectedToCleared>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameSecondaryLanguageWasDefined>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameStatusWasCorrectedToRemoved>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameStatusWasRemoved>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            // TODO: review
            //When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameWasApproved>>(async (_, message, ct) =>
            //{
            //    await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            //});

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameWasCorrectedToCurrent>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameWasCorrectedToProposed>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameWasCorrectedToRetired>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameWasMigrated>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameWasMigratedToMunicipality>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameWasNamed>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameWasProposed>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            // TODO: review
            //When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameWasProposedV2>>(async (_, message, ct) =>
            //{
            //    await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            //});

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameWasRegistered>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameWasRemoved>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameWasRetired>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });

            When<Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Envelope<Domain.StreetNameBecameComplete>>(async (_, message, ct) =>
            {
                await Produce(message.Message.StreetNameId, message.Message.ToContract(), ct);
            });
        }

        // TODO: review usage of streetNameId ...
        private async Task Produce<T>(Guid streetNameId, T message, CancellationToken cancellationToken = default)
            where T : class, IQueueMessage
        {
            var result = await KafkaProducer.Produce(_kafkaOptions, _topic, streetNameId.ToString("D"), message, cancellationToken);
            if (!result.IsSuccess)
            {
                throw new ApplicationException(result.Error + Environment.NewLine + result.ErrorReason); //TODO: create custom exception
            }
        }
    }
}
