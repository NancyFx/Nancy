namespace Nancy.Tests.Unit.Conventions
{
    using System;
    using System.Collections.Generic;

    using Nancy.Conventions;

    using Xunit;

    public class DefaultStaticContentsConventionsFixture
    {
        private readonly NancyConventions conventions;
        private readonly DefaultStaticContentsConventions staticContentsConventions;

        public DefaultStaticContentsConventionsFixture()
        {
            this.conventions = new NancyConventions();
            this.staticContentsConventions = new DefaultStaticContentsConventions();
        }

        [Fact]
        public void Should_not_be_valid_when_view_location_conventions_is_null()
        {
            // Given
            this.conventions.StaticContentsConventions = null;

            // When
            var result = this.staticContentsConventions.Validate(this.conventions);

            // Then
            result.Item1.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_correct_error_message_when_not_valid_because_view_location_conventions_is_null()
        {
            // Given
            this.conventions.StaticContentsConventions = null;

            // When
            var result = this.staticContentsConventions.Validate(this.conventions);

            // Then
            result.Item2.ShouldEqual("The static contents conventions cannot be null.");
        }

        [Fact]
        public void Should_not_be_valid_when_view_location_conventions_is_empty()
        {
            // Given
            this.conventions.StaticContentsConventions =
                new List<Func<NancyContext, string, Response>>();

            // When
            var result = this.staticContentsConventions.Validate(this.conventions);

            // Then
            result.Item1.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_correct_error_message_when_not_valid_because_view_location_conventions_is_empty()
        {
            // Given
            this.conventions.StaticContentsConventions = new List<Func<NancyContext, string, Response>>();

            // When
            var result = this.staticContentsConventions.Validate(this.conventions);

            // Then
            result.Item2.ShouldEqual("The static contents conventions cannot be empty.");
        }

        [Fact]
        public void Should_be_valid_when_view_conventions_is_not_empty()
        {
            // Given
            this.conventions.StaticContentsConventions =
                new List<Func<NancyContext, string, Response>>
                {
                    (ctx, folder) => {
                        return new Response();
                    }
                };

            // When
            var result = this.staticContentsConventions.Validate(this.conventions);

            // Then
            result.Item1.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_empty_error_message_when_valid()
        {
            // Given
            this.conventions.StaticContentsConventions =
                new List<Func<NancyContext, string, Response>>
                {
                    (ctx, folder) => {
                        return new Response();
                    }
                };

            // When
            var result = this.staticContentsConventions.Validate(this.conventions);

            // Then
            result.Item2.ShouldBeEmpty();
        }

        [Fact]
        public void Should_add_conventions_when_initialised()
        {
            // Given, When
            this.staticContentsConventions.Initialise(this.conventions);

            // Then
            this.conventions.StaticContentsConventions.Count.ShouldBeGreaterThan(0);
        }
    }
}