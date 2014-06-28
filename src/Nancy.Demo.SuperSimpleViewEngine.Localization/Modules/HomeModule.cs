namespace Nancy.Demo.SuperSimpleViewEngine.Localization
{
    using System.Globalization;

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {

            Get["/"] = parameters =>
            {
                var m = new
                {
                    Text = this.Text
                };

                return View["Index", m];
            };

            Get["/cultureview"] = parameters =>
            {
                var m = new
                {
                    CurrentLocale = Context.Culture.Name
                };
                return View["CultureView", m];
            };

            Get["/cultureviewgerman"] = parameters =>
                                        {
                                            this.Context.Culture = new CultureInfo("de-DE");
                                            return View["CultureView"];
                                        };
        }
    }
}