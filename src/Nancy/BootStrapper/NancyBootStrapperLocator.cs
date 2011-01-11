using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.BootStrapper
{
    public class NancyBootStrapperLocator
    {
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
                                       from type in assembly.GetExportedTypes()
                                       where !type.IsAbstract
                                       where bootStrapperInterface.IsAssignableFrom(type)
                                       where type != defaultBootStrapper
                                       select type;

            var bootStrapperType = locatedBootStrappers.FirstOrDefault() ?? defaultBootStrapper;

            BootStrapper = (INancyBootStrapper) Activator.CreateInstance(bootStrapperType);
        }
    }
}
