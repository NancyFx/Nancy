namespace Nancy.Tests.Unit.Diagnostics
{
    using System.Linq;

    using Nancy.Diagnostics;

    using Xunit;

    public class ConcurrentLimitedCollectionFixture
    {
        private const int MaxSize = 5;

        private readonly ConcurrentLimitedCollection<object> collection;

        public ConcurrentLimitedCollectionFixture()
        {
            this.collection = new ConcurrentLimitedCollection<object>(MaxSize);
        }

        [Fact]
        public void Should_be_able_to_add_items()
        {
            var obj = new object();
            var obj2 = new object();

            this.collection.Add(obj);
            this.collection.Add(obj2);

            this.collection.Any(o => ReferenceEquals(obj, o)).ShouldBeTrue();
            this.collection.Any(o => ReferenceEquals(obj2, o)).ShouldBeTrue();
        }

        [Fact]
        public void Should_be_able_to_get_current_size()
        {
            var obj = new object();
            var obj2 = new object();

            this.collection.Add(obj);
            this.collection.Add(obj2);

            this.collection.CurrentSize.ShouldEqual(2);
        }

        [Fact]
        public void Should_be_able_to_clear()
        {
            var obj = new object();
            var obj2 = new object();
            this.collection.Add(obj);
            this.collection.Add(obj2);

            this.collection.Clear();

            this.collection.CurrentSize.ShouldEqual(0);
        }

        [Fact]
        public void Should_not_grow_beyond_limit()
        {
            var obj = new object();
            var obj2 = new object();
            var obj3 = new object();
            var obj4 = new object();
            var obj5 = new object();
            var obj6 = new object();

            this.collection.Add(obj);
            this.collection.Add(obj2);
            this.collection.Add(obj3);
            this.collection.Add(obj4);
            this.collection.Add(obj5);
            this.collection.Add(obj6);
            
            this.collection.CurrentSize.ShouldEqual(MaxSize);
        }

        [Fact]
        public void First_element_should_drop_off_when_over_limit()
        {
            var obj = new object();
            var obj2 = new object();
            var obj3 = new object();
            var obj4 = new object();
            var obj5 = new object();
            var obj6 = new object();

            this.collection.Add(obj);
            this.collection.Add(obj2);
            this.collection.Add(obj3);
            this.collection.Add(obj4);
            this.collection.Add(obj5);
            this.collection.Add(obj6);

            this.collection.Any(o => ReferenceEquals(obj, o)).ShouldBeFalse();
        }
    }
}