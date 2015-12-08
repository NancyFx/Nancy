namespace Nancy.Tests.Unit
{
    using System.IO;
    using System.Text;

    using Xunit;

    public class HttpMultipartBoundaryFixture
    {
        [Fact]
        public void Should_extract_name_from_boundary_headers()
        {
            // Given
            var stream = BuildStreamForSingleFile(
                "name", 
                null,
                null,
                null
                );

            // When
            var boundary = new HttpMultipartBoundary(stream);

            // Then
            boundary.Name.ShouldEqual("name");
        }

        [Fact]
        public void Should_handle_non_ASCII_filenames()
        {
            // Given
            var stream = BuildStreamForSingleFile(
                "file_content",
                "Данные.txt",
                "application/octet-stream",
                null
                );

            // When
            var boundary = new HttpMultipartBoundary(stream);

            // Then
            boundary.Filename.ShouldEqual("Данные.txt");
        }

        [Fact]
        public void Should_set_file_name_to_empty_when_it_could_not_be_found_in_header()
        {
            // Given
            var stream = BuildStreamForSingleFile(
                "name",
                null,
                null,
                null
                );

            // When
            var boundary = new HttpMultipartBoundary(stream);

            // Then
            boundary.Filename.ShouldBeEmpty();
        }

        [Fact]
        public void Should_extract_file_name_from_boundary_headers_when_available()
        {
            // Given
            var stream = BuildStreamForSingleFile(
                "name",
                "sample.txt",
                null,
                null
                );

            // When
            var boundary = new HttpMultipartBoundary(stream);

            // Then
            boundary.Filename.ShouldEqual("sample.txt");
        }

        [Fact]
        public void Should_set_content_type_to_empty_when_it_could_not_be_found_in_header()
        {
            // Given
            var stream = BuildStreamForSingleFile(
                "name",
                null,
                null,
                null
                );

            // When
            var boundary = new HttpMultipartBoundary(stream);

            // Then
            boundary.ContentType.ShouldBeEmpty();
        }

        [Fact]
        public void Should_extract_content_type_from_boundary_headers_when_available()
        {
            // Given
            var stream = BuildStreamForSingleFile(
                "name",
                null,
                "text/plain",
                null
                );

            // When
            var boundary = new HttpMultipartBoundary(stream);

            // Then
            boundary.ContentType.ShouldEqual("text/plain");
        }

        [Fact]
        public void Should_extract_value_from_boundary_when_available()
        {
            // Given
            var stream = BuildStreamForSingleFile(
                "name",
                "sample.txt",
                "text/plain",
                "This is some contents"
                );

            // When
            var boundary = new HttpMultipartBoundary(stream);

            // Then
            GetValueAsString(boundary.Value).ShouldEqual("This is some contents");
        }

        [Fact]
        public void Should_extract_empty_value_from_boundary_when_not_available()
        {
            // Given
            var stream = BuildStreamForSingleFile(
                "name",
                "sample.txt",
                "text/plain",
                null
                );

            // When
            var boundary = new HttpMultipartBoundary(stream);

            // Then
            GetValueAsString(boundary.Value).ShouldBeEmpty();
        }

        private static string GetValueAsString(Stream value)
        {
            return new StreamReader(value, Encoding.UTF8).ReadToEnd();
        }

        private static HttpMultipartSubStream BuildStreamForSingleFile(string name, string filename, string contentType, string content)
        {
            var memory = new MemoryStream(BuildBoundaryWithSingleFile(name, filename, contentType, content));

            return new HttpMultipartSubStream(memory, 0, memory.Length);
        }

        private static byte[] BuildBoundaryWithSingleFile(string name, string filename, string contentType, string content)
        {
            var boundaryBuilder = new StringBuilder();

            boundaryBuilder.AppendFormat(
                !string.IsNullOrEmpty(filename)
                    ? "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\")"
                    : "Content-Disposition: form-data; name=\"{0}\")", name, filename);

            boundaryBuilder.Append('\r');
            boundaryBuilder.Append('\n');

            if (!string.IsNullOrEmpty(contentType))
            {
                boundaryBuilder.AppendFormat("Content-Type: {0}", contentType);
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
            }

            if (!string.IsNullOrEmpty(content))
            {
                boundaryBuilder.Append('\r');
                boundaryBuilder.Append('\n');
                boundaryBuilder.Append(content);
            }

            return Encoding.UTF8.GetBytes(boundaryBuilder.ToString());
        }
    }
}