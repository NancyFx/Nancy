namespace Nancy.Tests.Unit.Routing
{
    using Nancy.Testing;
    using Xunit;
    using Xunit.Extensions;

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
            //Given, When
            var result = this.browser.Get("/");

            //Then
            result.Body.AsString().ShouldEqual("Root");
        }

        [Fact]
        public void Should_resolve_correct_route_based_on_method()
        {
            //Given, When
            var result = this.browser.Post("/");

            //Then
            result.Body.AsString().ShouldEqual("PostRoot");
        }

        [Theory]
        [InlineData("/foo")]
        [InlineData("/FOO")]
        public void Should_resolve_single_literal(string path)
        {
            //Given, When
            var result = this.browser.Get(path);

            //Then
            result.Body.AsString().ShouldEqual("SingleLiteral");
        }

        [Theory]
        [InlineData("/foo/bar/baz")]
        [InlineData("/FOO/BAR/BAZ")]
        public void Should_resolve_multi_literal(string path)
        {
            //Given, When
            var result = this.browser.Get(path);

            //Then
            result.Body.AsString().ShouldEqual("MultipleLiteral");
        }

        [Theory]
        [InlineData("/foo/testing/plop", "testing")]
        [InlineData("/FOO/TESTING/PLOP", "TESTING")]
        public void Should_resolve_single_capture(string path, string expected)
        {
            //Given, When
            var result = this.browser.Get(path);

            //Then
            result.Body.AsString().ShouldEqual("Captured " + expected);
        }

        [Theory]
        [InlineData("/moo/hoo/moo", "hoo")]
        [InlineData("/MOO/HOO/MOO", "HOO")]
        public void Should_resolve_optional_capture_with_optional_specified(string path, string expected)
        {
            //Given, When
            var result = this.browser.Get(path);

            //Then
            result.Body.AsString().ShouldEqual("OptionalCapture " + expected);
        }

        [Theory]
        [InlineData("/moo/moo")]
        [InlineData("/MOO/MOO")]
        public void Should_resolve_optional_capture_with_optional_not_specified(string path)
        {
            //Given, When
            var result = this.browser.Get("/moo/moo");

            //Then
            result.Body.AsString().ShouldEqual("OptionalCapture default");
        }

        [Theory]
        [InlineData("/boo/badger/laa", "badger")]
        [InlineData("/BOO/BADGER/LAA", "BADGER")]
        public void Should_resolve_optional_capture_with_default_with_optional_specified(string path, string expected)
        {
            //Given, When
            var result = this.browser.Get(path);

            //Then
            result.Body.AsString().ShouldEqual("OptionalCaptureWithDefault " + expected);
        }

        [Theory]
        [InlineData("/boo/laa")]
        [InlineData("/BOO/LAA")]
        public void Should_resolve_optional_capture_with_default_with_optional_not_specified(string path)
        {
            //Given, When
            var result = this.browser.Get(path);

            //Then
            result.Body.AsString().ShouldEqual("OptionalCaptureWithDefault test");
        }

        [Theory]
        [InlineData("/bleh/this/is/some/stuff", "this/is/some/stuff")]
        [InlineData("/BLEH/THIS/IS/SOME/STUFF", "THIS/IS/SOME/STUFF")]
        public void Should_capture_greedy_on_end(string path, string expected)
        {
            //Given, When
            var result = this.browser.Get(path);

            //Then
            result.Body.AsString().ShouldEqual("GreedyOnEnd " + expected);
        }

        [Theory]
        [InlineData("/bleh/this/is/some/stuff/bar", "this/is/some/stuff")]
        [InlineData("/BLEH/THIS/IS/SOME/STUFF/BAR", "THIS/IS/SOME/STUFF")]
        public void Should_capture_greedy_in_middle(string path, string expected)
        {
            //Given, When
            var result = this.browser.Get(path);

            //Then
            result.Body.AsString().ShouldEqual("GreedyInMiddle " + expected);
        }

        [Theory]
        [InlineData("/greedy/this/is/some/stuff/badger/blah", "this/is/some/stuff blah")]
        [InlineData("/GREEDY/THIS/IS/SOME/STUFF/BADGER/BLAH", "THIS/IS/SOME/STUFF BLAH")]
        public void Should_capture_greedy_and_normal_capture(string path, string expected)
        {
            //Given, When
            var result = this.browser.Get(path);

            //Then
            result.Body.AsString().ShouldEqual("GreedyAndCapture " + expected);
        }

        [Theory]
        [InlineData("/multipleparameters/file.extension", "file.extension")]
        [InlineData("/MULTIPLEPARAMETERS/FILE.EXTENSION", "FILE.EXTENSION")]
        public void Should_capture_node_with_multiple_parameters(string path, string expected)
        {
            //Given, When
            var result = this.browser.Get(path);

            //Then
            result.Body.AsString().ShouldEqual("Multiple parameters " + expected);
        }

        [Theory]
        [InlineData("/capturenodewithliteral/testing.html", "testing")]
        [InlineData("/CAPTURENODEWITHLITERAL/TESTING.HTML", "TESTING")]
        public void Should_capture_node_with_literal(string path, string expected)
        {
            //Given, When
            var result = this.browser.Get(path);

            //Then
            result.Body.AsString().ShouldEqual("CaptureNodeWithLiteral " + expected + ".html");
        }

        [Theory]
        [InlineData("/regex/123/moo", "123 moo")]
        [InlineData("/REGEX/123/MOO", "123 MOO")]
        public void Should_capture_regex(string path, string expected)
        {
            //Given, When
            var result = this.browser.Get(path);

            //Then
            result.Body.AsString().ShouldEqual("RegEx " + expected);
        }

        [Fact]
        public void Should_handle_head_requests()
        {
            //Given, When
            var result = this.browser.Head("/");

            //Then
            result.StatusCode.ShouldEqual(HttpStatusCode.OK);
            result.Body.AsString().ShouldEqual(string.Empty);
        }

        [Fact]
        public void Should_handle_options_requests()
        {
            //Given, When
            var result = this.browser.Options("/");

            //Then
            result.StatusCode.ShouldEqual(HttpStatusCode.OK);
            result.Headers["Allow"].ShouldContain("GET");
            result.Headers["Allow"].ShouldContain("POST");
        }

        [Fact]
        public void Should_return_404_if_no_root_found_when_requesting_it()
        {
            //Given
            var localBrowser = new Browser(with => with.Module<NoRootModule>());

            //When
            var result = localBrowser.Get("/");

            //Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_return_405_if_requested_method_is_not_permitted_but_others_are_available_and_not_disabled()
        {
            // Given
            StaticConfiguration.DisableMethodNotAllowedResponses = false;
            var localBrowser = new Browser(with => with.Module<MethodNotAllowedModule>());

            // When
            var result = localBrowser.Get("/");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public void Should_not_return_405_if_requested_method_is_not_permitted_but_others_are_available_and_disabled()
        {
            // Given
            StaticConfiguration.DisableMethodNotAllowedResponses = true;
            var localBrowser = new Browser(with => with.Module<MethodNotAllowedModule>());

            // When
            var result = localBrowser.Get("/");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_allowed_method_on_response_when_returning_405()
        {
            // Given
            StaticConfiguration.DisableMethodNotAllowedResponses = false;
            var localBrowser = new Browser(with => with.Module<MethodNotAllowedModule>());
            
            // When
            var result = localBrowser.Get("/");

            // Then
            result.Headers["Allow"].ShouldEqual("DELETE, POST");
        }

        private class MethodNotAllowedModule : NancyModule
        {
            public MethodNotAllowedModule()
            {
                Delete["/"] = x => 200;
                
                Post["/"] = x => 200;
            }
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

                Get["/multipleparameters/{file}.{extension}"] = _ => "Multiple parameters " + _.file + "." + _.extension;

                Get["/capturenodewithliteral/{file}.html"] = _ => "CaptureNodeWithLiteral " + _.file + ".html";
                
                Get[@"/regex/(?<foo>\d{2,4})/{bar}"] = x => string.Format("RegEx {0} {1}", x.foo, x.bar);
            }
        }
    }
}
