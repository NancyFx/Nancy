namespace Nancy.Tests.Unit
{
    using System;
    using FakeItEasy;
    using Nancy.Bootstrapper;
    using Nancy.Diagnostics;
    using Nancy.ErrorHandling;
    using Nancy.Extensions;
    using Nancy.Routing;
    using Nancy.Tests.Fakes;
    using Xunit;
    using ResolveResult = System.Tuple<Nancy.Routing.Route, DynamicDictionary, System.Func<NancyContext, Response>, System.Action<NancyContext>, System.Func<NancyContext, System.Exception, Response>>;

    public class NancyEngineFixture
    {
        private readonly INancyEngine engine;
        private readonly IRouteResolver resolver;
        private readonly FakeRoute route;
        private readonly NancyContext context;
        private readonly INancyContextFactory contextFactory;
        private readonly Response response;
        private readonly IStatusCodeHandler statusCodeHandler;
        private readonly IRouteInvoker routeInvoker;
        private readonly IRequestDispatcher requestDispatcher;

        public NancyEngineFixture()
        {
            this.resolver = A.Fake<IRouteResolver>();
            this.response = new Response();
            this.route = new FakeRoute(response);
            this.context = new NancyContext();
            this.statusCodeHandler = A.Fake<IStatusCodeHandler>();
            this.requestDispatcher = A.Fake<IRequestDispatcher>();

            A.CallTo(() => this.requestDispatcher.Dispatch(A<NancyContext>._)).Invokes(x => this.context.Response = new Response());

            A.CallTo(() => this.statusCodeHandler.HandlesStatusCode(A<HttpStatusCode>.Ignored, A<NancyContext>.Ignored)).Returns(false);

            contextFactory = A.Fake<INancyContextFactory>();
            A.CallTo(() => contextFactory.Create()).Returns(context);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, null, null, null));

            var applicationPipelines = new Pipelines();

            this.routeInvoker = A.Fake<IRouteInvoker>();

            A.CallTo(() => this.routeInvoker.Invoke(A<Route>._, A<DynamicDictionary>._, A<NancyContext>._)).ReturnsLazily(arg =>
            {
                return (Response)((Route)arg.Arguments[0]).Action.Invoke((DynamicDictionary)arg.Arguments[1]);
            });

            this.engine =
                new NancyEngine(this.requestDispatcher, contextFactory, new[] { this.statusCodeHandler }, A.Fake<IRequestTracing>())
                {
                    RequestPipelinesFactory = ctx => applicationPipelines
                };
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_dispatcher()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(null, A.Fake<INancyContextFactory>(), new[] { this.statusCodeHandler }, A.Fake<IRequestTracing>()));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_context_factory()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(this.requestDispatcher, null, new[] { this.statusCodeHandler }, A.Fake<IRequestTracing>()));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_status_handler()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(this.requestDispatcher, A.Fake<INancyContextFactory>(), null, A.Fake<IRequestTracing>()));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
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

            A.CallTo(() => this.requestDispatcher.Dispatch(this.context)).Invokes(x => this.context.Response = this.response);

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
        public void Should_ask_status_handler_if_it_can_handle_status_code()
        {
            // Given
            var request = new Request("GET", "/", "http");

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.statusCodeHandler.HandlesStatusCode(A<HttpStatusCode>.Ignored, A<NancyContext>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_not_invoke_status_handler_if_not_supported_status_code()
        {
            // Given
            var request = new Request("GET", "/", "http");

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.statusCodeHandler.Handle(A<HttpStatusCode>.Ignored, A<NancyContext>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void Should_invoke_status_handler_if_supported_status_code()
        {
            // Given
            var request = new Request("GET", "/", "http");
            A.CallTo(() => this.statusCodeHandler.HandlesStatusCode(A<HttpStatusCode>.Ignored, A<NancyContext>.Ignored)).Returns(true);

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.statusCodeHandler.Handle(A<HttpStatusCode>.Ignored, A<NancyContext>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_set_status_code_to_500_if_route_throws()
        {
            // Given
            var resolvedRoute = new ResolveResult(
                new FakeRoute(),
                DynamicDictionary.Empty,
                null,
                null,
                null);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(resolvedRoute);

            A.CallTo(() => this.requestDispatcher.Dispatch(context)).Throws(new NotImplementedException());

            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.Response.StatusCode.ShouldEqual(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public void Should_store_exception_details_if_dispatcher_throws()
        {
            // Given
            var resolvedRoute = new ResolveResult(
                new FakeRoute(),
                DynamicDictionary.Empty,
                null,
                null,
                null);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(resolvedRoute);
            
            A.CallTo(() => this.requestDispatcher.Dispatch(context)).Throws(new NotImplementedException());

            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.GetExceptionDetails().ShouldContain("NotImplementedException");
        }

        [Fact]
        public void Should_invoke_the_error_request_hook_if_one_exists_when_dispatcher_throws()
        {
            // Given
            var testEx = new Exception();
            
            var errorRoute = 
                new Route("GET", "/", null, x => { throw testEx; });

            var resolvedRoute = new ResolveResult(
                errorRoute,
                DynamicDictionary.Empty,
                null,
                null,
                null);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(resolvedRoute);

            A.CallTo(() => this.requestDispatcher.Dispatch(context)).Throws(testEx);

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
                new ResolveResult(routeUnderTest, DynamicDictionary.Empty, null, null, null);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(resolved);

            A.CallTo(() => this.routeInvoker.Invoke(A<Route>._, A<DynamicDictionary>._, A<NancyContext>._)).Invokes((x) =>
            {
                routeUnderTest.Action.Invoke(DynamicDictionary.Empty);
            });

            A.CallTo(() => this.requestDispatcher.Dispatch(context)).Throws(new Exception());

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

            var resolvedRoute = new ResolveResult(
               new FakeRoute(),
               DynamicDictionary.Empty,
               null,
               null,
               null);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(resolvedRoute);

            A.CallTo(() => this.requestDispatcher.Dispatch(context)).Throws(expectedException);

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
            var resolvedRoute = new ResolveResult(
               new FakeRoute(),
               DynamicDictionary.Empty,
               null,
               null,
               null);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(resolvedRoute);

            A.CallTo(() => this.requestDispatcher.Dispatch(context)).Throws(new Exception());

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

            var resolvedRoute = new ResolveResult(
                new FakeRoute(), 
                DynamicDictionary.Empty,
                null,
                null,
                null);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored)).Returns(resolvedRoute);

            A.CallTo(() => this.requestDispatcher.Dispatch(context)).Throws(expectedException);

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