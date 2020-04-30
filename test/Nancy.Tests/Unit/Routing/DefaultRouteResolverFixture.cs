namespace Nancy.Tests.Unit.Routing
{
    using System.Threading.Tasks;
    using Nancy.Testing;
    using Xunit;

    public class DefaultRouteResolverFixture
    {
        [Fact]
        public async Task Should_resolve_root()
        {
            //Given, When
            var browser = InitBrowser(caseSensitive: false);
            var result = await browser.Get("/");

            //Then
            result.Body.AsString().ShouldEqual("Root");
        }

        [Fact]
        public async Task Should_resolve_correct_route_based_on_method()
        {
            //Given, When
            var browser = InitBrowser(caseSensitive: false);
            var result = await browser.Post("/");

            //Then
            result.Body.AsString().ShouldEqual("PostRoot");
        }

        [Theory]
        [InlineData("/foo", true)]
        [InlineData("/foo", false)]
        [InlineData("/FOO", true)]
        [InlineData("/FOO", false)]
        public async Task Should_resolve_single_literal(string path, bool caseSensitive)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        public async Task Should_resolve_multi_literal(string path, bool caseSensitive)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        public async Task Should_resolve_single_capture(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        public async Task Should_resolve_optional_capture_with_optional_specified(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        public async Task Should_resolve_optional_capture_with_optional_not_specified(string path, bool caseSensitive)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        public async Task Should_resolve_optional_capture_with_default_with_optional_specified(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        public async Task Should_resolve_optional_capture_with_default_with_optional_not_specified(string path, bool caseSensitive)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        [InlineData("/api", "Single optional segment")]
        [InlineData("/api/", "Single optional segment")]
        [InlineData("/api/arg1", "Single optional segment")]
        [InlineData("/api/arg1/", "Single optional segment")]
        [InlineData("/api/arg1/arg2", "Two optional segments")]
        [InlineData("/api/arg1/arg2/", "Two optional segments")]
        [InlineData("/api/arg1/arg2/arg3", "Three optional segments")]
        [InlineData("/api/arg1/arg2/arg3/", "Three optional segments")]
        public async Task Should_Resolve_Optionals_Correctly(string path, string expected)
        {
            var browser = InitBrowser(caseSensitive: false);
            var result = await browser.Get(path);
            result.Body.AsString().ShouldEqual(expected);
        }

        [Theory]
        [InlineData("/api/greedy/arg1", "Greedy match")]
        [InlineData("/api/greedy/arg1/arg2", "Greedy match")]
        public async Task Should_Resolve_Greedy_Alongside_Optionals(string path, string expected)
        {
            var browser = InitBrowser(caseSensitive: false);
            var result = await browser.Get(path);
            result.Body.AsString().ShouldEqual(expected);
        }

        [Theory]
        [InlineData("/optional/literal/bar", "Single optional segment, literal segment at end")]
        [InlineData("/optional/literal/arg1/bar", "Single optional segment, literal segment at end")]
        [InlineData("/optional/literal/arg1/arg2/bar", "Two optional segments, literal segment at end")]
        public async Task Should_Resolve_Optionals_with_Literal_Ends(string path, string expected)
        {
            var browser = InitBrowser(caseSensitive: false);
            var result = await browser.Get(path);
            result.Body.AsString().ShouldEqual(expected);
        }
        
        [Theory]
        [InlineData("/optional/variable/hello", "Single  hello")]
        [InlineData("/optional/variable/hello/there", "Single hello there")]
        [InlineData("/optional/variable/hello/there/everybody", "Double hello there everybody")]
        public async Task Should_Resolve_Optionals_with_Variable_Ends(string path, string expected)
        {
            var browser = InitBrowser(caseSensitive: false);
            var result = await browser.Get(path);            
            result.Body.AsString().ShouldEqual(expected);
        }
        
        [Theory]        
        [InlineData("/too/greedy/arg1", "One arg1")]
        [InlineData("/too/greedy/arg1/hello", "Literal arg1")]
        public async Task Should_Not_Be_Too_Greedy(string path, string expected)
        {
            var browser = InitBrowser(caseSensitive: false);
            var result = await browser.Get(path);
            var woot = result.Body.AsString();
            result.Body.AsString().ShouldEqual(expected);
        }

        [Theory]
        [InlineData("/bleh/this/is/some/stuff", true, "this/is/some/stuff")]
        [InlineData("/bleh/this/is/some/stuff", false, "this/is/some/stuff")]
        [InlineData("/BLEH/THIS/IS/SOME/STUFF", true, "NA")]
        [InlineData("/BLEH/THIS/IS/SOME/STUFF", false, "THIS/IS/SOME/STUFF")]
        public async Task Should_capture_greedy_on_end(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        public async Task Should_capture_greedy_in_middle(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        public async Task Should_capture_greedy_and_normal_capture(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        public async Task Should_capture_node_with_multiple_parameters(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        public async Task Should_capture_node_with_literal(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        public async Task Should_capture_regex(string path, bool caseSensitive, string expected)
        {
            //Given, When
            var browser = InitBrowser(caseSensitive);
            var result = await browser.Get(path);

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
        public async Task Should_handle_head_requests()
        {
            //Given, When
            var browser = InitBrowser(caseSensitive: false);
            var result = await browser.Head("/");

            //Then
            result.StatusCode.ShouldEqual(HttpStatusCode.OK);
            result.Body.AsString().ShouldEqual(string.Empty);
        }

        [Fact]
        public async Task Should_handle_options_requests()
        {
            //Given, When
            var browser = InitBrowser(caseSensitive: false);
            var result = await browser.Options("/");

            //Then
            result.StatusCode.ShouldEqual(HttpStatusCode.OK);
            result.Headers["Allow"].ShouldContain("GET");
            result.Headers["Allow"].ShouldContain("POST");
        }

        [Fact]
        public async Task Should_return_404_if_no_root_found_when_requesting_it()
        {
            //Given
            var browser = new Browser(with => with.Module<NoRootModule>());

            //When
            var result = await browser.Get("/");

            //Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_return_405_if_requested_method_is_not_permitted_but_others_are_available_and_not_disabled()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.Module<MethodNotAllowedModule>();
                with.Configure(env =>
                {
                    env.Routing(disableMethodNotAllowedResponses: false);
                });
            });

            // When
            var result = await browser.Get("/");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task Should_not_return_405_if_requested_method_is_not_permitted_but_others_are_available_and_disabled()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.Module<MethodNotAllowedModule>();
                with.Configure(env =>
                {
                    env.Routing(disableMethodNotAllowedResponses: true);
                });
            });

            // When
            var result = await browser.Get("/");

            // Then
            result.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Should_set_allowed_method_on_response_when_returning_405()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.Module<MethodNotAllowedModule>();
                with.Configure(env =>
                {
                    env.Routing(disableMethodNotAllowedResponses: false);
                });
            });

            // When
            var result = await browser.Get("/");

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
                Delete("/", args => 200);

                Post("/", args => 200);
            }
        }

        private class NoRootModule : NancyModule
        {
            public NoRootModule()
            {
                Get("/notroot", args => "foo");
            }
        }


        private class TestModule : NancyModule
        {
            public TestModule()
            {
                Get("/", args => "Root");

                Post("/", args => "PostRoot");

                Get("/foo", args => "SingleLiteral");

                Get("/foo/bar/baz", args => "MultipleLiteral");

                Get("/foo/{bar}/plop", args => "Captured " + args.bar);

                Get("/moo/baa", args => "Dummy");

                Get("/moo/baa/cheese", args => "Dummy");

                Get("/moo/{test?}/moo", args => "OptionalCapture " + args.test.Default("default"));

                Get("/boo/{woo?test}/laa", args => "OptionalCaptureWithDefault " + args.woo);

                Get("/bleh/{test*}", args => "GreedyOnEnd " + args.test);

                Get("/bleh/{test*}/bar", args => "GreedyInMiddle " + args.test);

                Get("/greedy/{test*}/badger/{woo}", args => "GreedyAndCapture " + args.test + " " + args.woo);

                Get("/multipleparameters/{file}.{extension}", args => "Multiple parameters " + args.file + "." + args.extension);

                Get("/capturenodewithliteral/{file}.html", args => "CaptureNodeWithLiteral " + args.file + ".html");

                Get(@"/regex/(?<foo>\d{2,4})/{bar}", args => string.Format("RegEx {0} {1}", args.foo, args.bar));

                Get("/api/{arg1?}", args => "Single optional segment");

                Get("/api/{arg1?}/{arg2?}", args => "Two optional segments");

                Get("/api/{arg1?}/{arg2?}/{arg3?}", args => "Three optional segments");

                Get("/api/greedy/{something*}", args => "Greedy match");

                Get("/optional/literal/{arg1?}/bar", args => "Single optional segment, literal segment at end");

                Get("/optional/literal/{arg1?}/{arg2?}/bar", args => "Two optional segments, literal segment at end");

                Get("/optional/variable/{arg1?}/{variable}", args => string.Format("Single {0} {1}", args.arg1.Value, args.variable.Value));

                Get("/optional/variable/{arg1?}/{arg2?}/{variable}", args => string.Format("Double {0} {1} {2}", args.arg1.Value, args.arg2.Value, args.variable.Value));

                Get("/too/greedy/{arg1*}", args => string.Format("One {0}", args.arg1.Value));

                Get("/too/greedy/{arg1*}/hello", args => string.Format("Literal {0}", args.arg1.Value));                
            }
        }
    }
}
