namespace Nancy.ViewEngines.Razor
{
    public class RazorViewEngine : ViewEngine
    {
        public RazorViewEngine(IViewLocator viewLocator) : base(viewLocator, new RazorViewCompiler())
        {
        }
    }
}