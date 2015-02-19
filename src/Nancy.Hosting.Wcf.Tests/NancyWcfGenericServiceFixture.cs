namespace Nancy.Hosting.Wcf.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Threading;

    using FakeItEasy;

    using Nancy.Bootstrapper;
    using Nancy.Helpers;
    using Nancy.Tests;
    using Nancy.Tests.xUnitExtensions;

    using Xunit;

    using HttpStatusCode = Nancy.HttpStatusCode;

    /// <remarks>
    /// These tests attempt to listen on port 56297, and so require either administrative 
    /// privileges or that a command similar to the following has been run with
    /// administrative privileges:
    /// <code>netsh http add urlacl url=http://+:56297/base user=DOMAIN\user</code>
    /// See http://msdn.microsoft.com/en-us/library/ms733768.aspx for more information.
    /// </remarks>
    public class NancyWcfGenericServiceFixture
    {
        private static readonly Uri BaseUri = new Uri("http://localhost:56297/base/");

        [SkippableFact]
        public void Should_be_able_to_get_any_header_from_selfhost()
        {
            // Given
            using (CreateAndOpenWebServiceHost())
            {
                var request = WebRequest.Create(new Uri(BaseUri, "rel/header/?query=value"));
                request.Method = "GET";

                // When
                var header = request.GetResponse().Headers["X-Some-Header"];

                // Then
                header.ShouldEqual("Some value");
            }
        }

        [SkippableFact]
        public void Should_set_query_string_and_uri_correctly()
        {
            // Given
            Request nancyRequest = null;
            var fakeEngine = A.Fake<INancyEngine>();
            A.CallTo(() => fakeEngine.HandleRequest(A<Request>.Ignored, A<Func<NancyContext, NancyContext>>.Ignored, A<CancellationToken>.Ignored))
                .Invokes(f => nancyRequest = (Request)f.Arguments[0])
                .Returns(TaskHelpers.GetCompletedTask(new NancyContext()));
            var fakeBootstrapper = A.Fake<INancyBootstrapper>();
            A.CallTo(() => fakeBootstrapper.GetEngine()).Returns(fakeEngine);

            // When
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

            // Then
            nancyRequest.Path.ShouldEqual("/test/stuff");
            Assert.True(nancyRequest.Query.query.HasValue);
            Assert.True(nancyRequest.Query.query2.HasValue);
        }

        [SkippableFact]
        public void Should_set_path_and_url_correctly_without_trailing_slash()
        {
            // Given
            Request nancyRequest = null;
            var fakeEngine = A.Fake<INancyEngine>();
            A.CallTo(() => fakeEngine.HandleRequest(A<Request>.Ignored, A<Func<NancyContext, NancyContext>>.Ignored, A<CancellationToken>.Ignored))
                .Invokes(f => nancyRequest = (Request)f.Arguments[0])
                .Returns(TaskHelpers.GetCompletedTask(new NancyContext()));
            var fakeBootstrapper = A.Fake<INancyBootstrapper>();
            A.CallTo(() => fakeBootstrapper.GetEngine()).Returns(fakeEngine);

            var baseUriWithoutTrailingSlash = new Uri("http://localhost:56297/base");

            // When
            using(CreateAndOpenWebServiceHost(fakeBootstrapper, baseUriWithoutTrailingSlash))
            {
                var request = WebRequest.Create(new Uri(BaseUri, "test/stuff"));
                request.Method = "GET";

                try
                {
                    request.GetResponse();
                }
                catch(WebException)
                {
                    // Will throw because it returns 404 - don't care.
                }
            }

            // Then
            nancyRequest.Path.ShouldEqual("/test/stuff");
            nancyRequest.Url.ToString().ShouldEqual("http://localhost:56297/base/test/stuff");
        }

        [SkippableFact]
        public void Should_be_able_to_get_from_selfhost()
        {
            // Given
            using (CreateAndOpenWebServiceHost())
            {
                var reader =
                    new StreamReader(WebRequest.Create(new Uri(BaseUri, "rel")).GetResponse().GetResponseStream());

                // When
                var response = reader.ReadToEnd();

                // Then
                response.ShouldEqual("This is the site route");
            }
        }

        [SkippableFact]
        public void Should_be_able_to_post_body_to_selfhost()
        {
            // Given
            using (CreateAndOpenWebServiceHost())
            {
                const string testBody = "This is the body of the request";

                var request = 
                    WebRequest.Create(new Uri(BaseUri, "rel"));
                request.Method = "POST";

                var writer = 
                    new StreamWriter(request.GetRequestStream()) {AutoFlush = true};
                writer.Write(testBody);

                // When
                var responseBody = 
                    new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();

                // Then
                responseBody.ShouldEqual(testBody);
            }
        }

        [SkippableFact]
        public void Should_nancyrequest_contain_hostname_port_and_scheme()
        {
            // Given
            Request nancyRequest = null;
            var fakeEngine = A.Fake<INancyEngine>();
            var fakeBootstrapper = A.Fake<INancyBootstrapper>();

            A.CallTo(() => fakeEngine.HandleRequest(A<Request>.Ignored, A<Func<NancyContext, NancyContext>>.Ignored, A<CancellationToken>.Ignored))
                .Invokes(f => nancyRequest = (Request)f.Arguments[0])
                .Returns(TaskHelpers.GetCompletedTask(new NancyContext()));
            A.CallTo(() => fakeBootstrapper.GetEngine()).Returns(fakeEngine);

            // When 
            using (CreateAndOpenWebServiceHost(fakeBootstrapper)) 
            {
                var request = WebRequest.Create(BaseUri);
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

            // Then
            Assert.Equal(56297, nancyRequest.Url.Port);
            Assert.Equal("localhost", nancyRequest.Url.HostName);
            Assert.Equal("http", nancyRequest.Url.Scheme);
        }

        [SkippableFact]
        public void Should_not_have_content_type_header_for_not_modified_responses()
        {
            // Given
            var fakeEngine = A.Fake<INancyEngine>();
            var fakeBootstrapper = A.Fake<INancyBootstrapper>();

            // Context sends back a 304 Not Modified
            var context = new NancyContext
            {
                Response = new Response
                {
                    ContentType = null,
                    StatusCode = HttpStatusCode.NotModified
                }
            };

            A.CallTo(() => fakeEngine.HandleRequest(A<Request>.Ignored, A<Func<NancyContext, NancyContext>>.Ignored, A<CancellationToken>.Ignored))
                .Returns(TaskHelpers.GetCompletedTask(context));
            A.CallTo(() => fakeBootstrapper.GetEngine()).Returns(fakeEngine);

            // When a request is made and responded to with a status of 304 Not Modified
            WebResponse response = null;
            using (CreateAndOpenWebServiceHost(fakeBootstrapper)) 
            {
                var request = WebRequest.Create(new Uri(BaseUri, "notmodified"));
                request.Method = "GET";
                try 
                {
                    request.GetResponse();
                }
                catch (WebException notModifiedEx) 
                {
                    // Will throw because it returns 304
                    response = notModifiedEx.Response;
                }
            }

            // Then
            Assert.NotNull(response);
            Assert.False(response.Headers.AllKeys.Any(header => header == "Content-Type"));
        }

        private static WebServiceHost CreateAndOpenWebServiceHost(INancyBootstrapper nancyBootstrapper = null, Uri baseUri = null)
        {
            if (nancyBootstrapper == null)
            {
                nancyBootstrapper = new DefaultNancyBootstrapper();
            }

            var host = new WebServiceHost(
                new NancyWcfGenericService(nancyBootstrapper),
                baseUri ?? BaseUri);

            host.AddServiceEndpoint(typeof (NancyWcfGenericService), new WebHttpBinding(), "");
            try
            {
                host.Open();
            }
            catch (AddressAccessDeniedException)
            {
                throw new SkipException("Skipped due to no Administrator access - please see test fixture for more information.");
            }

            return host;
        }
    }
}
