namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Diagnostics;
    using FakeItEasy;
    using Nancy.Bootstrapper;
    using Nancy.Diagnostics;
    using Nancy.ErrorHandling;
    using Nancy.Extensions;
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
        private readonly IErrorHandler errorHandler;
        private readonly IRouteInvoker routeInvoker;

        public NancyEngineFixture()
        {
            this.resolver = A.Fake<IRouteResolver>();
            this.response = new Response();
            this.route = new FakeRoute(response);
            this.context = new NancyContext();
            this.errorHandler = A.Fake<IErrorHandler>();

            A.CallTo(() => errorHandler.HandlesStatusCode(A<HttpStatusCode>.Ignored, A<NancyContext>.Ignored)).Returns(false);

            contextFactory = A.Fake<INancyContextFactory>();
            A.CallTo(() => contextFactory.Create()).Returns(context);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, null, null));

            var applicationPipelines = new Pipelines();

            this.routeInvoker = A.Fake<IRouteInvoker>();

            A.CallTo(() => this.routeInvoker.Invoke(A<Route>._, A<DynamicDictionary>._)).ReturnsLazily(arg =>
            {
                return (Response)((Route)arg.Arguments[0]).Action.Invoke((DynamicDictionary)arg.Arguments[1]);
            });

            this.engine =
                new NancyEngine(resolver, contextFactory, new[] { this.errorHandler }, A.Fake<IRequestTracing>(), this.routeInvoker)
                {
                    RequestPipelinesFactory = ctx => applicationPipelines
                };
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_resolver()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(null, A.Fake<INancyContextFactory>(), new[] { this.errorHandler }, A.Fake<IRequestTracing>(), this.routeInvoker));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_context_factory()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(A.Fake<IRouteResolver>(), null, new[] { this.errorHandler }, A.Fake<IRequestTracing>(), this.routeInvoker));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_error_handler()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(A.Fake<IRouteResolver>(), A.Fake<INancyContextFactory>(), null, A.Fake<IRequestTracing>(), this.routeInvoker));

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
            // Given,
            Request request = null;

            // When
            var exception = Record.Exception(() => engine.HandleRequest(request));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void HandleRequest_should_get_context_from_context_factory()
        {
            // Given
            var request = new Request("GET", "/", "http");

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.contextFactory.Create()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void HandleRequest_should_set_correct_response_on_returned_context()
        {
            // Given
            var request = new Request("GET", "/", "http");
            
            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.Response.ShouldBeSameAs(this.response);
        }

        [Fact]
        public void Should_not_add_nancy_version_number_header_on_returned_response()
        {
            // NOTE: Regression for removal of nancy-version from response headers
            // Given
            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.Response.Headers.ContainsKey("Nancy-Version").ShouldBeFalse();
        }

        [Fact]
        public void Should_not_throw_exception_when_handlerequest_is_invoked_and_pre_request_hook_is_null()
        {
            // Given
            var pipelines = new Pipelines { BeforeRequest = null };
            engine.RequestPipelinesFactory = (ctx) => pipelines;

            // When
            var request = new Request("GET", "/", "http");

            // Then
            this.engine.HandleRequest(request);
        }

        [Fact]
        public void Should_not_throw_exception_when_handlerequest_is_invoked_and_post_request_hook_is_null()
        {
            // Given
            var pipelines = new Pipelines { AfterRequest = null };
            engine.RequestPipelinesFactory = (ctx) => pipelines;

            // When
            var request = new Request("GET", "/", "http");

            // Then
            this.engine.HandleRequest(request);
        }

        [Fact]
        public void Should_call_pre_request_hook_should_be_invoked_with_request_from_context()
        {
            // Given
            Request passedRequest = null;

            var pipelines = new Pipelines();
            pipelines.BeforeRequest.AddItemToStartOfPipeline((ctx) =>
            {
                passedRequest = ctx.Request;
                return null;
            });

            engine.RequestPipelinesFactory = (ctx) => pipelines;

            var request = new Request("GET", "/", "http");

            // When
            this.engine.HandleRequest(request);

            // Then
            passedRequest.ShouldBeSameAs(request);
        }

        [Fact]
        public void Should_return_response_from_pre_request_hook_when_not_null()
        {
            // Given
            var returnedResponse = A.Fake<Response>();

            var pipelines = new Pipelines();
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx => returnedResponse);

            engine.RequestPipelinesFactory = (ctx) => pipelines;

            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.Response.ShouldBeSameAs(returnedResponse);
        }

        [Fact]
        public void Should_allow_post_request_hook_to_modify_context_items()
        {
            // Given
            var pipelines = new Pipelines();
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx =>
            {
                ctx.Items.Add("PostReqTest", new object());
                return null;
            });

            engine.RequestPipelinesFactory = (ctx) => pipelines;

            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.Items.ContainsKey("PostReqTest").ShouldBeTrue();
        }

        [Fact]
        public void Should_allow_post_request_hook_to_replace_response()
        {
            // Given
            var newResponse = new Response();

            var pipelines = new Pipelines();
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx => ctx.Response = newResponse);
            engine.RequestPipelinesFactory = (ctx) => pipelines;

            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.Response.ShouldBeSameAs(newResponse);
        }

        [Fact]
        public void HandleRequest_should_call_route_prereq_then_invoke_route_then_call_route_postreq()
        {
            // Given
            var executionOrder = new List<String>();
            Func<NancyContext, Response> preHook = (ctx) => { executionOrder.Add("Prehook"); return null; };
            Action<NancyContext> postHook = (ctx) => executionOrder.Add("Posthook");

            this.route.Action = (d) => { executionOrder.Add("RouteInvoke"); return null; };
            var prePostResolver = A.Fake<IRouteResolver>();

            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, preHook, postHook));

            var pipelines = new Pipelines();

            var localEngine =
                new NancyEngine(prePostResolver, contextFactory, new[] { this.errorHandler }, A.Fake<IRequestTracing>(), this.routeInvoker)
                {
                    RequestPipelinesFactory = ctx => pipelines
                };

            var request = new Request("GET", "/", "http");

            // When
            localEngine.HandleRequest(request);

            // Then
            executionOrder.Count().ShouldEqual(3);
            executionOrder.SequenceEqual(new[] { "Prehook", "RouteInvoke", "Posthook" }).ShouldBeTrue();
        }

        [Fact]
        public void HandleRequest_should_not_invoke_route_if_route_prereq_returns_response()
        {
            // Given
            var executionOrder = new List<String>();
            Func<NancyContext, Response> preHook = (ctx) => { executionOrder.Add("Prehook"); return new Response(); };
            Action<NancyContext> postHook = (ctx) => executionOrder.Add("Posthook");
            this.route.Action = (d) => { executionOrder.Add("RouteInvoke"); return null; };
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, preHook, postHook));

            var pipelines = new Pipelines();

            var localEngine =
                new NancyEngine(prePostResolver, contextFactory, new IErrorHandler[] { this.errorHandler }, A.Fake<IRequestTracing>(), this.routeInvoker)
                {
                    RequestPipelinesFactory = ctx => pipelines
                };

            var request = new Request("GET", "/", "http");

            // When
            localEngine.HandleRequest(request);

            // Then
            executionOrder.Contains("RouteInvoke").ShouldBeFalse();
        }

        [Fact]
        public void HandleRequest_should_return_response_from_route_prereq_if_one_returned()
        {
            // Given
            var preResponse = new Response();
            Func<NancyContext, Response> preHook = (ctx) => preResponse;
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, preHook, null));

            var pipelines = new Pipelines();

            var localEngine =
                new NancyEngine(prePostResolver, contextFactory, new IErrorHandler[] { this.errorHandler }, A.Fake<IRequestTracing>(), this.routeInvoker)
                {
                    RequestPipelinesFactory = ctx => pipelines
                };

            var request = new Request("GET", "/", "http");

            // When
            var result = localEngine.HandleRequest(request);

            // Then
            result.Response.ShouldBeSameAs(preResponse);
        }

        [Fact]
        public void HandleRequest_should_allow_route_postreq_to_change_response()
        {
            // Given
            var postResponse = new Response();
            Action<NancyContext> postHook = (ctx) => ctx.Response = postResponse;
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, null, postHook));

            var pipelines = new Pipelines();

            var localEngine =
                new NancyEngine(prePostResolver, contextFactory, new IErrorHandler[] { this.errorHandler }, A.Fake<IRequestTracing>(), this.routeInvoker)
                {
                    RequestPipelinesFactory = ctx => pipelines
                };

            var request = new Request("GET", "/", "http");

            // When
            var result = localEngine.HandleRequest(request);

            // Then
            result.Response.ShouldBeSameAs(postResponse);
        }

        [Fact]
        public void HandleRequest_should_allow_route_postreq_to_add_items_to_context()
        {
            // Given
            Action<NancyContext> postHook = (ctx) => ctx.Items.Add("RoutePostReq", new object());
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, null, postHook));

            var pipelines = new Pipelines();

            var localEngine =
                new NancyEngine(prePostResolver, contextFactory, new IErrorHandler[] { this.errorHandler }, A.Fake<IRequestTracing>(), this.routeInvoker)
                {
                    RequestPipelinesFactory = ctx => pipelines
                };

            var request = new Request("GET", "/", "http");

            // When
            var result = localEngine.HandleRequest(request);

            // Then
            result.Items.ContainsKey("RoutePostReq").ShouldBeTrue();
        }

        [Fact]
        public void HandleRequest_prereq_returns_response_should_still_run_postreq()
        {
            // Given
            var returnedResponse = A.Fake<Response>();
            var postReqCalled = false;

            var pipelines = new Pipelines();
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx => returnedResponse);
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx => postReqCalled = true);

            engine.RequestPipelinesFactory = (ctx) => pipelines;

            var request = new Request("GET", "/", "http");

            // When
            this.engine.HandleRequest(request);

            // Then
            postReqCalled.ShouldBeTrue();
        }

        [Fact]
        public void HandleRequest_route_prereq_returns_response_should_still_run_route_postreq_and_postreq()
        {
            // Given
            var executionOrder = new List<String>();
            Action<NancyContext> postHook = (ctx) => executionOrder.Add("Posthook");
            Func<NancyContext, Response> routePreHook = (ctx) => { executionOrder.Add("Routeprehook"); return new Response(); };
            Action<NancyContext> routePostHook = (ctx) => executionOrder.Add("Routeposthook");
            this.route.Action = (d) => { executionOrder.Add("RouteInvoke"); return null; };
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, routePreHook, routePostHook));

            var pipelines = new Pipelines();
            pipelines.AfterRequest.AddItemToStartOfPipeline(postHook);

            var localEngine =
                new NancyEngine(prePostResolver, contextFactory, new IErrorHandler[] { this.errorHandler }, A.Fake<IRequestTracing>(), this.routeInvoker)
                {
                    RequestPipelinesFactory = ctx => pipelines
                };

            var request = new Request("GET", "/", "http");

            // When
            localEngine.HandleRequest(request);

            // Then
            executionOrder.Count().ShouldEqual(3);
            executionOrder.SequenceEqual(new[] { "Routeprehook", "Routeposthook", "Posthook" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_ask_error_handler_if_it_can_handle_status_code()
        {
            // Given
            var request = new Request("GET", "/", "http");


            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.errorHandler.HandlesStatusCode(A<HttpStatusCode>.Ignored, A<NancyContext>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_not_invoke_error_handler_if_not_supported_status_code()
        {
            // Given
            var request = new Request("GET", "/", "http");

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.errorHandler.Handle(A<HttpStatusCode>.Ignored, A<NancyContext>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void Should_invoke_error_handler_if_supported_status_code()
        {
            // Given
            var request = new Request("GET", "/", "http");
            A.CallTo(() => this.errorHandler.HandlesStatusCode(A<HttpStatusCode>.Ignored, A<NancyContext>.Ignored)).Returns(true);

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.errorHandler.Handle(A<HttpStatusCode>.Ignored, A<NancyContext>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_set_status_code_to_500_if_route_throws()
        {
            // Given
            var errorRoute = new Route("GET", "/", null, x => { throw new NotImplementedException(); });
            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(new ResolveResult(errorRoute, DynamicDictionary.Empty, null, null));
            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.Response.StatusCode.ShouldEqual(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public void Should_store_exception_details_if_route_throws()
        {
            // Given
            var errorRoute = new Route("GET", "/", null, x => { throw new NotImplementedException(); });
            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(new ResolveResult(errorRoute, DynamicDictionary.Empty, null, null));
            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.GetExceptionDetails().ShouldContain("NotImplementedException");
        }

        [Fact]
        public void Should_set_the_route_parameters_to_the_nancy_context_before_calling_the_module_before()
        {
            // Given
            dynamic parameters = new DynamicDictionary();
            parameters.Foo = "Bar";
            Func<NancyContext, Response> moduleBefore = (ctx) => { Assert.Equal(this.context.Parameters, parameters); return null; };
            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(new ResolveResult(route, parameters, moduleBefore, null));
            var request = new Request("GET", "/", "http");

            // When
            engine.HandleRequest(request);

            // Then
            Assert.Equal(this.context.Parameters, parameters);
        }

        [Fact]
        public void Should_invoke_the_error_request_hook_if_one_exists_when_route_throws()
        {
            // Given
            var testEx = new Exception();
            var errorRoute = new Route("GET", "/", null, x => { throw testEx; });

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(new ResolveResult(errorRoute, DynamicDictionary.Empty, null, null));

            Exception handledException = null;
            NancyContext handledContext = null;
            var errorResponse = new Response();

            Func<NancyContext, Exception, Response> routeErrorHook = (ctx, ex) =>
            {
                handledContext = ctx;
                handledException = ex;
                return errorResponse;
            };

            var pipelines = new Pipelines();
            pipelines.OnError.AddItemToStartOfPipeline(routeErrorHook);

            engine.RequestPipelinesFactory = (ctx) => pipelines;

            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            Assert.Equal(testEx, handledException);
            Assert.Equal(result, handledContext);
            Assert.Equal(result.Response, errorResponse);
        }

        [Fact]
        public void Should_add_unhandled_exception_to_context_as_requestexecutionexception()
        {
            // Given
            var routeUnderTest =
                new Route("GET", "/", null, x => { throw new Exception(); });

            var resolved =
                new ResolveResult(routeUnderTest, DynamicDictionary.Empty, null, null);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(resolved);

            A.CallTo(() => this.routeInvoker.Invoke(A<Route>._, A<DynamicDictionary>._)).Invokes((x) =>
            {
                routeUnderTest.Action.Invoke(DynamicDictionary.Empty);
            });

            var pipelines = new Pipelines();
            pipelines.OnError.AddItemToStartOfPipeline((ctx, exception) => null);
            engine.RequestPipelinesFactory = (ctx) => pipelines;

            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.Items.Keys.Contains("ERROR_EXCEPTION").ShouldBeTrue();
            result.Items["ERROR_EXCEPTION"].ShouldBeOfType<RequestExecutionException>();
        }

        [Fact]
        public void Should_persist_original_exception_in_requestexecutionexception()
        {
            // Given
            var expectedException = new Exception();

            var routeUnderTest =
                new Route("GET", "/", null, x => { throw expectedException; });

            var resolved =
                new ResolveResult(routeUnderTest, DynamicDictionary.Empty, null, null);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(resolved);

            var pipelines = new Pipelines();
            pipelines.OnError.AddItemToStartOfPipeline((ctx, exception) => null);
            engine.RequestPipelinesFactory = (ctx) => pipelines;

            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);
            var returnedException = result.Items["ERROR_EXCEPTION"] as RequestExecutionException;

            // Then
            returnedException.InnerException.ShouldBeSameAs(expectedException);
        }

        [Fact]
        public void Should_add_requestexecutionexception_to_context_when_pipeline_is_null()
        {
            // Given
            var routeUnderTest =
                new Route("GET", "/", null, x => { throw new Exception(); });

            var resolved =
                new ResolveResult(routeUnderTest, DynamicDictionary.Empty, null, null);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(resolved);

            var pipelines = new Pipelines { OnError = null };
            engine.RequestPipelinesFactory = (ctx) => pipelines;

            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.Items.Keys.Contains("ERROR_EXCEPTION").ShouldBeTrue();
            result.Items["ERROR_EXCEPTION"].ShouldBeOfType<RequestExecutionException>();
        }

        [Fact]
        public void Should_persist_original_exception_in_requestexecutionexception_when_pipeline_is_null()
        {
            // Given
            var expectedException = new Exception();

            var routeUnderTest =
                new Route("GET", "/", null, x => { throw expectedException; });

            var resolved =
                new ResolveResult(routeUnderTest, DynamicDictionary.Empty, null, null);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(resolved);

            var pipelines = new Pipelines { OnError = null };
            engine.RequestPipelinesFactory = (ctx) => pipelines;

            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);
            var returnedException = result.Items["ERROR_EXCEPTION"] as RequestExecutionException;

            // Then
            returnedException.InnerException.ShouldBeSameAs(expectedException);
        }
    }
}
