namespace Nancy.Tests.Unit.ModelBinding
{
    using System;

    using FakeItEasy;

    using Nancy.ModelBinding;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class DefaultModelBinderLocatorFixture
    {
        private readonly DefaultBinder defaultBinder;

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

            var result = locator.GetBinderForType(typeof(Model), A<NancyContext>.Ignored);

            result.ShouldBeSameAs(this.defaultBinder);
        }

        [Fact]
        public void Should_not_return_a_binder_that_returns_false_for_canbind()
        {
            var fakeBinder = A.Fake<IModelBinder>();
            A.CallTo(() => fakeBinder.CanBind(A<Type>.Ignored)).Returns(false);
            var locator = new DefaultModelBinderLocator(new IModelBinder[] { fakeBinder }, this.defaultBinder);

            var result = locator.GetBinderForType(typeof(Model), A<NancyContext>.Ignored);

            result.ShouldNotBeSameAs(fakeBinder);
        }

        [Fact]
        public void Should_return_a_binder_that_returns_true_for_canbind()
        {
            var fakeBinder = A.Fake<IModelBinder>();
            A.CallTo(() => fakeBinder.CanBind(A<Type>.Ignored)).Returns(true);
            var locator = new DefaultModelBinderLocator(new IModelBinder[] { fakeBinder }, this.defaultBinder);

            var result = locator.GetBinderForType(typeof(Model), A<NancyContext>.Ignored);

            result.ShouldBeSameAs(fakeBinder);
        }

        [Fact]
        public void Should_be_able_to_bind_interfaces()
        {
            var binder = new InterfaceModelBinder();
            var locator = new DefaultModelBinderLocator(new IModelBinder[] { binder }, this.defaultBinder);
            var locatedBinder = locator.GetBinderForType(typeof(Concrete), A<NancyContext>.Ignored);

            var result = locatedBinder.Bind(null, typeof(Concrete), null, new BindingConfig()) as IAmAnInterface;

            result.ShouldNotBeNull();
        }

        [Fact]
        public void Should_be_able_to_bind_interfaces_using_module_extensions()
        {
            var binder = 
                new InterfaceModelBinder();
            
            var locator = 
                new DefaultModelBinderLocator(new IModelBinder[] { binder }, this.defaultBinder);
            
            var module = new TestBindingModule
            {
                Context = new NancyContext() { Request = new FakeRequest("GET", "/") },
                ModelBinderLocator = locator
            };

            var result = module.TestBindInterface();
            var result2 = module.TestBindConcrete();

            result.ShouldNotBeNull();
            result2.ShouldNotBeNull();
        }

        private class TestBindingModule : NancyModule
        {
            public IAmAnInterface TestBindInterface()
            {
                var result = this.Bind<IAmAnInterface>();

                return result;
            }

            public IAmAnInterface TestBindConcrete()
            {
                var result = this.Bind<Concrete>();

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
            public object Bind(NancyContext context, Type modelType, object instance, BindingConfig configuration, params string[] blackList)
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