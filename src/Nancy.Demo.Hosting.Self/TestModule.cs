namespace Nancy.Demo.Hosting.Self
{
    using Nancy.Routing.CORS;

    public class TestModule : NancyModule
    {
        public TestModule()
        {
            this.EnableCORS();

            Get["/"] = parameters => {
                return View["staticview", Request.Url];
            };

            Get["/testing"] = parameters =>
            {
                return View["staticview", Request.Url];
            };
        }
    }
}