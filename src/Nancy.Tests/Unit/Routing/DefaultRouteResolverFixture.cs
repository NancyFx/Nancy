namespace Nancy.Tests.Unit.Routing
{
    using Nancy.Testing;
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

        [Fact]
        public void Should_resolve_single_literal()
        {

            //Given, When
            var result = this.browser.Get("/foo");

            //Then

            result.Body.AsString().ShouldEqual("SingleLiteral");
        }

        [Fact]
        public void Should_resolve_multi_literal()
        {
            //Given, When
            var result = this.browser.Get("/foo/bar/baz");

            //Then
            result.Body.AsString().ShouldEqual("MultipleLiteral");
        }

        [Fact]
        public void Should_resolve_single_capture()
        {
            //Given, When
            var result = this.browser.Get("/foo/testing/plop");

            //Then

            result.Body.AsString().ShouldEqual("Captured testing");
        }

        [Fact]
        public void Should_resolve_optional_capture_with_optional_specified()
        {

            //Given, When
            var result = this.browser.Get("/moo/hoo/moo");

            //Then
            result.Body.AsString().ShouldEqual("OptionalCapture hoo");
        }

        [Fact]
        public void Should_resolve_optional_capture_with_optional_not_specified()
        {
            //Given, When
            var result = this.browser.Get("/moo/moo");

            //Then
            result.Body.AsString().ShouldEqual("OptionalCapture default");
        }

        [Fact]
        public void Should_resolve_optional_capture_with_default_with_optional_specified()
        {
            //Given, When
            var result = this.browser.Get("/boo/badger/laa");

            //Then
            result.Body.AsString().ShouldEqual("OptionalCaptureWithDefault badger");
        }

        [Fact]
        public void Should_resolve_optional_capture_with_default_with_optional_not_specified()
        {
            //Given, When
            var result = this.browser.Get("/boo/laa");

            //Then
            result.Body.AsString().ShouldEqual("OptionalCaptureWithDefault test");
        }

        [Fact]
        public void Should_capture_greedy_on_end()
        {
            //Given, When
            var result = this.browser.Get("/bleh/this/is/some/stuff");

            //Then

            result.Body.AsString().ShouldEqual("GreedyOnEnd this/is/some/stuff");
        }

        [Fact]
        public void Should_capture_greedy_in_middle()
        {
            //Given, When
            var result = this.browser.Get("/bleh/this/is/some/stuff/bar");

            //Then

            result.Body.AsString().ShouldEqual("GreedyInMiddle this/is/some/stuff");
        }

        [Fact]
        public void Should_capture_greedy_and_normal_capture()
        {
            //Given, When
            var result = this.browser.Get("/greedy/this/is/some/stuff/badger/blah");

            //Then
            result.Body.AsString().ShouldEqual("GreedyAndCapture this/is/some/stuff blah");
        }

        [Fact]
        public void Should_capture_node_with_multiple_parameters()
        {
            //Given, When
            var result = this.browser.Get("/multipleparameters/file.extension");

            //Then
            result.Body.AsString().ShouldEqual("Multiple parameters file.extension");
        }

        [Fact] 
        public void Should_capture_node_with_literal()
        {
            //Given, When
            var result = this.browser.Get("/capturenodewithliteral/file.html");

            //Then
            result.Body.AsString().ShouldEqual("CaptureNodeWithLiteral file.html");
        }

        [Fact]
        public void Should_capture_regex()
        {
           //Given, When
            var result = this.browser.Get("/regex/123/moo");

            //Then

            result.Body.AsString().ShouldEqual("RegEx 123 moo");
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


                Get[@"/regex/(?<foo>\d{2,4})/{bar}"] = x =>
                {
                    return string.Format("RegEx {0} {1}", x.foo, x.bar);
                };
            }
        }
    }
}
