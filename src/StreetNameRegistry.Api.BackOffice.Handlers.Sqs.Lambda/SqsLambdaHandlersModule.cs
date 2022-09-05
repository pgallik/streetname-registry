namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda
{
    using Autofac;
    using Handlers;

    public class SqsLambdaHandlersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IdempotentCommandHandler>()
                .As<IIdempotentCommandHandler>();
        }
    }
}
