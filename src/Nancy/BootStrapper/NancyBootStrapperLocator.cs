namespace Nancy.BootStrapper
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
    public class NancyBootStrapperLocator
    {
        // TODO - not very testable as it is, may be worth pushing the logic into a non-static and making this class have a static singleton of it.

        /// <summary>
        /// Gets the located bootstrapper
        /// </summary>
        public static INancyBootStrapper BootStrapper;

        static NancyBootStrapperLocator()
        {
            // Get the first non-abstract implementation of INancyBootStrapper if one exists in the
            // app domain. If none exist then just use the default one.
            var bootStrapperInterface = typeof(INancyBootStrapper);
            var defaultBootStrapper = typeof(DefaultNancyBootStrapper);

            var locatedBootStrappers = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                       where !assembly.IsDynamic
                                       from type in assembly.SafeGetExportedTypes()
                                       where !type.IsAbstract
                                       where bootStrapperInterface.IsAssignableFrom(type)
                                       where type != defaultBootStrapper
                                       select type;

            var bootStrapperType = locatedBootStrappers.FirstOrDefault() ?? defaultBootStrapper;

            BootStrapper = (INancyBootStrapper) Activator.CreateInstance(bootStrapperType);
        }
    }
}