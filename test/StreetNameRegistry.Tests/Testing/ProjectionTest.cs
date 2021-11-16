namespace StreetNameRegistry.Tests.Testing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Testing;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using FluentAssertions.Execution;
    using Generate;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Projections.Legacy;
    using Xunit.Abstractions;

    public class ConnectedProjectionScenarioWrapper<TContext> where TContext : DbContext
    {
        private readonly Func<TContext> _contextFactory;
        private ConnectedProjectionScenario<TContext> _inner;
        private readonly Random _random;
        private readonly Action<string> _logAction;

        public ConnectedProjectionScenarioWrapper(ConnectedProjectionScenario<TContext> inner, Func<TContext> contextFactory, Action<string> logAction)
        {
            _inner = inner;
            _contextFactory = contextFactory;
            _logAction = logAction;
        }

        public ConnectedProjectionScenarioWrapper(ConnectedProjectionScenario<TContext> inner, Func<TContext> contextFactory, Random random, Action<string> logAction)
        {
            _inner = inner;
            _contextFactory = contextFactory;
            _random = random;
            _logAction = logAction;
        }

        public async Task Then(Func<TContext, Task> assertions)
        {
            var test = CreateTest(assertions);

            using (var context = _contextFactory())
            {
                context.Database.EnsureCreated();

                foreach (var message in test.Messages)
                {
                    await new ConnectedProjector<TContext>(test.Resolver)
                        .ProjectAsync(context, message);

                    await context.SaveChangesAsync();
                }

                var result = await test.Verification(context, CancellationToken.None);
                if (result.Failed) throw new AssertionFailedException(result.Message);
            }
        }

        //DO NOT MOVE TO LIB
        public ConnectedProjectionScenarioWrapper<TContext> Project<T>(IGenerator<T> eventGenerator)
        {
            var @event = eventGenerator.Generate(_random);
            _logAction($"Projecting event\r\n{@event.ToLoggableString(Formatting.Indented)}");
            _inner = _inner.Given(new Envelope<T>(new Envelope(@event, new ConcurrentDictionary<string, object>())));

            return this;
        }

        private ConnectedProjectionTestSpecification<TContext> CreateTest(Func<TContext, Task> assertions)
        {
            return _inner.Verify(async context =>
            {
                try
                {
                    await assertions(context);
                    return VerificationResult.Pass();
                }
                catch (Exception e)
                {
                    return VerificationResult.Fail(e.Message);
                }
            });
        }
    }

    public abstract class ProjectionTest<TContext, TProjection> : TestBase //DO NOT MOVE INHERITANCE TO LIB
        where TProjection : ConnectedProjection<TContext>, new() where TContext : DbContext
    {
        public ConnectedProjectionScenario<TContext> When
        {
            get
            {
                var projection = new TProjection();
                var resolver = ConcurrentResolve.WhenEqualToHandlerMessageType(projection.Handlers);
                return new ConnectedProjectionScenario<TContext>(resolver);
            }
        }

        protected ProjectionTest(ITestOutputHelper output) : base(output)
        {
        }

        //DO NOT MOVE TO LIB
        public ConnectedProjectionScenarioWrapper<TContext> GivenEvents(IGenerator<IEnumerable<object>> eventsGenerator)
        {
            var events = Arrange(eventsGenerator).ToList();
            LogArrange($"GivenEvents events:\r\n{events.ToLoggableString(Formatting.Indented)}");
            var envelopes = events
            .Select(e => new Envelope(e, new ConcurrentDictionary<string, object>()).ToGenericEnvelope());
            return new ConnectedProjectionScenarioWrapper<TContext>(
                When.Given(envelopes),
                CreateContext,
                Random, LogAct);
        }

        //DO NOT MOVE TO LIB
        public ConnectedProjectionScenarioWrapper<TContext> GivenEvents(params IGenerator<object>[] eventGenerators)
        {
            var envelopes = eventGenerators
                .Select(Arrange)
                .Select(e => new Envelope(e, new ConcurrentDictionary<string, object>()).ToGenericEnvelope());
            return new ConnectedProjectionScenarioWrapper<TContext>(
                When.Given(envelopes),
                CreateContext,
                Random, LogAct);
        }

        //MOVE TO LIB
        public ConnectedProjectionScenarioWrapper<TContext> Given(params object[] events)
        {
            return new ConnectedProjectionScenarioWrapper<TContext>(
                When.Given(events.Select(e => new Envelope(e, new ConcurrentDictionary<string, object>()).ToGenericEnvelope())),
                CreateContext,
                Random, LogAct);
        }

        //Move to lib
        protected TContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return CreateContext(options);
        }

        protected abstract TContext CreateContext(DbContextOptions<TContext> options);
    }

    public abstract class StreetNameRegistryProjectionTest<TProjection> : ProjectionTest<LegacyContext, TProjection>
        where TProjection : ConnectedProjection<LegacyContext>, new()
    {
        protected StreetNameRegistryProjectionTest(ITestOutputHelper output) : base(output)
        {
        }

        protected override LegacyContext CreateContext(DbContextOptions<LegacyContext> options)
        {
            return new LegacyContext(options);
        }
    }
}
