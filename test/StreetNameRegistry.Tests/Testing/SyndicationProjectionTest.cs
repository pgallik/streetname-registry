namespace StreetNameRegistry.Tests.Testing
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication;
    using FluentAssertions.Execution;
    using Microsoft.EntityFrameworkCore;
    using Xunit.Abstractions;

    public abstract class SyndicationProjectionTest<TContext, TEvent, TContent, TModule> : TestBase
        where TModule : AtomEntryProjectionHandlerModule<TEvent, TContent, TContext>, new()
        where TContext : DbContext
        where TEvent : struct
    {
        protected SyndicationProjectionTest(ITestOutputHelper output) : base(output)
        {
        }

        public AtomEntryProjectionHandlerModuleScenario<TEvent, TContent, TContext> When
        {
            get
            {
                var module = new TModule();
                var resolver = Resolve.WhenEqualToEvent(module.ProjectionHandlers);
                return new AtomEntryProjectionHandlerModuleScenario<TEvent, TContent, TContext>(resolver, CreateContext);
            }
        }

        public AtomEntryProjectionHandlerModuleScenario<TEvent, TContent, TContext> Given(params AtomEntry[] entries)
        {
            return When.Given(entries);
        }

        public TContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return CreateContext(options);
        }

        protected abstract TContext CreateContext(DbContextOptions<TContext> options);
    }

    public sealed class AtomEntryProjectionHandlerModuleScenario<TEvent, TContent, TContext>
        where TEvent : struct
        where TContext : DbContext
    {
        private readonly AtomEntryProjectionHandlerResolver<TEvent, TContext> _resolver;
        private readonly AtomEntry[] _entries;
        private readonly Func<TContext> _contextFactory;

        public AtomEntryProjectionHandlerModuleScenario(
            AtomEntryProjectionHandlerResolver<TEvent, TContext> resolver,
            Func<TContext> contextFactory)
        {
            _resolver = resolver;
            _contextFactory = contextFactory;
            _entries = new AtomEntry[0];
        }

        private AtomEntryProjectionHandlerModuleScenario(
            AtomEntryProjectionHandlerResolver<TEvent, TContext> resolver,
            AtomEntry[] entries,
            Func<TContext> contextFactory)
        {
            _resolver = resolver;
            _entries = entries;
            _contextFactory = contextFactory;
        }

        public AtomEntryProjectionHandlerModuleScenario<TEvent, TContent, TContext> Given(params AtomEntry[] entries)
        {
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            return new AtomEntryProjectionHandlerModuleScenario<TEvent, TContent, TContext>(
                _resolver,
                _entries.Concat(entries).ToArray(),
                _contextFactory);
        }

        public AtomEntryProjectionHandlerModuleScenario<TEvent, TContent, TContext> Project(params AtomEntry[] entries)
        {
            return Given(entries);
        }

        public async Task Then(Func<TContext, Task> assertions)
        {
            using (var context = _contextFactory())
            {
                await context.Database.EnsureCreatedAsync();

                foreach (var entry in _entries)
                {
                    try
                    {
                        foreach (var atomEntryProjectionHandler in _resolver(entry))
                        {
                            await atomEntryProjectionHandler
                                .Handler
                                .Invoke(entry, context, CancellationToken.None);
                        }
                    }
                    catch (InvalidOperationException)
                    { }
                }

                await context.SaveChangesAsync();

                try
                {
                    await assertions(context);
                }
                catch (Exception e)
                {
                    throw new AssertionFailedException(e.Message);
                }
            }
        }
    }
}
