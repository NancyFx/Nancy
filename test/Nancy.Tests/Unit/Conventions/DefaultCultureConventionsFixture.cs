namespace Nancy.Tests.Unit.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Nancy.Conventions;

    using Xunit;

    public class DefaultCultureConventionsFixture
    {
        private readonly NancyConventions conventions;
        private readonly DefaultCultureConventions cultureConventions;

        public DefaultCultureConventionsFixture()
        {
            this.conventions = new NancyConventions();
            this.cultureConventions = new DefaultCultureConventions();
        }

        [Fact]
        public void Should_not_be_valid_when_view_culture_conventions_is_null()
        {
            // Given
            this.conventions.CultureConventions = null;

            // When
            var result = this.cultureConventions.Validate(this.conventions);

            // Then
            result.Item1.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_correct_error_message_when_not_valid_because_culture_conventions_is_null()
        {
            // Given
            this.conventions.CultureConventions = null;

            // When
            var result = this.cultureConventions.Validate(this.conventions);

            // Then
            result.Item2.ShouldEqual("The culture conventions cannot be null.");
        }

        [Fact]
        public void Should_not_be_valid_when_culture_conventions_is_empty()
        {
            // Given
            this.conventions.CultureConventions =
                new List<Func<NancyContext, CultureInfo>>();

            // When
            var result = this.cultureConventions.Validate(this.conventions);

            // Then
            result.Item1.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_correct_error_message_when_not_valid_because_culture_conventions_is_empty()
        {
            // Given
            this.conventions.CultureConventions = new List<Func<NancyContext, CultureInfo>>();

            // When
            var result = this.cultureConventions.Validate(this.conventions);

            // Then
            result.Item2.ShouldEqual("The culture conventions cannot be empty.");
        }

        [Fact]
        public void Should_be_valid_when_culture_conventions_is_not_empty()
        {
            // Given
            this.conventions.CultureConventions =
                new List<Func<NancyContext, CultureInfo>>
                {
                    (ctx) => {
                        return new CultureInfo("en-GB");
                    }
                };

            // When
            var result = this.cultureConventions.Validate(this.conventions);

            // Then
            result.Item1.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_empty_error_message_when_valid()
        {
            // Given
            this.conventions.CultureConventions =
                new List<Func<NancyContext, CultureInfo>>
                {
                    (ctx) => {
                        return new CultureInfo("en-GB");
                    }
                };

            // When
            var result = this.cultureConventions.Validate(this.conventions);

            // Then
            result.Item2.ShouldBeEmpty();
        }

        [Fact]
        public void Should_add_conventions_when_initialised()
        {
            // Given, When
            this.cultureConventions.Initialise(this.conventions);

            // Then
            this.conventions.CultureConventions.Count.ShouldBeGreaterThan(0);
        }
    }
}
