namespace Nancy.Tests.Unit.Responses
{
    using System.Collections.Generic;
    using System.IO;
    using Nancy.Configuration;
    using Nancy.Responses;
    using Xunit;
    using Xunit.Abstractions;

    public class GenericFileResponseFixture
    {
        private readonly string filePath;
        private readonly string fileContentType = "foo/bar";
        private readonly NancyContext context;

        public GenericFileResponseFixture(ITestOutputHelper output)
        {
            var environment = new DefaultNancyEnvironment();
            environment.StaticContent(safepaths: this.GetLocation());

            this.context = new NancyContext { Environment = environment };

            this.filePath = this.GetFilePath();
        }

        private string GetLocation()
        {
#if CORE
            return Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationBasePath;
#else
            return Path.GetDirectoryName(this.GetType().Assembly.Location);
#endif
        }

        private string GetFilePath()
        {
#if CORE
            return Path.Combine("Resources", "test.txt");
#else
            return Path.GetFileName(this.GetType().Assembly.Location);
#endif
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_name_is_empty()
        {
            // Given, When
            var response = new GenericFileResponse(string.Empty, this.fileContentType, this.context);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_name_is_null()
        {
            // Given, When
            var response = new GenericFileResponse(null, this.fileContentType, this.context);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_name_does_not_contain_extension()
        {
            // Given
            var fileNameWithoutExtensions = Path.GetFileNameWithoutExtension(this.filePath);

            // When
            var response = new GenericFileResponse(fileNameWithoutExtensions, this.fileContentType, this.context);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_does_not_exist()
        {
            // Given
            // When
            var response = new GenericFileResponse("nancy", this.fileContentType, this.context);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_is_above_root_path()
        {
            // Given
            var path =
                Path.Combine(this.filePath, "..", "..");

            // When
            var response = new GenericFileResponse(path, this.fileContentType, this.context);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_ok()
        {
            // Given
            // When
            var response = new GenericFileResponse(this.filePath, "text/plain", this.context);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_return_file_unchanged()
        {
            // Given
            var path = Path.Combine(this.GetLocation(), this.filePath);
            var expected = File.ReadAllBytes(path);
            var response = new GenericFileResponse(this.filePath, this.fileContentType, this.context);

            // When
            var result = GetResponseContents(response);

            // Then
            result.ShouldEqualSequence(expected);
        }

        [Fact]
        public void Should_set_filename_property_to_filename()
        {
            // Given
            // When
            var response = new GenericFileResponse(this.filePath, this.fileContentType, this.context);

            // Then
            response.Filename.ShouldEqual(Path.GetFileName(this.filePath));
        }

        [Fact]
        public void Should_contain_etag_in_response_header()
        {
            // Given
            // When
            var response = new GenericFileResponse(this.filePath, this.fileContentType, this.context);

            // Then
            response.Headers["ETag"].ShouldStartWith("\"");
            response.Headers["ETag"].ShouldEndWith("\"");
        }

        [Fact]
        public void Should_set_content_length_in_response_header()
        {
            // Given, when
            var path = Path.Combine(this.GetLocation(), this.filePath);
            var expected = new FileInfo(path).Length.ToString();
            var response = new GenericFileResponse(this.filePath, this.fileContentType, this.context);

            // Then
            response.Headers["Content-Length"].ShouldEqual(expected);
        }

        private static IEnumerable<byte> GetResponseContents(Response response)
        {
            var ms = new MemoryStream();
            response.Contents(ms);
            ms.Flush();

            return ms.ToArray();
        }
    }
}
