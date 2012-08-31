using System.Collections.Generic;

namespace Nancy.Tests.Functional.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Bootstrapper;
    using IO;
    using Responses.Negotiation;
    using Testing;
    using Xunit;

    public class ContentNegotiationFixture
    {
        [Fact]
        public void Should_return_int_value_from_get_route_as_response_with_status_code_set_to_value()
        {
            // Given
            var module = new ConfigurableNancyModule(with =>
            {
                with.Get("/int", x => 200);
            });

            var browser = new Browser(with =>
            {
                with.Module(module);
            });

            // When
            var response = browser.Get("/int");

            // Then
            Assert.Equal((HttpStatusCode)200, response.StatusCode);
        }

        [Fact]
        public void Should_return_string_value_from_get_route_as_response_with_content_set_as_value()
        {
            // Given
            var module = new ConfigurableNancyModule(with =>
            {
                with.Get("/string", x => "hello");
            });

            var browser = new Browser(with =>
            {
                with.Module(module);
            });

            // When
            var response = browser.Get("/string");

            // Then
            Assert.Equal("hello", response.Body.AsString());
        }

        [Fact]
        public void Should_return_httpstatuscode_value_from_get_route_as_response_with_content_set_as_value()
        {
            // Given
            var module = new ConfigurableNancyModule(with =>
            {
                with.Get("/httpstatuscode", x => HttpStatusCode.Accepted);
            });

            var browser = new Browser(with =>
            {
                with.Module(module);
            });

            // When
            var response = browser.Get("/httpstatuscode");

            // Then
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Fact]
        public void Should_return_action_value_as_response_with_content_set_as_value()
        {
            // Given
            var module = new ConfigurableNancyModule(with =>
            {
                with.Get("/action", x =>
                {
                    Action<Stream> result = stream =>
                    {
                        var wrapper = new UnclosableStreamWrapper(stream);
                        using (var writer = new StreamWriter(wrapper))
                        {
                            writer.Write("Hiya Nancy!");
                        }
                    };

                    return result;
                });
            });

            var browser = new Browser(with =>
            {
                with.Module(module);
            });

            // When
            var response = browser.Get("/action");

            // Then
            Assert.Equal("Hiya Nancy!", response.Body.AsString());
        }

        [Fact]
        public void Should_add_negotiated_headers_to_response()
        {
            // Given

            var module = new ConfigurableNancyModule(with =>
            {
                with.Get("/headers", x =>
                {
                    var context =
                        new NancyContext { NegotiationContext = new NegotiationContext() };

                    var negotiator =
                        new Negotiator(context);
                    negotiator.WithHeader("foo", "bar");

                    return negotiator;
                });
            });

            var brower = new Browser(with =>
            {
                with.ResponseProcessor<TestProcessor>();

                with.Module(module);
            });

            // When
            var response = brower.Get("/headers");

            // Then
            Assert.True(response.Headers.ContainsKey("foo"));
            Assert.Equal("bar", response.Headers["foo"]);
        }

        [Fact]
        public void Should_apply_default_accept_when_no_accept_header_sent()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessor<TestProcessor>();

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/", parameters =>
                    {
                        var context =
                            new NancyContext { NegotiationContext = new NegotiationContext() };

                        var negotiator =
                            new Negotiator(context);

                        return negotiator;
                    });
                }));
            });

            // When
            var response = browser.Get("/");

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Should_ignore_stupid_browsers_that_ask_for_xml()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessor<TestProcessor>();

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/", parameters =>
                    {
                        var context =
                            new NancyContext { NegotiationContext = new NegotiationContext() };

                        var negotiator =
                            new Negotiator(context);

                        negotiator.WithAllowedMediaRange("application/xml");
                        negotiator.WithAllowedMediaRange("text/html");

                        return negotiator;
                    });
                }));
            });

            // When
            var response = browser.Get("/", with =>
            {
                with.Header("User-Agent", "Mozilla/5.0 (Windows; U; MSIE 7.0; Windows NT 6.0; en-US)");
                with.Accept("application/xml", 1.0m);
                with.Accept("application/xhtml+xml", 1.0m);
                with.Accept("*/*", 0.9m);
            });

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Body.AsString().Contains("text/html"), "Media type mismatch");
        }

        [Fact]
        public void Should_boost_html_priority_if_set_to_the_same_priority_as_others()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessor<TestProcessor>();

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/", parameters =>
                    {
                        var context =
                            new NancyContext { NegotiationContext = new NegotiationContext() };

                        var negotiator =
                            new Negotiator(context);

                        negotiator.WithAllowedMediaRange("application/xml");
                        negotiator.WithAllowedMediaRange("text/html");

                        return negotiator;
                    });
                }));
            });

            // When
            var response = browser.Get("/", with =>
            {
                with.Header("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru-RU) AppleWebKit/533.19.4 (KHTML, like Gecko) Version/5.0.3 Safari/533.19.4");
                with.Accept("application/xml", 0.9m);
                with.Accept("text/html", 0.9m);
            });

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Body.AsString().Contains("text/html"), "Media type mismatch");
        }

        public class TestProcessor : IResponseProcessor
        {
            private const string ResponseTemplate = "{0}\n{1}";

            public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings { get { return Enumerable.Empty<Tuple<string, MediaRange>>(); } }

            public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
            {
                return new ProcessorMatch
                           {
                               RequestedContentTypeResult = MatchResult.DontCare,
                               ModelResult = MatchResult.DontCare
                           };
            }

            public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
            {
                return string.Format(ResponseTemplate, requestedMediaRange, model == null ? "None" : model.GetType());
            }
        }
    }
}