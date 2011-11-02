namespace Nancy.Testing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Linq;

    using Nancy.Extensions;
    using Nancy.Tests;
    using Nancy.Helpers;
    using Nancy.Session;
    using Nancy.Tests;
    using Xunit;

    public class BrowserFixture
    {
        private readonly Browser browser;

        public BrowserFixture()
        {
            var bootstrapper =
                new ConfigurableBootstrapper(config => config.Modules(typeof(EchoModule)));

            CookieBasedSessions.Enable(bootstrapper);

            browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_be_able_to_send_string_in_body()
        {
            // Given
            const string thisIsMyRequestBody = "This is my request body";

            // When
            var result = browser.Post("/", with =>
                                           {
                                               with.HttpRequest();
                                               with.Body(thisIsMyRequestBody);
                                           });

            // Then
            result.Body.AsString().ShouldEqual(thisIsMyRequestBody);
        }

        [Fact]
        public void Should_be_able_to_send_stream_in_body()
        {
            // Given
            const string thisIsMyRequestBody = "This is my request body";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(thisIsMyRequestBody);
            writer.Flush();
            // When
            var result = browser.Post("/", with =>
                                           {
                                               with.HttpRequest();
                                               with.Body(stream, "text/plain");
                                           });

            // Then
            result.Body.AsString().ShouldEqual(thisIsMyRequestBody);
        }

        [Fact]
        public void Should_be_able_to_send_json_in_body()
        {
            // Given
            var model = new EchoModel { SomeString = "Some String", SomeInt = 29, SomeBoolean = true };

            // When
            var result = browser.Post("/", with =>
                                            {
                                                with.JsonBody(model);
                                            });

            // Then
            var actualModel = result.Body.DeserializeJson<EchoModel>();

            actualModel.ShouldNotBeNull();
            actualModel.SomeString.ShouldEqual(model.SomeString);
            actualModel.SomeInt.ShouldEqual(model.SomeInt);
            actualModel.SomeBoolean.ShouldEqual(model.SomeBoolean);
        }

        [Fact]
        public void Should_add_basic_authentication_credentials_to_the_headers_of_the_request()
        {
            // Given
            var context = new BrowserContext();

            // When
            context.BasicAuth("username", "password");

            // Then
            IBrowserContextValues values = context;

            var credentials = string.Format("{0}:{1}", "username", "password");
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            values.Headers["Authorization"].ShouldHaveCount(1);
            values.Headers["Authorization"].First().ShouldEqual("Basic " + encodedCredentials);
        }

        [Fact]
        public void Should_add_cookies_to_the_request()
        {
            // Given
            var context = new BrowserContext();

            var cookies =
                new Dictionary<string, string>
                {
                    {"CookieName", "CookieValue"},
                    {"SomeCookieName", "SomeCookieValue"}
                };

            // When
            context.Cookie(cookies);

            // Then
            IBrowserContextValues values = context;

            var cookieString = cookies.Aggregate(string.Empty, (current, cookie) => current + string.Format("{0}={1};", HttpUtility.UrlEncode(cookie.Key), HttpUtility.UrlEncode(cookie.Value)));

            values.Headers["Cookie"].ShouldHaveCount(1);
            values.Headers["Cookie"].First().ShouldEqual(cookieString);
        }

        [Fact]
        public void Should_add_cookie_to_the_request()
        {
            // Given
            var context = new BrowserContext();

            var cookies =
                new Dictionary<string, string>
                {
                    {"CookieName", "CookieValue"},
                    {"SomeCookieName", "SomeCookieValue"}
                };

            // When
            foreach (var cookie in cookies)
            {
                context.Cookie(cookie.Key, cookie.Value);
            }

            // Then
            IBrowserContextValues values = context;

            var cookieString = cookies.Aggregate(string.Empty, (current, cookie) => current + string.Format("{0}={1};", HttpUtility.UrlEncode(cookie.Key), HttpUtility.UrlEncode(cookie.Value)));

            values.Headers["Cookie"].ShouldHaveCount(1);
            values.Headers["Cookie"].First().ShouldEqual(cookieString);
        }

        [Fact]
        public void Should_add_cookies_to_the_request_and_get_cookies_in_response()
        {
            // Given
            var cookies =
                new Dictionary<string, string>
                {
                    {"CookieName", "CookieValue"},
                    {"SomeCookieName", "SomeCookieValue"}
                };

            // When
            var result = browser.Get("/cookie", with =>
            {
                with.Cookie(cookies);
            });

            // Then
            result.Cookies.Single(x => x.Name == "CookieName").Value.ShouldEqual("CookieValue");
            result.Cookies.Single(x => x.Name == "SomeCookieName").Value.ShouldEqual("SomeCookieValue");
        }

        [Fact]
        public void Should_add_a_cookie_to_the_request_and_get_a_cookie_in_response()
        {
            // Given, When
            var result = browser.Get("/cookie", with => with.Cookie("CookieName", "CookieValue"));

            // Then
            result.Cookies.Single(x => x.Name == "CookieName").Value.ShouldEqual("CookieValue");
        }

        [Fact]
        public void Should_be_able_to_continue_with_another_request()
        {
            // Given
            const string FirstRequestBody = "This is my first request body";
            const string SecondRequestBody = "This is my second request body";
            var firstRequestStream = new MemoryStream();
            var firstRequestWriter = new StreamWriter(firstRequestStream);
            firstRequestWriter.Write(FirstRequestBody);
            firstRequestWriter.Flush();
            var secondRequestStream = new MemoryStream();
            var secondRequestWriter = new StreamWriter(secondRequestStream);
            secondRequestWriter.Write(SecondRequestBody);
            secondRequestWriter.Flush();

            // When
            var result = browser.Post("/", with =>
            {
                with.HttpRequest();
                with.Body(firstRequestStream, "text/plain");
            }).Then.Post("/", with =>
            {
                with.HttpRequest();
                with.Body(secondRequestStream, "text/plain");
            });

            // Then
            result.Body.AsString().ShouldEqual(SecondRequestBody);
        }

        [Fact]
        public void Should_maintain_cookies_when_chaining_requests()
        {
            // Given
            // When
            var result = browser.Get(
                    "/session",
                    with => with.HttpRequest())
                .Then
                .Get(
                    "/session",
                    with => with.HttpRequest());

            result.Body.AsString().ShouldEqual("Current session value is: I've created a session!");
        }

        [Fact]
        public void Should_maintain_cookies_even_if_not_set_on_directly_preceding_request()
        {
            // Given
            // When
            var result = browser.Get(
                    "/session",
                    with => with.HttpRequest())
                .Then
                .Get(
                    "/nothing",
                    with => with.HttpRequest())
                .Then
                .Get(
                    "/session",
                    with => with.HttpRequest());

            result.Body.AsString().ShouldEqual("Current session value is: I've created a session!");
        }

        [Fact]
        public void Should_be_able_to_not_specify_delegate_for_basic_http_request()
        {
            var result = browser.Get("/type");

            result.Body.AsString().ShouldEqual("http");
        }

        [Fact]
        public void Should_add_ajax_header()
        {
            var result = browser.Get("/ajax", with => with.AjaxRequest());

            result.Body.AsString().ShouldEqual("ajax");
        }

        public class EchoModel
        {
            public string SomeString { get; set; }
            public int SomeInt { get; set; }
            public bool SomeBoolean { get; set; }
        }

        public class EchoModule : NancyModule
        {
            public EchoModule()
            {

                Post["/"] = ctx =>
                    {
                        var body = new StreamReader(Context.Request.Body).ReadToEnd();
                        return new Response
                                {
                                    Contents = stream =>
                                                {
                                                    var writer = new StreamWriter(stream);
                                                    writer.Write(body);
                                                    writer.Flush();
                                                }
                                };
                    };

                Get["/cookie"] = ctx =>
                {
                    var response = (Response)"Cookies";

                    foreach (var cookie in Request.Cookies)
                    {
                        response.AddCookie(cookie.Key, cookie.Value);
                    }

                    return response;
                };

                Get["/nothing"] = ctx => string.Empty;

                Get["/session"] = ctx =>
                    {
                        var value = Session["moo"] ?? "";

                        var output = "Current session value is: " + value;

                        if (string.IsNullOrEmpty(value.ToString()))
                        {
                            Session["moo"] = "I've created a session!";
                        }

                        var response = (Response)output;

                        return response;
                    };

                Get["/type"] = _ => Request.Url.Scheme.ToLower();

                Get["/ajax"] = _ => this.Request.IsAjaxRequest() ? "ajax" : "not-ajax";
            }

        }
    }
}