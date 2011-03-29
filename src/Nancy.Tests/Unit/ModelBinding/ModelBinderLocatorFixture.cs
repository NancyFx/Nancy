namespace Nancy.Tests.Unit.ModelBinding
{
    using System;

    using FakeItEasy;

    using Nancy.ModelBinding;

    using Xunit;

    public class ModelBinderLocatorFixture
    {
        private DefaultBinder defaultBinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ModelBinderLocatorFixture()
        {
            this.defaultBinder = new DefaultBinder(new ITypeConverter[] { }, new IBodyDeserializer[] { });
        }

        [Fact]
        public void Should_not_throw_if_null_binders_collection_is_passed()
        {
            var result = Record.Exception(() => new DefaultModelBinderLocator(null, this.defaultBinder));

            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_default_binder_if_no_specific_binder_exists()
        {
            var fakeBinder = A.Fake<IModelBinder>();
            A.CallTo(() => fakeBinder.CanBind(A<Type>.Ignored)).Returns(false);
            var locator = new DefaultModelBinderLocator(new IModelBinder[] { fakeBinder }, this.defaultBinder);

            var result = locator.GetBinderForType(typeof(Model));

            result.ShouldBeSameAs(this.defaultBinder);
        }

        [Fact]
        public void Should_not_return_a_binder_that_returns_false_for_canbind()
        {
            var fakeBinder = A.Fake<IModelBinder>();
            A.CallTo(() => fakeBinder.CanBind(A<Type>.Ignored)).Returns(false);
            var locator = new DefaultModelBinderLocator(new IModelBinder[] { fakeBinder }, this.defaultBinder);

            var result = locator.GetBinderForType(typeof(Model));

            result.ShouldNotBeSameAs(fakeBinder);
        }

        [Fact]
        public void Should_return_a_binder_that_returns_true_for_canbind()
        {
            var fakeBinder = A.Fake<IModelBinder>();
            A.CallTo(() => fakeBinder.CanBind(A<Type>.Ignored)).Returns(true);
            var locator = new DefaultModelBinderLocator(new IModelBinder[] { fakeBinder }, this.defaultBinder);

            var result = locator.GetBinderForType(typeof(Model));

            result.ShouldBeSameAs(fakeBinder);
        }
    }
}