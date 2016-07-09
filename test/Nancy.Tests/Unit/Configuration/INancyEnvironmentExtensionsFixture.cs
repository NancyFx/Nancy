namespace Nancy.Tests.Unit.Configuration
{
    using System.Collections.Generic;
    using Nancy.Configuration;
    using Xunit;

    public class INancyEnvironmentExtensionsFixture
    {
        [Fact]
        public void Should_add_element_with_full_name_of_type_as_key_when_invoking_addvalue()
        {
            // Given
            var environment = new DefaultNancyEnvironment();
            var expected = typeof(INancyEnvironmentExtensionsFixture).FullName;

            // When
            environment.AddValue<INancyEnvironmentExtensionsFixture>(null);

            // Then
            environment.ContainsKey(expected).ShouldBeTrue();
        }

        [Fact]
        public void Should_add_element_with_value_when_invoking_addvalue()
        {
            // Given
            var environment = new DefaultNancyEnvironment();
            var expected = new object();

            // When
            environment.AddValue(expected);

            // Then
            ((IReadOnlyDictionary<string, object>)environment)[typeof(object).FullName].ShouldBeSameAs(expected);
        }

        [Fact]
        public void Should_retrieve_value_using_full_name_of_type_when_invoking_getvalue_with_type()
        {
            // Given
            var environment = new DefaultNancyEnvironment();
            var expected = new object();

            environment.AddValue(typeof(object).FullName, expected);

            // When
            var result = environment.GetValue<object>();

            // Then
            result.ShouldBeSameAs(expected);
        }

        [Fact]
        public void Should_retrieve_value_when_invoking_getvalue_with_string_key()
        {
            // Given
            const string key = "thekey";

            var environment = new DefaultNancyEnvironment();
            var expected = new object();

            environment.AddValue(key, expected);

            // When
            var result = environment.GetValue<object>(key);

            // Then
            result.ShouldBeSameAs(expected);
        }

        [Fact]
        public void Should_retrieve_value_using_full_name_of_type_when_invoking_getvaluewithdefault_with_type_and_element_exists()
        {
            // Given
            var environment = new DefaultNancyEnvironment();
            var expected = new object();

            environment.AddValue(typeof(object).FullName, expected);

            // When
            var result = environment.GetValueWithDefault(new object());

            // Then
            result.ShouldBeSameAs(expected);
        }

        [Fact]
        public void Should_retrieve_default_value_using_full_name_of_type_when_invoking_getvaluewithdefault_with_type_and_element_does_not_exists()
        {
            // Given
            var environment = new DefaultNancyEnvironment();
            var defaultValue = new object();

            environment.AddValue(typeof(string).FullName, new object());

            // When
            var result = environment.GetValueWithDefault(defaultValue);

            // Then
            result.ShouldBeSameAs(defaultValue);
        }

        [Fact]
        public void Should_retrieve_value_when_invoking_getvaluewithdefault_with_string_key_and_element_exists()
        {
            // Given
            const string key = "thekey";
            var environment = new DefaultNancyEnvironment();
            var expected = new object();

            environment.AddValue(key, expected);

            // When
            var result = environment.GetValueWithDefault(key, new object());

            // Then
            result.ShouldBeSameAs(expected);
        }

        [Fact]
        public void Should_retrieve_default_value_when_invoking_getvaluewithdefault_with_string_key_and_element_does_not_exists()
        {
            // Given
            const string key = "thekey";
            var environment = new DefaultNancyEnvironment();
            var defaultValue = new object();

            environment.AddValue(typeof(string).FullName, new object());

            // When
            var result = environment.GetValueWithDefault(key, defaultValue);

            // Then
            result.ShouldBeSameAs(defaultValue);
        }
    }
}