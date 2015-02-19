#if !__MonoCS__
namespace Nancy.Hosting.Self.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;

    using FakeItEasy;

    using Nancy.Bootstrapper;
    using Nancy.Helpers;
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
    public class NancySelfHostFixture
    {
        private static readonly Uri BaseUri = new Uri("http://localhost:1234/base/");

        [SkippableFact]
        public void Should_be_get_an_exception_indicating_a_conflict_when_trying_to_listen_on_a_used_prefix()
        {
            Exception ex;

            // Given
            using (CreateAndOpenSelfHost())
            {
                // When
                ex = Record.Exception(() =>
                    {
                        using (var host = new NancyHost(BaseUri))
                        {
                            host.Start();
                        }
                    });
            }

            // Then
            ex.Message.ShouldContain("conflict");
        }

        [SkippableFact]
        public void Should_be_able_to_get_any_header_from_selfhost()
        {
            // Given
            using (CreateAndOpenSelfHost())
            {
                // When
                var request = WebRequest.Create(new Uri(BaseUri, "rel/header/?query=value"));
                request.Method = "GET";

                // Then
                request.GetResponse().Headers["X-Some-Header"].ShouldEqual("Some value");
            }
        }

        [SkippableFact]
        public void Should_set_query_string_and_uri_correctly()
        {
            // Given
            Request nancyRequest = null;
            var fakeEngine = A.Fake<INancyEngine>();
            A.CallTo(() => fakeEngine.HandleRequest(A<Request>.Ignored, A<Func<NancyContext, NancyContext>>.Ignored,A<CancellationToken>.Ignored))
                .Invokes(f => nancyRequest = (Request)f.Arguments[0])
                .ReturnsLazily(c => TaskHelpers.GetCompletedTask(new NancyContext { Request = (Request)c.Arguments[0], Response = new Response() }));

            var fakeBootstrapper = A.Fake<INancyBootstrapper>();
            A.CallTo(() => fakeBootstrapper.GetEngine()).Returns(fakeEngine);

            // When
            using (CreateAndOpenSelfHost(fakeBootstrapper))
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
        public void Should_be_able_to_get_from_selfhost()
        {
            using (CreateAndOpenSelfHost())
            {
                var reader =
                    new StreamReader(WebRequest.Create(new Uri(BaseUri, "rel")).GetResponse().GetResponseStream());

                var response = reader.ReadToEnd();

                response.ShouldEqual("This is the site route");
            }
        }

        [SkippableFact]
        public void Should_be_able_to_get_from_chunked_selfhost()
        {
            using (CreateAndOpenSelfHost())
            {
                var response = WebRequest.Create(new Uri(BaseUri, "rel")).GetResponse();

                Assert.Equal("chunked", response.Headers["Transfer-Encoding"]);
                Assert.Equal(null, response.Headers["Content-Length"]);

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var contents = reader.ReadToEnd();
                    contents.ShouldEqual("This is the site route");
                }
            }
        }

        [SkippableFact]
        public void Should_be_able_to_get_from_contentlength_selfhost()
        {
            HostConfiguration configuration = new HostConfiguration()
            {
                AllowChunkedEncoding = false
            };
            using (CreateAndOpenSelfHost(null, configuration))
            {
                var response = WebRequest.Create(new Uri(BaseUri, "rel")).GetResponse();

                Assert.Equal(null, response.Headers["Transfer-Encoding"]);
                Assert.Equal(22, Convert.ToInt32(response.Headers["Content-Length"]));

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var contents = reader.ReadToEnd();
                    contents.ShouldEqual("This is the site route");
                }
            }
        }

        [SkippableFact]
        public void Should_be_able_to_post_body_to_selfhost()
        {
            using (CreateAndOpenSelfHost())
            {
                const string testBody = "This is the body of the request";

                var request =
                    WebRequest.Create(new Uri(BaseUri, "rel"));
                request.Method = "POST";

                var writer =
                    new StreamWriter(request.GetRequestStream()) { AutoFlush = true };
                writer.Write(testBody);

                var responseBody =
                    new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();

                responseBody.ShouldEqual(testBody);
            }
        }

        [SkippableFact]
        public void Should_be_able_to_get_from_selfhost_with_slashless_uri()
        {
            using (CreateAndOpenSelfHost())
            {
                var reader =
                    new StreamReader(WebRequest.Create(BaseUri.ToString().TrimEnd('/')).GetResponse().GetResponseStream());

                var response = reader.ReadToEnd();

                response.ShouldEqual("This is the site home");
            }
        }

        private static NancyHostWrapper CreateAndOpenSelfHost(INancyBootstrapper nancyBootstrapper = null, HostConfiguration configuration = null)
        {
            if (nancyBootstrapper == null)
            {
                nancyBootstrapper = new DefaultNancyBootstrapper();
            }

            var host = new NancyHost(
                nancyBootstrapper,
                configuration,
                BaseUri);

            try
            {
                host.Start();
            }
            catch
            {
                throw new SkipException("Skipped due to no Administrator access - please see test fixture for more information.");
            }

            return new NancyHostWrapper(host);
        }


        [SkippableFact]
        public void Should_be_able_to_recover_from_rendering_exception()
        {
            using (CreateAndOpenSelfHost())
            {

                var reader =
                    new StreamReader(WebRequest.Create(new Uri(BaseUri, "exception")).GetResponse().GetResponseStream());

                var response = reader.ReadToEnd();

                response.ShouldEqual("Content");
            }
        }

        [SkippableFact]
        public void Should_be_serializable()
        {
            var type = typeof(NancyHost);
            Assert.True(type.Attributes.ToString().Contains("Serializable"));
        }

        [Fact]
        public void Should_include_default_port_in_uri_prefixes()
        {
            // Given
            var host = new NancyHost(new Uri("http://localhost/"));

            // When
            var prefix = host.GetPrefixes().Single();

            // Then
            prefix.ShouldEqual("http://+:80/");
        }

        private class NancyHostWrapper : IDisposable
        {
            private readonly NancyHost host;

            public NancyHostWrapper(NancyHost host)
            {
                this.host = host;
            }

            public void Dispose()
            {
                host.Stop();
            }
        }
    }
}
#endif
