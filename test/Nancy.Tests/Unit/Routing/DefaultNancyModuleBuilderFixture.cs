namespace Nancy.Tests.Unit.Routing
{
    using FakeItEasy;

    using Nancy.ModelBinding;
    using Nancy.Routing;
    using Nancy.Tests.Fakes;
    using Nancy.Validation;
    using Nancy.ViewEngines;

    using Xunit;

    public class DefaultNancyModuleBuilderFixture
    {
        private readonly DefaultNancyModuleBuilder builder;
        private readonly IResponseFormatterFactory responseFormatterFactory;
        private readonly IViewFactory viewFactory;
        private readonly NancyModule module;
        private readonly IModelBinderLocator modelBinderLocator;
        private readonly IModelValidatorLocator validatorLocator;

        public DefaultNancyModuleBuilderFixture()
        {
            this.module = new FakeNancyModule();

            this.responseFormatterFactory =
                A.Fake<IResponseFormatterFactory>();

            this.viewFactory = A.Fake<IViewFactory>();
            this.modelBinderLocator = A.Fake<IModelBinderLocator>();
            this.validatorLocator = A.Fake<IModelValidatorLocator>();

            this.builder = new DefaultNancyModuleBuilder(this.viewFactory, this.responseFormatterFactory, this.modelBinderLocator, this.validatorLocator);
        }

        [Fact]
        public void Should_return_instance_that_was_returned_by_module_catalog()
        {
            // Given
            var context = new NancyContext();

            // When
            var result = this.builder.BuildModule(this.module, context);

            // Then
            result.ShouldBeSameAs(this.module);
        }

        [Fact]
        public void Should_set_context_on_module_to_provided_context_instance()
        {
            // Given
            var context = new NancyContext();

            // When
            var result = this.builder.BuildModule(this.module, context);

            // Then
            result.Context.ShouldBeSameAs(context);
        }

        [Fact]
        public void Should_set_view_factory_on_module_to_resolved_view_factory()
        {
            // Given
            var context = new NancyContext();

            // When
            var result = this.builder.BuildModule(this.module, context);

            // Then
            result.ViewFactory.ShouldBeSameAs(this.viewFactory);
        }

        [Fact]
        public void Should_pass_context_to_response_formatter_factory()
        {
            // Given
            var context = new NancyContext();

            // When
            this.builder.BuildModule(this.module, context);

            // Then
            A.CallTo(() => this.responseFormatterFactory.Create(context)).MustHaveHappened();
        }
        
        [Fact]
        public void Should_set_response_on_module_to_resolved_response_formatter()
        {
            // Given
            var context = new NancyContext();
            var formatter = A.Fake<IResponseFormatter>();
            A.CallTo(() => this.responseFormatterFactory.Create(context)).Returns(formatter);

            // When
            var result = this.builder.BuildModule(this.module, context);

            // Then
            result.Response.ShouldBeSameAs(formatter);
        }

        [Fact]
        public void Should_set_binder_locator_on_module_to_resolved_binder_locator()
        {
            // Given
            var context = new NancyContext();

            // When
            var result = this.builder.BuildModule(this.module, context);

            // Then
            result.ModelBinderLocator.ShouldBeSameAs(this.modelBinderLocator);
        }
    }
}