namespace Nancy.Tests.Unit.Routing
{
    using Nancy.Testing;
    using Nancy.Tests;
    using Xunit;

    public class DefaultRouteResolverFixture
    {
        private readonly Browser browser;

        public DefaultRouteResolverFixture()
        {
            this.browser = new Browser(with => with.Module<TestModule>());
        }

        [Fact]
        public void Should_resolve_root()
        {
            var result = this.browser.Get("/");

            result.Body.AsString().ShouldEqual("Root");
        }

        [Fact]
        public void Should_resolve_correct_route_based_on_method()
        {
            var result = this.browser.Post("/");

            result.Body.AsString().ShouldEqual("PostRoot");
        }

        [Fact]
        public void Should_resolve_single_literal()
        {
            var result = this.browser.Get("/foo");

            result.Body.AsString().ShouldEqual("SingleLiteral");
        }

        [Fact]
        public void Should_resolve_multi_literal()
        {
            var result = this.browser.Get("/foo/bar/baz");

            result.Body.AsString().ShouldEqual("MultipleLiteral");
        }

        [Fact]
        public void Should_resolve_single_capture()
        {
            var result = this.browser.Get("/foo/testing/plop");

            result.Body.AsString().ShouldEqual("Captured testing");
        }

        [Fact]
        public void Should_resolve_optional_capture_with_optional_specified()
        {
            var result = this.browser.Get("/moo/hoo/moo");

            result.Body.AsString().ShouldEqual("OptionalCapture hoo");
        }

        [Fact]
        public void Should_resolve_optional_capture_with_optional_not_specified()
        {
            var result = this.browser.Get("/moo/moo");

            result.Body.AsString().ShouldEqual("OptionalCapture default");
        }

        [Fact]
        public void Should_resolve_optional_capture_with_default_with_optional_specified()
        {
            var result = this.browser.Get("/boo/badger/laa");

            result.Body.AsString().ShouldEqual("OptionalCaptureWithDefault badger");
        }

        [Fact]
        public void Should_resolve_optional_capture_with_default_with_optional_not_specified()
        {
            var result = this.browser.Get("/boo/laa");

            result.Body.AsString().ShouldEqual("OptionalCaptureWithDefault test");
        }

        [Fact]
        public void Should_capture_greedy_on_end()
        {
            var result = this.browser.Get("/bleh/this/is/some/stuff");

            result.Body.AsString().ShouldEqual("GreedyOnEnd this/is/some/stuff");
        }

        [Fact]
        public void Should_capture_greedy_in_middle()
        {
            var result = this.browser.Get("/bleh/this/is/some/stuff/bar");

            result.Body.AsString().ShouldEqual("GreedyInMiddle this/is/some/stuff");
        }

        [Fact]
        public void Should_capture_greedy_and_normal_capture()
        {
            var result = this.browser.Get("/greedy/this/is/some/stuff/badger/blah");

            result.Body.AsString().ShouldEqual("GreedyAndCapture this/is/some/stuff blah");
        }

        [Fact]
        public void Should_capture_regex()
        {
            var result = this.browser.Get("/regex/123/moo");

            result.Body.AsString().ShouldEqual("RegEx 123 moo");
        }

        [Fact]
        public void Should_handle_head_requests()
        {
            var result = this.browser.Head("/");

            result.StatusCode.ShouldEqual(HttpStatusCode.OK);
            result.Body.AsString().ShouldEqual(string.Empty);
        }

        [Fact]
        public void Should_handle_options_requests()
        {
            var result = this.browser.Options("/");

            result.StatusCode.ShouldEqual(HttpStatusCode.OK);
            result.Headers["Allow"].ShouldContain("GET");
            result.Headers["Allow"].ShouldContain("POST");
        }

        [Fact]
        public void Should_return_404_if_no_root_found_when_requesting_it()
        {
            var localBrowser = new Browser(with => with.Module<NoRootModule>());

            var result = localBrowser.Get("/");

            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        private class NoRootModule : NancyModule
        {
            public NoRootModule()
            {
                Get["/notroot"] = _ => "foo";
            }        
        }

        private class TestModule : NancyModule
        {
            public TestModule()
            {
                Get["/"] = _ => "Root";

                Post["/"] = _ => "PostRoot";

                Get["/foo"] = _ => "SingleLiteral";

                Get["/foo/bar/baz"] = _ => "MultipleLiteral";

                Get["/foo/{bar}/plop"] = _ => "Captured " + _.bar;

                Get["/moo/baa"] = _ => "Dummy";

                Get["/moo/baa/cheese"] = _ => "Dummy";

                Get["/moo/{test?}/moo"] = _ => "OptionalCapture " + _.test.Default("default");

                Get["/boo/{woo?test}/laa"] = _ => "OptionalCaptureWithDefault " + _.woo;

                Get["/bleh/{test*}"] = _ => "GreedyOnEnd " + _.test;

                Get["/bleh/{test*}/bar"] = _ => "GreedyInMiddle " + _.test;

                Get["/greedy/{test*}/badger/{woo}"] = _ => "GreedyAndCapture " + _.test + " " + _.woo;

                Get[@"/regex/(?<foo>\d{2,4})/{bar}"] = x =>
                {
                    return string.Format("RegEx {0} {1}", x.foo, x.bar);
                };
            }
        }
    }
}
