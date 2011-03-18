using Nancy.MimeType;
using Xunit;

namespace Nancy.Tests.MimeType
{
    public class DefaultMimeTypeRegistryFixture
    {
        private readonly IMimeTypeRegistry r = new DefaultMimeTypeRegistry();

        [Fact] public void StandardExtensionsAreAsExpected()
        {
            r.GetMimeTypeFor("js").ShouldEqual("application/javascript");
            r.GetMimeTypeFor("css").ShouldEqual("text/css");
            r.GetMimeTypeFor("png").ShouldEqual("image/png");
            r.GetMimeTypeFor("gif").ShouldEqual("image/gif");
            r.GetMimeTypeFor("jpg").ShouldEqual("image/jpeg");
        }

        [Fact] public void NumbericalExtensionsSupported()
        {
            r.GetMimeTypeFor("3ds").ShouldEqual("image/x-3ds");
        }

        [Fact] public void SpecialCharacterExtensionsSupported()
        {
            r.GetMimeTypeFor("c++").ShouldEqual("text/x-c++src");
            r.GetMimeTypeFor("h++").ShouldEqual("text/x-c++hdr");
        }
    }
}
