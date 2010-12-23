namespace Nancy.ViewEngines.Razor
{
    public class RazorViewEngine : ViewEngine
    {
        public RazorViewEngine() : base(new AspNetTemplateLocator(), new RazorViewCompiler())
        {
        }
    }
}