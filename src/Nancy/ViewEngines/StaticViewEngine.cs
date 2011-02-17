namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class StaticViewEngine : IViewEngine
    {
        private readonly IViewLocator viewTemplateLocator;

        public StaticViewEngine(IViewLocator viewTemplateLocator)
        {
            this.viewTemplateLocator = viewTemplateLocator;
        }

        public ViewResult RenderView<TModel>(string viewTemplate, TModel model)
        {
            var result = viewTemplateLocator.GetViewLocation(viewTemplate, Enumerable.Empty<string>());

            var view = new StaticView(result.Contents);
            return new ViewResult(view, result.Location);
        }

        public IEnumerable<string> Extensions
        {
            get { return new[] { "html", "htm" }; }
        }

        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model)
        {
            return stream =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(viewLocationResult.Contents.ReadToEnd());
                    writer.Flush();
                }
            };
        }
    }
}