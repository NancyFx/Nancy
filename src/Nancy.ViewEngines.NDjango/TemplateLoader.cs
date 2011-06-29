namespace Nancy.ViewEngines.NDjango
{
    using System;
    using System.IO;
    using global::NDjango.Interfaces;

    public class TemplateLoader : ITemplateLoader
    {
        private readonly IRenderContext renderContext;

        public TemplateLoader(IRenderContext renderContext)
        {
            this.renderContext = renderContext;
        }

        public TextReader GetTemplate(string path)
        {
            var view = renderContext.LocateView(path, null);
            var template = renderContext.ViewCache.GetOrAdd(view, x => view.Contents().ReadToEnd());

            return new StringReader(template);
        }

        public bool IsUpdated(string path, DateTime timestamp)
        {
            return true;
        }
    }
}