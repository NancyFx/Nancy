using System.IO;

namespace Nancy.Hosting.Wcf.Tests
{
    public class TestModule : NancyModule
    {
        public TestModule()
        {
            Get["/rel"] = parameters => { return "This is the site route"; };
            Post["/rel"] = parameters => { return new StreamReader(Request.Body).ReadToEnd(); };
        }
    }
}