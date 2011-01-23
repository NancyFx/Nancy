namespace Nancy.Hosting.Wcf.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Nancy.Tests;
    using Xunit;
    using Nancy.Tests.xUnitExtensions;

    /// <remarks>
    /// These tests attempt to listen on port 1234, and so require either administrative 
    /// privileges or that a command similar to the following has been run with
    /// administrative privileges:
    /// <code>netsh http add urlacl url=http://+:1234/base user=DOMAIN\user</code>
    /// See http://msdn.microsoft.com/en-us/library/ms733768.aspx for more information.
    /// </remarks>
    public class NancyWcfGenericServiceFixture
    {
        [SkippableFact]
        public void Should_be_able_to_get_from_selfhost()
        {
            using (CreateAndOpenWebServiceHost())
            {
                var reader = 
                    new StreamReader(WebRequest.Create(new Uri(BaseUri, "rel")).GetResponse().GetResponseStream());

                var response = reader.ReadToEnd();

                response.ShouldEqual("This is the site route");
            }
        }

        [SkippableFact]
        public void Should_be_able_to_post_body_to_selfhost()
        {
            using (CreateAndOpenWebServiceHost())
            {
                const string testBody = "This is the body of the request";

                var request = 
                    WebRequest.Create("http://localhost:1234/base/rel");
                request.Method = "POST";

                var writer = 
                    new StreamWriter(request.GetRequestStream()) {AutoFlush = true};
                writer.Write(testBody);

                var responseBody = 
                    new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();

                responseBody.ShouldEqual(testBody);
            }
        }

        private static WebServiceHost CreateAndOpenWebServiceHost()
        {
            var host = new WebServiceHost(
                new NancyWcfGenericService(new DefaultNancyBootStrapper()),
                new Uri("http://localhost:1234/base/"));

            host.AddServiceEndpoint(typeof (NancyWcfGenericService), new WebHttpBinding(), "");
            try
            {
                host.Open();
            }
            catch (System.ServiceModel.AddressAccessDeniedException)
            {
                throw new SkipException("Skipped due to no Administrator access - please see test fixture for more information.");
            }

            return host;
        }
    }
}
