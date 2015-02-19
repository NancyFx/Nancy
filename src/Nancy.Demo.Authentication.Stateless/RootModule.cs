namespace Nancy.Demo.Authentication.Stateless
{
    public class RootModule : NancyModule
    {
        public RootModule()
        {
            Get["/"] = _ => this.Response.AsText("This is a REST API. It is in another VS project to " +
                                                 "demonstrate how a common REST API might behave when " +
                                                 "accessing it from another website or application. To " +
                                                 "see how a website can access this API, run the " +
                                                 "Nancy.Demo.Authentication.Stateless.Website project " +
                                                 "(in the same Nancy solution).");
        }
    }
}