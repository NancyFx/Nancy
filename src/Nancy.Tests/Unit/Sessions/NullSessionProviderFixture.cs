namespace Nancy.Tests.Unit.Sessions
{
    using System;

    using Nancy.Session;

    using Xunit;

    public class NullSessionProviderFixture
    {
        private NullSessionProvider provider;

        public NullSessionProviderFixture()
        {
            this.provider = new NullSessionProvider();
        }

        [Fact]
        public void Should_throw_when_get_enumerator_called()
        {
            var exception = Record.Exception(() => provider.GetEnumerator());

            exception.ShouldBeOfType(typeof(InvalidOperationException));
        }

        [Fact]
        public void Should_throw_when_get_count_getter_called()
        {
            var exception = Record.Exception(() => provider.Count);

            exception.ShouldBeOfType(typeof(InvalidOperationException));
        }

        [Fact]
        public void Should_throw_when_deleteall_called()
        {
            var exception = Record.Exception(() => provider.DeleteAll());

            exception.ShouldBeOfType(typeof(InvalidOperationException));
        }

        [Fact]
        public void Should_throw_when_delete_called()
        {
            var exception = Record.Exception(() => provider.Delete("test"));

            exception.ShouldBeOfType(typeof(InvalidOperationException));
        }

        [Fact]
        public void Should_throw_when_indexer_accessed()
        {
            var exception = Record.Exception(() => provider["test"]);

            exception.ShouldBeOfType(typeof(InvalidOperationException));
        }

        [Fact]
        public void Should_return_false_for_haschanged()
        {
            var result = provider.HasChanged;

            result.ShouldBeFalse();
        }
    }
}