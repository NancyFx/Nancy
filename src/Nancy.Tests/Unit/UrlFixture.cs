namespace Nancy.Tests.Unit
{
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
            result.ShouldEndWith("https://[::1]");
            
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
        public void Should_append_fragment_when_converting_to_string()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;
            this.url.BasePath = "/base";
            this.url.Path = "/";
            this.url.Query = "?foo=some%20text";
            this.url.Fragment = "anchor";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://www.nancyfx.org:1234/base?foo=some%20text#anchor");
        }
    }
}