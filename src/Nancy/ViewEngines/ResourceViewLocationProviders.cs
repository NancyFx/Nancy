namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Nancy.Bootstrapper;

    /// <summary>
    /// Contains the functionality for locating a view that has been embedded into an assembly resource.
    /// </summary>
    public class ResourceViewLocationProviders : IViewLocationProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceViewLocationProviders"/> class.
        /// </summary>
        public ResourceViewLocationProviders()
        {
        }

        /// <summary>
        /// Returns an <see cref="ViewLocationResult"/> instance for all the views that could be located by the provider.
        /// </summary>
        /// <param name="supportedViewExtensions">An <see cref="IEnumerable{T}"/> instance, containing the view engine file extensions that is supported by the running instance of Nancy.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ViewLocationResult"/> instances for the located views.</returns>
        /// <remarks>If no views could be located, this method should return an empty enumerable, never <see langword="null"/>.</remarks>
        public IEnumerable<ViewLocationResult> GetLocatedViews(IEnumerable<string> supportedViewExtensions)
        {
            if (supportedViewExtensions == null)
            {
                return null;
            }

            var resourceStreamMatches =
                GetResourceStreamMatches(supportedViewExtensions);

            return !resourceStreamMatches.Any() ?
                Enumerable.Empty<ViewLocationResult>() :
                GetViewLocationsWithQualifiedLocations(resourceStreamMatches);
        }

        private static IEnumerable<ViewLocationResult> GetViewLocationsWithQualifiedLocations(IEnumerable<Tuple<string, Stream>> resources)
        {
            var commonResourceNamespace =
                ExtractCommonResourceNamespace(resources.Select(x => x.Item1));

            return
                from resource in resources
                let resourceFileName = GetResourceFileName(resource.Item1)
                select new ViewLocationResult(
                    GetEncodedResouceName(commonResourceNamespace, resource.Item1, resourceFileName),
                    resourceFileName,
                    GetResourceNameExtension(resource.Item1),
                    () => new StreamReader(resource.Item2));
        }

        private static string GetEncodedResouceName(string commonResourceNamespace, string location, string viewName)
        {
            if (commonResourceNamespace.Equals(viewName, StringComparison.OrdinalIgnoreCase))
            {
                return viewName;
            }

            var locationWithoutViewName =
                location.Replace(viewName, string.Empty);

            var commonResourceNamespaceWithoutViewName =
                commonResourceNamespace.Replace(viewName, string.Empty);

            var encodedLocation = locationWithoutViewName
                .Replace(commonResourceNamespaceWithoutViewName, string.Empty)
                .TrimEnd(new[] { '.' })
                .Replace(".", @"\");

            if (encodedLocation.Length != 0)
            {
                encodedLocation = string.Concat(encodedLocation, @"\");
            }

            return string.Concat(encodedLocation, viewName);
        }

        private static string ExtractCommonResourceNamespace(IEnumerable<string> resources)
        {
            if (resources.Count() == 1)
            {
                return resources.First();
            }

            var commonPathSegments = resources.Select(s => new { parts = s.Split('.') })
                .Aggregate((previous, current) => new { parts = current.parts.TakeWhile((step, index) => step == previous.parts.ElementAtOrDefault(index)).ToArray() });

            var commonResourceNamespace =
                string.Join(".", commonPathSegments.parts) + ".";

            return commonResourceNamespace;
        }

        private static string GetResourceFileName(string resourceName)
        {
            var nameSegments =
                resourceName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            var segmentCount =
                nameSegments.Count();

            return (segmentCount < 2) ?
                string.Empty :
                nameSegments.ElementAt(segmentCount - 2);
        }

        private static string GetResourceNameExtension(string resourceName)
        {
            var extension =
                Path.GetExtension(resourceName);

            return string.IsNullOrEmpty(extension) ? string.Empty : extension.TrimStart('.');
        }

        private static IEnumerable<Tuple<string, Stream>> GetResourceStreamMatches(IEnumerable<string> supportedViewEngineExtensions)
        {
            var resourceStreams =
                from assembly in AppDomainAssemblyTypeScanner.Assemblies
                from resourceName in assembly.GetManifestResourceNames()
                from viewEngineExtension in supportedViewEngineExtensions
                where resourceName.EndsWith(viewEngineExtension, StringComparison.OrdinalIgnoreCase)
                select new Tuple<string, Stream>(resourceName, assembly.GetManifestResourceStream(resourceName));

            return resourceStreams.ToList();
        }
    }
}