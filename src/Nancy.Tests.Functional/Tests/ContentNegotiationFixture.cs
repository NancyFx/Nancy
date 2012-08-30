namespace Nancy.Tests.Functional.Tests
{
    using System;
    using System.IO;
    using System.Linq;
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
            var module = new ConfigurableNancyModule(with => {
                with.Get("/int", x => 200);
            });

            var browser = new Browser(with => {
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
            var module = new ConfigurableNancyModule(with => {
                with.Get("/string", x => "hello");
            });

            var browser = new Browser(with => {
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
            var module = new ConfigurableNancyModule(with => {
                with.Get("/httpstatuscode", x => HttpStatusCode.Accepted);
            });

            var browser = new Browser(with => {
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
            var module = new ConfigurableNancyModule(with => {
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

            var browser = new Browser(with => {
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
            var processor = new ConfigurableResponseProcessor();

            var module = new ConfigurableNancyModule(with =>{
                with.Get("/headers", x =>{
                    var context = 
                        new NancyContext {NegotiationContext = new NegotiationContext()};

                    var negotiator =
                        new Negotiator(context);
                    negotiator.WithHeader("foo", "bar");

                    return negotiator;
                });
            });

            var brow = new Browser(with =>{
                with.Module(module);
                with.ResponseProcessor(processor);
            });

            // When
            var response = brow.Get("/headers");

            // Then
            Assert.True(response.Headers.ContainsKey("foo"));
            Assert.Equal("bar", response.Headers["foo"]);
        }
    }
}