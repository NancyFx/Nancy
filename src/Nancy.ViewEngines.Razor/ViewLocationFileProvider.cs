using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Nancy.ViewEngines.Razor
{
    public class ViewLocationFileProvider : IFileProvider
    {
        private readonly IViewLocationProvider locationProvider;
        private readonly IRenderContext renderContext;
        private readonly dynamic model;

        public ViewLocationFileProvider(IViewLocationProvider locationProvider, IRenderContext renderContext, dynamic model)
        {
            this.locationProvider = locationProvider;
            this.renderContext = renderContext;
            this.model = model;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return new ViewLocationFileInfo(this.renderContext.LocateView(subpath.TrimStart(new char[] { '/', '\\' }), this.model));
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new LocationProviderDirectoryContents(this.locationProvider, subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return new EmptyChangeToken();
        }

    }
}