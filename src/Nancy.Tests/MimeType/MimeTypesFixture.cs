using Xunit;

namespace Nancy.Tests.MimeType
{
    public class MimeTypesFixture
    {
        [Fact] public void Should_return_appropriate_mime_for_common_extensions()
        {
            MimeTypes.GetMimeType(".js").ShouldEqual("application/x-javascript");
            MimeTypes.GetMimeType(".css").ShouldEqual("text/css");
            MimeTypes.GetMimeType(".png").ShouldEqual("image/png");
            MimeTypes.GetMimeType(".gif").ShouldEqual("image/gif");
            MimeTypes.GetMimeType(".jpg").ShouldEqual("image/jpeg");
            MimeTypes.GetMimeType(".xml").ShouldEqual("application/xml");
        }

        [Fact] public void Should_return_octet_stream_if_unknown_mime_type()
        {
            MimeTypes.GetMimeType(".crazyext").ShouldEqual("application/octet-stream");
        }

        [Fact] public void Should_return_appropriate_mime_for_numerical_extensions()
        {
            MimeTypes.GetMimeType(".323").ShouldEqual("text/h323");
        }

        [Fact] public void Should_return_appropriate_mime_for_Special_char_extensions()
        {
            MimeTypes.GetMimeType(".c++").ShouldEqual("text/plain");
        }
    }
}
