using Nancy.Extensions;

namespace Nancy.Tests.Unit.Extensions
{
    using System.Collections.Generic;
    using Fakes;
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
    }
}
