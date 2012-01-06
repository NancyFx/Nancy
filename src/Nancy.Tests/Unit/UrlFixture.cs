namespace Nancy.Tests.Unit
{
    using Xunit;

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
            result.ShouldStartWith("https://");
        }

        [Fact]
        public void Should_add_suffix_to_schema()
        {
            // Given
            this.url.Scheme = "https";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldStartWith("https://");
        }

        [Fact]
        public void Should_append_hostname_with_slash_suffix_when_converted_to_string()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://www.nancyfx.org/");
        }

        [Fact]
        public void Should_enclose_ipv6_hostname_in_square_brackets()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "::1";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://[::1]/");
            
        }

        [Fact]
        public void Should_leave_ipv4_hostname_untouched()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "127.0.0.1";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://127.0.0.1/");
        }

        [Fact]
        public void Should_append_port_if_available()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://www.nancyfx.org:1234/");
        }

        [Fact]
        public void Should_append_basepath_with_trimmed_slash_prefix()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;
            this.url.BasePath = "/base";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldStartWith("https://www.nancyfx.org:1234/base");
        }

        [Fact]
        public void Should_append_slash_after_basepath_when_available()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;
            this.url.BasePath = "/base";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://www.nancyfx.org:1234/base/");
        }

        [Fact]
        public void Should_append_path_trimmed_suffix_slash_when_available()
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
            result.ShouldStartWith("https://www.nancyfx.org:1234/base/path");
        }

        [Fact]
        public void Should_append_slash_after_path_when_available()
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
            result.ShouldEndWith("https://www.nancyfx.org:1234/base/path/");
        }

        [Fact]
        public void Should_not_append_path_when_rooted()
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
            result.ShouldEndWith("https://www.nancyfx.org:1234/base/");
        }

        [Fact]
        public void Should_append_url_encoded_query_when_available()
        {
            // Given
            this.url.Scheme = "https";
            this.url.HostName = "www.nancyfx.org";
            this.url.Port = 1234;
            this.url.BasePath = "/base";
            this.url.Path = "/";
            this.url.Query = "";

            // When
            var result = this.url.ToString();

            // Then
            result.ShouldEndWith("https://www.nancyfx.org:1234/base/");
        }
    }
}