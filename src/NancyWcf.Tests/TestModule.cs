namespace Nancy.Hosting.Wcf.Tests
{
    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["/rel"] = parameters => {
                return "This is the site route";
            };
        }
    }
}