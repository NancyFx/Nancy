using Nancy.Responses;

namespace Nancy.Tests.Responses
{
  using System.IO;
  using Xunit;

  public class ResponseConsistencyFixture
  {
    private const string imagePath = @"Responses\testfiles\zip.png";
    private const string imageContentType = "image/png";

    private const string txtPath = @"Responses\testfiles\test.txt";
    private const string txtContentType = "text/txt";

    [Fact] public void GenericFileResponse_consistency() { ConsistencyTestImpl(imagePath, new GenericFileResponse(imagePath, imageContentType)); }

    private static void ConsistencyTestImpl(string filePath, Response r)
    {
      Assert.Equal(HttpStatusCode.OK, r.StatusCode);

      byte[] expected = File.ReadAllBytes(filePath);
      
      MemoryStream ms = new MemoryStream();
      r.Contents(ms);
      ms.Flush();
      byte[] actual = ms.ToArray();
          
      Assert.Equal(expected, actual);
    }
  }
}
