namespace Nancy.Hosting.Self.Tests
{
    using System.IO;

	public class TestModule : NancyModule
    {
        public TestModule()
        {
        	Get["/"] = parameters => "This is the site home";

            Get["/rel"] = parameters => "This is the site route";

            Get["/rel/header"] = parameters =>
                {
                    var response = new Response();
                    response.Headers["X-Some-Header"] = "Some value";

                    return response;
                };

            Post["/rel"] = parameters => new StreamReader(Request.Body).ReadToEnd();
        }
    }
}