namespace Nancy.Tests.Unit.Routing
{
    using System;
    using Nancy.Routing;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class RouteDictionaryFixture
    {
        private readonly RouteDictionary routes;
        private readonly string path;
        private readonly Func<bool> condition;
        private readonly Func<dynamic, Response> action;

        public RouteDictionaryFixture()
        {
            this.routes = new RouteDictionary(new FakeNancyModuleWithoutBasePath());
            this.path = "/route/path";
            this.condition = () => { return true; };
            this.action = parameters => { return new Response(); };
        }

        [Fact]
        public void Should_store_route_with_specified_path_when_route_indexer_is_invoked()
        {
            // Given
            this.routes[this.path] = this.action;

            // When
            var information = this.routes.GetRoute(this.path);

            // Then
            information.Route.ShouldEqual(this.path);
        }

        [Fact]
        public void Should_store_route_with_null_condition_when_route_indexer_is_invoked()
        {
            // Given
            this.routes[this.path] = this.action;

            // When
            var information = this.routes.GetRoute(this.path);

            // Then
            information.Condition.ShouldBeNull();
        }

        [Fact]
        public void Should_store_route_with_specified_action_when_route_indexer_is_invoked()
        {
            // Given
            this.routes[this.path] = this.action;

            // When
            var information = this.routes.GetRoute(this.path);

            // Then
            information.Condition.ShouldBeNull();
        }

        [Fact]
        public void Should_store_route_with_specified_path_when_route_and_condition_indexer_is_invoked()
        {
            // Given
            this.routes[this.path, this.condition] = this.action;

            // When
            var information = this.routes.GetRoute(this.path);

            // Then
            information.Route.ShouldEqual(this.path);
        }

        [Fact]
        public void Should_store_route_with_specified_condition_when_route_and_condition_indexer_is_invoked()
        {
            // Given
            this.routes[this.path, this.condition] = this.action;

            // When
            var information = this.routes.GetRoute(this.path);

            // Then
            information.Condition.ShouldEqual(this.condition);
        }

        [Fact]
        public void Should_store_route_with_specified_action_when_route_indexer_and_condition_is_invoked()
        {
            // Given
            this.routes[this.path, this.condition] = this.action;

            // When
            var information = this.routes.GetRoute(this.path);

            // Then
            information.Action.ShouldEqual(this.action);
        }

        [Fact]
        public void Should_set_module_property_when_initialized()
        {
            // Given
            var module = new FakeNancyModuleWithBasePath();

            // When
            var rootBasedRoutes = new RouteDictionary(module);

            // Then
            rootBasedRoutes.Module.ShouldBeSameAs(module);
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_initialized_with_null()
        {
            // Given, When
            var exception = Record.Exception(() => new RouteDictionary(null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_store_route_combined_with_root_when_route_indexer_is_invoked_and_root_is_not_empty()
        {
            // Given
            var module = new FakeNancyModuleWithBasePath();
            var rootBasedRoutes = new RouteDictionary(module);
            rootBasedRoutes[this.path] = this.action;
            
            // When
            var information = rootBasedRoutes.GetRoute(this.path);

            // Then
            information.Route.ShouldEqual(string.Concat(rootBasedRoutes.Module.ModulePath, this.path));
        }
    }
}