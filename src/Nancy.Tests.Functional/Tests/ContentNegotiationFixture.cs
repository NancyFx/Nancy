namespace Nancy.Tests.Functional.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Nancy.Cookies;
    using Nancy.ErrorHandling;
    using Nancy.IO;
    using Nancy.Responses.Negotiation;
    using Nancy.Testing;
    using Nancy.Tests.Functional.Modules;
    using Xunit;
    using Xunit.Extensions;

    public class ContentNegotiationFixture
    {
        [Fact]
        public void Should_return_int_value_from_get_route_as_response_with_status_code_set_to_value()
        {
            // Given
            var module = new ConfigurableNancyModule(with =>
            {
                with.Get("/int", (x,m) => 200);
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
                with.Get("/string", (x, m) => "hello");
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
                with.Get("/httpstatuscode", (x, m) => HttpStatusCode.Accepted);
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
                with.Get("/action", (x, m) =>
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
                with.Get("/headers", (x, m) =>
                {
                    var context =
                        new NancyContext();

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
        public void Should_set_reason_phrase_on_response()
        {
            // Given
            var module = new ConfigurableNancyModule(with =>
            {
                with.Get("/customPhrase", (x, m) =>
                {
                    var context =
                        new NancyContext();

                    var negotiator =
                        new Negotiator(context);
                    negotiator.WithReasonPhrase("The test is passing!");

                    return negotiator;
                });
            });

            var brower = new Browser(with =>
            {
                with.ResponseProcessor<TestProcessor>();

                with.Module(module);
            });

            // When
            var response = brower.Get("/customPhrase");

            // Then
            Assert.Equal("The test is passing!", response.ReasonPhrase);
        }

        [Fact]
        public void Should_add_negotiated_content_headers_to_response()
        {
          // Given

          var module = new ConfigurableNancyModule(with =>
          {
            with.Get("/headers", (x, m) =>
            {
              var context =
                  new NancyContext();

              var negotiator =
                  new Negotiator(context);
              negotiator.WithContentType("text/xml");

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
          Assert.Equal("text/xml", response.Context.Response.ContentType);
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
                    x.Get("/", (parameters, module) =>
                    {
                        var context =
                            new NancyContext();

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
        public void Should_boost_html_priority_if_set_to_the_same_priority_as_others()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessor<TestProcessor>();

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/", (parameters, module) =>
                    {
                        var context =
                            new NancyContext();

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

        [Fact]
        public void Should_override_with_extension()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessor<TestProcessor>();

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/test", (parameters, module) =>
                    {
                        var context =
                            new NancyContext();

                        var negotiator =
                            new Negotiator(context);

                        return negotiator;
                    });
                }));
            });

            // When
            var response = browser.Get("/test.foo", with =>
            {
                with.Header("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru-RU) AppleWebKit/533.19.4 (KHTML, like Gecko) Version/5.0.3 Safari/533.19.4");
                with.Accept("application/xml", 0.9m);
                with.Accept("text/html", 0.9m);
            });

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Body.AsString().Contains("foo/bar"), "Media type mismatch");
        }

        [Fact]
        public void Should_response_with_notacceptable_when_route_does_not_allow_any_of_the_accepted_formats()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessor<TestProcessor>();

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/test", CreateNegotiatedResponse(config =>
                    {
                        config.WithAllowedMediaRange("application/xml");
                    }));
                }));
            });

            // When
            var response = browser.Get("/test", with =>
            {
                with.Accept("foo/bar", 0.9m);
            });

            // Then
            Assert.Equal(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Fact]
        public void Should_respond_with_notacceptable_when_no_processor_can_process_media_range()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessor<NullProcessor>();
                with.Module<NegotiationModule>();
            });

            // When
            var response = browser.Get("/invalid-view-name", with => with.Accept("foo/bar"));

            // Then
            Assert.Equal(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Fact]
        public void Should_return_that_contains_default_model_when_no_media_range_specific_model_was_declared()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessor<ModelProcessor>();

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/", CreateNegotiatedResponse(config =>
                    {
                        config.WithModel("the model");
                        config.WithAllowedMediaRange("test/test");
                    }));
                }));
            });

            // When
            var response = browser.Get("/", with =>
            {
                with.Accept("test/test", 0.9m);
            });

            // Then
            Assert.Equal("the model", response.Body.AsString());
        }

        [Fact]
        public void Should_return_media_range_specific_model_when_declared()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessor<ModelProcessor>();

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/", CreateNegotiatedResponse(config =>
                    {
                        config.WithModel("the model");
                        config.WithAllowedMediaRange("test/test");
                        config.WithMediaRangeModel("test/test", "media model");
                    }));
                }));
            });

            // When
            var response = browser.Get("/", with =>
            {
                with.Accept("test/test", 0.9m);
            });

            // Then
            Assert.Equal("media model", response.Body.AsString());
        }

        [Fact]
        public void Should_add_vary_accept_header()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessors(typeof(XmlProcessor), typeof(JsonProcessor), typeof(TestProcessor));

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/", CreateNegotiatedResponse());
                }));
            });

            // When
            var response = browser.Get("/", with => with.Header("Accept", "application/json"));

            // Then
            Assert.True(response.Headers.ContainsKey("Vary"));
            Assert.True(response.Headers["Vary"].Contains("Accept"));
        }

        [Fact]
        public void Should_add_link_header_for_matching_response_processors()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessors(typeof(XmlProcessor), typeof(JsonProcessor), typeof(TestProcessor));

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/", CreateNegotiatedResponse());
                }));
            });

            // When
            var response = browser.Get("/");

            // Then
            Assert.True(response.Headers["Link"].Contains(@"</.foo>; rel=""foo/bar"""));
            Assert.True(response.Headers["Link"].Contains(@"</.json>; rel=""application/json"""));
            Assert.True(response.Headers["Link"].Contains(@"</.xml>; rel=""application/xml"""));
        }

        [Fact]
        public void Should_set_negotiated_status_code_to_response_when_set_as_integer()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessor<TestProcessor>();

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/", CreateNegotiatedResponse(config =>
                    {
                        config.WithStatusCode(507);
                    }));
                }));
            });

            // When
            var response = browser.Get("/", with =>
            {
                with.Accept("test/test", 0.9m);
            });

            // Then
            Assert.Equal(HttpStatusCode.InsufficientStorage, response.StatusCode);
        }

        [Fact]
        public void Should_set_negotiated_status_code_to_response_when_set_as_httpstatuscode()
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessor<TestProcessor>();

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/", CreateNegotiatedResponse(config =>
                    {
                        config.WithStatusCode(HttpStatusCode.InsufficientStorage);
                    }));
                }));
            });

            // When
            var response = browser.Get("/", with =>
            {
                with.Accept("test/test", 0.9m);
            });

            // Then
            Assert.Equal(HttpStatusCode.InsufficientStorage, response.StatusCode);
        }

        [Fact]
        public void Should_set_negotiated_cookies_to_response()
        {
            // Given
            var negotiatedCookie =
                new NancyCookie("test", "test");

            var browser = new Browser(with =>
            {
                with.ResponseProcessor<TestProcessor>();

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/", CreateNegotiatedResponse(config =>
                    {
                        config.WithCookie(negotiatedCookie);
                    }));
                }));
            });

            // When
            var response = browser.Get("/", with =>
            {
                with.Accept("test/test", 0.9m);
            });

            // Then
            Assert.Same(negotiatedCookie, response.Cookies.First());
        }

        [Fact]
        public void Should_throw_exception_if_view_location_fails()
        {
            var browser = new Browser(with =>
            {
                with.ResponseProcessor<ViewProcessor>();

                with.Module(new ConfigurableNancyModule(x => x.Get("/FakeModuleInvalidViewName", CreateNegotiatedResponse(neg => neg.WithView("blahblahblah")))));
            });

            // When
            var result = Record.Exception(() =>
                {
                    var response = browser.Get(
                        "/FakeModuleInvalidViewName",
                        with =>
                            { with.Accept("text/html", 1.0m); });
                });

            // Then
            Assert.NotNull(result);
            Assert.Contains("Unable to locate view", result.ToString());
        }

        [Fact]
        public void Should_use_next_processor_if_processor_returns_null()
        {
            // Given
            var browser = new Browser(with =>
                {
                with.ResponseProcessors(typeof(NullProcessor), typeof(TestProcessor));

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/test", CreateNegotiatedResponse(config =>
                    {
                        config.WithAllowedMediaRange("application/xml");
                    }));
                }));
            });

            // When
            var response = browser.Get("/test", with =>
            {
                with.Accept("application/xml", 0.9m);
            });

            // Then
            var bodyResult = response.Body.AsString();
            Assert.True(bodyResult.StartsWith("application/xml"), string.Format("Body should have started with 'application/xml' but was actually '{0}'", bodyResult));
        }

        [Theory]
        [InlineData("application/xhtml+xml; profile=\"http://www.wapforum. org/xhtml\"")]
        [InlineData("application/xhtml+xml; q=1; profile=\"http://www.wapforum. org/xhtml\"")]
        public void Should_not_throw_exception_because_of_uncommon_accept_header(string header)
        {
            // Given
            var browser = new Browser(with =>
            {
                with.ResponseProcessors(typeof(XmlProcessor), typeof(JsonProcessor), typeof(TestProcessor));

                with.Module(new ConfigurableNancyModule(x =>
                {
                    x.Get("/", CreateNegotiatedResponse());
                }));
            });

            // When
            var response = browser.Get("/", with =>
            {
                with.Header("Accept", header);
            });

            // Then
            Assert.Equal((HttpStatusCode)200, response.StatusCode);
        }

        [Fact]
        public void Should_not_try_and_serve_view_with_invalid_name()
        {
            // Given
            var browser = new Browser(with => with.Module<NegotiationModule>());

            // When
            var result = Record.Exception(() => browser.Get("/invalid-view-name"));

            // Then
            Assert.True(result.ToString().Contains("Unable to locate view"));
        }

        [Fact]
        public void Should_return_response_negotiated_based_on_media_range()
        {
            // Given
            var browser = new Browser(with => with.Module<NegotiationModule>());

            // When
            var result = browser.Get("/negotiate", with =>
            {
                with.Accept("text/html");
            });

            // Then
            Assert.Equal(HttpStatusCode.SeeOther, result.StatusCode);
        }

        [Fact]
        public void Can_negotiate_in_status_code_handler()
        {
            // Given
            var browser = new Browser(with => with.StatusCodeHandler<NotFoundStatusCodeHandler>());

            // When
            var result = browser.Get("/not-found", with => with.Accept("application/json"));

            var response = result.Body.DeserializeJson<NotFoundStatusCodeHandlerResult>();

            // Then
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("Not Found.", response.Message);
        }

        [Fact]
        public void Can_negotiate_in_error_pipeline()
        {
            // Given
            var browser = new Browser(with => with.Module<ThrowingModule>());

            // When
            var jsonResult = browser.Get("/", with => with.Accept("application/json"));
            var xmlResult = browser.Get("/", with => with.Accept("application/xml"));

            var jsonResponse = jsonResult.Body.DeserializeJson<ThrowingModule.Error>();
            var xmlResponse = xmlResult.Body.DeserializeXml<ThrowingModule.Error>();

            // Then
            Assert.Equal("Oh noes!", jsonResponse.Message);
            Assert.Equal("Oh noes!", xmlResponse.Message);
        }

        [Fact]
        public void Should_return_negotiated_not_found_response_when_accept_header_is_html()
        {
            // Given
            var browser = new Browser(with => with.StatusCodeHandler<DefaultStatusCodeHandler>());
            var contentType = "text/html";

            // When
            var result = browser.Get("/not-found", with => with.Accept(contentType));

            // Then
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal(contentType, result.ContentType);
        }

        [Fact]
        public void Should_return_negotiated_not_found_response_when_accept_header_is_json()
        {
            // Given
            var browser = new Browser(with => with.StatusCodeHandler<DefaultStatusCodeHandler>());
            var contentType = "application/json";

            // When
            var result = browser.Get("/not-found", with => with.Accept(contentType));

            // Then
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal(string.Format("{0}; charset=utf-8", contentType), result.ContentType);
        }

        [Fact]
        public void Should_return_negotiated_not_found_response_when_accept_header_is_xml()
        {
            // Given
            var browser = new Browser(with => with.StatusCodeHandler<DefaultStatusCodeHandler>());
            var contentType = "application/xml";

            // When
            var result = browser.Get("/not-found", with => with.Accept(contentType));

            // Then
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal(contentType, result.ContentType);
        }

        private static Func<dynamic, NancyModule, dynamic> CreateNegotiatedResponse(Action<Negotiator> action = null)
        {
            return (parameters, module) =>
                {
                    var negotiator = new Negotiator(module.Context);

                    if (action != null)
                    {
                        action.Invoke(negotiator);
                    }

                    return negotiator;
                };
        }

        /// <summary>
        /// Test response processor that will accept any type
        /// and put the content type and model type into the
        /// response body for asserting against.
        /// Hacky McHackmeister but it works :-)
        /// </summary>
        public class TestProcessor : IResponseProcessor
        {
            private const string ResponseTemplate = "{0}\n{1}";

            public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
            {
                get
                {
                    yield return new Tuple<string, MediaRange>("foo", "foo/bar");
                }
            }

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

        public class NullProcessor : IResponseProcessor
        {
            public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
            {
                get
                {
                    yield break;
                }
            }

            public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
            {
                return new ProcessorMatch
                {
                    RequestedContentTypeResult = MatchResult.ExactMatch,
                    ModelResult = MatchResult.ExactMatch
                };
            }

            public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
            {
                return null;
            }
        }

        public class ModelProcessor : IResponseProcessor
        {
            public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
            {
                get
                {
                    yield return new Tuple<string, MediaRange>("foo", "foo/bar");
                }
            }

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
                return (string) model;
            }
        }

        private class NegotiationModule : NancyModule
        {
            public NegotiationModule()
            {
                Get["/invalid-view-name"] = _ =>
                {
                    return this.GetModel();
                };

                Get["/negotiate"] = parameters =>
                {
                    return Negotiate
                        .WithMediaRangeResponse("text/html", Response.AsRedirect("/"))
                        .WithMediaRangeModel("application/json", new { Name = "Nancy" });
                };
            }

            private IEnumerable<Foo> GetModel()
            {
                yield return new Foo();
            }

            private class Foo
            {
            }
        }

        private class NotFoundStatusCodeHandler : IStatusCodeHandler
        {
            private readonly IResponseNegotiator responseNegotiator;

            public NotFoundStatusCodeHandler(IResponseNegotiator responseNegotiator)
            {
                this.responseNegotiator = responseNegotiator;
            }

            public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
            {
                return statusCode == HttpStatusCode.NotFound;
            }

            public void Handle(HttpStatusCode statusCode, NancyContext context)
            {
                var error = new NotFoundStatusCodeHandlerResult
                {
                    StatusCode = statusCode,
                    Message = "Not Found."
                };

                context.Response = this.responseNegotiator.NegotiateResponse(error, context);
            }
        }

        private class NotFoundStatusCodeHandlerResult
        {
            public HttpStatusCode StatusCode { get; set; }

            public string Message { get; set; }
        }
    }
}