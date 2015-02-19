namespace Nancy.Demo.Hosting.Wcf
{
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