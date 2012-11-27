using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nancy.Demo.Sessions
{
    public class SessionsModule : NancyModule
    {
        public SessionsModule()
        {
            Get["/"] = p =>
            {
                string data = Session.Foo.HasValue ? Session.Foo : "{no value}";
                return View["Index", data];
            };
            Post["/"] = p =>
            {
                Session.Foo = Request.Form.Foo;
                return Response.AsRedirect("/");
            };
        }
    }
}