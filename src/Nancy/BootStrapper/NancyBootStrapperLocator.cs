namespace Nancy.Bootstrapper
{
    using System;
    using System.Linq;
    using Nancy.Extensions;

    /// <summary>
    /// Class for locating an INancyBootstrapper implementation.
    /// 
    /// Will search the app domain for a non-abstract one, and if it can't find one
    /// it will use the default nancy one that uses TinyIoC.
    /// </summary>
    public class NancyBootstrapperLocator
    {
        // TODO - not very testable as it is, may be worth pushing the logic into a non-static and making this class have a static singleton of it.

        /// <summary>
        /// Gets the located bootstrapper
        /// </summary>
        public static INancyBootstrapper Bootstrapper;

        static NancyBootstrapperLocator()
        {
            // Get the first non-abstract implementation of INancyBootstrapper if one exists in the
            // app domain. If none exist then just use the default one.
            var bootstrapperInterface = typeof(INancyBootstrapper);
            var defaultBootstrapper = typeof(DefaultNancyBootstrapper);

            var locatedBootstrappers =
                from type in AppDomainAssemblyTypeScanner.Types
                where bootstrapperInterface.IsAssignableFrom(type)
                where type != defaultBootstrapper
                select type;

            var bootstrapperType = locatedBootstrappers.FirstOrDefault() ?? defaultBootstrapper;

            Bootstrapper = (INancyBootstrapper)Activator.CreateInstance(bootstrapperType);
        }
    }
}