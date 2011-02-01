namespace Nancy.ViewEngines.Razor
{
    public class RazorViewRegistry : IViewEngineRegistry
    {
        private readonly RazorViewEngine viewEngine;

        public RazorViewRegistry(IViewLocator viewLocator)
        {
            viewEngine = new RazorViewEngine(viewLocator);
        }

        public IViewEngine ViewEngine
        {
            get { return viewEngine; }
        }

        public string Extension
        {
            get { return ".cshtml"; }
        }
    }
}
