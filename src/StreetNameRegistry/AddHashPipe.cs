namespace StreetNameRegistry
{
    using System;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;

    public static class AddHashPipe
    {
        public const string HashMetadataKey = "EventHash";

        public static ICommandHandlerBuilder<CommandMessage<TCommand>> AddHash<TCommand, TAggregate>(
            this ICommandHandlerBuilder<CommandMessage<TCommand>> commandHandlerBuilder,
            Func<ConcurrentUnitOfWork> getUnitOfWork)
            where TAggregate : IAggregateRootEntity
        {
            return commandHandlerBuilder.Pipe(next => async (commandMessage, ct) =>
            {
                var result = await next(commandMessage, ct);

                AddHash<TAggregate>(getUnitOfWork);

                return result;
            });
        }

        public static void AddHash<TAggregate>(
            Func<ConcurrentUnitOfWork> getUnitOfWork)
            where TAggregate : IAggregateRootEntity
        {
            var aggregates = getUnitOfWork()
                .GetChanges()
                .Select(aggregate => aggregate.Root)
                .OfType<TAggregate>();

            foreach (var aggregate in aggregates)
            {
                var events = aggregate
                    .GetChangesWithMetadata();

                foreach (var eventWithMetadata in events)
                {
                    if (eventWithMetadata.Event is IHaveHash @event)
                    {
                        eventWithMetadata.Metadata[AddHashPipe.HashMetadataKey] = @event.GetHash();
                    }
                }
            }
        }
    }
}
