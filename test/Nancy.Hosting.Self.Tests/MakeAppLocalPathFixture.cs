namespace Nancy.Hosting.Self.Tests
{
    using System;

    using Nancy.Tests;

    using Xunit;

    public class MakeAppLocalPathFixture
    {
        [Fact]
        public void Should_return_path_as_local_path()
        {
            // Given
            var uri = new Uri("http://host/base/");

            // When
            string result = uri.MakeAppLocalPath(new Uri("http://host/base/rel"));

            // Then
            result.ShouldEqual("/rel");
        }

        [Fact]
        public void Should_return_root_path_with_trailing_slash_as_slash()
        {
            // Given
            var uri = new Uri("http://host/base/");

            // When
            string result = uri.MakeAppLocalPath(new Uri("http://host/base/"));

            // Then
            result.ShouldEqual("/");
        }

        [Fact]
        public void Should_return_root_path_without_trailing_slash_as_slash()
        {
            // Given
            var uri = new Uri("http://host/base/");

            // When
            string result = uri.MakeAppLocalPath(new Uri("http://host/base"));

            // Then
            result.ShouldEqual("/");
        }

        [Fact]
        public void Should_return_path_with_same_casing_as_full_uri()
        {
            // Given
            var uri = new Uri("http://host/base/");

            // When
            string result = uri.MakeAppLocalPath(new Uri("http://host/base/ReL"));

            // Then
            result.ShouldEqual("/ReL");
        }

        [Fact]
        public void Should_support_extended_site_root()
        {
            // Given
            var uri = new Uri("http://host/");

            // When
            string result = uri.MakeAppLocalPath(new Uri("http://host/rel/file"));

            // Then
            result.ShouldEqual("/rel/file");
        }

        [Fact]
        public void Should_support_site_root_without_trailing_slash()
        {
            // Given
            var uri = new Uri("http://host/");

            // When
            string result = uri.MakeAppLocalPath(new Uri("http://host"));

            // Then
            result.ShouldEqual("/");
        }

        [Fact]
        public void Should_return_path_with_case_insensitive_base_uri_comparison()
        {
            // Given
            var uri = new Uri("http://host/base/");

            // When
            string result = uri.MakeAppLocalPath(new Uri("http://host/Base/rel"));

            // Then
            result.ShouldEqual("/rel");
        }
    }
}