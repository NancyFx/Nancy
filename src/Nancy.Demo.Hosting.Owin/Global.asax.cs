namespace Nancy.Demo.Hosting.Owin
{
    using System;
    using System.Web.Routing;

    using Nancy.Bootstrapper;
    using Nancy.Owin;

    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            var application = new NancyOwinHost(null, NancyBootstrapperLocator.Bootstrapper, new HostConfiguration());

            RouteTable.Routes.Add(new Route("{*pathInfo}", new SimpleOwinAspNetRouteHandler(application.Invoke)));
        }
    }
}