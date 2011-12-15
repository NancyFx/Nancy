namespace Nancy.Tests.Unit.Routing
{
    using System.Diagnostics;
    using FakeItEasy;
    using Fakes;
    using Testing;
    using Xunit;

    public class DefaultRouteResolverPerformanceFixture
    {
        private readonly int numberOfTimesToResolveRoute;

        public DefaultRouteResolverPerformanceFixture()
        {
            this.numberOfTimesToResolveRoute = 10000;
        }

        //[Fact(Skip = "This is a performance test")]
        [Fact]
        public void Should_fail_to_resolve_route_because_it_does_have_an_invalid_condition()
        {
            // Given
            var cache = new FakeRouteCache(with => {
                with.AddGetRoute("/invalidcondition", "modulekey", ctx => false);
            });

            var bootstrapper = new ConfigurableBootstrapper(with =>{
                with.RouteCache(cache);
            });

            var browser = new Browser(bootstrapper);

            // When
            var timer = new Stopwatch();
            timer.Start();

            for (var i = 0; i < numberOfTimesToResolveRoute; i++)
            {
                var result = browser.Get("/invalidcondition");
                result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }

            timer.Stop();

            // Then
            Debug.WriteLine(" took {0} to execute {1} times", timer.Elapsed, numberOfTimesToResolveRoute);
        }

        //[Fact(Skip = "This is a performance test")]
        [Fact]
        public void Should_not_find_exact_path()
        {
            // Given
            var cache = new FakeRouteCache(with => {
                with.AddGetRoute("/somepath", "modulekey");
            });

            var bootstrapper = new ConfigurableBootstrapper(with => {
                with.RouteCache(cache);
            });

            var browser = new Browser(bootstrapper);

            // When
            var timer = new Stopwatch();
            timer.Start();

            for (var i = 0; i < numberOfTimesToResolveRoute; i++)
            {
                var result = browser.Get("/nomatchpath");
                result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }

            timer.Stop();

            // Then
            Debug.WriteLine(" took {0} to execute {1} times", timer.Elapsed, numberOfTimesToResolveRoute);
        }

        //[Fact(Skip = "This is a performance test")]
        [Fact]
        public void Should_not_find_matching_method()
        {
            // Given
            var cache = new FakeRouteCache(with =>
            {
                with.AddGetRoute("/path", "modulekey");
            });

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.RouteCache(cache);
            });

            var browser = new Browser(bootstrapper);

            // When
            var timer = new Stopwatch();
            timer.Start();

            for (var i = 0; i < numberOfTimesToResolveRoute; i++)
            {
                var result = browser.Post("/path");
                result.StatusCode.ShouldEqual(HttpStatusCode.MethodNotAllowed);
            }

            timer.Stop();

            // Then
            Debug.WriteLine(" took {0} to execute {1} times", timer.Elapsed, numberOfTimesToResolveRoute);
        }

        //[Fact(Skip = "This is a performance test")]
        [Fact]
        public void Should_find_exact_match()
        {
            // Given
            var catalog = A.Fake<INancyModuleCatalog>();
            A.CallTo(() => catalog.GetModuleByKey(A<string>._, A<NancyContext>._)).Returns(new FakeNancyModule());

            var cache = new FakeRouteCache(with =>
            {
                with.AddGetRoute("/foo", "modulekey");
            });

            var module = new FakeNancyModule(with => {
                with.AddGetRoute("/foo");
            });

            var bootstrapper = new ConfigurableBootstrapper(with =>{
                with.Module(module, "modulekey");
                with.RouteCache(cache);
            });

            var browser = new Browser(bootstrapper);

            // When
            var timer = new Stopwatch();
            timer.Start();

            for (var i = 0; i < numberOfTimesToResolveRoute; i++)
            {
                var result = browser.Get("/foo");
                result.StatusCode.ShouldEqual(HttpStatusCode.OK);
            }

            timer.Stop();

            // Then
            Debug.WriteLine(" took {0} to execute {1} times", timer.Elapsed, numberOfTimesToResolveRoute);
        }

        //[Fact(Skip = "This is a performance test")]
        [Fact]
        public void Should_select_top_match()
        {
            // Given
            var catalog = A.Fake<INancyModuleCatalog>();
            A.CallTo(() => catalog.GetModuleByKey(A<string>._, A<NancyContext>._)).Returns(new FakeNancyModule());

            var cache = new FakeRouteCache(with =>
            {
                with.AddGetRoute("/foo/{x}", "modulekey");
                with.AddGetRoute("/foo/{x}/{y}", "modulekey");
            });

            var module = new FakeNancyModule(with =>
            {
                with.AddGetRoute("/foo/{x}");
                with.AddGetRoute("/foo/{x}/{y}");
            });

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module(module, "modulekey");
                with.RouteCache(cache);
            });

            var browser = new Browser(bootstrapper);

            // When
            var timer = new Stopwatch();
            timer.Start();

            for (var i = 0; i < numberOfTimesToResolveRoute; i++)
            {
                var result = browser.Get("/foo/test/bar");
                result.StatusCode.ShouldEqual(HttpStatusCode.OK);
            }

            timer.Stop();

            // Then
            Debug.WriteLine(" took {0} to execute {1} times", timer.Elapsed, numberOfTimesToResolveRoute);
        }
    }
}