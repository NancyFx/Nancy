namespace Nancy.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Fakes;
    using IO;
    using Nancy.Bootstrapper;
    using Xunit;

    public class BrowserTestFixture
    {
        private readonly Browser browser;

        public BrowserTestFixture()
        {
            var bootstrapper = 
                new FakeDefaultNancyBootstrapper();

            this.browser = new Browser(bootstrapper);
        }

        [Fact]
        public void Should_just_work_got_damit()
        {
            var context = browser.Get("/", (with) => {
                with.Header("Content-Length", "0");
                with.Header("Accepted-Type", "text/plain");
                with.HttpRequest();
            });

            context.Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_work_with_post_requests()
        {
            // Given, When
            var context = browser.Post("/fake/", (with) => {
                with.HttpRequest();
            });

            // Then
            context.Response.Contents.AsString().ShouldEqual("Action result");
        }

        [Fact]
        public void Should_add_multipart_formdata_encoded_files_to_request_filestream()
        {
            // When
            var stream = 
                CreateFakeFileStream("This is the contents of a file");

            var multipart = new BrowserContextMultipartFormData(x => {
                x.AddFile("foo", "foo.txt", "text/plain", stream);
            });
            
            // Given
            var context = browser.Get("/", (with) => {
                with.HttpRequest();
                with.MultiPartFormData(multipart);
            }); 

            // Then
            context.Request.Files.ShouldHaveCount(1);
            context.Request.Files.First().ContentType.ShouldEqual("text/plain");
            context.Request.Files.First().Name.ShouldEqual("foo.txt");
            context.Request.Files.First().Value.AsString().ShouldEqual("This is the contents of a file");
        }

        private static MemoryStream CreateFakeFileStream(string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }
    }

    public static class AssertExtensions
    {
        public static string AsString(this Stream source)
        {
            var reader =
                new StreamReader(source);

            return reader.ReadToEnd();
        }

        public static string AsString(this Action<Stream> source)
        {
            var stream = 
                new MemoryStream();

            source.Invoke(stream);
            stream.Position = 0;

            var reader = 
                new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }

    public static class BrowserContextExtensions
    {
        public static void MultiPartFormData(this BrowserContext browserContext, BrowserContextMultipartFormData multipartFormData)
        {
            var contextValues =
                (IBrowserContextValues)browserContext;

            contextValues.Body = multipartFormData.Body;
            contextValues.Headers["Content-Type"] = new[] { "multipart/form-data; boundary=NancyMultiPartBoundary123124" };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Browser : IHideObjectMembers
    {
        private readonly INancyBootstrapper bootstrapper;
        private readonly INancyEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="Browser"/> class.
        /// </summary>
        /// <param name="bootstrapper">A <see cref="INancyBootstrapper"/> instance that determins the Nancy configuration that should be used by the browser.</param>
        public Browser(INancyBootstrapper bootstrapper)
        {
            this.bootstrapper = bootstrapper;
            this.bootstrapper.Initialise();
            this.engine = this.bootstrapper.GetEngine();
        }

        /// <summary>
        /// Performs a DELETE requests against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="NancyContext"/> instance of the executed request.</returns>
        public NancyContext Delete(string path, Action<BrowserContext> browserContext)
        {
            return this.HandleRequest("DELETE", path, browserContext);
        }

        /// <summary>
        /// Performs a GET requests against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="NancyContext"/> instance of the executed request.</returns>
        public NancyContext Get(string path, Action<BrowserContext> browserContext)
        {
            return this.HandleRequest("GET", path, browserContext);
        }

        /// <summary>
        /// Performs a POST requests against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="NancyContext"/> instance of the executed request.</returns>
        public NancyContext Post(string path, Action<BrowserContext> browserContext)
        {
            return this.HandleRequest("POST", path, browserContext);
        }

        /// <summary>
        /// Performs a PUT requests against Nancy.
        /// </summary>
        /// <param name="path">The path that is being requested.</param>
        /// <param name="browserContext">An closure for providing browser context for the request.</param>
        /// <returns>An <see cref="NancyContext"/> instance of the executed request.</returns>
        public NancyContext Put(string path, Action<BrowserContext> browserContext)
        {
            return this.HandleRequest("PUT", path, browserContext);
        }

        private NancyContext HandleRequest(string method, string path, Action<BrowserContext> browserContext)
        {
            var request =
                CreateRequest(method, path, browserContext);

            return this.engine.HandleRequest(request);
        }

        private static Request CreateRequest(string method, string path, Action<BrowserContext> browserContext)
        {
            var context =
                new BrowserContext();

            browserContext.Invoke(context);

            var contextValues =
                (IBrowserContextValues)context;

            var requestStream =
                RequestStream.FromStream(contextValues.Body);

            return new Request(method, path, contextValues.Headers, requestStream, contextValues.Protocol);
        }
    }

    public interface IBrowserContextValues : IHideObjectMembers
    {
        /// <summary>
        /// Gets or sets the that should be sent with the HTTP request.
        /// </summary>
        /// <value>A <see cref="Stream"/> that contains the body that should be sent with the HTTP request.</value>
        Stream Body { get; set; }

        /// <summary>
        /// Gets or sets the headers that should be sent with the HTTP request.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance that contains the headers that should be sent with the HTTP request.</value>
        IDictionary<string, IEnumerable<string>> Headers { get; set; }

        /// <summary>
        /// Gets or sets the protocol that should be sent with the HTTP request.
        /// </summary>
        /// <value>A <see cref="string"/> contains the the protocol that should be sent with the HTTP request..</value>
        string Protocol { get; set; }
    }

    public class BrowserContext : IBrowserContextValues
    {
        public BrowserContext()
        {
            this.Values.Headers = new Dictionary<string, IEnumerable<string>>();
            this.Values.Protocol = "http";
        }

        public void Header(string name, string value)
        {
            if (!this.Values.Headers.ContainsKey(name))
            {
                this.Values.Headers.Add(name, new List<string>());
            }

            var values = (List<string>)this.Values.Headers[name];
            values.Add(value);

            this.Values.Headers[name] = values;
        }

        public void HttpRequest()
        {
            this.Values.Protocol = "http";
        }

        public void HttpsRequest()
        {
            this.Values.Protocol = "https";
        }

        public void Body(Stream body)
        {
            this.Values.Body = body;
        }

        private IBrowserContextValues Values
        {
            get { return this; }
        }

        /// <summary>
        /// Gets or sets the that should be sent with the HTTP request.
        /// </summary>
        /// <value>A <see cref="Stream"/> that contains the body that should be sent with the HTTP request.</value>
        Stream IBrowserContextValues.Body { get; set; }

        /// <summary>
        /// Gets or sets the protocol that should be sent with the HTTP request.
        /// </summary>
        /// <value>A <see cref="string"/> contains the the protocol that should be sent with the HTTP request..</value>
        string IBrowserContextValues.Protocol { get; set; }

        /// <summary>
        /// Gets or sets the headers that should be sent with the HTTP request.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance that contains the headers that should be sent with the HTTP request.</value>
        IDictionary<string, IEnumerable<string>> IBrowserContextValues.Headers { get; set; }
    }
}