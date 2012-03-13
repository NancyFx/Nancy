namespace Nancy.Tests.Unit.ModelBinding
{
    using System;
    using FakeItEasy;
    using Nancy.ModelBinding;
    using Xunit;

    public class DynamicModelBinderAdapterFixture
    {
        [Fact]
        public void Should_throw_if_locator_is_null()
        {
            // Given, When
            var result = Record.Exception(() => new DynamicModelBinderAdapter(null, new NancyContext()));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_context_is_null()
        {
            // Given, When
            var result = Record.Exception(() => new DynamicModelBinderAdapter(A.Fake<IModelBinderLocator>(), null));

            // Then
            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_pass_type_to_locator_when_cast_implcitly()
        {
            // Given
            var fakeModelBinder = A.Fake<IModelBinder>();
            var returnModel = new Model();
            A.CallTo(() => fakeModelBinder.Bind(null, null, null)).WithAnyArguments().Returns(returnModel);

            var fakeLocator = A.Fake<IModelBinderLocator>();
            A.CallTo(() => fakeLocator.GetBinderForType(A<Type>.Ignored, A<NancyContext>.Ignored)).Returns(fakeModelBinder);
            dynamic adapter = new DynamicModelBinderAdapter(fakeLocator, new NancyContext());

            // When
            Model result = adapter;

            // Then
            A.CallTo(() => fakeLocator.GetBinderForType(typeof(Model), A<NancyContext>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_invoke_binder_with_context()
        {
            // Given
            var context = new NancyContext();

            var fakeModelBinder = A.Fake<IModelBinder>();
            var returnModel = new Model();
            A.CallTo(() => fakeModelBinder.Bind(null, null, null)).WithAnyArguments().Returns(returnModel);

            var fakeLocator = A.Fake<IModelBinderLocator>();
            A.CallTo(() => fakeLocator.GetBinderForType(A<Type>.Ignored, context)).Returns(fakeModelBinder);
            dynamic adapter = new DynamicModelBinderAdapter(fakeLocator, context);

            // When
            Model result = adapter;

            // Then
            A.CallTo(() => fakeLocator.GetBinderForType(A<Type>.Ignored, context)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_pass_type_to_locator_when_cast_explicitly()
        {
            // Given
            var fakeModelBinder = A.Fake<IModelBinder>();
            var returnModel = new Model();
            A.CallTo(() => fakeModelBinder.Bind(null, null, null)).WithAnyArguments().Returns(returnModel);

            var fakeLocator = A.Fake<IModelBinderLocator>();
            A.CallTo(() => fakeLocator.GetBinderForType(A<Type>.Ignored, A<NancyContext>.Ignored)).Returns(fakeModelBinder);
            dynamic adapter = new DynamicModelBinderAdapter(fakeLocator, new NancyContext());

            // When
            var result = (Model)adapter;

            // Then
            A.CallTo(() => fakeLocator.GetBinderForType(typeof(Model), A<NancyContext>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_return_object_from_binder_if_binder_doesnt_return_null()
        {
            // Given
            var fakeModelBinder = A.Fake<IModelBinder>();
            var returnModel = new Model();
            A.CallTo(() => fakeModelBinder.Bind(null, null, null)).WithAnyArguments().Returns(returnModel);

            var fakeLocator = A.Fake<IModelBinderLocator>();
            A.CallTo(() => fakeLocator.GetBinderForType(A<Type>.Ignored, A<NancyContext>.Ignored)).Returns(fakeModelBinder);
            dynamic adapter = new DynamicModelBinderAdapter(fakeLocator, new NancyContext());

            // When
            Model result = adapter;

            // Then
            result.ShouldNotBeNull();
            result.ShouldBeSameAs(returnModel);
        }

        [Fact]
        public void Should_throw_if_locator_does_not_return_binder()
        {
            // Given
            var fakeLocator = A.Fake<IModelBinderLocator>();
            A.CallTo(() => fakeLocator.GetBinderForType(A<Type>.Ignored, A<NancyContext>.Ignored)).Returns(null);
            dynamic adapter = new DynamicModelBinderAdapter(fakeLocator, new NancyContext());

            // When
            var result = Record.Exception(() => (Model)adapter);

            // Then
            result.ShouldBeOfType(typeof(ModelBindingException));
        }
    }

    public class Model
    {
    }
}