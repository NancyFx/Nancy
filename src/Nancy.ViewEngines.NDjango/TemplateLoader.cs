namespace Nancy.ViewEngines.NDjango
{
    using System;
    using System.IO;
    using global::NDjango.Interfaces;

    public class TemplateLoader : ITemplateLoader
    {
        private readonly IRenderContext renderContext;

        private ViewLocationResult mainTemplateLocation;

        public TemplateLoader(IRenderContext renderContext, ViewLocationResult viewLocationResult)
        {
            this.renderContext = renderContext;
            this.mainTemplateLocation = viewLocationResult;
        }

        public TextReader GetTemplate(string path)
        {
            var view = String.Equals(path, this.mainTemplateLocation.Location)
                           ? this.mainTemplateLocation
                           : this.renderContext.LocateView(path, null);

            var template = renderContext.ViewCache.GetOrAdd(view, x => view.Contents().ReadToEnd());

            return new StringReader(template);
        }

        public bool IsUpdated(string path, DateTime timestamp)
        {
            return true;
        }
    }
}