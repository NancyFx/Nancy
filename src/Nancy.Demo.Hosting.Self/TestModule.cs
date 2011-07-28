namespace Nancy.Demo.Hosting.Self
{
    public class TestModule : NancyModule
    {
        public TestModule()
        {
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