namespace Nancy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Bootstrapper;
    using FakeItEasy;
    using Hosting.Owin;
    using Hosting.Owin.Tests.Fakes;
    using Xunit;

    using BodyDelegate = System.Func<System.Func<System.ArraySegment<byte>, // data
                                 System.Action,                         // continuation
                                 bool>,                                 // continuation will be invoked
                     System.Action<System.Exception>,                   // onError
                     System.Action,                                     // on Complete
                     System.Action>;                                    // cancel

    using ResponseCallBack = System.Action<string, System.Collections.Generic.IDictionary<string, string>, System.Func<System.Func<System.ArraySegment<byte>, System.Action, bool>, System.Action<System.Exception>, System.Action, System.Action>>;

    public class NancyOwinHostFixture
    {
        private readonly NancyOwinHost host;

        private readonly ResponseCallBack fakeResponseCallback;

        private readonly Action<Exception> fakeErrorCallback;
        private readonly Dictionary<string, object> environment;
        private readonly INancyEngine fakeEngine;
        private readonly INancyBootstrapper fakeBootstrapper;

        public NancyOwinHostFixture()
        {
            this.fakeEngine = A.Fake<INancyEngine>();

            this.fakeBootstrapper = A.Fake<INancyBootstrapper>();

            A.CallTo(() => this.fakeBootstrapper.GetEngine()).Returns(this.fakeEngine);

            this.host = new NancyOwinHost(fakeBootstrapper);

            this.fakeResponseCallback = (status, headers, bodyDelegate) => { };

            this.fakeErrorCallback = (ex) => { };

            this.environment = new Dictionary<string, object>()
                                   {
                                       { "owin.RequestMethod", "GET" },
                                       { "owin.RequestPath", "/test" },
                                       { "owin.RequestPathBase", "/root" },
                                       { "owin.RequestQueryString", "var=value" },
                                       { "owin.RequestHeaders", new Dictionary<string, string> { { "Host", "testserver" } } },
                                       { "owin.RequestBody", null },
                                       { "owin.RequestScheme", "http" },
                                       { "owin.Version", "1.0" }
                                   };
        }

        [Fact]
        public void Should_throw_if_owin_version_is_incorrect()
        {
            this.environment["owin.Version"] = "1.2";

            var result = Record.Exception(
                () => this.host.ProcessRequest(environment, fakeResponseCallback, fakeErrorCallback));

            result.ShouldBeOfType(typeof(InvalidOperationException));
        }

        [Fact]
        public void Should_immediately_invoke_nancy_if_no_request_body_delegate()
        {
            this.host.ProcessRequest(environment, fakeResponseCallback, fakeErrorCallback);

            A.CallTo(() => this.fakeEngine.HandleRequest(A<Request>.Ignored, A<Action<NancyContext>>.Ignored, A<Action<Exception>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_invoke_request_body_delegate_if_one_exists()
        {
            var invoked = false;
            BodyDelegate bodyDelegate = (onNext, onError, onComplete) => { invoked = true; return () => { }; };
            this.environment["owin.RequestBody"] = bodyDelegate;

            this.host.ProcessRequest(environment, fakeResponseCallback, fakeErrorCallback);

            invoked.ShouldBeTrue();
        }

        [Fact]
        public void Should_invoke_nancy_on_request_body_delegate_oncomplete()
        {
            Action complete = null;
            BodyDelegate bodyDelegate = (onNext, onError, onComplete) => { complete = onComplete; return () => { }; };
            this.environment["owin.RequestBody"] = bodyDelegate;
            this.host.ProcessRequest(environment, fakeResponseCallback, fakeErrorCallback);

            complete.Invoke();

            A.CallTo(() => this.fakeEngine.HandleRequest(A<Request>.Ignored, A<Action<NancyContext>>.Ignored, A<Action<Exception>>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_invoke_error_callback_if_nancy_invoke_throws()
        {
            var called = false;
            A.CallTo(() => this.fakeEngine.HandleRequest(A<Request>.Ignored, A<Action<NancyContext>>.Ignored, A<Action<Exception>>.Ignored)).Throws(new NotSupportedException());

            this.host.ProcessRequest(environment, fakeResponseCallback, (e) => called = true);

            called.ShouldBeTrue();
        }

        [Fact]
        public void Should_invoke_response_delegate_when_nancy_returns()
        {
            var fakeResponse = new Response() { StatusCode = HttpStatusCode.OK };
            var fakeContext = new NancyContext() { Response = fakeResponse };
            this.SetupFakeNancyCompleteCallback(fakeContext);
            var called = false;
            ResponseCallBack callback = (status, headers, bodyDelegate) => called = true;

            this.host.ProcessRequest(environment, callback, fakeErrorCallback);

            called.ShouldBeTrue();
        }

        [Fact]
        public void Should_invoke_view_delegate_to_get_response()
        {
            var called = false;
            var fakeResponse = new Response()
                                   {
                                       StatusCode = HttpStatusCode.OK,
                                       Contents = s => called = true
                                   };
            var fakeContext = new NancyContext() { Response = fakeResponse };
            this.SetupFakeNancyCompleteCallback(fakeContext);
            var fakeConsumer = new FakeConsumer(false);
            ResponseCallBack callback = (r, h, b) => fakeConsumer.InvokeBodyDelegate(b);

            this.host.ProcessRequest(environment, callback, fakeErrorCallback);

            called.ShouldBeTrue();
        }

        [Fact]
        public void Should_set_return_code_in_response_callback()
        {
            var fakeResponse = new Response()
            {
                StatusCode = HttpStatusCode.OK,
                Contents = s => { }
            };
            var fakeContext = new NancyContext() { Response = fakeResponse };
            this.SetupFakeNancyCompleteCallback(fakeContext);
            string statusCode = "";
            ResponseCallBack callback = (r, h, b) => statusCode = r;

            this.host.ProcessRequest(environment, callback, fakeErrorCallback);

            statusCode.ShouldEqual("200 OK");            
        }

        [Fact]
        public void Should_set_headers_in_response_callback()
        {
            var fakeResponse = new Response()
            {
                StatusCode = HttpStatusCode.OK,
                Headers = new Dictionary<string, string>() { { "TestHeader", "TestValue" } },
                Contents = s => { }
            };
            var fakeContext = new NancyContext() { Response = fakeResponse };
            this.SetupFakeNancyCompleteCallback(fakeContext);
            IDictionary<string, string> headers = null;
            ResponseCallBack callback = (r, h, b) => headers = h;

            this.host.ProcessRequest(environment, callback, fakeErrorCallback);

            // 2 headers because the default content-type is text/html
            headers.Count.ShouldEqual(2);
            headers["Content-Type"].ShouldEqual("text/html");
            headers["TestHeader"].ShouldEqual("TestValue");
        }

        [Fact]
        public void Should_set_contenttype_in_response_callback()
        {
            var fakeResponse = new Response
                                   {
                                       StatusCode = HttpStatusCode.OK,
                                       ContentType = "text/html",
                                       Contents = s => { }
                                   };
            var fakeContext = new NancyContext {Response = fakeResponse};
            SetupFakeNancyCompleteCallback(fakeContext);
            IDictionary<string, string> headers = null;
            ResponseCallBack callback = (r, h, b) => headers = h;

            host.ProcessRequest(environment, callback, fakeErrorCallback);
            
            headers.Count.ShouldEqual(1);
            headers["Content-Type"].ShouldEqual("text/html");
        }

        [Fact]
        public void Should_send_null_continuation()
        {
            var fakeResponse = new Response()
            {
                StatusCode = HttpStatusCode.OK,
                Contents = s => s.WriteByte(12)
            };
            var fakeContext = new NancyContext() { Response = fakeResponse };
            this.SetupFakeNancyCompleteCallback(fakeContext);
            var fakeConsumer = new FakeConsumer(false);
            ResponseCallBack callback = (r, h, b) => fakeConsumer.InvokeBodyDelegate(b);

            this.host.ProcessRequest(environment, callback, fakeErrorCallback);

            fakeConsumer.ContinuationSent.ShouldBeFalse();
        }

        [Fact]
        public void Should_send_entire_body()
        {
            var data1 = Encoding.ASCII.GetBytes("Some content");
            var data2 = Encoding.ASCII.GetBytes("Some more content");
            var fakeResponse = new Response()
            {
                StatusCode = HttpStatusCode.OK,
                Contents = s =>
                    {
                        s.Write(data1, 0, data1.Length);
                        s.Write(data2, 0, data2.Length);
                    }
            };
            var fakeContext = new NancyContext() { Response = fakeResponse };
            this.SetupFakeNancyCompleteCallback(fakeContext);
            var fakeConsumer = new FakeConsumer(false);
            ResponseCallBack callback = (r, h, b) => fakeConsumer.InvokeBodyDelegate(b);

            this.host.ProcessRequest(environment, callback, fakeErrorCallback);

            fakeConsumer.ConsumedData.SequenceEqual(data1.Concat(data2)).ShouldBeTrue();
        }

        [Fact]
        public void Should_dispose_context_on_completion_of_body_delegate()
        {
            var data1 = Encoding.ASCII.GetBytes("Some content");
            var fakeResponse = new Response()
            {
                StatusCode = HttpStatusCode.OK,
                Contents = s => s.Write(data1, 0, data1.Length)
            };
            var fakeContext = new NancyContext() { Response = fakeResponse };
            var mockDisposable = A.Fake<IDisposable>();
            fakeContext.Items.Add("Test",  mockDisposable);
            this.SetupFakeNancyCompleteCallback(fakeContext);
            var fakeConsumer = new FakeConsumer(false);
            ResponseCallBack callback = (r, h, b) => fakeConsumer.InvokeBodyDelegate(b);

            this.host.ProcessRequest(environment, callback, fakeErrorCallback);

            A.CallTo(() => mockDisposable.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_dispose_context_if_body_delegate_throws()
        {
            var fakeResponse = new Response()
            {
                StatusCode = HttpStatusCode.OK,
                Contents = s => { throw new InvalidOperationException(); }
            };
            var fakeContext = new NancyContext() { Response = fakeResponse };
            var mockDisposable = A.Fake<IDisposable>();
            fakeContext.Items.Add("Test", mockDisposable);
            this.SetupFakeNancyCompleteCallback(fakeContext);
            var fakeConsumer = new FakeConsumer(false);
            ResponseCallBack callback = (r, h, b) => fakeConsumer.InvokeBodyDelegate(b);

            this.host.ProcessRequest(environment, callback, fakeErrorCallback);

            A.CallTo(() => mockDisposable.Dispose()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_read_entire_request_body_when_theres_no_continuation()
        {
            var requestBody = Encoding.ASCII.GetBytes("This is some request body content");
            var fakeRequestBodyDelegate = new FakeProducer(false, requestBody, 5, false);
            this.environment["owin.RequestBody"] = (BodyDelegate)fakeRequestBodyDelegate;
            Request request = null;
            A.CallTo(() => this.fakeEngine.HandleRequest(A<Request>.Ignored, A<Action<NancyContext>>.Ignored, A<Action<Exception>>.Ignored))
                .Invokes(i => request = (Request)i.Arguments[0]);

            this.host.ProcessRequest(environment, fakeResponseCallback, fakeErrorCallback);
            fakeRequestBodyDelegate.SendAll();

            var read = new StreamReader(request.Body);
            var output = read.ReadToEnd();
            output.ShouldEqual("This is some request body content");
        }

        [Fact]
        public void Should_read_entire_request_body_when_there_is_a_continuation()
        {
            var requestBody = Encoding.ASCII.GetBytes("This is some request body content");
            var fakeRequestBodyDelegate = new FakeProducer(true, requestBody, 5, false);
            this.environment["owin.RequestBody"] = (BodyDelegate)fakeRequestBodyDelegate;
            Request request = null;
            A.CallTo(() => this.fakeEngine.HandleRequest(A<Request>.Ignored, A<Action<NancyContext>>.Ignored, A<Action<Exception>>.Ignored))
                .Invokes(i => request = (Request)i.Arguments[0]);

            this.host.ProcessRequest(environment, fakeResponseCallback, fakeErrorCallback);
            fakeRequestBodyDelegate.SendAll();

            var read = new StreamReader(request.Body);
            var output = read.ReadToEnd();
            output.ShouldEqual("This is some request body content");
        }

        [Fact]
        public void Should_set_cookie_with_valid_header()
        {
            var fakeResponse = new Response() { StatusCode = HttpStatusCode.OK };
            fakeResponse.AddCookie("test", "testvalue");
            var fakeContext = new NancyContext() { Response = fakeResponse };
            
            this.SetupFakeNancyCompleteCallback(fakeContext);
            var respHeaders = new Dictionary<string, string>();
            ResponseCallBack callback = (status, headers, bodyDelegate) =>respHeaders=(Dictionary<string,string>)headers;

            this.host.ProcessRequest(environment, callback, fakeErrorCallback);
            respHeaders.ContainsKey("Set-Cookie").ShouldBeTrue();
            (respHeaders["Set-Cookie"]=="test=testvalue; path=/").ShouldBeTrue();
        }
        /// <summary>
        /// Sets the fake nancy engine to execute the complete callback with the given context
        /// </summary>
        /// <param name="context">Context to return</param>
        private void SetupFakeNancyCompleteCallback(NancyContext context)
        {
            A.CallTo(() => this.fakeEngine.HandleRequest(A<Request>.Ignored, A<Action<NancyContext>>.Ignored, A<Action<Exception>>.Ignored))
                .Invokes((i => ((Action<NancyContext>)i.Arguments[1]).Invoke(context)));
        }
    }
}