using System.IO;
using Nancy.Tests;
using Xunit;

namespace Nancy.Testing.Tests
{
    public class BrowserFixture
    {
        private readonly Browser browser;

        public BrowserFixture()
        {
            var bootstrapper =
                new ConfigurableBootstrapper(config => config.Modules(typeof (EchoModule)));

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
                                               with.Body(stream);
                                           });

            // Then
            result.Body.AsString().ShouldEqual(thisIsMyRequestBody);
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
            }

        }
    }
}