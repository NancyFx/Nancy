namespace Nancy.Tests.Unit.Extensions
{
    using System.Collections.Generic;

    using Nancy.Extensions;
    using Nancy.Tests.Fakes;

    using Xunit;
    using Xunit.Extensions;

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

        [Fact]
        public void Should_return_same_path_when_parsing_path_if_path_doesnt_contain_tilde()
        {
            // Given
            const string input = "/scripts/test.js";
            var url = new Url
            {
                BasePath = "/base/path",
                Path = "/"
            };
            var request = new Request("GET", url);
            var nancyContext = new NancyContext { Request = request };

            // When
            var result = nancyContext.ToFullPath(input);

            // Then
            result.ShouldEqual(input);
        }

        [Fact]
        public void Should_replace_tilde_with_base_path_when_parsing_path_if_one_present()
        {
            // Given
            const string input = "~/scripts/test.js";
            var url = new Url
            {
                BasePath = "/base/path/",
                Path = "/"
            };
            var request = new Request("GET", url);
            var nancyContext = new NancyContext { Request = request };

            // When
            var result = nancyContext.ToFullPath(input);

            // Then
            result.ShouldEqual("/base/path/scripts/test.js");
        }

        [Fact]
        public void Should_replace_tilde_with_nothing_when_parsing_path_if_one_present_and_base_path_is_null()
        {
            // Given
            const string input = "~/scripts/test.js";
            var url = new Url
            {
                BasePath = null,
                Path = "/"
            };
            var request = new Request("GET", url);
            var nancyContext = new NancyContext { Request = request };

            // When
            var result = nancyContext.ToFullPath(input);

            // Then
            result.ShouldEqual("/scripts/test.js");
        }

        [Fact]
        public void Should_report_simple_relative_path_as_local()
        {
            // Given
            var context = this.CreateContext();

            // When
            var result = context.IsLocalUrl("/stuff");

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_report_same_host_absolute_url_as_local()
        {
            // Given
            var context = this.CreateContext();

            // When
            var result = context.IsLocalUrl("http://test.com/someotherpath");

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_report_empty_string_as_nonlocal()
        {
            // Given
            var context = this.CreateContext();

            // When
            var result = context.IsLocalUrl("");

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_report_different_host_absolute_url_as_nonlocal()
        {
            // Given
            var context = this.CreateContext();

            // When
            var result = context.IsLocalUrl("http://anothertest.com/someotherpath");

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_report_protocol_relative_url_different_host_as_nonlocal()
        {
            // Given
            var context = this.CreateContext();

            // When
            var result = context.IsLocalUrl("//anothertest.com");

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_report_protocol_relative_url_same_host_as_local()
        {
            // Given
            var context = this.CreateContext();

            // When
            var result = context.IsLocalUrl("//test.com");

            // Then
            result.ShouldBeTrue();
        }

        [Theory]
        [InlineData(@"/\anothertest.com")]
        [InlineData(@"\/anothertest.com")]
        public void Should_report_malformed_url_as_nonlocal(string malformedUrl)
        {
            // Given
            var context = this.CreateContext();
            
            // When
            var result = context.IsLocalUrl(malformedUrl);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_empty_string_if_no_exception_details()
        {
            // Given
            var context = this.CreateContext();

            // When
            var exceptionDetails = context.GetExceptionDetails();

            // Then
            exceptionDetails.ShouldBeEmpty();
        }

        private NancyContext CreateContext(Url url = null)
        {
            var request = new Request(
                "GET",
                url ?? new Url() { Scheme = "http", BasePath = "testing", HostName = "test.com", Path = "test" });

            return new NancyContext() { Request = request };
        }
    }
}
