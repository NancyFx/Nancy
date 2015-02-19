namespace Nancy.Tests.Unit
{
    using System.IO;
    using System.Text;

    using FakeItEasy;

    using Xunit;

    public class TextFormatterFixture
    {
        private readonly IResponseFormatter formatter;
        private readonly Response response;

        public TextFormatterFixture()
        {
            this.formatter = A.Fake<IResponseFormatter>();
            this.response = this.formatter.AsText("sample text");
        }

        [Fact]
        public void Should_return_a_response_with_content_type_text_plain()
        {
            response.ContentType.ShouldEqual("text/plain");
        }

        [Fact]
        public void Should_return_a_response_with_status_code_200_ok()
        {
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Fact]
        public void Should_set_return_valid_response_string()
        {
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);

                Encoding.UTF8.GetString(stream.ToArray()).ShouldEqual("sample text");
            }
        }

        [Fact]
        public void Should_override_the_content_type()
        {
            var response = formatter.AsText("sample text", "text/cache-manifest");
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                response.ContentType.ShouldEqual("text/cache-manifest");
            }
        }
    }
}