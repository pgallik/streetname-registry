namespace StreetNameRegistry.Consumer.Infrastructure.Projections
{
    using System;
    using System.Threading.Tasks;
    using Autofac;

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
}
