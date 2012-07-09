namespace Nancy.Demo.Authentication.Stateless
{
    public class RootModule : NancyModule
    {
        public RootModule()
        {
            Get["/"] = _ => Response.AsText("This is a JSON API. You shouldn't be trying to access this from a browser.");
        }

    }
}