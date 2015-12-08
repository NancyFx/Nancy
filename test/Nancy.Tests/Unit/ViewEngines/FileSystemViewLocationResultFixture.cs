namespace Nancy.Tests.Unit.ViewEngines
{
    using System;

    using FakeItEasy;

    using Nancy.ViewEngines;

    using Xunit;

    public class FileSystemViewLocationResultFixture
    {
        private readonly IFileSystemReader reader;

        public FileSystemViewLocationResultFixture()
        {
            this.reader = A.Fake<IFileSystemReader>();
        }

        [Fact]
        public void Should_return_stale_if_never_read_before()
        {
            A.CallTo(() => this.reader.GetLastModified(A<string>._)).Returns(DateTime.UtcNow);
            var vlr = new FileSystemViewLocationResult("here", "there", "everywhere", () => null, "full", this.reader);

            var result = vlr.IsStale();

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_stale_if_last_modified_changed()
        {
            A.CallTo(() => this.reader.GetLastModified(A<string>._)).Returns(DateTime.UtcNow);
            var vlr = new FileSystemViewLocationResult("here", "there", "everywhere", () => null, "full", this.reader);
            A.CallTo(() => this.reader.GetLastModified(A<string>._)).Returns(DateTime.UtcNow);

            var result = vlr.IsStale();

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_not_stale_if_modified_changed_not_changed_since_last_read()
        {
            A.CallTo(() => this.reader.GetLastModified(A<string>._)).Returns(DateTime.UtcNow);
            var vlr = new FileSystemViewLocationResult("here", "there", "everywhere", () => null, "full", this.reader);
            A.CallTo(() => this.reader.GetLastModified(A<string>._)).Returns(DateTime.UtcNow);

            vlr.Contents();
            var result = vlr.IsStale();

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_stale_if_modified_changed_since_last_read()
        {
            var vlr = new FileSystemViewLocationResult("here", "there", "everywhere", () => null, "full", this.reader);
            A.CallTo(() => this.reader.GetLastModified(A<string>._)).Returns(DateTime.UtcNow.AddMinutes(-1));
            vlr.Contents();
            A.CallTo(() => this.reader.GetLastModified(A<string>._)).Returns(DateTime.UtcNow);

            var result = vlr.IsStale();

            result.ShouldBeTrue();
        }
    }
}