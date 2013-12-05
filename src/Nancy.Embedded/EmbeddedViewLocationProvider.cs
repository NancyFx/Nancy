namespace Nancy.Embedded
{
    using Nancy.ViewEngines;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Contains the functionality for locating a view that has been embedded into an assembly resource.
    /// </summary>
    public class EmbeddedViewLocationProvider : IViewLocationProvider
    {
        private readonly IResourceReader resourceReader;
        private readonly IResourceAssemblyProvider resourceAssemblyProvider;

        /// <summary>
        /// User-configured root namespaces for assemblies.
        /// </summary>
        public readonly static IDictionary<Assembly, string> RootNamespaces = new Dictionary<Assembly, string>();

        /// <summary>
        /// A list of assemblies to ignore when scanning for embedded views.
        /// </summary>
        public readonly static IList<Assembly> Ignore = new List<Assembly>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedViewLocationProvider"/> class.
        /// </summary>
        /// <param name="resourceReader">An <see cref="IResourceReader"/> instance that should be used when extracting embedded views.</param>
        /// <param name="resourceAssemblyProvider">An <see cref="IResourceAssemblyProvider"/> instance that should be used to determine which assemblies to scan for embedded views.</param>
        public EmbeddedViewLocationProvider(IResourceReader resourceReader, IResourceAssemblyProvider resourceAssemblyProvider)
        {
            this.resourceReader = resourceReader;
            this.resourceAssemblyProvider = resourceAssemblyProvider;
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
                return Enumerable.Empty<ViewLocationResult>();
            }

            var assembliesToScan = this.resourceAssemblyProvider
                .GetAssembliesToScan()
                .Union(RootNamespaces.Keys)
                .Where(assembly => !Ignore.Contains(assembly));

            return assembliesToScan.SelectMany(assembly => GetViewLocations(assembly, supportedViewExtensions));
        }

        /// <summary>
        /// Returns an <see cref="ViewLocationResult"/> instance for all the views matching the viewName that could be located by the provider.
        /// </summary>
        /// <param name="supportedViewExtensions">An <see cref="IEnumerable{T}"/> instance, containing the view engine file extensions that is supported by the running instance of Nancy.</param>
        /// <param name="location">The location of the view to try and find</param>
        /// <param name="viewName">The name of the view to try and find</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ViewLocationResult"/> instances for the located views.</returns>
        /// <remarks>If no views could be located, this method should return an empty enumerable, never <see langword="null"/>.</remarks>
        public IEnumerable<ViewLocationResult> GetLocatedViews(IEnumerable<string> supportedViewExtensions, string location, string viewName)
        {
            return GetLocatedViews(supportedViewExtensions).Where(result => IsMatch(location, viewName, result));
        }

        private static bool IsMatch(string location, string viewName, ViewLocationResult result)
        {
            return result.Location.Equals(location, StringComparison.OrdinalIgnoreCase)
                && result.Name.Equals(viewName, StringComparison.OrdinalIgnoreCase);
        }

        private IEnumerable<ViewLocationResult> GetViewLocations(Assembly assembly, IEnumerable<string> supportedViewExtensions)
        {
            var resourceStreams = this.resourceReader.GetResourceStreamMatches(assembly, supportedViewExtensions);
            if (!resourceStreams.Any())
            {
                yield break;
            }

            string rootNamespace;
            if (!RootNamespaces.TryGetValue(assembly, out rootNamespace))
            {
                RootNamespaces.Add(assembly, (rootNamespace = GetRootNamespace(resourceStreams)));
            }

            if (string.IsNullOrWhiteSpace(rootNamespace))
            {
                yield break;
            }

            var resources = resourceStreams
                .Select(resource => new ResourceViewInfo(resource))
                .Where(resource => !string.IsNullOrWhiteSpace(resource.Name));

            foreach (var resource in resources)
            {
                var location = GetLocation(resource, rootNamespace);
                var fileName = Path.GetFileNameWithoutExtension(resource.Name);
                var extension = GetExtension(resource.FullName);

                yield return new ViewLocationResult(location, fileName, extension, resource.Contents);
            }
        }

        private static string GetRootNamespace(IEnumerable<Tuple<string, Func<StreamReader>>> resourceStreams)
        {
            var resourceNames = resourceStreams.Select(x => x.Item1).ToList();

            if (resourceNames.Count == 1)
            {
                var resourceName = resourceNames.First();

                var fileName = GetFileName(resourceName);

                return resourceName.Replace(fileName, string.Empty).TrimEnd('.');
            }

            var pathSegments = resourceNames.Select(name => name.Split('.').AsEnumerable());

            var commonSegments = pathSegments.Aggregate((previous, current) => CreateSegment(current, previous));

            return string.Join(".", commonSegments);
        }

        private static string GetFileName(string resourceName)
        {
            var segments = resourceName.Split('.');
            if (segments.Length < 2)
            {
                return string.Empty;
            }

            return string.Join(".", segments.Reverse().Take(2).Reverse());
        }

        private static IEnumerable<string> CreateSegment(IEnumerable<string> current, IEnumerable<string> previous)
        {
            return current.TakeWhile((step, index) => step == previous.ElementAtOrDefault(index)).ToArray();
        }

        private static string GetExtension(string resourceName)
        {
            var extension = Path.GetExtension(resourceName);
            if (extension == null)
            {
                return string.Empty;
            }

            return extension.Substring(1);
        }

        private static string GetLocation(ResourceViewInfo resource, string rootNamespace)
        {
            return resource.FullName
                .Replace(rootNamespace, string.Empty)
                .Replace(resource.Name, string.Empty)
                .Trim('.')
                .Replace(".", "/");
        }

        private class ResourceViewInfo
        {
            public ResourceViewInfo(Tuple<string, Func<StreamReader>> resource)
            {
                FullName = resource.Item1;
                Name = GetFileName(resource.Item1);
                Contents = resource.Item2;
            }

            public string Name { get; private set; }

            public string FullName { get; private set; }

            public Func<StreamReader> Contents { get; private set; }
        }
    }
}