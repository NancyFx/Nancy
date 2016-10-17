namespace Nancy.ViewEngines.Razor
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.FileProviders;

    public class LocationProviderDirectoryContents : IDirectoryContents
    {
        private readonly IViewLocationProvider locationProvider;
        private readonly string subpath;

        public LocationProviderDirectoryContents(IViewLocationProvider locationProvider, string subpath)
        {
            this.locationProvider = locationProvider;
            this.subpath = subpath;
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return this.locationProvider
                .GetLocatedViews(RazorViewEngine.SupportedExtensions)
                .Where(viewLocation => viewLocation.Location == this.subpath)
                .Select(viewLocation => new ViewLocationFileInfo(viewLocation))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Exists
        {
            get { return this.Any(); }
        }
    }
}