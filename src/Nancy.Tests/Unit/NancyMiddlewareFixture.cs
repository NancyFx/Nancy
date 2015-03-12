namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Nancy.Bootstrapper;
    using Nancy.Helpers;
    using Nancy.Owin;

    using Xunit;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>,
       System.Threading.Tasks.Task>;

    using MidFunc = System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>,
            System.Threading.Tasks.Task>>;

    public class NancyMiddlewareFixture
    {
        private readonly Dictionary<string, object> environment;
        private readonly INancyBootstrapper fakeBootstrapper;
        private readonly INancyEngine fakeEngine;
        private readonly AppFunc host;

        public NancyMiddlewareFixture()
        {
            this.fakeEngine = A.Fake<INancyEngine>();
            this.fakeBootstrapper = A.Fake<INancyBootstrapper>();
            A.CallTo(() => this.fakeBootstrapper.GetEngine()).Returns(this.fakeEngine);
            this.host = NancyMiddleware.UseNancy(new NancyOptions(this.fakeBootstrapper))(null);
            this.environment = new Dictionary<string, object>
            {
                {"owin.RequestMethod", "GET"},
                {"owin.RequestPath", "/test"},
                {"owin.RequestPathBase", "/root"},
                {"owin.RequestQueryString", "var=value"},
                {"owin.RequestBody", Stream.Null},
                {"owin.RequestHeaders", new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)},
                {"owin.RequestScheme", "http"},
                {"owin.ResponseHeaders", new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)},
                {"owin.ResponseBody", new MemoryStream()},
                {"owin.ResponseReasonPhrase", string.Empty},
                {"owin.Version", "1.0"},
                {"owin.CallCancelled", CancellationToken.None}
            };
        }

        [Fact]
        public void Should_immediately_invoke_nancy_if_no_request_body_delegate()
        {
            // Given
            var fakeResponse = new Response { StatusCode = HttpStatusCode.OK, Contents = s => { } };
            var fakeContext = new NancyContext { Response = fakeResponse };
            this.SetupFakeNancyCompleteCallback(fakeContext);

            // When
            this.host(this.environment);

            // Then
            A.CallTo(() =>  this.fakeEngine.HandleRequest(
                    A<Request>.Ignored,
                    A<Func<NancyContext, NancyContext>>.Ignored,
                    (CancellationToken)this.environment["owin.CallCancelled"]))
             .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_set_return_code_in_response_callback()
        {
            // Given
            var fakeResponse = new Response {StatusCode = HttpStatusCode.OK, Contents = s => { }};
            var fakeContext = new NancyContext {Response = fakeResponse};
            this.SetupFakeNancyCompleteCallback(fakeContext);

            // When
            this.host.Invoke(this.environment);

            // Then
            ((int)this.environment["owin.ResponseStatusCode"]).ShouldEqual(200);
        }

        [Fact]
        public void Should_set_headers_in_response_callback()
        {
            // Given
            var fakeResponse = new Response
            {
                StatusCode = HttpStatusCode.OK,
                Headers = new Dictionary<string, string> {{"TestHeader", "TestValue"}},
                Contents = s => { }
            };
            var fakeContext = new NancyContext {Response = fakeResponse};
            this.SetupFakeNancyCompleteCallback(fakeContext);

            // When
            this.host.Invoke(this.environment);
            var headers = (IDictionary<string, string[]>)this.environment["owin.ResponseHeaders"];

            // Then
            // 2 headers because the default content-type is text/html
            headers.Count.ShouldEqual(2);
            headers["Content-Type"][0].ShouldEqual("text/html");
            headers["TestHeader"][0].ShouldEqual("TestValue");
        }

        [Fact]
        public void Should_send_entire_body()
        {
            // Given
            var data1 = Encoding.ASCII.GetBytes("Some content");
            var data2 = Encoding.ASCII.GetBytes("Some more content");
            var fakeResponse = new Response
            {
                StatusCode = HttpStatusCode.OK,
                Contents = s =>
                {
                    s.Write(data1, 0, data1.Length);
                    s.Write(data2, 0, data2.Length);
                }
            };
            var fakeContext = new NancyContext {Response = fakeResponse};
            this.SetupFakeNancyCompleteCallback(fakeContext);

            // When
            this.host.Invoke(this.environment);

            var data = ((MemoryStream)this.environment["owin.ResponseBody"]).ToArray();

            // Then
            data.ShouldEqualSequence(data1.Concat(data2));
        }

        [Fact]
        public void Should_dispose_context_on_completion_of_body_delegate()
        {
            // Given
            var data1 = Encoding.ASCII.GetBytes("Some content");
            var fakeResponse = new Response {StatusCode = HttpStatusCode.OK, Contents = s => s.Write(data1, 0, data1.Length)};
            var fakeContext = new NancyContext {Response = fakeResponse};
            var mockDisposable = A.Fake<IDisposable>();
            fakeContext.Items.Add("Test", mockDisposable);
            this.SetupFakeNancyCompleteCallback(fakeContext);

            // When
            this.host.Invoke(environment);

            // Then
            A.CallTo(() => mockDisposable.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_set_cookie_with_valid_header()
        {
            // Given
            var fakeResponse = new Response {StatusCode = HttpStatusCode.OK};
            fakeResponse.WithCookie("test", "testvalue");
            fakeResponse.WithCookie("test1", "testvalue1");
            var fakeContext = new NancyContext {Response = fakeResponse};

            this.SetupFakeNancyCompleteCallback(fakeContext);

            // When
            this.host.Invoke(this.environment).Wait();
            var respHeaders = Get<IDictionary<string, string[]>>(this.environment, "owin.ResponseHeaders");

            // Then
            respHeaders.ContainsKey("Set-Cookie").ShouldBeTrue();
            (respHeaders["Set-Cookie"][0] == "test=testvalue; path=/").ShouldBeTrue();
            (respHeaders["Set-Cookie"][1] == "test1=testvalue1; path=/").ShouldBeTrue();
        }

        [Fact]
        public void Should_append_setcookie_headers()
        {
            //Given
            var respHeaders = Get<IDictionary<string, string[]>>(this.environment, "owin.ResponseHeaders");
            const string middlewareSetCookie = "other=othervalue; path=/";
            respHeaders.Add("Set-Cookie", new[] { middlewareSetCookie });

            var fakeResponse = new Response { StatusCode = HttpStatusCode.OK };
            fakeResponse.WithCookie("test", "testvalue");
            var fakeContext = new NancyContext { Response = fakeResponse };

            this.SetupFakeNancyCompleteCallback(fakeContext);

            //When
            this.host.Invoke(this.environment).Wait();

            //Then
            respHeaders["Set-Cookie"].Length.ShouldEqual(2);
            (respHeaders["Set-Cookie"][0] == middlewareSetCookie).ShouldBeTrue();
            (respHeaders["Set-Cookie"][1] == "test=testvalue; path=/").ShouldBeTrue();
        }

        [Fact]
        public async Task Should_flow_katana_user()
        {
            // Given
            IPrincipal user = new ClaimsPrincipal(new GenericIdentity("testuser"));
            this.environment.Add("server.User", user);

            var fakeResponse = new Response { StatusCode = HttpStatusCode.OK, Contents = s => { } };
            var fakeContext = new NancyContext { Response = fakeResponse };
            this.SetupFakeNancyCompleteCallback(fakeContext);

            // When
            await this.host.Invoke(this.environment);

            // Then
            fakeContext.CurrentUser.ShouldEqual(user);
        }

        [Fact]
        public async Task Should_flow_owin_user()
        {
            // Given
            var user = new ClaimsPrincipal(new GenericIdentity("testuser"));
            this.environment.Add("owin.RequestUser", user);

            var fakeResponse = new Response { StatusCode = HttpStatusCode.OK, Contents = s => { } };
            var fakeContext = new NancyContext { Response = fakeResponse };
            this.SetupFakeNancyCompleteCallback(fakeContext);

            // When
            await this.host.Invoke(this.environment);

            // Then
            fakeContext.CurrentUser.ShouldEqual(user);
        }

        /// <summary>
        /// Sets the fake nancy engine to execute the complete callback with the given context
        /// </summary>
        /// <param name="context">Context to return</param>
        private void SetupFakeNancyCompleteCallback(NancyContext context)
        {
            A.CallTo(() => this.fakeEngine.HandleRequest(
                A<Request>.Ignored,
                A<Func<NancyContext, NancyContext>>.Ignored,
                A<CancellationToken>.Ignored))
             .Invokes((Request _, Func<NancyContext, NancyContext> preRequest, CancellationToken __) => preRequest(context))
             .Returns(TaskHelpers.GetCompletedTask(context));
        }

        private static T Get<T>(IDictionary<string, object> env, string key)
        {
            object value;
            return env.TryGetValue(key, out value) && value is T ? (T)value : default(T);
        }
    }
}