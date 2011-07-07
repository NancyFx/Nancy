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
			this.imagePath = Path.Combine(@"..", @"..", "Resources", "zip.png");
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
		
		private static IEnumerable<byte> GetResponseContents(Response response)
		{
			var ms = new MemoryStream();
			response.Contents(ms);
			ms.Flush();
			
			return ms.ToArray();
		}
	}
}
