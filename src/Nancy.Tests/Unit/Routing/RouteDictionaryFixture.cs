namespace Nancy.Tests.Unit.Routing
{
    using System;
    using Nancy.Routing;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class RouteDictionaryFixture
    {
        private readonly RouteCollection routes;
        private readonly string path;
        private readonly Func<Request, bool> condition;
        private readonly Func<dynamic, Response> action;

        public RouteDictionaryFixture()
        {
            this.routes = new RouteCollection(new FakeNancyModuleWithoutBasePath(), "GET");
            this.path = "/route/path";
            this.condition = ir => { return true; };
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
            information.Path.ShouldEqual(this.path);
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
            information.Path.ShouldEqual(this.path);
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
            var rootBasedRoutes = new RouteCollection(module, "GET");

            // Then
            rootBasedRoutes.Module.ShouldBeSameAs(module);
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_initialized_with_null()
        {
            // Given, When
            var exception = Record.Exception(() => new RouteCollection(null, "GET"));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_store_route_combined_with_root_when_route_indexer_is_invoked_and_root_is_not_empty()
        {
            // Given
            var module = new FakeNancyModuleWithBasePath();
            var rootBasedRoutes = new RouteCollection(module, "GET");
            rootBasedRoutes[this.path] = this.action;
            var moduleRelativePath = string.Concat(module.ModulePath, this.path);

            // When
            var description = rootBasedRoutes.GetRoute(moduleRelativePath);

            // Then
            description.Path.ShouldEqual(moduleRelativePath);
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_initialized_with_null_method()
        {
            // Given, When
            var exception =
                Record.Exception(() => new RouteCollection(new FakeNancyModuleWithBasePath(), null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentoutofrangeexception_when_initialized_with_empty_method()
        {
            // Given, When
            var exception =
                Record.Exception(() => new RouteCollection(new FakeNancyModuleWithBasePath(), string.Empty));

            // Then
            exception.ShouldBeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_set_method_property_to_value_of_method_parameter_when_initialized()
        {
            // Given, When, Then
            this.routes.Method.ShouldEqual("GET");
        }

        [Fact]
        public void Should_set_route_dictionary_method_on_retrieved_description()
        {
            // Given
            this.routes[this.path, this.condition] = this.action;

            // When
            var information = this.routes.GetRoute(this.path);

            // Then
            information.Method.ShouldEqual(this.routes.Method);
        }
    }
}