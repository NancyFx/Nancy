//namespace Nancy.Testing.Tests
//{
//    using System;
//    using System.IO;
//    using System.Linq;
//    using Nancy.Tests;
//    using Xunit;

//    public class BrowserFixture
//    {
//        private readonly Browser browser;

//        public BrowserFixture()
//        {
//            var bootstrapper =
//                new DefaultNancyBootstrapper();

//            this.browser = new Browser(bootstrapper);
//        }

//        [Fact]
//        public void Should_just_work_got_damit()
//        {
//            var context = browser.Get("/", (with) =>
//            {
//                with.Header("Content-Length", "0");
//                with.Header("Accepted-Type", "text/plain");
//                with.HttpRequest();
//            });

//            context.Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
//        }

//        [Fact]
//        public void Should_work_with_post_requests()
//        {
//            // Given, When
//            var context = browser.Post("/fake/", (with) =>
//            {
//                with.HttpRequest();
//            });

//            // Then
//            context.Response.Contents.AsString().ShouldEqual("Action result");
//        }

//        [Fact]
//        public void Should_add_multipart_formdata_encoded_files_to_request_filestream()
//        {
//            // When
//            var stream =
//                CreateFakeFileStream("This is the contents of a file");

//            var multipart = new BrowserContextMultipartFormData(x =>
//            {
//                x.AddFile("foo", "foo.txt", "text/plain", stream);
//            });

//            // Given
//            var context = browser.Get("/", (with) =>
//            {
//                with.HttpRequest();
//                with.MultiPartFormData(multipart);
//            });

//            // Then
//            context.Request.Files.ShouldHaveCount(1);
//            context.Request.Files.First().ContentType.ShouldEqual("text/plain");
//            context.Request.Files.First().Name.ShouldEqual("foo.txt");
//            context.Request.Files.First().Value.AsString().ShouldEqual("This is the contents of a file");
//        }

//        private static MemoryStream CreateFakeFileStream(string content)
//        {
//            var stream = new MemoryStream();
//            var writer = new StreamWriter(stream);

//            writer.Write(content);
//            writer.Flush();
//            stream.Position = 0;

//            return stream;
//        }
//    }

//    public static class AssertExtensions
//    {
//        public static string AsString(this Stream source)
//        {
//            var reader =
//                new StreamReader(source);

//            return reader.ReadToEnd();
//        }

//        public static string AsString(this Action<Stream> source)
//        {
//            var stream =
//                new MemoryStream();

//            source.Invoke(stream);
//            stream.Position = 0;

//            var reader =
//                new StreamReader(stream);

//            return reader.ReadToEnd();
//        }
//    }
//}