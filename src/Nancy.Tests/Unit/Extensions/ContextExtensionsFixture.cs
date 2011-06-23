using System.Collections.Generic;
using Nancy.Extensions;
using Nancy.Tests.Fakes;
using Xunit;

namespace Nancy.Tests.Unit.Extensions
{
    public class ContextExtensionsFixture
    {
        [Fact]
        public static void IsAjaxRequest_should_return_true_if_request_is_ajax()
        {
            // Given 
            var headers =
                new Dictionary<string, IEnumerable<string>>
                    {
                        { "X-Requested-With", new[] { "XMLHttpRequest" } }
                    };

            // When
            var context = new NancyContext
                              {
                                  Request = new FakeRequest("POST", "/", headers)
                              };

            // Then
            Assert.True(context.IsAjaxRequest());
        }

        [Fact]
        public void IsAjaxRequest_should_return_false_if_request_is_null()
        {
            // Given when
            var context = new NancyContext();

            // Then
            Assert.False(context.IsAjaxRequest());
        }

        [Fact]
        public void IsAjaxRequest_should_return_false_if_request_is_not_ajax()
        {
            // Given when
            var context = new NancyContext
                              {
                                  Request = new FakeRequest("POST", "/")
                              };

            // Then
            Assert.False(context.IsAjaxRequest());
        }
    }
}
