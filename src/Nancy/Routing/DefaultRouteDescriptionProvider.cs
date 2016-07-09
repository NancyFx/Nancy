namespace Nancy.Routing
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    /// <summary>
    /// Default implementation of the <see cref="IRouteDescriptionProvider"/> interface. Will look for
    /// route descriptions in resource files. The resource files should have the same name as the module
    /// for which it defines routes.
    /// </summary>
    public class DefaultRouteDescriptionProvider : IRouteDescriptionProvider
    {
        /// <summary>
        /// Get the description for a route.
        /// </summary>
        /// <param name="module">The module that the route is defined in.</param>
        /// <param name="path">The path of the route that the description should be retrieved for.</param>
        /// <returns>A <see cref="string"/> containing the description of the route if it could be found, otherwise <see cref="string.Empty"/>.</returns>
        public string GetDescription(INancyModule module, string path)
        {
            var assembly =
                module.GetType().GetTypeInfo().Assembly;

            if (assembly.IsDynamic)
            {
                return string.Empty;
            }

            var moduleName =
                string.Concat(module.GetType().FullName, ".resources");

            var resourceName = assembly
                .GetManifestResourceNames()
                .FirstOrDefault(x => x.Equals(moduleName, StringComparison.OrdinalIgnoreCase));

            if (resourceName != null)
            {
                var manager =
                    new ResourceManager(resourceName.Replace(".resources", string.Empty), assembly);

                return manager.GetString(path);
            }

            return string.Empty;
        }
    }
}