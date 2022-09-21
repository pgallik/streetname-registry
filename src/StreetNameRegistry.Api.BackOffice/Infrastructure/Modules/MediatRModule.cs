namespace StreetNameRegistry.Api.BackOffice.Infrastructure.Modules
{
    using System.Reflection;
    using Autofac;
    using Handlers;
    using Handlers.Sqs.Handlers;
    using MediatR;
    using Module = Autofac.Module;

    public sealed class MediatRModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            // request & notification handlers
            builder.Register<ServiceFactory>(context =>
            {
                var ctx = context.Resolve<IComponentContext>();
                return type => ctx.Resolve(type);
            });

            builder.RegisterAssemblyTypes(typeof(StreetNameProposeHandler).GetTypeInfo().Assembly).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(SqsStreetNameProposeHandler).GetTypeInfo().Assembly).AsImplementedInterfaces();
        }
    }
}
