namespace Nancy.Tests.Unit
{
    using System;
    using Xunit;

    public class HttpLinkFixture
    {
        [Fact]
        public void ToString_link_with_boolean_parameter_should_exist_without_value_in_string()
        {
            // Given
            var link = new HttpLink("http://nancyfx.org/", "up");
            link.Parameters.Add("parameter", null);

            // When
            var stringValue = link.ToString();

            // Then
            stringValue.ShouldEqual("<http://nancyfx.org/>; rel=\"up\"; parameter");
        }

        [Fact]
        public void ToString_link_with_empty_parameter_should_not_exist_in_string()
        {
            // Given
            var link = new HttpLink("http://nancyfx.org/", "up");
            link.Parameters.Add(string.Empty, null);

            // When
            var stringValue = link.ToString();

            // Then
            stringValue.ShouldEqual("<http://nancyfx.org/>; rel=\"up\"");
        }

        [Fact]
        public void ToString_link_with_extension_parameter_should_exist_in_string()
        {
            // Given
            var link = new HttpLink("http://nancyfx.org/", "up");
            link.Parameters.Add("ext", "extension-param");

            // When
            var stringValue = link.ToString();

            // Then
            stringValue.ShouldEqual("<http://nancyfx.org/>; rel=\"up\"; ext=\"extension-param\"");
        }

        [Fact]
        public void ToString_link_with_relation_should_exist_in_string()
        {
            // Given
            var link = new HttpLink("http://nancyfx.org/", "up");

            // When
            var stringValue = link.ToString();

            // Then
            stringValue.ShouldEqual("<http://nancyfx.org/>; rel=\"up\"");
        }

        [Fact]
        public void ToString_link_with_type_should_exist_in_string()
        {
            // Given
            var link = new HttpLink("http://nancyfx.org/", "up", "application/json");

            // When
            var stringValue = link.ToString();

            // Then
            stringValue.ShouldEqual("<http://nancyfx.org/>; rel=\"up\"; type=\"application/json\"");
        }

        [Fact]
        public void ToString_link_with_whitespace_parameter_should_not_exist_in_string()
        {
            // Given
            var link = new HttpLink("http://nancyfx.org/", "up");
            link.Parameters.Add("    ", null);

            // When
            var stringValue = link.ToString();

            // Then
            stringValue.ShouldEqual("<http://nancyfx.org/>; rel=\"up\"");
        }

        [Fact]
        public void Add_duplicate_parameter_in_different_casing_throws_argument_exception()
        {
            // Given
            var link = new HttpLink("http://nancyfx.org/", "home");
            link.Parameters.Add("up", "up");


            // When
            var exception = Assert.Throws<ArgumentException>(() => link.Parameters.Add("UP", "UP"));

            // Then
            exception.Message.ShouldEqual("An item with the same key has already been added.");
        }
    }
}
