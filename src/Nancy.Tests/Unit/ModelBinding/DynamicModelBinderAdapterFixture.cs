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
            var result = Record.Exception(() => new DynamicModelBinderAdapter(null, new NancyContext()));

            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_throw_if_context_is_null()
        {
            var result = Record.Exception(() => new DynamicModelBinderAdapter(A.Fake<IModelBinderLocator>(), null));

            result.ShouldBeOfType(typeof(ArgumentNullException));
        }

        [Fact]
        public void Should_pass_type_to_locator_when_cast_implcitly()
        {
            var fakeModelBinder = A.Fake<IModelBinder>();
            var returnModel = new Model();
            A.CallTo(() => fakeModelBinder.Bind(A<NancyContext>.Ignored, A<Type>.Ignored)).Returns(returnModel);
            var fakeLocator = A.Fake<IModelBinderLocator>();
            A.CallTo(() => fakeLocator.GetBinderForType(A<Type>.Ignored)).Returns(fakeModelBinder);
            dynamic adapter = new DynamicModelBinderAdapter(fakeLocator, new NancyContext());

            Model result = adapter;

            A.CallTo(() => fakeLocator.GetBinderForType(typeof(Model))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_pass_type_to_locator_when_cast_explicitly()
        {
            var fakeModelBinder = A.Fake<IModelBinder>();
            var returnModel = new Model();
            A.CallTo(() => fakeModelBinder.Bind(A<NancyContext>.Ignored, A<Type>.Ignored)).Returns(returnModel);
            var fakeLocator = A.Fake<IModelBinderLocator>();
            A.CallTo(() => fakeLocator.GetBinderForType(A<Type>.Ignored)).Returns(fakeModelBinder);
            dynamic adapter = new DynamicModelBinderAdapter(fakeLocator, new NancyContext());

            var result = (Model)adapter;

            A.CallTo(() => fakeLocator.GetBinderForType(typeof(Model))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_return_object_from_binder_if_binder_doesnt_return_null()
        {
            var fakeModelBinder = A.Fake<IModelBinder>();
            var returnModel = new Model();
            A.CallTo(() => fakeModelBinder.Bind(A<NancyContext>.Ignored, A<Type>.Ignored)).Returns(returnModel);
            var fakeLocator = A.Fake<IModelBinderLocator>();
            A.CallTo(() => fakeLocator.GetBinderForType(A<Type>.Ignored)).Returns(fakeModelBinder);
            dynamic adapter = new DynamicModelBinderAdapter(fakeLocator, new NancyContext());

            Model result = adapter;

            result.ShouldNotBeNull();
            result.ShouldBeSameAs(returnModel);
        }

        [Fact]
        public void Should_throw_if_locator_does_not_return_binder()
        {
            var fakeLocator = A.Fake<IModelBinderLocator>();
            A.CallTo(() => fakeLocator.GetBinderForType(A<Type>.Ignored)).Returns(null);
            dynamic adapter = new DynamicModelBinderAdapter(fakeLocator, new NancyContext());

            var result = Record.Exception(() => (Model)adapter);

            result.ShouldBeOfType(typeof(ModelBindingException));
        }
    }

    public class Model
    {
    }
}