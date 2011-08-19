namespace Nancy.Testing.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FakeItEasy;
    using Nancy;
    using Nancy.Tests;
    using Xunit;

    public class BrowserResponseBodyWrapperFixture
    {
        public BrowserResponseBodyWrapperFixture()
        {
        }

        [Fact]
        public void Should_contain_response_body()
        {
            // Given
            var body = new BrowserResponseBodyWrapper(new Response
            {
                Contents = stream => {
                    var writer = new StreamWriter(stream);
                    writer.Write("This is the content");
                    writer.Flush();
                }
            });

            var content = Encoding.ASCII.GetBytes("This is the content");

            // When
            var result = body.SequenceEqual(content);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_querywrapper_for_css_selector_match()
        {
            // Given
            var body = new BrowserResponseBodyWrapper(new Response
            {
                Contents = stream =>
                {
                    var writer = new StreamWriter(stream);
                    writer.Write("<div>Outer and <div id='#bar'>inner</div></div>");
                    writer.Flush();
                }
            });

            // When
            var result = body["#bar"];

            // Then
            result.ShouldContain("inner");
        }
    }
}