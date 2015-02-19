namespace Nancy.Tests.Unit.Extensions
{
    using System.Collections.Generic;

    using Nancy.Extensions;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class RequestExtensionsFixture
    {
        [Fact]
        public void IsAjaxRequest_should_return_true_if_request_is_ajax()
        {
            // Given 
            var headers =
                new Dictionary<string, IEnumerable<string>>
                    {
                        { "X-Requested-With", new[] { "XMLHttpRequest" } }
                    };

            // When
            var request = new FakeRequest("POST", "/", headers);

            // Then
            Assert.True(request.IsAjaxRequest());
        }

        [Fact]
        public void IsAjaxRequest_should_return_false_if_request_is_not_ajax()
        {
            // Given when
            var request = new FakeRequest("POST", "/");

            // Then
            Assert.False(request.IsAjaxRequest());
        }

        [Fact]
        public void IsLocal_should_return_true_if_userHostAddr_is_localhost_IPV6()
        {
            // Given when
            var request = new FakeRequest("GET", "/", string.Empty, "::1");
            request.Url.HostName = "localhost";

            // Then
            Assert.True(request.IsLocal());
        }

        [Fact]
        public void IsLocal_should_return_false_if_userHostAddr_is_empty()
        {
            // Given when
            var request = new FakeRequest("GET", "/", string.Empty,  string.Empty);

            // Then
            Assert.False(request.IsLocal());
        }

        [Fact]
        public void IsLocal_should_return_false_if_urlString_is_empty()
        {
            // Given when
            var request = new FakeRequest("GET", string.Empty, string.Empty, string.Empty);

            // Then
            Assert.False(request.IsLocal());
        }

        [Fact]
        public void IsLocal_should_return_true_if_userHostAddr_is_localhost_IPV4()
        {
            // Given when
            var request = new FakeRequest("POST", "/", string.Empty, "127.0.0.1");
            request.Url.HostName = "localhost";
            
            // Then
            Assert.True(request.IsLocal());
        }

        [Fact]
        public void IsLocal_should_return_false_if_userHostAddr_is_not_localhost()
        {
            // Given when
            var request = new FakeRequest("GET", "/", string.Empty, "86.13.73.12");
            request.Url.HostName = "anotherhost";

            // Then
            Assert.False(request.IsLocal());
        }
    }
}
