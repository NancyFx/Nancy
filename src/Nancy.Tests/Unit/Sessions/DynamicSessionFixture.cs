namespace Nancy.Tests.Unit.Sessions
{
    using System.Collections.Generic;
    using Nancy.Session;
    using Xunit;

    public class DynamicSessionFixture
    {
        private dynamic provider;

        public DynamicSessionFixture()
        {
            provider = new DynamicSession();
        }

        [Fact]
        public void Should_track_changes_via_member()
        {
            provider.Hello = "World";
            Assert.True(provider.HasChanged);
        }

        [Fact]
        public void Should_track_changes_via_index()
        {
            provider["Hello"] = "World";
            Assert.True(provider.HasChanged);
        }

        [Fact]
        public void Should_track_changes_via_add()
        {
            provider.Add("Hello", "World");
            Assert.True(provider.HasChanged);
        }

        [Fact]
        public void Should_track_changes_via_add_pair()
        {
            provider.Add(new KeyValuePair<string, dynamic>("Hello", "World"));
            Assert.True(provider.HasChanged);
        }

        [Fact]
        public void Should_track_changes_via_clear()
        {
            provider.Clear();
            Assert.True(provider.HasChanged);
        }

        [Fact]
        public void Should_track_changes_via_remove()
        {
            provider.Remove("Hello");
            Assert.True(provider.HasChanged);
        }

        [Fact]
        public void Should_track_changes_via_remove_pair()
        {
            provider.Remove(new KeyValuePair<string, dynamic>("Hello", "World"));
            Assert.True(provider.HasChanged);
        }

        [Fact]
        public void Should_start_unchanged()
        {
            dynamic session = new DynamicSession(new Dictionary<string, dynamic>() { { "Hello", "World" } });
            Assert.False(session.HasChanged);
            Assert.Equal("World", session.Hello);
        }
    }
}
