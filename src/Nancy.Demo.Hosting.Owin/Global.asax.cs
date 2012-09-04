namespace Nancy.Demo.Hosting.Owin
{
    using System;
    using System.Web.Routing;

    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            var application = new Nancy.Hosting.Owin.NancyOwinHost();

            RouteTable.Routes.Add(new Route("{*pathInfo}", new OwinAspNetRouteHandler(application.ProcessRequest)));
        }
    }
}