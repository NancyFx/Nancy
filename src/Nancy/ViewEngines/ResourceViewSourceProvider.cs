namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Contains the functionality for locating a view that has been embedded into an assembly resource.
    /// </summary>
    public class ResourceViewSourceProvider : IViewSourceProvider
    {
        /// <summary>
        /// Attemptes to locate the view, specified by the <paramref name="viewName"/> parameter, in the underlaying source.
        /// </summary>
        /// <param name="viewName">The name of the view that should be located.</param>
        /// <param name="supportedViewEngineExtensions">The supported view engine extensions that the view is allowed to use.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise <see langword="null"/>.</returns>
        public ViewLocationResult LocateView(string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            var resourceStreamMatch =
                GetResourceStreamMatch(viewName, supportedViewEngineExtensions);

            if (resourceStreamMatch == null)
            {
                return null;
            }

            return new ViewLocationResult(
                resourceStreamMatch.Item1,
                GetResourceNameExtension(resourceStreamMatch.Item1),
                new StreamReader(resourceStreamMatch.Item2)
            );
        }

        private static string GetResourceFileName(string resourceName)
        {
            var nameSegments =
                resourceName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            var segmentCount =
                nameSegments.Count();

            return (segmentCount < 2) ?
                string.Empty :
                string.Format("{0}.{1}", nameSegments.ElementAt(segmentCount - 2), nameSegments.Last());
        }

        private static string GetResourceNameExtension(string resourceName)
        {
            var extension =
                Path.GetExtension(resourceName);

            return string.IsNullOrEmpty(extension) ? string.Empty : extension.TrimStart('.');
        }

        private static Tuple<string, Stream> GetResourceStreamMatch(string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            var resourceStreams =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                where !assembly.IsDynamic
                from resourceName in assembly.GetManifestResourceNames()
                from viewEngineExtension in supportedViewEngineExtensions
                let inspectedResourceName = string.Concat(viewName, ".", viewEngineExtension)
                where GetResourceFileName(resourceName).Equals(inspectedResourceName, StringComparison.OrdinalIgnoreCase)
                select new Tuple<string, Stream>(
                    resourceName,
                    assembly.GetManifestResourceStream(resourceName)
                 );

            return resourceStreams.FirstOrDefault();
        }
    }
}