namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using Nancy.Routing;
    using Nancy.Tests.Fakes;
    using Xunit;
    using ResolveResult = System.Tuple<Nancy.Routing.Route, DynamicDictionary, System.Func<NancyContext, Response>, System.Action<NancyContext>>;

    public class NancyEngineFixture
    {
        private readonly INancyEngine engine;
        private readonly IRouteResolver resolver;
        private readonly FakeRoute route;
        private readonly NancyContext context;
        private readonly INancyContextFactory contextFactory;
        private readonly Response response;

        public NancyEngineFixture()
        {
            this.resolver = A.Fake<IRouteResolver>();
            this.response = new Response();
            this.route = new FakeRoute(response);
            this.context = new NancyContext();

            contextFactory = A.Fake<INancyContextFactory>();
            A.CallTo(() => contextFactory.Create()).Returns(context);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored.Argument)).Returns(new ResolveResult(route, DynamicDictionary.Empty, null, null));

            this.engine = new NancyEngine(resolver, A.Fake<IRouteCache>(), contextFactory);
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_resolver()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(null, A.Fake<IRouteCache>(), A.Fake<INancyContextFactory>()));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_routecache()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(A.Fake<IRouteResolver>(), null, A.Fake<INancyContextFactory>()));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_context_factory()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(A.Fake<IRouteResolver>(), A.Fake<IRouteCache>(), null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_invoke_resolved_route()
        {
            // Given
            var request = new Request("GET", "/", "http");

            // When
            this.engine.HandleRequest(request);

            // Then
            this.route.ActionWasInvoked.ShouldBeTrue();
        }

        [Fact]
        public void HandleRequest_Should_Throw_ArgumentNullException_When_Given_A_Null_Request()
        {
            var exception = Record.Exception(() => engine.HandleRequest(null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void HandleRequest_should_get_context_from_context_factory()
        {
            var request = new Request("GET", "/", "http");

            this.engine.HandleRequest(request);

            A.CallTo(() => this.contextFactory.Create()).MustHaveHappened(Repeated.Once);
        }

        [Fact]
        public void HandleRequest_should_set_correct_response_on_returned_context()
        {
            var request = new Request("GET", "/", "http");

            var result = this.engine.HandleRequest(request);

            result.Response.ShouldBeSameAs(this.response);
        }

        [Fact]
        public void HandleRequest_Null_PreRequest_Should_Not_Throw()
        {
            engine.PreRequestHook = null;

            var request = new Request("GET", "/", "http");

            this.engine.HandleRequest(request);
        }

        [Fact]
        public void HandleRequest_Null_PostRequest_Should_Not_Throw()
        {
            engine.PostRequestHook = null;

            var request = new Request("GET", "/", "http");

            this.engine.HandleRequest(request);
        }

        [Fact]
        public void HandleRequest_NonNull_PreRequest_Should_Call_PreRequest_With_Request_In_Context()
        {
            Request passedReqest = null;
            engine.PreRequestHook = (ctx) =>
            {
                passedReqest = ctx.Request;
                return null;
            };
            var request = new Request("GET", "/", "http");

            this.engine.HandleRequest(request);

            passedReqest.ShouldBeSameAs(request);
        }

        [Fact]
        public void HandleRequest_PreRequest_Returns_NonNull_Response_Should_Return_That_Response()
        {
            var response = A.Fake<Response>();
            engine.PreRequestHook = req => response;
            var request = new Request("GET", "/", "http");

            var result = this.engine.HandleRequest(request);

            result.Response.ShouldBeSameAs(response);
        }

        [Fact]
        public void HandleRequest_should_allow_post_request_hook_to_modify_context_items()
        {
            engine.PostRequestHook = ctx => ctx.Items.Add("PostReqTest", new object());
            var request = new Request("GET", "/", "http");

            var result = this.engine.HandleRequest(request);

            result.Items.ContainsKey("PostReqTest").ShouldBeTrue();
        }

        [Fact]
        public void HandleRequest_should_allow_post_request_hook_to_replace_response()
        {
            var newResponse = new Response();
            engine.PreRequestHook = ctx => ctx.Response = newResponse;
            var request = new Request("GET", "/", "http");

            var result = this.engine.HandleRequest(request);

            result.Response.ShouldBeSameAs(newResponse);
        }

        [Fact]
        public void HandleRequest_should_call_route_prereq_then_invoke_route_then_call_route_postreq()
        {
            var executionOrder = new List<String>();
            Func<NancyContext, Response> preHook = (ctx) => { executionOrder.Add("Prehook"); return null; };
            Action<NancyContext> postHook = (ctx) => { executionOrder.Add("Posthook"); };
            this.route.Action = (d) => { executionOrder.Add("RouteInvoke"); return null; };
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored.Argument)).Returns(new ResolveResult(route, DynamicDictionary.Empty, preHook, postHook));
            var localEngine = new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory);
            var request = new Request("GET", "/", "http");

            localEngine.HandleRequest(request);

            executionOrder.Count().ShouldEqual(3);
            executionOrder.SequenceEqual(new[] { "Prehook", "RouteInvoke", "Posthook" }).ShouldBeTrue();
        }

        [Fact]
        public void HandleRequest_should_not_invoke_route_if_route_prereq_returns_response()
        {
            var executionOrder = new List<String>();
            Func<NancyContext, Response> preHook = (ctx) => { executionOrder.Add("Prehook"); return new Response(); };
            Action<NancyContext> postHook = (ctx) => { executionOrder.Add("Posthook"); };
            this.route.Action = (d) => { executionOrder.Add("RouteInvoke"); return null; };
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored.Argument)).Returns(new ResolveResult(route, DynamicDictionary.Empty, preHook, postHook));
            var localEngine = new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory);
            var request = new Request("GET", "/", "http");

            localEngine.HandleRequest(request);

            executionOrder.Contains("RouteInvoke").ShouldBeFalse();
        }

        [Fact]
        public void HandleRequest_should_return_response_from_route_prereq_if_one_returned()
        {
            var preResponse = new Response();
            Func<NancyContext, Response> preHook = (ctx) => preResponse;
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored.Argument)).Returns(new ResolveResult(route, DynamicDictionary.Empty, preHook, null));
            var localEngine = new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory);
            var request = new Request("GET", "/", "http");

            var result = localEngine.HandleRequest(request);

            result.Response.ShouldBeSameAs(preResponse);
        }

        [Fact]
        public void HandleRequest_should_allow_route_postreq_to_change_response()
        {
            var postResponse = new Response();
            Action<NancyContext> postHook = (ctx) => ctx.Response = postResponse;
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored.Argument)).Returns(new ResolveResult(route, DynamicDictionary.Empty, null, postHook));
            var localEngine = new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory);
            var request = new Request("GET", "/", "http");

            var result = localEngine.HandleRequest(request);

            result.Response.ShouldBeSameAs(postResponse);
        }

        [Fact]
        public void HandleRequest_should_allow_route_postreq_to_add_items_to_context()
        {
            Action<NancyContext> postHook = (ctx) => ctx.Items.Add("RoutePostReq", new object());
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored.Argument)).Returns(new ResolveResult(route, DynamicDictionary.Empty, null, postHook));
            var localEngine = new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory);
            var request = new Request("GET", "/", "http");

            var result = localEngine.HandleRequest(request);

            result.Items.ContainsKey("RoutePostReq").ShouldBeTrue();
        }

        [Fact]
        public void HandleRequest_prereq_returns_response_should_still_run_postreq()
        {
            var response = A.Fake<Response>();
            var postReqCalled = false;
            engine.PreRequestHook = req => response;
            engine.PostRequestHook = req => postReqCalled = true;
            var request = new Request("GET", "/", "http");

            this.engine.HandleRequest(request);

            postReqCalled.ShouldBeTrue();
        }

        [Fact]
        public void HandleRequest_route_prereq_returns_response_should_still_run_route_postreq_and_postreq()
        {
            var executionOrder = new List<String>();
            Action<NancyContext> postHook = (ctx) => { executionOrder.Add("Posthook"); };
            Func<NancyContext, Response> routePreHook = (ctx) => { executionOrder.Add("Routeprehook"); return new Response(); };
            Action<NancyContext> routePostHook = (ctx) => { executionOrder.Add("Routeposthook"); };
            this.route.Action = (d) => { executionOrder.Add("RouteInvoke"); return null; };
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored.Argument)).Returns(new ResolveResult(route, DynamicDictionary.Empty, routePreHook, routePostHook));
            var localEngine = new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory);
            localEngine.PostRequestHook = postHook;
            var request = new Request("GET", "/", "http");

            localEngine.HandleRequest(request);

            executionOrder.Count().ShouldEqual(3);
            executionOrder.SequenceEqual(new[] { "Routeprehook", "Routeposthook", "Posthook" }).ShouldBeTrue();
        }
    }
}
