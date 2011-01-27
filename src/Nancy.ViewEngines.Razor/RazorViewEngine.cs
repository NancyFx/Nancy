namespace Nancy.ViewEngines.Razor
{
    public class RazorViewEngine : ViewEngine
    {
        public RazorViewEngine() : base(new AspNetTemplateLocator(), new RazorViewCompiler())
        {}

        public RazorViewEngine(IViewLocator locator) : base(locator, new RazorViewCompiler())
        {}
    }
}