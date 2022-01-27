namespace StreetNameRegistry.Consumer
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.GrAr.Contracts.MunicipalityRegistry;
    using Be.Vlaanderen.Basisregisters.GrAr.Provenance;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple;
    using Be.Vlaanderen.Basisregisters.MessageHandling.Kafka.Simple.Extensions;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Confluent.Kafka;
    using Newtonsoft.Json;
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

    public static class KafkaConsumer
    {
        public static async Task<Result> Consume(
            KafkaOptions options,
            string consumerGroupId,
            string topic,
            Func<object, Task> messageHandler,
            CancellationToken cancellationToken = default)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = options.BootstrapServers,
                GroupId = consumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            var serializer = JsonSerializer.CreateDefault(options.JsonSerializerSettings);

            using var consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();
            try
            {
                consumer.Subscribe(topic);

                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(3));
                    if(consumeResult == null)
                        continue;
                    
                    var kafkaJsonMessage = serializer.Deserialize<KafkaJsonMessage>(consumeResult.Message.Value) ?? throw new ArgumentException("Kafka json message is null.");
                    var messageData = kafkaJsonMessage.Map() ?? throw new ArgumentException("Kafka message data is null.");

                    await messageHandler(messageData);
                    consumer.Commit(consumeResult);
                }

                return Result.Success();
            }
            catch (ConsumeException ex)
            {
                return Result.Failure(ex.Error.Code.ToString(), ex.Error.Reason);
            }
            catch (OperationCanceledException)
            {
                return Result.Success();
            }
            finally
            {
                consumer.Unsubscribe();
            }
        }
    }

    public class KafkaJsonMessage
    {
        public string Type { get; set; }
        public string Data { get; set; }

        public KafkaJsonMessage(string type, string data)
        {
            Type = type;
            Data = data;
        }

        public object Map()
        {
            var assembly = GetAssemblyNameContainingType(Type);
            var type = assembly.GetType(Type);

            return JsonConvert.DeserializeObject(Data, type!)!;
        }

        private static Assembly? GetAssemblyNameContainingType(string typeName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = assembly.GetType(typeName, false, true);
                if (t != null) { return assembly; }
            }

            return null;
        }

        public static KafkaJsonMessage Create<T>([DisallowNull] T message, JsonSerializer serializer)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            return new KafkaJsonMessage(message.GetType().FullName!, serializer.Serialize(message));
        }
    }


}
