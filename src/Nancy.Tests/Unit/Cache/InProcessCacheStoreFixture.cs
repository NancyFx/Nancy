namespace Nancy.Tests.Unit.Cache
{
    using Nancy.Cache;
    using Xunit;

    public class InProcessCacheStoreFixture
    {
        private InProcessCacheStore store;

        public InProcessCacheStoreFixture()
        {
            store = new InProcessCacheStore();
            store.Store("testValue", "Hello");
        }

        [Fact]
        public void Should_be_able_to_store_an_item()
        {
            store.Store("test", 1);

            int i;
            store.TryLoad("test", out i).ShouldBeTrue();
            i.ShouldEqual(1);
        }

        [Fact]
        public void Should_be_able_to_load_a_value()
        {
            string s;
            store.TryLoad("testValue", out s).ShouldBeTrue();
            s.ShouldEqual("Hello");
        }

        [Fact]
        public void Should_be_able_to_remove_a_value()
        {
            store.Store("test", 1);
            store.Remove("test");
            int i;
            store.TryLoad("test", out i).ShouldBeFalse();
        }

        [Fact]
        public void When_a_value_cant_be_loaded_it_should_return_default()
        {
            object o;
            int i;

            store.TryLoad("123", out o).ShouldBeFalse();
            store.TryLoad("456", out i).ShouldBeFalse();
            o.ShouldBeNull();
            i.ShouldEqual(0);
        }

    }
}
