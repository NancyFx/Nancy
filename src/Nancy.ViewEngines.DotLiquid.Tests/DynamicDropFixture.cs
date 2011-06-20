namespace Nancy.ViewEngines.DotLiquid.Tests
{
    using System.Dynamic;
    using Nancy.Tests;
    using Xunit;

    public class DynamicDropFixture
    {
        [Fact]
        public void Should_return_model_is_null_message_when_model_is_null()
        {
            // Given
            var drop = new DynamicDrop(null);

            // When
            var result = drop.BeforeMethod(string.Empty);

            // Then
            result.ShouldEqual("[Model is null]");
        }

        [Fact]
        public void Should_return_invalid_model_property_name_message_when_property_name_is_empty()
        {
            // Given
            var drop = new DynamicDrop(new object());

            // When
            var result = drop.BeforeMethod(string.Empty);

            // Then
            result.ShouldEqual("[Invalid model property name]");
        }

        [Fact]
        public void Should_return_invalid_model_property_name_message_when_property_name_is_null()
        {
            // Given
            var drop = new DynamicDrop(new object());

            // When
            var result = drop.BeforeMethod(null);

            // Then
            result.ShouldEqual("[Invalid model property name]");
        }

        [Fact]
        public void Should_return_message_about_property_not_being_found_when_called_with_invalid_property_name_and_model_is_expandoobject()
        {
            // Given
            dynamic model = new ExpandoObject();
            model.Name = "Nancy";

            var drop = new DynamicDrop(model);

            // When
            var result = drop.BeforeMethod("age");

            // Then
            result.ShouldEqual("[Can't find :age in the model]");
        }

        [Fact]
        public void Should_return_model_value_when_property_name_is_valid_and_model_is_expandoobject()
        {
            // Given
            dynamic model = new ExpandoObject();
            model.Name = "Nancy";

            var drop = new DynamicDrop(model);

            // When
            var result = drop.BeforeMethod("Name");

            // Then
            result.ShouldEqual("Nancy");
        }

        [Fact]
        public void Should_return_message_about_property_not_being_found_when_called_with_invalid_property_name_and_model_is_object()
        {
            // Given
            var model = new FakeModel { Name = "Nancy" };
            var drop = new DynamicDrop(model);

            // When
            var result = drop.BeforeMethod("age");

            // Then
            result.ShouldEqual("[Can't find :age in the model]");
        }

        public void Should_return_model_value_when_property_name_is_valid_and_model_is_object()
        {
            // Given
            var model = new FakeModel { Name = "Nancy" };
            var drop = new DynamicDrop(model);

            // When
            var result = drop.BeforeMethod("Name");

            // Then
            result.ShouldEqual("Nancy");
        }
    }
}