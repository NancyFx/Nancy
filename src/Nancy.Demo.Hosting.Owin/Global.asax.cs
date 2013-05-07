namespace Nancy.Demo.Hosting.Owin
{
    using System;
    using System.Web;
    using System.Web.Routing;

    using Nancy.Hosting.Owin;

    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var application = new NancyOwinHost();

            RouteTable.Routes.Add(new Route("{*pathInfo}", new SimpleOwinAspNetRouteHandler(application.ProcessRequest)));
        }
    }
}