namespace Nancy.ViewEngines.Razor
{
    using System.Web.Razor;

    public class NancyRazorEngineHostFactory : IRazorEngineHostFactory
    {
        public NancyRazorEngineHost Create(RazorCodeLanguage language, string viewLocation)
        {
            if (viewLocation.StartsWith("/App_Code/"))
            {
                return new NancyRazorCodeEngineHost(language, viewLocation);
            }

            return new NancyRazorEngineHost(language);
        }
    }
}