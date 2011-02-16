namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public interface IViewLocator
    {
        ViewLocationResult GetViewLocation(string viewName, IEnumerable<string> supportedViewEngineExtensions);
    }

    public class ViewLocator : IViewLocator
    {
        private readonly IEnumerable<IViewSourceProvider> viewSourceProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLocator"/> class.
        /// </summary>
        /// <param name="viewSourceProviders">The view source providers.</param>
        public ViewLocator(IEnumerable<IViewSourceProvider> viewSourceProviders)
        {
            if (viewSourceProviders == null)
            {
                throw new ArgumentNullException("viewSourceProviders", "The value of the viewSourceProviders parameter cannot be null.");
            }

            this.viewSourceProviders = viewSourceProviders;
        }

        public ViewLocationResult GetViewLocation(string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            if(this.NotEnoughInformationAvailableToLocateView(viewName, supportedViewEngineExtensions))
            {
                return null;
            }

            return this.LocateView(viewName, supportedViewEngineExtensions);
        }

        private bool NotEnoughInformationAvailableToLocateView(string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            if (viewName == null)
            {
                return true;
            }

            if (viewName.Length == 0)
            {
                return true;
            }

            if (supportedViewEngineExtensions == null)
            {
                return true;
            }

            if (!supportedViewEngineExtensions.Any())
            {
                return true;
            }

            return !this.viewSourceProviders.Any();
        }

        private ViewLocationResult LocateView(string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            foreach (var viewSourceProvider in viewSourceProviders)
            {
                var result =
                    LocateViewAndSupressExceptions(viewSourceProvider, viewName, supportedViewEngineExtensions);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static ViewLocationResult LocateViewAndSupressExceptions(IViewSourceProvider viewSourceProvider, string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            try
            {
                return viewSourceProvider.LocateView(viewName, supportedViewEngineExtensions);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public interface IViewSourceProvider
    {
        ViewLocationResult LocateView(string viewName, IEnumerable<string> supportedViewEngineExtensions);
    }

    public class ResourceViewSourceProvider : IViewSourceProvider
    {
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
                from resourceName in assembly.GetManifestResourceNames()
                from viewEngineExtension in supportedViewEngineExtensions
                let inspectedResourceName = string.Concat(viewName, ".", viewEngineExtension)
                where resourceName.EndsWith(inspectedResourceName, StringComparison.OrdinalIgnoreCase)
                select new Tuple<string, Stream>(
                    resourceName,
                    assembly.GetManifestResourceStream(resourceName)
                );

            return resourceStreams.FirstOrDefault();
        }
    }
}