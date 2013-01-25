namespace Nancy.Demo.Hosting.Self
{
    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["/"] = parameters => {
                return View["staticview", this.Request.Url];
            };

            Get["/testing"] = parameters =>
            {
                return View["staticview", this.Request.Url];
            };
        }
    }
}