namespace Nancy.Hosting.Wcf.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Bootstrapper;
    using FakeItEasy;
    using Nancy.Tests;
    using Nancy.Tests.xUnitExtensions;
    using Xunit;

    /// <remarks>
    /// These tests attempt to listen on port 1234, and so require either administrative 
    /// privileges or that a command similar to the following has been run with
    /// administrative privileges:
    /// <code>netsh http add urlacl url=http://+:1234/base user=DOMAIN\user</code>
    /// See http://msdn.microsoft.com/en-us/library/ms733768.aspx for more information.
    /// </remarks>
    public class NancyWcfGenericServiceFixture
    {
        private static readonly Uri BaseUri = new Uri("http://localhost:1234/base/");

        [SkippableFact]
        public void Should_be_able_to_get_any_header_from_selfhost()
        {
            using (CreateAndOpenWebServiceHost())
            {
                var request = WebRequest.Create(new Uri(BaseUri, "rel/header/?query=value"));
                request.Method = "GET";

                request.GetResponse().Headers["X-Some-Header"].ShouldEqual("Some value");
            }
        }

        [SkippableFact]
        public void Should_set_query_string_and_uri_correctly()
        {
            Request nancyRequest = null;
            var fakeEngine = A.Fake<INancyEngine>();
            A.CallTo(() => fakeEngine.HandleRequest(A<Request>.Ignored))
                .Invokes((f) => nancyRequest = (Request)f.Arguments[0]);
            var fakeBootstrapper = A.Fake<INancyBootstrapper>();
            A.CallTo(() => fakeBootstrapper.GetEngine()).Returns(fakeEngine);

            using (CreateAndOpenWebServiceHost(fakeBootstrapper))
            {
                var request = WebRequest.Create(new Uri(BaseUri, "test/stuff?query=value&query2=value2"));
                request.Method = "GET";

                try
                {
                    request.GetResponse();
                }
                catch (WebException)
                {
                    // Will throw because it returns 404 - don't care.
                }
            }

            nancyRequest.Path.ShouldEqual("/test/stuff");
            Assert.True(nancyRequest.Query.query.HasValue);
            Assert.True(nancyRequest.Query.query2.HasValue);
        }

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
                    WebRequest.Create(new Uri(BaseUri, "rel"));
                request.Method = "POST";

                var writer = 
                    new StreamWriter(request.GetRequestStream()) {AutoFlush = true};
                writer.Write(testBody);

                var responseBody = 
                    new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();

                responseBody.ShouldEqual(testBody);
            }
        }

        [SkippableFact]
        public void NancyRequest_should_contain_hostname_port_and_scheme()
        {
            Request nancyRequest = null;
            var fakeEngine = A.Fake<INancyEngine>();
            A.CallTo(() => fakeEngine.HandleRequest(A<Request>.Ignored))
                .Invokes((f) => nancyRequest = (Request)f.Arguments[0]);
            var fakeBootstrapper = A.Fake<INancyBootstrapper>();
            A.CallTo(() => fakeBootstrapper.GetEngine()).Returns(fakeEngine);

            using (CreateAndOpenWebServiceHost(fakeBootstrapper)) {
                var request = WebRequest.Create(BaseUri);
                request.Method = "GET";

                try {
                    request.GetResponse();
                }
                catch (WebException) {
                    // Will throw because it returns 404 - don't care.
                }
            }

            Assert.Equal(1234, nancyRequest.Url.Port);
            Assert.Equal("localhost", nancyRequest.Url.HostName);
            Assert.Equal("http", nancyRequest.Url.Scheme);
        }

        private static WebServiceHost CreateAndOpenWebServiceHost(INancyBootstrapper nancyBootstrapper = null)
        {
            if (nancyBootstrapper == null)
            {
                nancyBootstrapper = new DefaultNancyBootstrapper();
            }

            var host = new WebServiceHost(
                new NancyWcfGenericService(nancyBootstrapper),
                BaseUri);

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
