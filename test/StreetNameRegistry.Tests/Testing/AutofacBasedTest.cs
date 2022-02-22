namespace StreetNameRegistry.Tests.Testing
{
    using System;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.AggregateSource.SqlStreamStore.Autofac;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing.Comparers;
    using Be.Vlaanderen.Basisregisters.AggregateSource.Testing.SqlStreamStore.Autofac;
    using KellermanSoftware.CompareNetObjects;
    using Microsoft.Extensions.Logging;
    using Municipality;
    using Newtonsoft.Json;
    using Xunit.Abstractions;

    public abstract class AutofacBasedTest
    {
        private readonly Lazy<IContainer> _container;

        protected IContainer Container => _container.Value;

        protected IExceptionCentricTestSpecificationRunner ExceptionCentricTestSpecificationRunner => Container.Resolve<IExceptionCentricTestSpecificationRunner>();

        protected IEventCentricTestSpecificationRunner EventCentricTestSpecificationRunner => Container.Resolve<IEventCentricTestSpecificationRunner>();

        protected IFactComparer FactComparer => Container.Resolve<IFactComparer>();

        protected IExceptionComparer ExceptionComparer => Container.Resolve<IExceptionComparer>();

        protected ILogger Logger => Container.Resolve<ILogger>();

        protected AutofacBasedTest(ITestOutputHelper testOutputHelper)
        {
            _container = new Lazy<IContainer>(() =>
            {
                var containerBuilder = new ContainerBuilder();

                ConfigureEventHandling(containerBuilder);
                ConfigureCommandHandling(containerBuilder);
                containerBuilder.RegisterModule(new SqlStreamStoreModule());

                containerBuilder.UseAggregateSourceTesting(CreateFactComparer(), CreateExceptionComparer());

                containerBuilder.RegisterInstance(testOutputHelper);
                containerBuilder.RegisterType<XUnitLogger>().AsImplementedInterfaces();
                containerBuilder.RegisterType<FakePersistentLocalIdGenerator>().As<IPersistentLocalIdGenerator>();

                return containerBuilder.Build();
            });

            Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Testing.LogExtensions.LogSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        protected abstract void ConfigureCommandHandling(ContainerBuilder builder);
        protected abstract void ConfigureEventHandling(ContainerBuilder builder);

        protected virtual IFactComparer CreateFactComparer()
        {
            var comparer = new CompareLogic();
            comparer.Config.MembersToIgnore.Add("Provenance");
            return new CompareNetObjectsBasedFactComparer(comparer);
        }

        protected virtual IExceptionComparer CreateExceptionComparer()
        {
            var comparer = new CompareLogic();
            comparer.Config.MembersToIgnore.Add("Source");
            comparer.Config.MembersToIgnore.Add("StackTrace");
            comparer.Config.MembersToIgnore.Add("TargetSite");
            return new CompareNetObjectsBasedExceptionComparer(comparer);
        }

        protected void Assert(IExceptionCentricTestSpecificationBuilder builder)
            => builder.Assert(ExceptionCentricTestSpecificationRunner, ExceptionComparer, Logger);

        protected void Assert(IEventCentricTestSpecificationBuilder builder)
            => builder.Assert(EventCentricTestSpecificationRunner, FactComparer, Logger);
        }
}
