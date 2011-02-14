namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    // Notice that the template might not have a "Path".
    // For example, it could be embedded. So that's why this 
    // returns a reader.
    public interface IViewLocator
    {
        ViewLocationResult GetViewLocation(string viewName);
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

        public ViewLocationResult GetViewLocation(string viewName)
        {
            if(this.NotEnoughInformationAvailableToLocateView(viewName))
            {
                return null;
            }

            return this.LocateView(viewName);
        }

        private bool NotEnoughInformationAvailableToLocateView(string viewName)
        {
            if (viewName == null)
                return true;

            if (viewName.Length == 0)
                return true;

            return !this.viewSourceProviders.Any();
        }

        private ViewLocationResult LocateView(string viewName)
        {
            foreach (var viewSourceProvider in viewSourceProviders)
            {
                var result =
                    LocateViewAndSupressExceptions(viewSourceProvider, viewName);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static ViewLocationResult LocateViewAndSupressExceptions(IViewSourceProvider viewSourceProvider, string viewName)
        {
            try
            {
                return viewSourceProvider.LocateView(viewName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public interface IViewSourceProvider
    {
        ViewLocationResult LocateView(string viewName);
    }

    public class ResourceViewSourceProvider : IViewSourceProvider
    {
        public ViewLocationResult LocateView(string viewName)
        {
            var resourceStream =
                GetResourceStream(viewName);

            if (resourceStream == null)
            {
                return null;
            }

            return new ViewLocationResult(
                resourceStream.Item1,
                new StreamReader(resourceStream.Item2)
            );
        }

        private static Tuple<string, Stream> GetResourceStream(string viewName)
        {
            var resourceStreams =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from resourceName in assembly.GetManifestResourceNames()
                where resourceName.EndsWith(viewName, StringComparison.OrdinalIgnoreCase)
                select new Tuple<string, Stream>(
                    resourceName,
                    assembly.GetManifestResourceStream(resourceName)
                );

            return resourceStreams.FirstOrDefault();
        }
    }
}