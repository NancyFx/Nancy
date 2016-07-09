namespace Nancy.Tests.Unit.Configuration
{
    using System.Linq;
    using Nancy.Configuration;
    using Xunit;
    using Xunit.Extensions;

    public class DefaultNancyEnvironmentFixture
    {
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void Should_add_key_to_environment_when_invoking_addvalue(int numberOfElementsToAdd)
        {
            // Given, When
            var environment = CreateEnvironment(numberOfElementsToAdd);

            // Then
            for (var value = 0; value < numberOfElementsToAdd; value++)
            {
                environment.ContainsKey(value.ToString()).ShouldBeTrue();
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(6)]
        public void Should_add_value_to_environment_when_invoking_addvalue(int numberOfElementsToAdd)
        {
            // Given, When
            var environment = CreateEnvironment(numberOfElementsToAdd);

            // Then
            for (var value = 0; value < numberOfElementsToAdd; value++)
            {
                environment[value.ToString()].Equals(value).ShouldBeTrue();
            }
        }

        [Theory]
        [InlineData(7)]
        [InlineData(9)]
        [InlineData(12)]
        public void Should_return_all_keys_when_invoking_keys(int numberOfElementsToAdd)
        {
            // Given
            var environment = CreateEnvironment(numberOfElementsToAdd);

            // When
            var returnedKeys = environment.Keys.ToArray();

            // Then
            returnedKeys.Count().ShouldEqual(numberOfElementsToAdd);

            for (var value = 0; value < numberOfElementsToAdd; value++)
            {
                returnedKeys.Contains(value.ToString()).ShouldBeTrue();
            }
        }

        [Theory]
        [InlineData(7)]
        [InlineData(9)]
        [InlineData(12)]
        public void Should_return_all_values_when_invoking_values(int numberOfElementsToAdd)
        {
            // Given
            var environment = CreateEnvironment(numberOfElementsToAdd);

            // When
            var returnedValues = environment.Values.ToArray();

            // Then
            returnedValues.Count().ShouldEqual(numberOfElementsToAdd);

            for (var value = 0; value < numberOfElementsToAdd; value++)
            {
                returnedValues.Contains(value).ShouldBeTrue();
            }
        }

        [Theory]
        [InlineData(7)]
        [InlineData(9)]
        [InlineData(12)]
        public void Should_return_count_when_invoking_count(int numberOfElementsToAdd)
        {
            // Given
            var environment = CreateEnvironment(numberOfElementsToAdd);

            // When
            var returnedCount = environment.Count;

            // Then
            returnedCount.ShouldEqual(numberOfElementsToAdd);
        }

        [Theory]
        [InlineData("nancy", "nancy", true)]
        [InlineData("nancy", "frank", false)]
        public void Should_return_correct_key_presence_when_invoking_containskey(string keyToAdd, string keyToLookFor, bool expectedResult)
        {
            // Given
            var environment = new DefaultNancyEnvironment();
            environment.AddValue(keyToAdd, new object());

            // When
            var result = environment.ContainsKey(keyToLookFor);

            // Then
            result.ShouldEqual(expectedResult);
        }

        [Fact]
        public void Should_return_value_if_key_exists_when_invoking_trygetvalue()
        {
            // Given
            var environment = new DefaultNancyEnvironment();
            var expectedValue = new object();
            environment.AddValue("nancy", expectedValue);
            object output;

            // When
            environment.TryGetValue("nancy", out output);

            // Then
            output.ShouldBeSameAs(expectedValue);
        }

        [Theory]
        [InlineData("nancy", true)]
        [InlineData("frank", false)]
        public void Should_return_correct_status_value_when_invoking_trygetvalue(string keyToLookFor, bool expectedResult)
        {
            // Given
            var environment = new DefaultNancyEnvironment();
            environment.AddValue("nancy", new object());
            object output;

            // When
            var result = environment.TryGetValue(keyToLookFor, out output);

            // Then
            result.ShouldEqual(expectedResult);
        }

        [Fact]
        public void Should_return_default_value_if_key_does_not_exist_when_invoking_trygetvalue()
        {
            // Given
            var environment = new DefaultNancyEnvironment();
            var expectedValue = new object();
            environment.AddValue("nancy", expectedValue);
            object output;

            // When
            environment.TryGetValue("foo", out output);

            // Then
            output.ShouldEqual(default(object));
        }

        private static INancyEnvironment CreateEnvironment(int numberOfElementsToAdd)
        {
            var environment = new DefaultNancyEnvironment();

            for (var value = 0; value < numberOfElementsToAdd; value++)
            {
                environment.AddValue(value.ToString(), value);
            }

            return environment;
        }
    }
}