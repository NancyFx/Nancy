namespace Nancy.Testing.Tests
{
    using System.IO;
    using FakeItEasy;
    using Nancy.Tests;

    using Xunit;

    public class BrowserResponseBodyWrapperExtensionsFixture
    {
        [Fact]
        public void Should_convert_to_string()
        {
            // Given
            var body = new BrowserResponseBodyWrapper(new Response
            {
                Contents = stream =>
                {
                    var writer = new StreamWriter(stream);
                    writer.Write("This is the content");
                    writer.Flush();
                }
            }, A.Dummy<BrowserContext>());

            // When
            var result = body.AsString();

            // Then
            result.ShouldEqual("This is the content");
        }
    }
}