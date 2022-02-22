namespace StreetNameRegistry
{
    using Autofac;
    using Be.Vlaanderen.Basisregisters.CommandHandling;
    using Municipality;
    using StreetName;

    public static class CommandHandlerModules
    {
        public static void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<CrabStreetNameProvenanceFactory>()
                .SingleInstance();

            containerBuilder
                .RegisterType<StreetNameLegacyProvenanceFactory>()
                .SingleInstance();

            containerBuilder
                .RegisterType<CrabStreetNameCommandHandlerModule>()
                .Named<CommandHandlerModule>(typeof(CrabStreetNameCommandHandlerModule).FullName)
                .As<CommandHandlerModule>();

            containerBuilder
                .RegisterType<StreetNameProvenanceFactory>()
                .SingleInstance();

            containerBuilder
                .RegisterType<StreetNameCommandHandlerModule>()
                .Named<CommandHandlerModule>(typeof(StreetNameCommandHandlerModule).FullName)
                .As<CommandHandlerModule>();

            containerBuilder
                .RegisterType<MunicipalityCommandHandlerModule>()
                .Named<CommandHandlerModule>(typeof(MunicipalityCommandHandlerModule).FullName)
                .As<CommandHandlerModule>();
        }
    }
}
