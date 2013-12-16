namespace Nancy.ViewEngines.Razor
{
    using System.Web.Razor;

    public interface IRazorEngineHostFactory
    {
        NancyRazorEngineHost Create(RazorCodeLanguage language, string viewLocation);
    }
}