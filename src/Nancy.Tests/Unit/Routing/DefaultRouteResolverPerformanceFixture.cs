namespace Nancy.Tests.Unit.Routing
{
    using System;
    using System.Diagnostics;
    using System.Timers;
    using Fakes;
    using Nancy.Routing;
    using Testing;
    using Xunit;

    public class DefaultRouteResolverPerformanceFixture
    {
        private Browser browser;

        public DefaultRouteResolverPerformanceFixture()
        {
            var bootstrapper = new ConfigurableBootstrapper(with => {
                with.Module<DefaultRouteResolverPerformanceModule>();
            });
            
            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_expectation()
        {
            // Given
            // When
            var timer = new Stopwatch();
            timer.Start();

            for (var i = 0; i < 10000; i++ )
            {
                var result = browser.Get("/");
                result.StatusCode.ShouldEqual(HttpStatusCode.OK);
            }

            timer.Stop();
            // Then
            
            Debug.WriteLine(" took {0} to execute 1000 times", timer.Elapsed);
        }

        [Fact]
        public void Should_2()
        {
            // Given
            // When
            var timer = new Stopwatch();
            timer.Start();

            for (var i = 0; i < 10000; i++)
            {
                var result = browser.Get("/willbecalledwithget");
                result.StatusCode.ShouldEqual(HttpStatusCode.MethodNotAllowed);
            }

            timer.Stop();
            // Then

            Debug.WriteLine(" took {0} to execute 1000 times", timer.Elapsed);
        }
    }

    public class DefaultRouteResolverPerformanceModule : NancyModule
    {
        public DefaultRouteResolverPerformanceModule()
        {
            Get["/"] = parameters => {
                return HttpStatusCode.OK;
            };

            Get["/multiplemethods"] = parameters => {
                return HttpStatusCode.OK;
            };

            Post["/multiplemethods"] = parameters => {
                return HttpStatusCode.OK;
            };

            Post["/willbecalledwithget"] = parameters => {
                return HttpStatusCode.OK;
            };
        }
    }
}