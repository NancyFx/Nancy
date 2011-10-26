namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using Nancy.Bootstrapper;
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

        public NancyEngineFixture()
        {
            this.resolver = A.Fake<IRouteResolver>();
            this.response = new Response();
            this.route = new FakeRoute(response);
            this.context = new NancyContext();
            this.errorHandler = A.Fake<IErrorHandler>();

            A.CallTo(() => errorHandler.HandlesStatusCode(A<HttpStatusCode>.Ignored)).Returns(false);

            contextFactory = A.Fake<INancyContextFactory>();
            A.CallTo(() => contextFactory.Create()).Returns(context);

            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, null, null));

            var applicationPipelines = new Pipelines();

            this.engine = 
                new NancyEngine(resolver, A.Fake<IRouteCache>(), contextFactory, new IErrorHandler[] { this.errorHandler })
                {
                    RequestPipelinesFactory = ctx => applicationPipelines
                };
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_resolver()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(null, A.Fake<IRouteCache>(), A.Fake<INancyContextFactory>(), new IErrorHandler[] { this.errorHandler }));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_routecache()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(A.Fake<IRouteResolver>(), null, A.Fake<INancyContextFactory>(), new IErrorHandler[] { this.errorHandler }));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_context_factory()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(A.Fake<IRouteResolver>(), A.Fake<IRouteCache>(), null, new IErrorHandler[] { this.errorHandler }));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_error_handler()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(A.Fake<IRouteResolver>(), A.Fake<IRouteCache>(), A.Fake<INancyContextFactory>(), null));

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

            A.CallTo(() => this.contextFactory.Create()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void HandleRequest_should_set_correct_response_on_returned_context()
        {
            var request = new Request("GET", "/", "http");

            var result = this.engine.HandleRequest(request);

            result.Response.ShouldBeSameAs(this.response);
        }

        [Fact]
        public void Should_add_nancy_version_number_header_on_returned_response()
        {
            // Given
            var request = new Request("GET", "/", "http");

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.Response.Headers.ContainsKey("Nancy-Version").ShouldBeTrue();
        }

        [Fact]
        public void Should_not_throw_exception_when_setting_nancy_version_header_and_it_already_existed()
        {
            // Given
            var cachedResponse = new Response();
            cachedResponse.Headers.Add("Nancy-Version", "1.2.3.4");
            Func<NancyContext, Response> preRequestHook = (ctx) => cachedResponse;

            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, preRequestHook, null));

            var pipelines = new Pipelines();

            var localEngine =
                new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory, new IErrorHandler[] { this.errorHandler })
                {
                    RequestPipelinesFactory = ctx => pipelines
                };

            var request = new Request("GET", "/", "http");

            // When
            var exception = Record.Exception(() => localEngine.HandleRequest(request));

            // Then
            exception.ShouldBeNull();
        }

        [Fact]
        public void Should_set_nancy_version_number_on_returned_response()
        {
            // Given
            var request = new Request("GET", "/", "http");
            var nancyVersion = typeof(INancyEngine).Assembly.GetName().Version;

            // When
            var result = this.engine.HandleRequest(request);

            // Then
            result.Response.Headers["Nancy-Version"].ShouldEqual(nancyVersion.ToString());
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
            Action<NancyContext> postHook = (ctx) => { executionOrder.Add("Posthook"); };
            
            this.route.Action = (d) => { executionOrder.Add("RouteInvoke"); return null; };
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, preHook, postHook));

            var pipelines = new Pipelines();

            var localEngine =
                new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory, new IErrorHandler[] { this.errorHandler })
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
            var executionOrder = new List<String>();
            Func<NancyContext, Response> preHook = (ctx) => { executionOrder.Add("Prehook"); return new Response(); };
            Action<NancyContext> postHook = (ctx) => { executionOrder.Add("Posthook"); };
            this.route.Action = (d) => { executionOrder.Add("RouteInvoke"); return null; };
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, preHook, postHook));

            var pipelines = new Pipelines();

            var localEngine =
                new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory, new IErrorHandler[] { this.errorHandler })
                {
                    RequestPipelinesFactory = ctx => pipelines
                };

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
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, preHook, null));

            var pipelines = new Pipelines();

            var localEngine =
                new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory, new IErrorHandler[] { this.errorHandler })
                {
                    RequestPipelinesFactory = ctx => pipelines
                };

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
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, null, postHook));

            var pipelines = new Pipelines();

            var localEngine =
                new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory, new IErrorHandler[] { this.errorHandler })
                {
                    RequestPipelinesFactory = ctx => pipelines
                };

            var request = new Request("GET", "/", "http");

            var result = localEngine.HandleRequest(request);

            result.Response.ShouldBeSameAs(postResponse);
        }

        [Fact]
        public void HandleRequest_should_allow_route_postreq_to_add_items_to_context()
        {
            Action<NancyContext> postHook = (ctx) => ctx.Items.Add("RoutePostReq", new object());
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, null, postHook));

            var pipelines = new Pipelines();

            var localEngine =
                new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory, new IErrorHandler[] { this.errorHandler })
                {
                    RequestPipelinesFactory = ctx => pipelines
                };

            var request = new Request("GET", "/", "http");

            var result = localEngine.HandleRequest(request);

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
            var executionOrder = new List<String>();
            Action<NancyContext> postHook = (ctx) => { executionOrder.Add("Posthook"); };
            Func<NancyContext, Response> routePreHook = (ctx) => { executionOrder.Add("Routeprehook"); return new Response(); };
            Action<NancyContext> routePostHook = (ctx) => { executionOrder.Add("Routeposthook"); };
            this.route.Action = (d) => { executionOrder.Add("RouteInvoke"); return null; };
            var prePostResolver = A.Fake<IRouteResolver>();
            A.CallTo(() => prePostResolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored)).Returns(new ResolveResult(route, DynamicDictionary.Empty, routePreHook, routePostHook));

            var pipelines = new Pipelines();
            pipelines.AfterRequest.AddItemToStartOfPipeline(postHook);

            var localEngine =
                new NancyEngine(prePostResolver, A.Fake<IRouteCache>(), contextFactory, new IErrorHandler[] { this.errorHandler })
                {
                    RequestPipelinesFactory = ctx => pipelines
                };

            var request = new Request("GET", "/", "http");

            localEngine.HandleRequest(request);

            executionOrder.Count().ShouldEqual(3);
            executionOrder.SequenceEqual(new[] { "Routeprehook", "Routeposthook", "Posthook" }).ShouldBeTrue();
        }

        [Fact]
        public void Should_ask_error_handler_if_it_can_handle_status_code()
        {
            var request = new Request("GET", "/", "http");

            this.engine.HandleRequest(request);

            A.CallTo(() => this.errorHandler.HandlesStatusCode(A<HttpStatusCode>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_not_invoke_error_handler_if_not_supported_status_code()
        {
            var request = new Request("GET", "/", "http");

            this.engine.HandleRequest(request);

            A.CallTo(() => this.errorHandler.Handle(A<HttpStatusCode>.Ignored, A<NancyContext>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void Should_invoke_error_handler_if_supported_status_code()
        {
            var request = new Request("GET", "/", "http");
            A.CallTo(() => this.errorHandler.HandlesStatusCode(A<HttpStatusCode>.Ignored)).Returns(true);

            this.engine.HandleRequest(request);

            A.CallTo(() => this.errorHandler.Handle(A<HttpStatusCode>.Ignored, A<NancyContext>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_set_status_code_to_500_if_route_throws()
        {
            var errorRoute = new Route("GET", "/", null, x => { throw new NotImplementedException(); });
            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored)).Returns(new ResolveResult(errorRoute, DynamicDictionary.Empty, null, null));
            var request = new Request("GET", "/", "http");

            var result = this.engine.HandleRequest(request);

            result.Response.StatusCode.ShouldEqual(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public void Should_store_exception_details_if_route_throws()
        {
            var errorRoute = new Route("GET", "/", null, x => { throw new NotImplementedException(); });
            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored)).Returns(new ResolveResult(errorRoute, DynamicDictionary.Empty, null, null));
            var request = new Request("GET", "/", "http");

            var result = this.engine.HandleRequest(request);

            result.GetExceptionDetails().ShouldContain("NotImplementedException");
        }

        [Fact]
        public void Should_set_the_route_parameters_to_the_nancy_context_before_calling_the_module_before()
        {
            // Given
            dynamic parameters = new DynamicDictionary();
            parameters.Foo = "Bar";
            Func<NancyContext, Response> moduleBefore = (ctx) =>  { Assert.Equal(this.context.Parameters, parameters); return null; };
            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored)).Returns(new ResolveResult(route, parameters, moduleBefore, null));
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
            
            A.CallTo(() => resolver.Resolve(A<NancyContext>.Ignored, A<IRouteCache>.Ignored)).Returns(new ResolveResult(errorRoute, DynamicDictionary.Empty, null, null));
            
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
    }
}
