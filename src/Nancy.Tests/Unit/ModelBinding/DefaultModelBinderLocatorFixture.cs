namespace Nancy.Tests.Unit.ModelBinding
{
    using System;

    using FakeItEasy;

    using Nancy.ModelBinding;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class DefaultModelBinderLocatorFixture
    {
        private DefaultBinder defaultBinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DefaultModelBinderLocatorFixture()
        {
            this.defaultBinder = new DefaultBinder(new ITypeConverter[] { }, new IBodyDeserializer[] { }, A.Fake<IFieldNameConverter>(), new BindingDefaults());
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

        [Fact]
        public void Should_be_able_to_bind_interfaces()
        {
            var binder = new InterfaceModelBinder();
            var locator = new DefaultModelBinderLocator(new IModelBinder[] { binder }, this.defaultBinder);
            var locatedBinder = locator.GetBinderForType(typeof(Concrete));

            var result = locatedBinder.Bind(null, typeof(Concrete)) as IAmAnInterface;

            result.ShouldNotBeNull();
        }

        [Fact]
        public void Should_be_able_to_bind_interfaces_using_module_extensions()
        {
            var binder = new InterfaceModelBinder();
            var locator = new DefaultModelBinderLocator(new IModelBinder[] { binder }, this.defaultBinder);
            var module = new TestBindingModule();
            module.Context = new NancyContext() { Request =  new FakeRequest("GET", "/") };
            module.ModelBinderLocator = locator;

            var result = module.TestBind() as IAmAnInterface;

            result.ShouldNotBeNull();
        }

        private class TestBindingModule : NancyModule
        {
            public object TestBind()
            {
                var result = this.Bind<IAmAnInterface>();

                return result;
            }
        }

        interface IAmAnInterface
        {
             
        }

        class Concrete : IAmAnInterface
        {
             
        }

        class InterfaceModelBinder : IModelBinder
        {
            public object Bind(NancyContext context, Type modelType, params string[] blackList)
            {
                return new Concrete() as IAmAnInterface;
            }

            public bool CanBind(Type modelType)
            {
                return typeof(IAmAnInterface).IsAssignableFrom(modelType);
            }
        }
    }
}