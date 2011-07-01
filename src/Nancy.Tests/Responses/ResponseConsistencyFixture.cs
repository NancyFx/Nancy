namespace Nancy.Tests.Responses
{
	using System.IO;
	using Nancy.Responses;
	using Nancy.Tests;
	using Xunit;
	
	public class ResponseConsistencyFixture
	{
		private readonly string imagePath;
		private string imageContentType = "image/png";
		
		public ResponseConsistencyFixture()
		{
			this.imagePath = Path.Combine("Responses", "testfiles", "zip.png");
		}
		
		[Fact]
		public void Should_set_status_code_to_ok()
		{
			// Given, When
			var response = new GenericFileResponse(this.imagePath, this.imageContentType);
						
			// Then
			response.StatusCode.ShouldEqual(HttpStatusCode.OK);
		}
		
		[Fact]
		public void Should_return_file_unchanged()
		{
			// Given
			var expected = File.ReadAllBytes(this.imagePath);
			var response = new GenericFileResponse(this.imagePath, this.imageContentType);
			
			// When
			var result = GetResponseContents(response);
			
			// Then
			result.ShouldEqualSequence(expected);
		}
		
		private static byte[] GetResponseContents(Response response)
		{
			MemoryStream ms = new MemoryStream();
			response.Contents(ms);
			ms.Flush();
			
			return ms.ToArray();
		}
	}
}
