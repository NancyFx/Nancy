namespace Nancy.Demo.Hosting.Wcf
{
    using Nancy;

    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = parameters => {
                return View["staticview"];
            };
        }
    }
}