namespace Nancy.Embedded.Tests.Unit
{
    using System.IO;
    using System.Reflection;
    using System.Text;

    using Nancy.Diagnostics;
    using Nancy.Embedded.Conventions;
    using Nancy.Responses;
    using Nancy.Tests;

    using Xunit;

    public class EmbeddedStaticContentConventionBuilderFixture
    {
        [Fact]
        public void Should_retrieve_static_content_with_urlencoded_dot()
        {
            // Given
            // When
            var result = GetEmbeddedStaticContent("Foo", "embedded%2etxt");

            // Then
            result.ShouldEqual("Embedded Text");
        }

        [Fact]
        public void Should_retrieve_static_content_in_subfolder()
        {
            // Given
            // When
            var result = GetEmbeddedStaticContent("Foo", "Subfolder/embedded2.txt");

            // Then
            result.ShouldEqual("Embedded2 Text");
        }

        [Fact]
        public void Should_retrieve_static_content_with_hyphens_in_subfolder()
        {
            // Given
            // When
            var result = GetEmbeddedStaticContent("Foo", "Subfolder-with-hyphen/embedded3.txt");

            // Then
            result.ShouldEqual("Embedded3 Text");
        }

        [Fact]
        public void Should_retrieve_static_content_with_relative_path()
        {
            // Given
            // When
            var result = GetEmbeddedStaticContent("Foo", "Subfolder/../embedded.txt");

            // Then
            result.ShouldEqual("Embedded Text");
        }

        [Fact]
        public void Should_retrieve_static_content_ignoring_casing()
        {
            // Given
            // When
            var result = GetEmbeddedStaticContent("Foo", "Subfolder/embedded2.txt", "resources");

            // Then
            result.ShouldEqual("Embedded2 Text");
        }

        private static string GetEmbeddedStaticContent(string virtualDirectory, string requestedFilename, string contentPath = null)
        {
            var resource =
                string.Format("/{0}/{1}", virtualDirectory, requestedFilename);

            var context =
                new NancyContext
                {
                    Request = new Request("GET", resource, "http"),
                    Trace = new DefaultRequestTrace
                    {
                        TraceLog = new DefaultTraceLog()
                    }
                };

            var assembly =
                Assembly.GetExecutingAssembly();

            var resolver =
                EmbeddedStaticContentConventionBuilder.AddDirectory(virtualDirectory, assembly, contentPath ?? "Resources");

            var response =
                resolver.Invoke(context, null) as EmbeddedFileResponse;

            if (response != null)
            {
                using (var stream = new MemoryStream())
                {
                    response.Contents(stream);
                    return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                }
            }

            return null;
        }
    }
}