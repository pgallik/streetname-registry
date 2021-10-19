namespace StreetNameRegistry.Infrastructure.Modules
{
    using Autofac;
    using StreetName;
    using Repositories;

    public class RepositoriesModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            // We could just scan the assembly for classes using Repository<> and registering them against the only interface they implement
            containerBuilder
                .RegisterType<StreetNames>()
                .As<IStreetNames>();

            containerBuilder
                .RegisterType<Municipalities>()
                .As<IMunicipalities>();
        }
    }
}
