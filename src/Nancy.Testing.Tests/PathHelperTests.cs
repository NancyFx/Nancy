namespace Nancy.Testing.Tests
{
    using Nancy.Tests;
    using Xunit;

    public class PathHelperTests
    {
        [Fact]
        public void Should_return_same_path_for_zero_levels()
        {
            var path = @"c:\this\this\the\other\path";

            var result = PathHelper.GetParent(path, 0);

            result.ShouldEqual(path);
        }

        [Fact]
        public void Should_go_up_correct_path_levels()
        {
            var path = @"c:\this\this\the\other\path";

            var result = PathHelper.GetParent(path, 2);

            result.ShouldEqual(@"c:\this\this\the");
        }

        [Fact]
        public void Should_not_go_passed_root()
        {
            var path = @"c:\this\this\the\other\path";

            var result = PathHelper.GetParent(path, 28);

            result.ShouldEqual(@"c:\");
        }
    }
}