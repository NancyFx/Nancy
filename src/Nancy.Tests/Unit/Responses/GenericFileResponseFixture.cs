namespace Nancy.Tests.Unit.Responses
{
    using System.Collections.Generic;
    using System.IO;
    using Nancy.Responses;
    using Xunit;

    public class GenericFileResponseFixture
	{
		private readonly string imagePath;
        private const string imageContentType = "image/png";

        public GenericFileResponseFixture()
        {
            var assemblyPath =
                Path.GetDirectoryName(this.GetType().Assembly.Location);

            GenericFileResponse.SafePaths = new List<string> {assemblyPath};

			this.imagePath =
                Path.GetFileName(this.GetType().Assembly.Location);
		}

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_name_is_empty()
        {
            // Given, When
            var response = new GenericFileResponse(string.Empty, imageContentType);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_name_is_null()
        {
            // Given, When
            var response = new GenericFileResponse(null, imageContentType);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_name_does_not_contain_extension()
        {
            // Given
            var path = Path.Combine("Resources", "zip");

            // When
            var response = new GenericFileResponse(path, imageContentType);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_set_status_code_to_not_found_when_file_does_not_exist()
        {
            // Given
            var path = Path.Combine("Resources", "thatsnotit.jpg");

            // When
            var response = new GenericFileResponse(path, imageContentType);

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
            var response = new GenericFileResponse(path, imageContentType);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

		[Fact]
		public void Should_set_status_code_to_ok()
		{
			// Given, When
			var response = new GenericFileResponse(this.imagePath, imageContentType);
						
			// Then
			response.StatusCode.ShouldEqual(HttpStatusCode.OK);
		}
		
		[Fact]
		public void Should_return_file_unchanged()
		{
			// Given
			var expected = File.ReadAllBytes(this.imagePath);
            var response = new GenericFileResponse(this.imagePath, imageContentType);
			
			// When
			var result = GetResponseContents(response);
			
			// Then
			result.ShouldEqualSequence(expected);
		}
        
        [Fact]
        public void Should_set_filename_property_to_filename()
        {
            // Given, When
            var response = new GenericFileResponse(this.imagePath, imageContentType);

            // Then
            response.Filename.ShouldEqual(Path.GetFileName(this.imagePath));
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
