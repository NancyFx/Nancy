namespace Nancy.Demo.Hosting.Self
{
    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["/"] = parameters => {
                return View["staticview"];
            };
        }
    }
}