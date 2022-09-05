namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs
{
    using Amazon;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.EventHandling;
    using Be.Vlaanderen.Basisregisters.MessageHandling.AwsSqs.Simple;

    public class SqsHandlersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(c => new SqsOptions(RegionEndpoint.EUWest1, EventsJsonSerializerSettingsProvider.CreateSerializerSettings()))
                .SingleInstance();

            builder.RegisterType<SqsQueue>()
                .As<ISqsQueue>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
