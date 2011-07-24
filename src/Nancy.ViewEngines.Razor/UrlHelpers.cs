namespace Nancy.ViewEngines.Razor
{
    public class UrlHelpers
    {
        private readonly RazorViewEngine razorViewEngine;
        private readonly IRenderContext renderContext;

        public UrlHelpers(RazorViewEngine razorViewEngine, IRenderContext renderContext)
        {
            this.razorViewEngine = razorViewEngine;
            this.renderContext = renderContext;
        }

        public string Content(string path)
        {
            return renderContext.ParsePath(path);
        }
    }
}