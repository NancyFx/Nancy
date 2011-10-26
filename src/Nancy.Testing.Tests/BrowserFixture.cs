namespace Nancy.Testing.Tests
{
    using System.IO;
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
            }

        }
    }
}