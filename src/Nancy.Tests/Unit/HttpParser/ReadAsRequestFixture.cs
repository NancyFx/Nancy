namespace Nancy.Tests.Unit.HttpParser
{
    using System;
    using System.IO;
    using System.Text;
    using Nancy.HttpParser;
    using Xunit;
    using Xunit.Extensions;

    public class ReadAsRequestFixture
    {
        [Fact]
        public void When_stream_is_null_throws_argument_null_exception()
        {
            // given
            Stream stream = null;

            // when/then
            var exception = Assert.Throws<ArgumentNullException>(() => stream.ReadAsRequest());
            exception.ParamName.ShouldEqual("stream");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void When_scheme_is_empty_or_whitespace_throws_argument_null_exception(string scheme)
        {
            // Given
            Stream stream = new MemoryStream();

            // when/then
            var exception = Assert.Throws<ArgumentNullException>(() => stream.ReadAsRequest(scheme: scheme));
            exception.ParamName.ShouldEqual("scheme");
        }

        [Fact]
        public void RelativeUrlGetFixture()
        {
            // given
            var rawRequestStream = new MemoryStream(Encoding.UTF8.GetBytes(@"GET / HTTP/1.1
User-Agent: Fiddler
Host: nancyfx.org

"));

            // when
            var result = rawRequestStream.ReadAsRequest();

            // then
            result.Method.ShouldEqual("GET");

            result.Url.Scheme.ShouldEqual("http");
            result.Url.Path.ShouldEqual("/");

            result.Headers.ShouldHaveCount(2);

            result.Body.Length.ShouldEqual(0L);
            result.UserHostAddress.ShouldBeNull();
        }

        [Fact]
        public void RelativeUrlPostFixture()
        {
            // given
            var rawRequestStream = new MemoryStream(Encoding.UTF8.GetBytes(@"POST / HTTP/1.1
User-Agent: Fiddler
Host: nancyfx.org
Content-Length: 4

asdf"));

            // when
            var result = rawRequestStream.ReadAsRequest();

            // then
            result.Method.ShouldEqual("POST");

            result.Url.Scheme.ShouldEqual("http");
            result.Url.Path.ShouldEqual("/");

            result.Headers.ShouldHaveCount(3);
            result.UserHostAddress.ShouldBeNull();

            using (var reader = new StreamReader(result.Body))
            {
                reader.ReadToEnd().ShouldEqual("asdf");
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("127.0.0.1")]
        [InlineData("192.168.1.5")]
        public void Should_correctly_set_user_host_address(string ip)
        {
            // given
            var rawRequestStream = new MemoryStream(Encoding.UTF8.GetBytes(@"POST / HTTP/1.1
User-Agent: Fiddler
Host: nancyfx.org
Content-Length: 4

asdf"));

            // when
            var result = rawRequestStream.ReadAsRequest(ip: ip);

            // then
            result.UserHostAddress.ShouldEqual(ip);
        }

        [Fact]
        public void RelativeUrlGetFixtureWithSchemeAndIp()
        {
            // given
            var rawRequestStream = new MemoryStream(Encoding.UTF8.GetBytes(
@"GET /search?q=nancy&oq=nancy&aqs=chrome.0.57j61l3j62l2.1586&sugexp=chrome,mod=9&sourceid=chrome&ie=UTF-8 HTTP/1.1
Host: www.google.com
Connection: keep-alive
User-Agent: Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.64 Safari/537.11
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
X-Chrome-Variations: COu1yQEIjrbJAQiZtskBCKa2yQEIqLbJAQiptskBCL62yQEIuYPKAQjkg8oB
Accept-Encoding: gzip,deflate,sdch
Accept-Language: en-US,en;q=0.8
Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3
Cookie: HSID=AcDWbTlAk2te13e6v; SSID=ADlPlPQUdYU9sYFko; APISID=CINzVUNGqU9XeH7Z/AyUAw5-o8-dHmiYXD; SAPISID=RhfYRwz0cLjNLWlx/A2d_HwfJlHpXwhoDx; PREF=ID=fbc069c5fbe31ad0:U=049c4423c184f188:FF=0:LD=en:TM=1351297728:LM=1352001209:GM=1:S=rKPgbpqXvNMVD8-E; NID=66=ARPHDECrPAihN0H5dE_ee2xU1G77IXnqNJfeze8nPTZJNJ8xkVQZIVDCctexNW3gx0Nhg0lZE9u_m4fVPvg_d4gl9vcqR1b01K1J5ypZixaYEbDXXDyanlzKB3dYxBWjV8HYWjekJ23kdYlk9EDOELe_6dveiD_sPkI4D11OyQ4Yv2fziR9s4batoDvUOg-TZwR7M-CO; SID=DQAAAIMBAADNixH7WUPsOcqiSiRiyuTPqJWF8ApoPQEyhFCgM-yS8pjY7cZKiQZ473IvCpaKLqRshKkv-UFhLsxbc5tC11G0gN4CbCHwzKv18Y2c5SbHU2hf9Tfy6GaqSu9UV4zbYhS94E7jMzQRM-zJxOerBS_RInfjZZX0b8ZKYMf6AmpdYDOCkMhWopCaVZdxIkIcJ-wkstggYx76Nzbirtf7kojGHdsNYFeIVt9nTEjVITOLDiT2Qobn7lR7-UQA0l6xL0o7A_9zAH341OO8jvcwL-7w2auxAHcjuyZJm5MtbKlvJlMA1vflNOJsnAAdLJpiDZHjAYWdiclORrxisQHdR-LbAkTDRq6gIgYOrToAfQ8wrvKaIruGEWbYzjYU2rhVcmB6lzkvk_02-zNVzxcvCFh66MiTql0bFVNl8mSqA9JBjW-jMiw2sHS9zeqWhjuhvidWUnSRLLYoUOkSCu5U6QtrFVgj7UgzugQtep7AX0xAaPnY-gvn6nflm6O90Vk8D0AlHRfSqpKPpytfKi6PuVA-

"));

            // when
            var result = rawRequestStream.ReadAsRequest(scheme: "https", ip: "192.168.0.1");

            // then
            result.Url.BasePath.ShouldBeEmpty();
            result.Url.Fragment.ShouldBeEmpty();
            result.Url.HostName.ShouldEqual("www.google.com");
            result.Url.Path.ShouldEqual("/search");
            result.Url.Port.ShouldBeNull();
            result.Url.Query.ShouldEqual("q=nancy&oq=nancy&aqs=chrome.0.57j61l3j62l2.1586&sugexp=chrome,mod=9&sourceid=chrome&ie=UTF-8");
            result.Url.Scheme.ShouldEqual("https");

            result.UserHostAddress.ShouldEqual("192.168.0.1");

            result.Headers.ShouldHaveCount(9);
            result.Headers.Host.ShouldEqual("www.google.com");
            result.Headers.Connection.ShouldEqual("keep-alive");
            result.Headers.UserAgent.ShouldEqual("Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.64 Safari/537.11");
            result.Headers["Accept"].ShouldEqualSequence(new[] { "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8" });
            result.Headers["X-Chrome-Variations"].ShouldEqualSequence(new[] { "COu1yQEIjrbJAQiZtskBCKa2yQEIqLbJAQiptskBCL62yQEIuYPKAQjkg8oB" });
            result.Headers.AcceptEncoding.ShouldEqualSequence(new[] { "gzip", "deflate", "sdch" });
            result.Headers["Accept-Language"].ShouldEqualSequence(new[] { "en-US,en;q=0.8" });
            result.Headers["Accept-Charset"].ShouldEqualSequence(new[] { "ISO-8859-1,utf-8;q=0.7,*;q=0.3" });
            result.Headers["Cookie"].ShouldEqualSequence(new[] { "HSID=AcDWbTlAk2te13e6v; SSID=ADlPlPQUdYU9sYFko; APISID=CINzVUNGqU9XeH7Z/AyUAw5-o8-dHmiYXD; SAPISID=RhfYRwz0cLjNLWlx/A2d_HwfJlHpXwhoDx; PREF=ID=fbc069c5fbe31ad0:U=049c4423c184f188:FF=0:LD=en:TM=1351297728:LM=1352001209:GM=1:S=rKPgbpqXvNMVD8-E; NID=66=ARPHDECrPAihN0H5dE_ee2xU1G77IXnqNJfeze8nPTZJNJ8xkVQZIVDCctexNW3gx0Nhg0lZE9u_m4fVPvg_d4gl9vcqR1b01K1J5ypZixaYEbDXXDyanlzKB3dYxBWjV8HYWjekJ23kdYlk9EDOELe_6dveiD_sPkI4D11OyQ4Yv2fziR9s4batoDvUOg-TZwR7M-CO; SID=DQAAAIMBAADNixH7WUPsOcqiSiRiyuTPqJWF8ApoPQEyhFCgM-yS8pjY7cZKiQZ473IvCpaKLqRshKkv-UFhLsxbc5tC11G0gN4CbCHwzKv18Y2c5SbHU2hf9Tfy6GaqSu9UV4zbYhS94E7jMzQRM-zJxOerBS_RInfjZZX0b8ZKYMf6AmpdYDOCkMhWopCaVZdxIkIcJ-wkstggYx76Nzbirtf7kojGHdsNYFeIVt9nTEjVITOLDiT2Qobn7lR7-UQA0l6xL0o7A_9zAH341OO8jvcwL-7w2auxAHcjuyZJm5MtbKlvJlMA1vflNOJsnAAdLJpiDZHjAYWdiclORrxisQHdR-LbAkTDRq6gIgYOrToAfQ8wrvKaIruGEWbYzjYU2rhVcmB6lzkvk_02-zNVzxcvCFh66MiTql0bFVNl8mSqA9JBjW-jMiw2sHS9zeqWhjuhvidWUnSRLLYoUOkSCu5U6QtrFVgj7UgzugQtep7AX0xAaPnY-gvn6nflm6O90Vk8D0AlHRfSqpKPpytfKi6PuVA-" });

            result.Cookies.ShouldHaveCount(7);
            result.Cookies["HSID"].ShouldEqual("AcDWbTlAk2te13e6v");
            result.Cookies["SSID"].ShouldEqual("ADlPlPQUdYU9sYFko");

            result.Body.ShouldNotBeNull();
            result.Body.Length.ShouldEqual(0L);
        }
    }
}