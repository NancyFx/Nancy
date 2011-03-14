namespace Nancy.Tests
{
    using System;
    using System.Collections.Generic;
    using Bootstrapper;
    using FakeItEasy;
    using Hosting.Owin;
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
                                       { "owin.RequestHeaders", new Dictionary<string, string>() },
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
    }
}