namespace Nancy.WcfHosting.Demo
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