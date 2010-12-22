namespace Nancy.ViewEngines.NDjango
{
    public class NDjangoViewEngine : ViewEngine
    {
        public NDjangoViewEngine() : base(new AspNetTemplateLocator(), new NDjangoViewCompiler())
        {
        }
    }
}