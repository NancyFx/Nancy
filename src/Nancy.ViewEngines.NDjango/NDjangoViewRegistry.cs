namespace Nancy.ViewEngines.NDjango
{
    public class NDjangoViewRegistry : IViewEngineRegistry
    {
        private readonly NDjangoViewEngine viewEngine;

        public NDjangoViewRegistry(IViewLocator viewLocator)
        {
            viewEngine = new NDjangoViewEngine(viewLocator);
        }

        public IViewEngine ViewEngine
        {
            get { return viewEngine; }
        }

        public string Extension
        {
            get { return ".django"; }
        }
    }
}