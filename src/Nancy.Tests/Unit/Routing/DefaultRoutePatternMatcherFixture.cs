using System;

namespace Nancy.Tests.Unit.Routing
{
    using Nancy.Routing;
    using Xunit;

    public class DefaultRoutePatternMatcherFixture
    {
        private readonly DefaultRoutePatternMatcher matcher;

        public DefaultRoutePatternMatcherFixture()
        {
            this.matcher = new DefaultRoutePatternMatcher();
        }

        [Fact]
        public void Should_not_trim_trailing_slash_if_requesting_root()
        {
            // Given, When
            var results = this.matcher.Match("/", "/", new string[] { }, null);

            // Then
            results.IsMatch.ShouldBeTrue();
        }

        [Fact]
        public void Should_ignore_trailing_slash_on_route_path()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar", "/foo/bar/", new[] { "foo", "bar" }, null);

            // Then
            results.IsMatch.ShouldBeTrue();
        }

        [Fact]
        public void Should_ignore_trailing_slash_on_request_uri()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar/", "/foo/bar", new[] { "foo", "bar" }, null);

            // Then
            results.IsMatch.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_match_result_when_paths_matched()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar", "/foo/bar", new[] { "foo", "bar" }, null);

            // Then
            results.IsMatch.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_match_result_with_context_of_current_request_when_paths_matched()
        {
            // Given
            var context = new NancyContext();
            
            // When
            var results = this.matcher.Match("/foo/bar", "/foo/bar", new[] { "foo", "bar" }, context);

            // Then
            results.Context.ShouldBeSameAs(context);
        }

        [Fact]
        public void Should_return_match_result_with_context_of_current_request_when_paths_do_not_match()
        {
            // Given
            var context = new NancyContext();

            // When
            var results = this.matcher.Match("/foo/bar", "/bar/foo", new[] { "foo", "bar" }, context);

            // Then
            results.Context.ShouldBeSameAs(context);
        }

        [Fact]
        public void Should_return_negative_match_result_when_paths_does_not_match()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar", "/bar/foo", new[] { "bar", "foo" }, null);

            // Then
            results.IsMatch.ShouldBeFalse();
        }

        [Fact]
        public void Should_be_case_insensitive_when_checking_for_match()
        {
            // Given, When
            var results = this.matcher.Match("/FoO/baR", "/fOO/bAr", new[] { "fOO", "bAr" }, null);

            // Then
            results.IsMatch.ShouldBeTrue();
        }

        [Fact]
        public void Should_capture_parameters()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar/baz", "/foo/{bar}/{baz}", new[] { "foo", "{bar}", "{baz}" }, null);

            // Then
            ((string)results.Parameters["bar"]).ShouldEqual("bar");
            ((string)results.Parameters["baz"]).ShouldEqual("baz");
        }

        [Fact]
        public void Should_treat_parameters_as_greedy()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar/baz", "/foo/{bar}", new[] { "foo", "{bar}" }, null);

            // Then
            ((string)results.Parameters["bar"]).ShouldEqual("bar/baz");
        }

        [Fact]
        public void Should_allow_regex_in_route_definition_and_capture_specified_parameters()
        {
            // Given, When
            var results = this.matcher.Match("/foo/1234", @"/(?<foo>foo)/(?<bar>\d{4})/", new[] { "(?<foo>foo)", @"(?<bar>\d{4})" }, null);

            // Then
            results.IsMatch.ShouldBeTrue();
            ((string)results.Parameters["foo"]).ShouldEqual("foo");
            ((string)results.Parameters["bar"]).ShouldEqual("1234");
        }

        [Fact]
        public void Should_allow_regex_in_route_definition_and_return_negative_result_when_it_does_not_match()
        {
            // Given, When
            var results = this.matcher.Match("/foo/bar", @"/foo/(?<bar>[0-9]*)", new[] { "foo", @"(?<bar>[0-9]*)" }, null);

            // Then
            results.IsMatch.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_url_decode_captured_parameters()
        {
            // Given
            var parameter = Uri.EscapeUriString("baa ram ewe{}");

            // When
            var results = this.matcher.Match("/foo/" + parameter, "/foo/{bar}", new[] { "foo", @"{bar}" }, null);

            //Then
            ((string)results.Parameters["bar"]).ShouldEqual(parameter);
        }

        [Fact]
        public void Should_allow_all_of_the_unreserved_rfc_1738_characters_in_the_capture_segments()
        {
            // Given
            const string parameter = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_.!*'()";

            // When
            var results = this.matcher.Match("/foo/" + parameter, "/foo/{bar}", new[] { "foo", @"{bar}" }, null);

            // Then
            ((string)results.Parameters["bar"]).ShouldEqual(parameter);
        }

        [Fact]
        public void Should_allow_all_of_the_unreserved_rfc_1738_characters_in_the_non_capture_segments()
        {
            // Given
            const string parameter = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_.!*'()";

            // When
            var results = this.matcher.Match("/foo/" + parameter, "/foo/" + parameter, new[] { "foo", parameter }, null);

            // Then
            results.IsMatch.ShouldBeTrue();
        }

        [Fact]
        public void Should_allow_underscore_in_parameter_key()
        {
            // Given
            const string parameter = "lol";

            // When
            var results = this.matcher.Match("/foo/" + parameter, "/foo/{b_ar}", new[] { "foo", "{b_ar}" }, null);

            // Then
            ((string)results.Parameters["b_ar"]).ShouldEqual(parameter);
        }

		[Fact]
        public void Should_capture_parameters_when_the_segment_contains_more_characters_after_parameter_declaration()
        {
            // Given
            const string parameter = "filename";

            // When
            var results = this.matcher.Match("/foo/" + parameter + ".cshtml", "/foo/{name}.cshtml", new[] { "foo", "{name}.cshtml" }, null);

            // Then
            ((string)results.Parameters["name"]).ShouldEqual(parameter);
        }

        [Fact]
        public void Should_capture_parameters_even_when_it_is_surrounded_by_additional_characters()
        {
            // Given
            const string parameter = "filename";

            // When
            var results = this.matcher.Match("/foo/bar" + parameter + ".cshtml", "/foo/bar{name}.cshtml", new[] { "foo", "bar{name}.cshtml" }, null);

            // Then
            ((string)results.Parameters["name"]).ShouldEqual(parameter);
        }

        [Fact]
        public void Should_capture_multiple_parameters()
        {
            // Given, When
            var results = this.matcher.Match("/foo/filename.cshtml", "/foo/{name}.{format}", new[] { "foo", "{name}.{format}" }, null);

            // Then
            ((string)results.Parameters["name"]).ShouldEqual("filename");
            ((string)results.Parameters["format"]).ShouldEqual("cshtml");
        }

        [Fact]
        public void Should_capture_multiple_parameters_that_are_surrounded_by_characters()
        {
            // Given, When
            var results = this.matcher.Match("/foo/barfilename.cshtmlbaz", "/foo/bar{name}.{format}baz", new[] { "foo", "bar{name}.{format}baz" }, null);

            // Then
            ((string)results.Parameters["name"]).ShouldEqual("filename");
            ((string)results.Parameters["format"]).ShouldEqual("cshtml");
        }
    }
}
