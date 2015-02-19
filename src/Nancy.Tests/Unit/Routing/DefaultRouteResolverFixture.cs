namespace Nancy.Tests.Unit.Routing
{
    using Nancy.Testing;

    using Xunit;
    using Xunit.Extensions;

    public class DefaultRouteResolverFixture
    {
        [Fact]
        public void Should_resolve_root()
        {
            //Given, When
            var browser = InitBrowser(caseSensitive: false);
            var result = browser.Get("/");

            //Then
            result.Body.AsString().ShouldEqual("Root");
        }

        [Fact]
        public void Should_resolve_correct_route_based_on_method()
        {
            //Given, When
            var browser = InitBrowser(caseSensitive: false);
            var result = browser.Post("/");

            //Then
            result.Body.AsString().ShouldEqual("PostRoot");
        }

        [Theory]
        [InlineData("/foo", true)]
        [InlineData("/foo", false)]
        [InlineData("/FOO", true)]
        [InlineData("/FOO", false)]
        public void Should_resolve_single_literal(string path, bool caseSensitive)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
                result.Body.AsString().ShouldEqual("SingleLiteral");
            }
            else
            {
                result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Theory]
        [InlineData("/foo/bar/baz", true)]
        [InlineData("/foo/bar/baz", false)]
        [InlineData("/FOO/BAR/BAZ", true)]
        [InlineData("/FOO/BAR/BAZ", false)]
        public void Should_resolve_multi_literal(string path, bool caseSensitive)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
                result.Body.AsString().ShouldEqual("MultipleLiteral");
            }
            else
            {
                result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Theory]
        [InlineData("/foo/testing/plop", true, "testing")]
        [InlineData("/foo/testing/plop", false, "testing")]
        [InlineData("/FOO/TESTING/PLOP", true, "NA")]
        [InlineData("/FOO/TESTING/PLOP", false, "TESTING")]
        public void Should_resolve_single_capture(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
                result.Body.AsString().ShouldEqual("Captured " + expected);
            }
            else
            {
                result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Theory]
        [InlineData("/moo/hoo/moo", true, "hoo")]
        [InlineData("/moo/hoo/moo", false, "hoo")]
        [InlineData("/MOO/HOO/MOO", true, "NA")]
        [InlineData("/MOO/HOO/MOO", false, "HOO")]
        public void Should_resolve_optional_capture_with_optional_specified(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
                result.Body.AsString().ShouldEqual("OptionalCapture " + expected);
            }
            else
            {
                result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Theory]
        [InlineData("/moo/moo", true)]
        [InlineData("/moo/moo", false)]
        [InlineData("/MOO/MOO", true)]
        [InlineData("/MOO/MOO", false)]
        public void Should_resolve_optional_capture_with_optional_not_specified(string path, bool caseSensitive)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
                result.Body.AsString().ShouldEqual("OptionalCapture default");
            }
            else
            {
                result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Theory]
        [InlineData("/boo/badger/laa", true, "badger")]
        [InlineData("/boo/badger/laa", false, "badger")]
        [InlineData("/BOO/BADGER/LAA", true, "NA")]
        [InlineData("/BOO/BADGER/LAA", false, "BADGER")]
        public void Should_resolve_optional_capture_with_default_with_optional_specified(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
              result.Body.AsString().ShouldEqual("OptionalCaptureWithDefault " + expected);
            }
            else
            {
              result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Theory]
        [InlineData("/boo/laa", true)]
        [InlineData("/boo/laa", false)]
        [InlineData("/BOO/LAA", true)]
        [InlineData("/BOO/LAA", false)]
        public void Should_resolve_optional_capture_with_default_with_optional_not_specified(string path, bool caseSensitive)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
              result.Body.AsString().ShouldEqual("OptionalCaptureWithDefault test");
            }
            else
            {
              result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Theory]
        [InlineData("/bleh/this/is/some/stuff", true, "this/is/some/stuff")]
        [InlineData("/bleh/this/is/some/stuff", false, "this/is/some/stuff")]
        [InlineData("/BLEH/THIS/IS/SOME/STUFF", true, "NA")]
        [InlineData("/BLEH/THIS/IS/SOME/STUFF", false, "THIS/IS/SOME/STUFF")]
        public void Should_capture_greedy_on_end(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
              result.Body.AsString().ShouldEqual("GreedyOnEnd " + expected);
            }
            else
            {
              result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Theory]
        [InlineData("/bleh/this/is/some/stuff/bar", true, "this/is/some/stuff")]
        [InlineData("/bleh/this/is/some/stuff/bar", false, "this/is/some/stuff")]
        [InlineData("/BLEH/THIS/IS/SOME/STUFF/BAR", true, "NA")]
        [InlineData("/BLEH/THIS/IS/SOME/STUFF/BAR", false, "THIS/IS/SOME/STUFF")]
        public void Should_capture_greedy_in_middle(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
              result.Body.AsString().ShouldEqual("GreedyInMiddle " + expected);
            }
            else
            {
              result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Theory]
        [InlineData("/greedy/this/is/some/stuff/badger/blah", true, "this/is/some/stuff blah")]
        [InlineData("/greedy/this/is/some/stuff/badger/blah", false, "this/is/some/stuff blah")]
        [InlineData("/GREEDY/THIS/IS/SOME/STUFF/BADGER/BLAH", true, "NA")]
        [InlineData("/GREEDY/THIS/IS/SOME/STUFF/BADGER/BLAH", false, "THIS/IS/SOME/STUFF BLAH")]
        public void Should_capture_greedy_and_normal_capture(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
              result.Body.AsString().ShouldEqual("GreedyAndCapture " + expected);
            }
            else
            {
              result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Theory]
        [InlineData("/multipleparameters/file.extension", true, "file.extension")]
        [InlineData("/multipleparameters/file.extension", false, "file.extension")]
        [InlineData("/MULTIPLEPARAMETERS/FILE.EXTENSION", true, "NA")]
        [InlineData("/MULTIPLEPARAMETERS/FILE.EXTENSION", false, "FILE.EXTENSION")]
        public void Should_capture_node_with_multiple_parameters(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
              result.Body.AsString().ShouldEqual("Multiple parameters " + expected);
            }
            else
            {
              result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Theory]
        [InlineData("/capturenodewithliteral/testing.html", true, "testing")]
        [InlineData("/capturenodewithliteral/testing.html", false, "testing")]
        [InlineData("/CAPTURENODEWITHLITERAL/TESTING.HTML", true, "NA")]
        [InlineData("/CAPTURENODEWITHLITERAL/TESTING.HTML", false, "TESTING")]
        public void Should_capture_node_with_literal(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
              result.Body.AsString().ShouldEqual("CaptureNodeWithLiteral " + expected + ".html");
            }
            else
            {
              result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Theory]
        [InlineData("/regex/123/moo", true, "123 moo")]
        [InlineData("/regex/123/moo", false, "123 moo")]
        [InlineData("/REGEX/123/MOO", true, "NA")]
        [InlineData("/REGEX/123/MOO", false, "123 MOO")]
        public void Should_capture_regex(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = browser.Get(path);

            //Then
            if (ShouldBeFound(path, caseSensitive))
            {
              result.Body.AsString().ShouldEqual("RegEx " + expected);
            }
            else
            {
              result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public void Should_handle_head_requests()
        {
            //Given, When
            var browser = InitBrowser(caseSensitive: false);
            var result = browser.Head("/");

            //Then
            result.StatusCode.ShouldEqual(HttpStatusCode.OK);
            result.Body.AsString().ShouldEqual(string.Empty);
        }

        [Fact]
        public void Should_handle_options_requests()
        {
            //Given, When
            var browser = InitBrowser(caseSensitive: false);
            var result = browser.Options("/");

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

        private Browser InitBrowser(bool caseSensitive)
        {
            StaticConfiguration.CaseSensitive = caseSensitive;
            return new Browser(with => with.Module<TestModule>());
        }

        private bool ShouldBeFound(string path, bool caseSensitive)
        {
            var isUpperCase = path == path.ToUpperInvariant();
            return !caseSensitive || !isUpperCase;
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
