namespace Nancy.Hosting.Wcf.Tests
{
    using System.IO;

    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["/rel"] = parameters => {
                return "This is the site route";
            };

            Post["/rel"] = parameters => {
                return new StreamReader(Request.Body).ReadToEnd();
            };
        }
    }
}