namespace Nancy.Tests.Unit
{
    using System;

    using Xunit;
    using Xunit.Extensions;

    public class UrlFixture
    {
        private readonly Url url;

        public UrlFixture()
        {
            this.url = new Url();
        }
        
        [Fact]
        public void Should_contain_schema_when_converted_to_string()
        {
            // Given
            this.url.Scheme = "https";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://");
        }

        [Fact]
        public void Should_append_hostname_when_converted_to_string()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://www.nancyfx.org");
        }

        [Fact]
        public void Should_enclose_ipv6_hostname_in_square_brackets_when_converted_to_string()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "::1";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldStartWith("https://[");
            result.ShouldEndWith("]");
        }

        [Fact]
        public void Should_leave_ipv4_hostname_untouched_when_converted_to_string()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "127.0.0.1";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://127.0.0.1");
        }

        [Fact]
        public void Should_append_port_if_available_when_converted_to_string()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://www.nancyfx.org:1234");
        }

        [Fact]
        public void Should_append_basepath_when_converted_to_string()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;
            this.url.BasePath = "/base";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://www.nancyfx.org:1234/base");
        }
        
        [Fact]
        public void Should_append_path_when_converted_to_string()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;
            this.url.BasePath = "/base";
            this.url.Path = "/path";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://www.nancyfx.org:1234/base/path");
        }

        [Fact]
        public void Should_not_append_path_when_rooted_when_converted_to_string()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;
            this.url.BasePath = "/base";
            this.url.Path = "/";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://www.nancyfx.org:1234/base");
        }

        [Fact]
        public void Should_append_query_when_converting_to_string()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;
            this.url.BasePath = "/base";
            this.url.Path = "/";
            this.url.Query = "?foo=some%20text";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://www.nancyfx.org:1234/base?foo=some%20text");
        }

        [Fact]
        public void Should_append_question_mark_to_querystring_when_missing()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;
            this.url.BasePath = "/base";
            this.url.Path = "/";
            this.url.Query = "foo=some%20text";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://www.nancyfx.org:1234/base?foo=some%20text");
        }

        [Fact]
        public void Should_implicitly_cast_from_string()
        {
            // Given
            string urlAsString = "https://www.nancyfx.org:1234/base?foo=some%20text#anchor";

            // When
            Url result = urlAsString;

            // Then
            result.Scheme.ShouldEqual("https");
            result.HostName.ShouldEqual("www.nancyfx.org");
            result.Port.ShouldEqual(1234);
            result.BasePath.ShouldEqual(string.Empty);
            result.Path.ShouldEqual("/base");
            result.Query.ShouldEqual("?foo=some%20text");
        }

        [Fact]
        public void Should_implicitly_cast_to_string()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;
            this.url.BasePath = "/base";
            this.url.Path = "/";
            this.url.Query = "?foo=some%20text";

            // When
            string result = this.url;

            // Then
            result.ShouldEqual("https://www.nancyfx.org:1234/base?foo=some%20text");
        }

        [Fact]
        public void Should_implicitly_cast_from_uri()
        {
            // Given
            Uri uri = new Uri("https://www.nancyfx.org:1234/base?foo=some%20text#anchor");

            // When
            Url result = uri;

            // Then
            result.Scheme.ShouldEqual("https");
            result.HostName.ShouldEqual("www.nancyfx.org");
            result.Port.ShouldEqual(1234);
            result.BasePath.ShouldEqual(string.Empty);
            result.Path.ShouldEqual("/base");
            result.Query.ShouldEqual("?foo=some%20text");
        }

        [Fact]
        public void Should_implicitly_cast_from_relative_uri()
        {
            // Given
            var uri = new Uri("/hello", UriKind.Relative);

            // When
            Url result = uri;

            // Then
            result.Scheme.ShouldEqual(Uri.UriSchemeHttp);
            result.HostName.ShouldEqual(string.Empty);
            result.Port.ShouldEqual(null);
            result.BasePath.ShouldEqual(string.Empty);
            result.Path.ShouldEqual("/hello");
            result.Query.ShouldEqual(string.Empty);
        }

        [Fact]
        public void Should_implicitly_cast_to_uri()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;
            this.url.BasePath = "/base";
            this.url.Path = "/";
            this.url.Query = "?foo=some%20text";

            // When
            Uri result = this.url;

            // Then
            result.Scheme.ShouldEqual("https");
            result.Host.ShouldEqual("www.nancyfx.org");
            result.Port.ShouldEqual(1234);
            result.AbsolutePath.ShouldEqual("/base");
            result.Query.ShouldEqual("?foo=some%20text");
            result.Fragment.ShouldBeEmpty();
        }

        [Fact]
        public void Should_implicitly_cast_to_absolute_uri()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;
            this.url.BasePath = "/base";
            this.url.Path = "/";
            this.url.Query = "?foo=some%20text";

            // When
            Uri result = this.url;

            // Then
            result.IsAbsoluteUri.ShouldBeTrue();
        }

        [Fact]
        public void Should_be_able_to_take_a_string_in_the_constructor()
        {
            //Given, When
            var url = new Url("https://www.nancyfx.org:1234/base?foo=some text");

            //Then
            url.Scheme.ShouldEqual("https");
            url.HostName.ShouldEqual("www.nancyfx.org");
            url.Port.ShouldEqual(1234);
            url.Path.ShouldEqual("/base");
            url.Query.ShouldEqual("?foo=some%20text");
        }
        
        [Theory]
        [InlineData("https://www.nancyfx.org:1234/base?foo=some%20text", "https", "www.nancyfx.org", 1234, "/base", "?foo=some%20text")]
        [InlineData("http://nancyfx.org", "http", "nancyfx.org", 80, "/", "")]
        [InlineData("http://nancyfx.org?foo=some%20text", "http", "nancyfx.org", 80, "/", "?foo=some%20text")]
        [InlineData("https://nancyfx.org/base/admin/area?foo=some%20text", "https", "nancyfx.org", 443, "/base/admin/area", "?foo=some%20text")]
        [InlineData("http://nancyfx.org/base/admin/area", "http", "nancyfx.org", 80, "/base/admin/area", "")]
        public void Should_implicitly_cast_uri_to_url(string fullurl, string scheme, string host, int port, string path, string query)
        {
            //Given
            var uri = new Uri(fullurl);

            //When
            Url result = uri;

            //Then
            Assert.Equal(scheme, result.Scheme);
            Assert.Equal(host,result.HostName);
            Assert.Equal(port, result.Port);
            Assert.Equal(path, result.Path);
            Assert.Equal(query, result.Query);
        }

        [Theory]
        [InlineData("https")]
        [InlineData("Https")]
        [InlineData("httPs")]
        [InlineData("HTTPS")]
        public void IsSecure_should_return_true_if_https(string scheme)
        {
            // Given
            this.url.Scheme = scheme;

            // When
            var result = this.url.IsSecure;

            // Then
            result.ShouldBeTrue();
        }

        [Theory]
        [InlineData("http")]
        [InlineData("Http")]
        [InlineData("blah")]
        [InlineData("blahs")]
        public void IsSecure_should_return_false_if_scheme_is_not_https(string scheme)
        {
            // Given
            this.url.Scheme = scheme;

            // When
            var result = this.url.IsSecure;

            // Then
            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsSecure_should_return_false_when_scheme_is_null_or_empty(string scheme)
        {
            // Given
            this.url.Scheme = scheme;

            // When
            var result = this.url.IsSecure;

            // Then
            result.ShouldBeFalse();
        }
    }
}