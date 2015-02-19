namespace Nancy.Testing.Tests
{
    using System;
    using System.IO;

    using Nancy.Tests;

    using Xunit;

    public class PathHelperTests
    {
        [Fact]
        public void Should_return_same_path_for_zero_levels()
        {
            // Given
            var path = Path.Combine("c:", "this", "this", "the", "other", "path");

            // When
            var result = PathHelper.GetParent(path, 0);

            // Then
            result.ShouldEqual(path);
        }

        [Fact]
        public void Should_go_up_correct_path_levels()
        {
            // Given
            var path = Path.Combine("c:", "this", "this", "the", "other", "path");
            var expectedPath = Path.Combine("c:", "this", "this", "the");
            
            // When
            var result = PathHelper.GetParent(path, 2);

            // Then
            result.ShouldEqual(expectedPath);
        }

        [Fact]
        public void Should_throw_invalidoperationexception_if_attempting_to_go_up_beyond_root()
        {
            // Given
            var path = Path.Combine("c:", "this", "this", "the", "other", "path");

            // When
            var exception = Record.Exception(() => PathHelper.GetParent(path, 28));

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_path_is_null()
        {
            // Given
            string path = null;            

            // When
            var exception = Record.Exception(() => PathHelper.GetParent(path, 28));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_path_is_empty()
        {
            // Given
            var path = string.Empty;

            // When
            var exception = Record.Exception(() => PathHelper.GetParent(path, 28));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_levels_is_less_than_zero()
        {
            // Given
            var path = Path.Combine("c:", "this", "this", "the", "other", "path");

            // When
            var exception = Record.Exception(() => PathHelper.GetParent(path, -10));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }
    }
}