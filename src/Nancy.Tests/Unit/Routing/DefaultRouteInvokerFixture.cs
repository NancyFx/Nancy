namespace Nancy.Tests.Unit.Routing
{
    using System;
    using System.IO;
    using System.Threading;

    using FakeItEasy;

    using Nancy.Diagnostics;
    using Nancy.ErrorHandling;
    using Nancy.Responses.Negotiation;
    using Nancy.Routing;
    using Nancy.Tests.Fakes;

    using Xunit;
    using Xunit.Extensions;

    public class DefaultRouteInvokerFixture
    {
        private readonly DefaultRouteInvoker invoker;
        private readonly IResponseNegotiator responseNegotiator;

        public DefaultRouteInvokerFixture()
        {
            this.responseNegotiator = A.Fake<IResponseNegotiator>();
            this.invoker = new DefaultRouteInvoker(responseNegotiator);
        }

        [Fact]
        public void Should_invoke_route_with_provided_parameters()
        {
            // Given
            var parameters = new DynamicDictionary();
            var route = new FakeRoute(10);
            var context = new NancyContext
            {
                Trace = new DefaultRequestTrace
                {
                    TraceLog = new DefaultTraceLog()
                }
            };

            // When
            this.invoker.Invoke(route, new CancellationToken(), parameters, context);

            // Then
            Assert.Same(route.ParametersUsedToInvokeAction, parameters);
        }

        [Fact]
        public void Should_return_response_when_route_returns_int()
        {
            // Given
            var parameters = new DynamicDictionary();
            var route = new FakeRoute(10);
            var context = new NancyContext
            {
                Trace = new DefaultRequestTrace
                {
                    TraceLog = new DefaultTraceLog()
                }
            };

            // When
            var result = this.invoker.Invoke(route, new CancellationToken(), parameters, context).Result;

            // Then
            Assert.IsType<Response>(result);
        }

        [Fact]
        public void Should_return_response_when_route_returns_string()
        {
            // Given
            var parameters = new DynamicDictionary();
            var route = new FakeRoute("Hello World");
            var context = new NancyContext
            {
                Trace = new DefaultRequestTrace
                {
                    TraceLog = new DefaultTraceLog()
                }
            };

            // When
            var result = this.invoker.Invoke(route, new CancellationToken(), parameters, context).Result;

            // Then
            Assert.IsType<Response>(result);
        }

        [Fact]
        public void Should_return_response_when_route_returns_status_code()
        {
            // Given
            var parameters = new DynamicDictionary();
            var route = new FakeRoute(HttpStatusCode.OK);
            var context = new NancyContext
            {
                Trace = new DefaultRequestTrace
                {
                    TraceLog = new DefaultTraceLog()
                }
            };

            // When
            var result = this.invoker.Invoke(route, new CancellationToken(), parameters, context).Result;

            // Then
            Assert.IsType<Response>(result);
        }

        [Fact]
        public void Should_return_response_when_route_returns_action()
        {
            // Given
            Action<Stream> action = s => { };
            var parameters = new DynamicDictionary();
            var route = new FakeRoute(action);
            var context = new NancyContext
            {
                Trace = new DefaultRequestTrace
                {
                    TraceLog = new DefaultTraceLog()
                }
            };

            // When
            var result = this.invoker.Invoke(route, new CancellationToken(), parameters, context).Result;

            // Then
            Assert.IsType<Response>(result);
        }

        [Fact]
        public void Should_handle_RouteExecutionEarlyExitException_gracefully()
        {
            // Given
            var response = new Response();
            var route = new FakeRoute((c, t) => { throw new RouteExecutionEarlyExitException(response); });
            var parameters = new DynamicDictionary();
            var context = new NancyContext
            {
                Trace = new DefaultRequestTrace
                {
                    TraceLog = new DefaultTraceLog()
                }
            };

            // When
            var result = this.invoker.Invoke(route, new CancellationToken(), parameters, context).Result;

            // Then
            result.ShouldBeSameAs(response);
        }

        [Fact]
        public void Should_log_the_reason_for_early_exits()
        {
            // Given
            var response = new Response();
            var route = new FakeRoute((c, t) => { throw new RouteExecutionEarlyExitException(response, "Reason Testing"); });
            var parameters = new DynamicDictionary();
            var context = new NancyContext
            {
                Trace = new DefaultRequestTrace
                {
                    TraceLog = new DefaultTraceLog()
                }
            };

            // When
            var result = this.invoker.Invoke(route, new CancellationToken(), parameters, context).Result;

            // Then
            context.Trace.TraceLog.ToString().ShouldContain("Reason Testing");
        }

        [Fact]
        public void Should_invoke_response_negotiator_if_route_result_is_not_response()
        {
            // Given
            var model = new Person { FirstName = "First", LastName = "Last" };
            var route = new FakeRoute(model);
            var parameters = new DynamicDictionary();
            var context = new NancyContext { Trace = new RequestTrace(true) };

            // When
            var result = this.invoker.Invoke(route, new CancellationToken(), parameters, context).Result;

            // Then
            A.CallTo(() => this.responseNegotiator.NegotiateResponse(model, context)).MustHaveHappened();
        }

        [Theory]
        [InlineData(200)]
        [InlineData("Hello")]
        [InlineData(HttpStatusCode.OK)]
        public void Should_not_invoke_response_negotiator_if_route_result_can_be_converted_to_response(object response)
        {
            // Given
            var route = new FakeRoute(response);
            var parameters = new DynamicDictionary();
            var context = new NancyContext { Trace = new RequestTrace(true) };

            // When
            var result = this.invoker.Invoke(route, new CancellationToken(), parameters, context).Result;

            // Then
            A.CallTo(() => this.responseNegotiator.NegotiateResponse(A<object>.Ignored, context)).MustNotHaveHappened();
        }
    }
}