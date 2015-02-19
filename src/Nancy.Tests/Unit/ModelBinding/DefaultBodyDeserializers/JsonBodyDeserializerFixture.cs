namespace Nancy.Tests.Unit.ModelBinding.DefaultBodyDeserializers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using FakeItEasy;

    using Nancy.Json;
    using Nancy.ModelBinding;
    using Nancy.ModelBinding.DefaultBodyDeserializers;

    using Xunit;
    using Xunit.Extensions;

    public class JsonBodyDeserializerFixture
    {
        private readonly JavaScriptSerializer serializer;
        private readonly JsonBodyDeserializer deserialize;
        private readonly TestModel testModel;
        private readonly string testModelJson;

        public JsonBodyDeserializerFixture()
        {
            this.deserialize = new JsonBodyDeserializer();

            this.testModel = new TestModel()
                {
                    IntProperty = 12,
                    StringProperty = "More cowbell",
                    DateProperty = DateTime.Parse("2011/12/25"),
                    ArrayProperty = new[] { "Ping", "Pong" }
                };

            this.serializer = new JavaScriptSerializer();
            this.serializer.RegisterConverters(JsonSettings.Converters);
            this.testModelJson = this.serializer.Serialize(this.testModel);
        }

        [Fact]
        public void Should_report_false_for_can_deserialize_for_non_json_format()
        {
            // Given
            const string contentType = "application/xml";

            // When
            var result = this.deserialize.CanDeserialize(contentType, A<BindingContext>._);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_report_true_for_can_deserialize_for_application_json()
        {
            // Given
            const string contentType = "application/json";

            // When
            var result = this.deserialize.CanDeserialize(contentType, A<BindingContext>._);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_report_true_for_can_deserialize_for_text_json()
        {
            // Given
            const string contentType = "text/json";

            // When
            var result = this.deserialize.CanDeserialize(contentType, A<BindingContext>._);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_report_true_for_can_deserialize_for_custom_json_format()
        {
            // Given
            const string contentType = "application/vnd.org.nancyfx.mything+json";

            // When
            var result = this.deserialize.CanDeserialize(contentType, A<BindingContext>._);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_be_case_insensitive_in_can_deserialize()
        {
            // Given
            const string contentType = "appLicaTion/jsOn";

            // When
            var result = this.deserialize.CanDeserialize(contentType, A<BindingContext>._);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_deserialize_timespan()
        {
            // Given
            var json = this.serializer.Serialize(TimeSpan.FromDays(14));
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var context = new BindingContext()
            {
                DestinationType = typeof(TimeSpan),
                ValidModelBindingMembers = BindingMemberInfo.Collect<TimeSpan>().ToList(),
            };

            // When
            var result = (TimeSpan)this.deserialize.Deserialize(
                            "application/json",
                            bodyStream,
                            context);

            // Then
            result.Days.ShouldEqual(14);
        }

        [Fact]
        public void Should_deserialize_enum()
        {
            // Given
            var json = this.serializer.Serialize(TestEnum.One);
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var context = new BindingContext()
            {
                DestinationType = typeof (TestEnum),
                ValidModelBindingMembers = BindingMemberInfo.Collect<TestEnum>().ToList(),
            };

            // When
            var result = (TestEnum)this.deserialize.Deserialize(
                "application/json",
                bodyStream,
                context);

            // Then
            result.ShouldEqual(TestEnum.One);
        }

        [Theory]
        [InlineData(TestEnum.Hundred)]
        [InlineData(null)]
        public void Should_deserialize_nullable_enum(TestEnum? propertyValue)
        {
            var context = new BindingContext()
            {
                DestinationType = typeof(TestModel),
                ValidModelBindingMembers = BindingMemberInfo.Collect<TestModel>().ToList(),
            };

            var model = new TestModel { NullableEnumProperty = propertyValue };

            var s = new JavaScriptSerializer();
            var serialized = s.Serialize(model);
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));

            // When
            var result = (TestModel)this.deserialize.Deserialize(
                "application/json",
                bodyStream,
                context);

            // Then
            result.NullableEnumProperty.ShouldEqual(propertyValue);
        }

        [Fact]
        public void Should_deserialize_list_of_primitives()
        {
            // Given
            var context = new BindingContext()
            {
                DestinationType = typeof (TestModel),
                ValidModelBindingMembers = BindingMemberInfo.Collect<TestModel>().ToList(),
            };

            var model =
                new TestModel
                {
                    ListOfPrimitivesProperty = new List<int> { 1, 3, 5 },
                    ListOfPrimitivesField = new List<int> { 2, 4, 6 },
                };

            var s = new JavaScriptSerializer();
            var serialized = s.Serialize(model);
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));

            // When
            var result = (TestModel)this.deserialize.Deserialize(
                            "application/json",
                            bodyStream,
                            context);

            // Then
            result.ListOfPrimitivesProperty.ShouldHaveCount(3);
            result.ListOfPrimitivesProperty[0].ShouldEqual(1);
            result.ListOfPrimitivesProperty[1].ShouldEqual(3);
            result.ListOfPrimitivesProperty[2].ShouldEqual(5);

            result.ListOfPrimitivesField.ShouldHaveCount(3);
            result.ListOfPrimitivesField[0].ShouldEqual(2);
            result.ListOfPrimitivesField[1].ShouldEqual(4);
            result.ListOfPrimitivesField[2].ShouldEqual(6);
        }

        [Fact]
        public void Should_deserialize_list_of_complex_objects()
        {
            // Given
            var context = new BindingContext()
            {
                DestinationType = typeof(TestModel),
                ValidModelBindingMembers = BindingMemberInfo.Collect<TestModel>().ToList(),
            };

            var model =
                new TestModel
                {
                    ListOfComplexObjectsProperty = new List<ModelWithStringValues>
                    {
                        new ModelWithStringValues() { Value1 = "one", Value2 = "two"},
                        new ModelWithStringValues() { Value1 = "three", Value2 = "four"}
                    },
                    ListOfComplexObjectsField = new List<ModelWithStringValues>
                    {
                        new ModelWithStringValues() { Value1 = "five", Value2 = "six"},
                        new ModelWithStringValues() { Value1 = "seven", Value2 = "eight"}
                    }
                };

            var s = new JavaScriptSerializer();
            var serialized = s.Serialize(model);
            var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));

            // When
            var result = (TestModel)this.deserialize.Deserialize(
                            "application/json",
                            bodyStream,
                            context);

            // Then
            result.ListOfComplexObjectsProperty.ShouldHaveCount(2);
            result.ListOfComplexObjectsProperty[0].Value1.ShouldEqual("one");
            result.ListOfComplexObjectsProperty[0].Value2.ShouldEqual("two");
            result.ListOfComplexObjectsProperty[1].Value1.ShouldEqual("three");
            result.ListOfComplexObjectsProperty[1].Value2.ShouldEqual("four");
            result.ListOfComplexObjectsField.ShouldHaveCount(2);
            result.ListOfComplexObjectsField[0].Value1.ShouldEqual("five");
            result.ListOfComplexObjectsField[0].Value2.ShouldEqual("six");
            result.ListOfComplexObjectsField[1].Value1.ShouldEqual("seven");
            result.ListOfComplexObjectsField[1].Value2.ShouldEqual("eight");
        }

        [Fact]
        public void Should_Deserialize_Signed_And_Unsigned_Nullable_Numeric_Types()
        {
            //Given
            const string json = "{P1: 1, P2: 2, P3: 3, F1: 4, F2: 5, F3: 6}";

            //When
            var model = this.serializer.Deserialize<ModelWithNullables> (json);

            //Should
            Assert.Equal (1, model.P1);
            Assert.Equal ((uint)2, model.P2);
            Assert.Equal ((uint)3, model.P3);
            Assert.Equal (4, model.F1);
            Assert.Equal ((uint)5, model.F2);
            Assert.Equal ((uint)6, model.F3);
        }

