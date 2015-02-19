namespace Nancy.Tests.Unit.Routing
{
    using System.Threading;

    using FakeItEasy;

    using Nancy.Diagnostics;
    using Nancy.ErrorHandling;
    using Nancy.Responses.Negotiation;
    using Nancy.Routing;
    using Nancy.Tests.Fakes;

    using Xunit;

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
        public void Should_invoke_response_negotiator_for_reference_model()
        {
            // Given
            var model = new Person { FirstName = "First", LastName = "Last" };
            var route = new FakeRoute(model);
            var parameters = new DynamicDictionary();
            var context = new NancyContext
            {
                Trace = new DefaultRequestTrace()
            };

            // When
            var result = this.invoker.Invoke(route, new CancellationToken(), parameters, context).Result;

            // Then
            A.CallTo(() => this.responseNegotiator.NegotiateResponse(model, context)).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_response_negotiator_for_value_type_model()
        {
            // Given
            var model = new StructModel();
            var route = new FakeRoute(model);
            var parameters = new DynamicDictionary();
            var context = new NancyContext
            {
                Trace = new DefaultRequestTrace()
            };

            // When
            var result = this.invoker.Invoke(route, new CancellationToken(), parameters, context).Result;

            // Then
            A.CallTo(() => this.responseNegotiator.NegotiateResponse(model, context)).MustHaveHappened();
        }
    }
}
