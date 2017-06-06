namespace Nancy.Prototype
{
    using Nancy.Prototype.Registration;

    internal static class PlatformServicesExtensions
    {
        public static IContainerRegistry GetRegistry(this IPlatform platform)
        {
            Check.NotNull(platform, nameof(platform));

            var registrations = new[]
            {
                platform.BootstrapperLocator.AsInstanceRegistration(),
                platform.AssemblyCatalog.AsInstanceRegistration(),
                platform.TypeCatalog.AsInstanceRegistration(),
                platform.AsInstanceRegistration()
            };

            return new ContainerRegistry(instanceRegistrations: registrations);
        }

        private static InstanceRegistration AsInstanceRegistration<TService>(this TService instance)
        {
            Check.NotNull(instance, nameof(instance));

            return new InstanceRegistration(typeof(TService), instance);
        }
    }
}
