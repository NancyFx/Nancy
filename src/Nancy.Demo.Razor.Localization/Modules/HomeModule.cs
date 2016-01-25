namespace Nancy.Demo.Razor.Localization.Modules
{
    using System.Globalization;

    public class HomeModule : LegacyNancyModule
    {
        public HomeModule()
        {

            Get["/"] = parameters => View["Index"];

            Get["/cultureview"] = parameters => View["CultureView"];

            Get["/cultureviewgerman"] = parameters =>
                                        {
                                            this.Context.Culture = new CultureInfo("de-DE");
                                            return View["CultureView"];
                                        };
        }
    }
}