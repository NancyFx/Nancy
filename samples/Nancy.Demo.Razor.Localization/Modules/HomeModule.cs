namespace Nancy.Demo.Razor.Localization.Modules
{
    using System.Globalization;

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", args => View["Index"]);

            Get("/cultureview", args => View["CultureView"]);

            Get("/cultureviewgerman", args =>
            {
                this.Context.Culture = new CultureInfo("de-DE");
                return View["CultureView"];
            });
        }
    }
}