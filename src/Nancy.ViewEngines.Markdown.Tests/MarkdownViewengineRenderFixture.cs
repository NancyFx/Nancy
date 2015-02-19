namespace Nancy.ViewEngines.Markdown.Tests
{
    using Nancy.Tests;

    using Xunit;

    public class MarkdownViewengineRenderFixture
    {
        [Fact]
        public void Should_handle_onload_and_css_attributes_on_body_tag()
        {
            //Given
            var html = @"<!DOCTYPE html><html xmlns='http://www.w3.org/1999/xhtml'><head></head><body class='mybodyclass'>#Markdown</body></html>";

            var expected = @"<!DOCTYPE html><html xmlns='http://www.w3.org/1999/xhtml'><head></head><body class='mybodyclass'><h1>Markdown</h1></body></html>";

            //When            
            var result = MarkdownViewengineRender.RenderMasterPage(html);

            //Then
            result.ShouldEqual(expected);
        }

        [Fact]
        public void Should_handle_empty_body_tag()
        {
            //Given
            var html = @"<!DOCTYPE html><html xmlns='http://www.w3.org/1999/xhtml'><head></head><body>#Markdown</body></html>";

            var expected = @"<!DOCTYPE html><html xmlns='http://www.w3.org/1999/xhtml'><head></head><body><h1>Markdown</h1></body></html>";

            //When            
            var result = MarkdownViewengineRender.RenderMasterPage(html);

            //Then
            result.ShouldEqual(expected);
        }
    }
}
