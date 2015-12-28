namespace Nancy.Tests.Unit
{
    using System;
    using System.Linq;

    using Nancy.Tests.Fakes;

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
            // Given, When
            this.module.Get["/test"] = d => null;

            // Then
            this.module.Routes.Count().ShouldEqual(1);
        }

        [Fact]
        public void Adds_route_when_put_indexer_used()
        {
            // Given, When
            this.module.Put["/test"] = d => null;

            // Then
            this.module.Routes.Count().ShouldEqual(1);
        }

        [Fact]
        public void Adds_route_when_post_indexer_used()
        {
            // Given, When
            this.module.Post["/test"] = d => null;

            // Then
            this.module.Routes.Count().ShouldEqual(1);
        }

        [Fact]
        public void Adds_route_when_delete_indexer_used()
        {
            // Given, When
            this.module.Delete["/test"] = d => null;

            // Then
            this.module.Routes.Count().ShouldEqual(1);
        }

        [Fact]
        public void Adds_route_when_options_indexer_userd()
        {
            // Given, When
            this.module.Options["/test"] = d => null;

            // Then
            this.module.Routes.Count().ShouldEqual(1);
        }

        [Fact]
        public void Should_store_route_with_specified_path_when_route_indexer_is_invoked_with_a_path_but_no_condition()
        {
            // Given, When
            this.module.Get["/test"] = d => null;

            // Then
            module.Routes.First().Description.Path.ShouldEqual("/test");
        }

        [Fact]
        public void Should_store_route_with_specified_path_when_route_indexer_is_invoked_with_a_path_and_condition()
        {
            // Given
            Func<NancyContext, bool> condition = r => true;

            // When
            this.module.Get["/test", condition] = d => null;

            // Then
            module.Routes.First().Description.Path.ShouldEqual("/test");
        }

        [Fact]
        public void Should_store_route_with_null_condition_when_route_indexer_is_invoked_without_a_condition()
        {
            // Given, When
            this.module.Get["/test"] = d => null;

            // Then
            module.Routes.First().Description.Condition.ShouldBeNull();
        }

        [Fact]
        public void Should_store_route_with_condition_when_route_indexer_is_invoked_with_a_condition()
        {
            // Given
            Func<NancyContext, bool> condition = r => true;

            // When
            this.module.Get["/test", condition] = d => null;

            // Then
            module.Routes.First().Description.Condition.ShouldBeSameAs(condition);
        }

        [Fact]
        public void Should_add_route_with_get_method_when_added_using_get_indexer()
        {
            // Given, When
            this.module.Get["/test"] = d => null;

            // Then
            module.Routes.First().Description.Method.ShouldEqual("GET");
        }

        [Fact]
        public void Should_add_route_with_put_method_when_added_using_get_indexer()
        {
            // Given, When
            this.module.Put["/test"] = d => null;

            // Then
            module.Routes.First().Description.Method.ShouldEqual("PUT");
        }

        [Fact]
        public void Should_add_route_with_post_method_when_added_using_get_indexer()
        {
            // Given, When
            this.module.Post["/test"] = d => null;

            // Then
            module.Routes.First().Description.Method.ShouldEqual("POST");
        }

        [Fact]
        public void Should_add_route_with_delete_method_when_added_using_get_indexer()
        {
            // Given, When
            this.module.Delete["/test"] = d => null;

            // Then
            module.Routes.First().Description.Method.ShouldEqual("DELETE");
        }

        [Fact]
        public void Should_store_route_combine_with_base_path_if_one_specified()
        {
            // Given
            var moduleWithBasePath = new FakeNancyModuleWithBasePath();

            // When
            moduleWithBasePath.Get["/NewRoute"] = d => null;

            // Then
            moduleWithBasePath.Routes.Last().Description.Path.ShouldEqual("/fake/NewRoute");
        }

        [Fact]
        public void Should_add_leading_slash_to_route_if_missing()
        {
            // Given
            var moduleWithBasePath = new FakeNancyModuleWithBasePath();

            // When
            moduleWithBasePath.Get["test"] = d => null;

            // Then
            moduleWithBasePath.Routes.Last().Description.Path.ShouldEqual("/fake/test");
        }

        [Fact]
        public void Should_store_two_routes_when_registering_single_get_method()
        {
            // Given
            var moduleWithBasePath = new CustomNancyModule();

            // When
            moduleWithBasePath.Get["/Test1", "/Test2"] = d => null;

            // Then
            moduleWithBasePath.Routes.First().Description.Path.ShouldEqual("/Test1");
            moduleWithBasePath.Routes.Last().Description.Path.ShouldEqual("/Test2");
        }

        [Fact]
        public void Should_store_single_route_when_calling_non_overridden_post_from_sub_module()
        {
            // Given
            var moduleWithBasePath = new CustomNancyModule();

            // When
            moduleWithBasePath.Post["/Test1"] = d => null;

            // Then
            moduleWithBasePath.Routes.Last().Description.Path.ShouldEqual("/Test1");
        }

        [Fact]
        public void Should_not_throw_when_null_passed_as_modulepath()
        {
            // Given
            var moduleWithNullPath = new CustomModulePathModule(null);

            // Then
            Assert.DoesNotThrow(() =>
            {
                moduleWithNullPath.Post["/Test1"] = d => null;
            });
        }

        [Fact]
        public void Adds_named_route_when_named_indexer_used()
        {
            // Given, When
            this.module.Get["Foo", "/test"] = d => null;

            // Then
            this.module.Routes.Count().ShouldEqual(1);
            this.module.Routes.First().Description.Name.ShouldEqual("Foo");
        }

        private class CustomModulePathModule : NancyModule
        {
            public CustomModulePathModule(string modulePath)
                : base(modulePath)
            {
            }
        }

        private class CustomNancyModule : NancyModule
        {
            public new CustomRouteBuilder Get
            {
                get { return new CustomRouteBuilder("GET", this); }
            }

            public class CustomRouteBuilder : RouteBuilder
            {
                public CustomRouteBuilder(string method, NancyModule parentModule)
                    : base(method, parentModule)
                {
                }

                public Func<dynamic, dynamic> this[params string[] paths]
                {
                    set
                    {
                        foreach (var path in paths)
                        {
                            this.AddRoute(String.Empty, path, null, value);
                        }
                    }
                }
            }
        }
    }
}
