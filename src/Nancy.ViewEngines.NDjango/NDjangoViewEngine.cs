namespace Nancy.ViewEngines.NDjango
{
    public class NDjangoViewEngine : ViewEngine
    {
        public NDjangoViewEngine(IViewLocator viewLocator) : base(viewLocator, new NDjangoViewCompiler())
        {
        }
    }
}