#if !__MonoCS__
        [Fact]
        public void Should_Serialize_Doubles_In_Different_Cultures()
        {
            // TODO - fixup on mono, seems to throw inside double.parse
            // Given
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            var modelWithDoubleValues =
                new ModelWithDoubleValues
                    {
                        Latitude = 50.933984,
                        Longitude = 7.330627
                    };

            var s = new JavaScriptSerializer();
            var serialized = s.Serialize(modelWithDoubleValues);

            // When
            var deserializedModelWithDoubleValues = s.Deserialize<ModelWithDoubleValues>(serialized);

            // Then
            Assert.Equal(modelWithDoubleValues.Latitude, deserializedModelWithDoubleValues.Latitude);
            Assert.Equal(modelWithDoubleValues.Longitude, deserializedModelWithDoubleValues.Longitude);
        }

#endif

        [Theory]
        [InlineData("\n")]
        [InlineData("\n\r")]
        [InlineData("\r\n")]
        [InlineData("\r")]
        public void Should_Serialize_Last_Prop_is_Bool_And_Trailing_NewLine(string lineEndings)
        {
            // Given
            var json = string.Concat("{\"Property\": true", lineEndings, "}");

            // When
            var s = new JavaScriptSerializer();
            var deserialized = (dynamic)s.DeserializeObject(json);

            // Then
            Assert.True(deserialized["Property"]);
        }

        [Fact]
        public void Should_Serialize_Last_Prop_is_Bool()
        {
            // Given
            var json = "{\"Property\": true}";

            // When
            var s = new JavaScriptSerializer();
            var deserialized = (dynamic)s.DeserializeObject(json);

            // Then
            Assert.True(deserialized["Property"]);
        }

        public class TestModel : IEquatable<TestModel>
        {
            public string StringProperty { get; set; }

            public int IntProperty { get; set; }

            public DateTime DateProperty { get; set; }

            public string[] ArrayProperty { get; set; }

            public TestEnum? NullableEnumProperty { get; set; }

            public List<int> ListOfPrimitivesProperty { get; set; }

            public List<int> ListOfPrimitivesField;

            public List<ModelWithStringValues> ListOfComplexObjectsProperty { get; set; }

            public List<ModelWithStringValues> ListOfComplexObjectsField { get; set; }

            public bool Equals(TestModel other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return other.StringProperty == this.StringProperty &&
                       other.IntProperty == this.IntProperty &&
                       !other.ArrayProperty.Except(this.ArrayProperty).Any() &&
                       other.DateProperty.ToShortDateString() == this.DateProperty.ToShortDateString();
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != typeof(TestModel))
                {
                    return false;
                }

                return Equals((TestModel)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = (this.StringProperty != null ? this.StringProperty.GetHashCode() : 0);
                    result = (result * 397) ^ this.IntProperty;
                    result = (result * 397) ^ this.DateProperty.GetHashCode();
                    result = (result * 397) ^ (this.ArrayProperty != null ? this.ArrayProperty.GetHashCode() : 0);
                    return result;
                }
            }

            public static bool operator ==(TestModel left, TestModel right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TestModel left, TestModel right)
            {
                return !Equals(left, right);
            }
        }

        public enum TestEnum
        {
            One = 1,
            Hundred = 100
        }
    }

    public class ModelWithStringValues
    {
        public string Value1 { get; set; }

        public string Value2;
    }

    public class ModelWithDoubleValues
    {
        public double Latitude { get; set; }

        public double Longitude;
    }

    public class ModelWithNullables 
    {
        public int? P1 { get; set; }
        public uint P2 { get; set; }
        public uint? P3 { get; set; }

        public int? F1;
        public uint F2;
        public uint? F3;
    }

}