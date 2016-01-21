namespace Nancy.Tests.Unit.Responses
{
    using System.Collections.Generic;
    using System.IO;

    using Nancy.Configuration;
    using Nancy.Responses;

    using Xunit;

    public class GenericFileResponseFixture
    {
        private readonly string imagePath;
        private const string imageContentType = "image/png";
        private readonly NancyContext context;

        public GenericFileResponseFixture()
        {
            var assemblyPath =
                Path.GetDirectoryName(this.GetType().Assembly.Location);
            
            var envrionment = new DefaultNancyEnvironment();
            envrionment.StaticContent(assemblyPath);

            this.context = new NancyContext { Environment = envrionment };

            this.imagePath =
                Path.GetFileName(this.GetType().Assembly.Location);
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_name_is_empty()
        {
            // Given, When
            var response = new GenericFileResponse(string.Empty, imageContentType, this.context);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_name_is_null()
        {
            // Given, When
            var response = new GenericFileResponse(null, imageContentType, this.context);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_name_does_not_contain_extension()
        {
            // Given
            var path = Path.Combine("Resources", "zip");

            // When
            var response = new GenericFileResponse(path, imageContentType, this.context);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_does_not_exist()
        {
            // Given
            var path = Path.Combine("Resources", "thatsnotit.jpg");

            // When
            var response = new GenericFileResponse(path, imageContentType, this.context);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_is_above_root_path()
        {
            // Given
            var path = 
                Path.Combine(this.imagePath, "..", "..");

            // When
            var response = new GenericFileResponse(path, imageContentType, this.context);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_ok()
        {
            // Given, When
            var response = new GenericFileResponse(this.imagePath, imageContentType, this.context);
                        
            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_return_file_unchanged()
        {
            // Given
            var expected = File.ReadAllBytes(this.imagePath);
            var response = new GenericFileResponse(this.imagePath, imageContentType, this.context);
            
            // When
            var result = GetResponseContents(response);
            
            // Then
            result.ShouldEqualSequence(expected);
        }

        [Fact]
        public void Should_set_filename_property_to_filename()
        {
            // Given, When
            var response = new GenericFileResponse(this.imagePath, imageContentType, this.context);

            // Then
            response.Filename.ShouldEqual(Path.GetFileName(this.imagePath));
        }

        [Fact]
        public void Should_contain_etag_in_response_header()
        {
            // Given, when
            var response =
                new GenericFileResponse(this.imagePath, imageContentType, this.context);

            // Then
            response.Headers["ETag"].ShouldStartWith("\"");
            response.Headers["ETag"].ShouldEndWith("\"");
        }

        [Fact]
        public void Should_set_content_length_in_response_header()
        {
            // Given, when
            var expected = new FileInfo(imagePath).Length.ToString();
            var response =
                new GenericFileResponse(this.imagePath, imageContentType, this.context);

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
