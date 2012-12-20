using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace Nancy.Demo.Localisation
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = parameters =>
                           {
                               return View["Index"];
                           };

            Get["/cultureview"] = parameters =>
                                      {
                                          return View["CultureView"];
                                      };

            Get["/cultureviewgerman"] = parameters =>
                                            {
                                                this.Context.Culture = new CultureInfo("de-DE");
                                                return View["CultureView"];
                                            };
        }
    }
}