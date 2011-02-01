namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.IO;

    public class RazorViewRegistry : IViewEngineRegistry
    {
        public Action<Stream> Execute<TModel>(string viewTemplate, TModel model)
        {
            var viewEngine = new RazorViewEngine();
            return stream =>
                       {
                           var result = viewEngine.RenderView(viewTemplate, model);
                           result.Execute(stream);
                       };
        }

        public string Extension
        {
            get { return ".cshtml"; }
        }
    }
}