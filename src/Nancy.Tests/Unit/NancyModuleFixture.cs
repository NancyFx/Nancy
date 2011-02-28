namespace Nancy.Tests.Unit
{
    using System;
    using System.Linq;
    using Fakes;
    using Xunit;

    public class NancyModuleFixture
    {
        private readonly NancyModule module;

        public NancyModuleFixture()
        {
            this.module = new FakeNancyModuleNoRoutes();
        }

        [Fact]
        public void Adds_route_when_get_indexer_used()
        {
            this.module.Get["/test"] = d => null;

            this.module.Routes.Count().ShouldEqual(1);
        }

        [Fact]
        public void Adds_route_when_put_indexer_used()
        {
            this.module.Put["/test"] = d => null;

            this.module.Routes.Count().ShouldEqual(1);
        }

        [Fact]
        public void Adds_route_when_post_indexer_used()
        {
            this.module.Post["/test"] = d => null;

            this.module.Routes.Count().ShouldEqual(1);
        }

        [Fact]
        public void Adds_route_when_delete_indexer_used()
        {
            this.module.Delete["/test"] = d => null;

            this.module.Routes.Count().ShouldEqual(1);
        }

        [Fact]
        public void Should_store_route_with_specified_path_when_route_indexer_is_invoked_with_a_path_but_no_condition()
        {
            this.module.Get["/test"] = d => null;

            module.Routes.First().Description.Path.ShouldEqual("/test");
        }

        [Fact]
        public void Should_store_route_with_specified_path_when_route_indexer_is_invoked_with_a_path_and_condition()
        {
            Func<NancyContext, bool> condition = r => true;

            this.module.Get["/test", condition] = d => null;

            module.Routes.First().Description.Path.ShouldEqual("/test");
        }

        [Fact]
        public void Should_store_route_with_null_condition_when_route_indexer_is_invoked_without_a_condition()
        {
            this.module.Get["/test"] = d => null;

            module.Routes.First().Description.Condition.ShouldBeNull();
        }

        [Fact]
        public void Should_store_route_with_condition_when_route_indexer_is_invoked_with_a_condition()
        {
            Func<NancyContext, bool> condition = r => true;
            
            this.module.Get["/test", condition] = d => null;

            module.Routes.First().Description.Condition.ShouldBeSameAs(condition);
        }

        [Fact]
        public void Should_add_route_with_get_method_when_added_using_get_indexer()
        {
            this.module.Get["/test"] = d => null;

            module.Routes.First().Description.Method.ShouldEqual("GET");
        }

        [Fact]
        public void Should_add_route_with_put_method_when_added_using_get_indexer()
        {
            this.module.Put["/test"] = d => null;

            module.Routes.First().Description.Method.ShouldEqual("PUT");
        }

        [Fact]
        public void Should_add_route_with_post_method_when_added_using_get_indexer()
        {
            this.module.Post["/test"] = d => null;

            module.Routes.First().Description.Method.ShouldEqual("POST");
        }

        [Fact]
        public void Should_add_route_with_delete_method_when_added_using_get_indexer()
        {
            this.module.Delete["/test"] = d => null;

            module.Routes.First().Description.Method.ShouldEqual("DELETE");
        }

        [Fact]
        public void Should_store_route_combine_with_base_path_if_one_specified()
        {
            var moduleWithBasePath = new FakeNancyModuleWithBasePath();

            moduleWithBasePath.Get["/NewRoute"] = d => null;

            moduleWithBasePath.Routes.Last().Description.Path.ShouldEqual("/fake/NewRoute");
        }
    }
}
