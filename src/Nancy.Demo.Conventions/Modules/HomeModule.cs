using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace Nancy.Demo.Conventions.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ =>
              {
                  return View["index"];
              };
            Get["/default"] = _ =>
            {
                return View["default"];
            };
            Get["/custom"] = _ =>
            {
                return View["custom"];
            };
            Get["/other"] = _ =>
            {
                return View["other"];
            };
        }
    }
}