namespace Nancy.Tests.Unit.Conventions
{
    using System;
    using System.Collections.Generic;
    using Nancy.Conventions;
    using Nancy.ViewEngines;
    using Xunit;

    public class DefaultViewLocationConventionsFixture
    {
        private readonly NancyConventions conventions;
        private readonly DefaultViewLocationConventions viewLocationConventions;

        public DefaultViewLocationConventionsFixture()
        {
            this.conventions = new NancyConventions();
            this.viewLocationConventions = new DefaultViewLocationConventions();
        }

        [Fact]
        public void Should_not_be_valid_when_view_location_conventions_is_null()
        {
            // Given
            this.conventions.ViewLocationConventions = null;

            // When
            var result = this.viewLocationConventions.Validate(this.conventions);

            // Then
            result.Item1.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_correct_error_message_when_not_valid_because_view_location_conventions_is_null()
        {
            // Given
            this.conventions.ViewLocationConventions = null;

            // When
            var result = this.viewLocationConventions.Validate(this.conventions);

            // Then
            result.Item2.ShouldEqual("The view conventions cannot be null.");
        }

        [Fact]
        public void Should_not_be_valid_when_view_location_conventions_is_empty()
        {
            // Given
            this.conventions.ViewLocationConventions = new List<Func<string, dynamic, ViewLocationContext, string>>();

            // When
            var result = this.viewLocationConventions.Validate(this.conventions);

            // Then
            result.Item1.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_correct_error_message_when_not_valid_because_view_location_conventions_is_empty()
        {
            // Given
            this.conventions.ViewLocationConventions = new List<Func<string, dynamic, ViewLocationContext, string>>();

            // When
            var result = this.viewLocationConventions.Validate(this.conventions);

            // Then
            result.Item2.ShouldEqual("The view conventions cannot be empty.");
        }

        [Fact]
        public void Should_be_valid_when_view_conventions_is_not_empty()
        {
            // Given
            this.conventions.ViewLocationConventions = 
                new List<Func<string, dynamic, ViewLocationContext, string>>
                {
                    (viewName, model, viewLocationContext) => {
                        return string.Empty;
                    }
                };

            // When
            var result = this.viewLocationConventions.Validate(this.conventions);

            // Then
            result.Item1.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_empty_error_message_when_valid()
        {
            // Given
            this.conventions.ViewLocationConventions =
                new List<Func<string, dynamic, ViewLocationContext, string>>
                {
                    (viewName, model, viewLocationContext) => {
                        return string.Empty;
                    }
                };

            // When
            var result = this.viewLocationConventions.Validate(this.conventions);

            // Then
            result.Item2.ShouldBeEmpty();
        }

        [Fact]
        public void Should_add_conventions_when_initialised()
        {
            // Given, When
            this.viewLocationConventions.Initialise(this.conventions);

            // Then
            this.conventions.ViewLocationConventions.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[0];

            // When
            var result = convention.Invoke(
                "viewname", 
                null, 
                new ViewLocationContext { ModulePath = string.Empty });

            // Then
            result.ShouldEqual("viewname");
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_views_folder()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[1];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = string.Empty });

            // Then
            result.ShouldEqual("views/viewname");
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulepath_subfolder_of_views_folder()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[2];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = "modulepath" });

            // Then
            result.ShouldEqual("views/modulepath/viewname");
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulepath_folder()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[3];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = "modulepath" });

            // Then
            result.ShouldEqual("modulepath/viewname");
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulename_subfolder_of_views_folder()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[4];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename" });

            // Then
            result.ShouldEqual("views/modulename/viewname");
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulename_folder()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[5];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename" });

            // Then
            result.ShouldEqual("modulename/viewname");
        }
    }
}