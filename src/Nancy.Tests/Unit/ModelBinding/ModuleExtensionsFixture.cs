


using System.Reflection;

namespace Nancy.Tests.Unit.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Xunit;
    using Fakes;
    using Nancy.ModelBinding;
    using FakeItEasy;

    public class ModuleExtensionsFixture
    {
        private readonly DefaultBinder defaultBinder;

        public ModuleExtensionsFixture()
        {
            defaultBinder = new DefaultBinder(new ITypeConverter[] { }, new IBodyDeserializer[] { }, A.Fake<IFieldNameConverter>(), new BindingDefaults());
        }

        [Fact]
        public void Should_be_able_to_bind_to_an_existing_instance()
        {
            // Given
            var binder = new ModuleExtensionsFixture.TestModelBinder();
            var locator = new DefaultModelBinderLocator(new IModelBinder[] { binder }, this.defaultBinder);
            var module = new ModuleExtensionsFixture.TestBindingModule();
            module.Context = new NancyContext() { Request = new FakeRequest("GET", "/") };
            module.Context.Request.Form["StringProperty"] = "Test";
            module.Context.Request.Query["IntProperty"] = "23";
            module.ModelBinderLocator = locator;

            var model = new TestModel {StringProperty = "String Value", IntProperty = 42};

            // When
            module.BindTo(model);

            // Then
            model.StringProperty.ShouldEqual("Test");
            model.IntProperty.ShouldEqual(42);

        }

        private static NancyContext CreateContextWithHeader(string name, IEnumerable<string> values)
        {
            var header = new Dictionary<string, IEnumerable<string>>
            {
                { name, values }
            };

            return new NancyContext()
            {
                Request = new FakeRequest("GET", "/", header),
                Parameters = DynamicDictionary.Empty
            };
        }

        private class TestBindingModule : NancyModule
        {

        }


        class TestModelBinder : IModelBinder
        {
            public object Bind(NancyContext context, Type modelType, params string[] blackList)
            {
                return new TestModel{StringProperty = "Test"};
            }

            public bool CanBind(Type modelType)
            {
                return modelType == typeof (TestModel);
            }

            public IEnumerable<PropertyInfo> BoundProperties
            {
                get
                {
                    return from prop in typeof(TestModel).GetProperties()
                           where prop.Name == "StringProperty"
                           select prop;
                }
            }
        }

        public class TestModel
        {
            public string StringProperty { get; set; }

            public int IntProperty { get; set; }

            public DateTime DateProperty { get; set; }

            public int this[int index]
            {
                get { return 0; }
                set { }
            }
        }
    }

}
