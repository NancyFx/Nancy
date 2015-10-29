namespace Nancy.Tests.Unit.ModelBinding
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    using Nancy.ModelBinding;

    using Xunit;
    using Xunit.Sdk;

    public class BindingMemberInfoFixture
    {
        [Fact]
        public void Should_return_MemberInfo_for_properties_or_fields()
        {
            // Given
            var type = typeof(TestModel);
            var underlyingFieldInfo = type.GetFields().First();
            var underlyingPropertyInfo = type.GetProperties().First();

            // When
            var fieldInfo = new BindingMemberInfo(underlyingFieldInfo);
            var propertyInfo = new BindingMemberInfo(underlyingPropertyInfo);

            // Then
            fieldInfo.MemberInfo.ShouldEqual(underlyingFieldInfo);
            propertyInfo.MemberInfo.ShouldEqual(underlyingPropertyInfo);
        }

        [Fact]
        public void Should_return_Name_for_properties_or_fields()
        {
            // Given
            var type = typeof(TestModel);
            var underlyingFieldInfo = type.GetFields().First();
            var underlyingPropertyInfo = type.GetProperties().First();

            // When
            var fieldInfo = new BindingMemberInfo(underlyingFieldInfo);
            var propertyInfo = new BindingMemberInfo(underlyingPropertyInfo);

            // Then
            fieldInfo.Name.ShouldEqual(underlyingFieldInfo.Name);
            propertyInfo.Name.ShouldEqual(underlyingPropertyInfo.Name);
        }

        [Fact]
        public void Should_return_PropertyType_for_properties_or_fields()
        {
            // Given
            var properties = BindingMemberInfo.Collect<TestModel>();

            // When

            // Then
            properties.ShouldHaveCount(4);

            foreach (var propInfo in properties)
            {
                if (propInfo.Name.StartsWith("Int"))
                {
                    propInfo.PropertyType.ShouldEqual(typeof(int));
                }
                else if (propInfo.Name.StartsWith("String"))
                {
                    propInfo.PropertyType.ShouldEqual(typeof(string));
                }
                else
                {
                    throw new AssertException("Internal error in unit test: Test model property/field name does not follow the expected convention: " + propInfo.Name);
                }
            }
        }

        [Fact]
        public void Should_get_fields()
        {
            // Given
            var propInfo = BindingMemberInfo.Collect<TestModel>().Where(prop => prop.Name.EndsWith("Field"));
            var model = new TestModel();

            // When
            model.IntField = 669;
            model.StringField = "testing";

            // Then
            propInfo.Single(prop => prop.PropertyType == typeof(int))
                .GetValue(model)
                .ShouldEqual(669);

            propInfo.Single(prop => prop.PropertyType == typeof(string))
                .GetValue(model)
                .ShouldEqual("testing");
        }

        [Fact]
        public void Should_set_fields()
        {
            // Given
            var propInfo = BindingMemberInfo.Collect<TestModel>().Where(prop => prop.Name.EndsWith("Field"));
            var model = new TestModel();

            // When
            propInfo.Single(prop => prop.PropertyType == typeof(int))
                .SetValue(model, 42);

            propInfo.Single(prop => prop.PropertyType == typeof(string))
                .SetValue(model, "nineteen");

            // Then
            model.IntField.ShouldEqual(42);
            model.StringField.ShouldEqual("nineteen");
        }

        [Fact]
        public void Should_get_properties()
        {
            // Given
            var propInfo = BindingMemberInfo.Collect<TestModel>().Where(prop => prop.Name.EndsWith("Property"));
            var model = new TestModel();

            // When
            model.IntProperty = 1701;
            model.StringProperty = "NancyFX Unit Testing";

            // Then
            propInfo.Single(prop => prop.PropertyType == typeof(int))
                .GetValue(model)
                .ShouldEqual(1701);

            propInfo.Single(prop => prop.PropertyType == typeof(string))
                .GetValue(model)
                .ShouldEqual("NancyFX Unit Testing");
        }

        [Fact]
        public void Should_set_properties()
        {
            // Given
            var propInfo = BindingMemberInfo.Collect<TestModel>().Where(prop => prop.Name.EndsWith("Property"));
            var model = new TestModel();

            // When
            propInfo.Single(prop => prop.PropertyType == typeof(int))
                .SetValue(model, 2600);

            propInfo.Single(prop => prop.PropertyType == typeof(string))
                .SetValue(model, "R2D2");

            // Then
            model.IntProperty.ShouldEqual(2600);
            model.StringProperty.ShouldEqual("R2D2");
        }

        [Fact]
        public void Should_collect_all_bindable_members_and_skip_all_others()
        {
            // Given

            // When
            var properties = BindingMemberInfo.Collect<BiggerTestModel>();

            // Then
            properties.ShouldHaveCount(16);

            foreach (var property in properties)
                property.Name.ShouldStartWith("Bindable");
        }

        [Fact]
        public void Should_be_able_to_use_except_on_two_lists_of_bindingMemberInfos()
        {
            // Given
            var properties = BindingMemberInfo.Collect<TestModel>();
            var except = BindingMemberInfo.Collect<TestModel>().Where(i => i.Name.Contains("Property"));

            // When
            var res = properties.Except(except).ToList();

            // Then
            res.Count.ShouldEqual(2);
            res[0].Name.ShouldEqual("IntField");
            res[1].Name.ShouldEqual("StringField");
        }

        public class TestModel
        {
            public int IntField;
            public string StringField;

            public int IntProperty { get; set; }
            public string StringProperty { get; set; }
        }

        public class BiggerTestModel
        {
            public int BindableIntField;
            public int BindableIntProperty { get; set; }
            public string BindableStringField;
            public string BindableStringProperty { get; set; }
            public TestModel BindableTestModelField;
            public TestModel BindableTestModelProperty { get; set; }
            public BiggerTestModel BindableBiggerTestModelField;
            public BiggerTestModel BindableBiggerTestModelProperty { get; set; }
            [XmlIgnore]
            public IEnumerable<int> BindableEnumerableField;
            [XmlIgnore]
            public IEnumerable<int> BindableEnumerableProperty { get; set; }
            public List<string> BindableListField;
            public List<string> BindableListProperty { get; set; }
            public double[] BindableArrayField;
            public double[] BindableArrayProperty { get; set; }
            public TestModel[] BindableArrayOfObjectsField;
            public TestModel[] BindableArrayOfObjectsProperty { get; set; }

            public int this[int index]
            {
                get { return 0; }
                set { }
            }

            public int this[string index, int index2]
            {
                get { return 0; }
                set { }
            }

            public readonly int UnbindableReadOnlyField = 74205;

            public string UnbindableReadOnlyProperty
            {
                get { return "hi"; }
            }

            public string UnbindableWriteOnlyProperty
            {
                set { }
            }

#pragma warning disable 169
            // ReSharper disable once InconsistentNaming
            private int UnbindablePrivateField;
#pragma warning restore 169

            public static int UnbindableStaticField;

            public static int UnbindableStaticProperty
            {
                get { return 0; }
                set { }
            }
        }
    }
}
