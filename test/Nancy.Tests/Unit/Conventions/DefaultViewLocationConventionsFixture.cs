namespace Nancy.Tests.Unit.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Nancy.Conventions;
    using Nancy.ViewEngines;

    using Xunit;
    using Xunit.Extensions;

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
            var convention = this.conventions.ViewLocationConventions[15];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = string.Empty });

            // Then
            result.ShouldEqual("viewname");
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_define_convention_that_returns_viewname_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[14];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = string.Empty, Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Concat("viewname-", culture));
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_views_folder()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[13];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = string.Empty });

            // Then
            result.ShouldEqual("views/viewname");
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_define_convention_that_returns_viewname_in_views_folder_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[12];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = string.Empty, Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Concat("views/viewname-", culture));
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulepath_subfolder_of_views_folder()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[5];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = "modulepath" });

            // Then
            result.ShouldEqual("views/modulepath/viewname");
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_define_convention_that_returns_viewname_in_modulepath_subfolder_of_views_folder_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[4];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = "modulepath", Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Concat("views/modulepath/viewname-", culture));
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulepath_subfolder_of_views_folder_when_modulepath_contains_leading_slash()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[5];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = "/modulepath" });

            // Then
            result.ShouldEqual("views/modulepath/viewname");
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_define_convention_that_returns_viewname_in_modulepath_subfolder_of_views_folder_when_modulepath_contains_leading_slash_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[4];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = "/modulepath", Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Concat("views/modulepath/viewname-", culture));
        }

        [Fact]
        public void Should_return_empty_result_convention_that_returns_viewname_in_modulepath_subfolder_of_views_folder_when_modulepath_is_empty()
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
            result.ShouldEqual(string.Empty);
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_return_empty_result_convention_that_returns_viewname_in_modulepath_subfolder_of_views_folder_when_modulepath_is_empty_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[0];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = string.Empty, Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Empty);
        }

        [Fact]
        public void Should_return_empty_result_convention_that_returns_viewname_in_modulepath_subfolder_of_views_folder_when_modulepath_is_null()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[1];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = null });

            // Then
            result.ShouldEqual(string.Empty);
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_return_empty_result_convention_that_returns_viewname_in_modulepath_subfolder_of_views_folder_when_modulepath_is_null_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[0];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = null, Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Empty);
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulepath_folder()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[7];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = "modulepath" });

            // Then
            result.ShouldEqual("modulepath/viewname");
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_define_convention_that_returns_viewname_in_modulepath_folder_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[6];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = "modulepath", Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Concat("modulepath/viewname-", culture));
        }

        [Fact]
        public void Should_return_empty_result_for_convention_that_returns_viewname_in_modulepath_folder_when_modulepath_is_empty()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[5];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = string.Empty });

            // Then
            result.ShouldEqual(string.Empty);
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_return_empty_result_for_convention_that_returns_viewname_in_modulepath_folder_when_modulepath_is_empty_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[4];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = string.Empty, Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Empty);
        }

        [Fact]
        public void Should_return_empty_result_for_convention_that_returns_viewname_in_modulepath_folder_when_modulepath_is_null()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[5];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = null });

            // Then
            result.ShouldEqual(string.Empty);
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_return_empty_result_for_convention_that_returns_viewname_in_modulepath_folder_when_modulepath_is_null_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[4];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = null, Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Empty);
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulepath_folder_when_modulepath_contains_leading_slash()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[7];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = "/modulepath" });

            // Then
            result.ShouldEqual("modulepath/viewname");
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_define_convention_that_returns_viewname_in_modulepath_folder_when_modulepath_contains_leading_slash_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[6];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModulePath = "/modulepath", Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Concat("modulepath/viewname-", culture));
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulename_subfolder_of_views_folder()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[9];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename" });

            // Then
            result.ShouldEqual("views/modulename/viewname");
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_define_convention_that_returns_viewname_in_modulename_subfolder_of_views_folder_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[8];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename", Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Concat("views/modulename/viewname-", culture));
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulename_folder()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[11];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename" });

            // Then
            result.ShouldEqual("modulename/viewname");
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_define_convention_that_returns_viewname_in_modulename_folder_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[10];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename", Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Concat("modulename/viewname-", culture));
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulename_folder_in_modulepath_folder()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[3];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename", ModulePath = "modulepath" });

            // Then
            result.ShouldEqual("modulepath/modulename/viewname");
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_define_convention_that_returns_viewname_in_modulename_folder_in_modulepath_folder_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[2];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename", ModulePath = "modulepath", Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Concat("modulepath/modulename/viewname-", culture));
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulename_folder_in_modulepath_folder_in_views_folder()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[1];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename", ModulePath = "modulepath" });

            // Then
            result.ShouldEqual("views/modulepath/modulename/viewname");
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_define_convention_that_returns_viewname_in_modulename_folder_in_modulepath_folder_in_views_folder_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[0];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename", ModulePath = "modulepath", Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Concat("views/modulepath/modulename/viewname-", culture));
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulename_folder_in_modulepath_folder_when_modulepath_contains_leading_slash()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[3];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename", ModulePath = "/modulepath" });

            // Then
            result.ShouldEqual("modulepath/modulename/viewname");
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_define_convention_that_returns_viewname_in_modulename_folder_in_modulepath_folder_when_modulepath_contains_leading_slash_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[2];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename", ModulePath = "/modulepath", Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Concat("modulepath/modulename/viewname-", culture));
        }

        [Fact]
        public void Should_define_convention_that_returns_viewname_in_modulename_folder_in_modulepath_folder_in_views_folder_when_modulepath_contains_leading_slash()
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[1];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename", ModulePath = "/modulepath" });

            // Then
            result.ShouldEqual("views/modulepath/modulename/viewname");
        }

        [Theory]
        [InlineData("en-GB")]
        [InlineData("de-DE")]
        public void Should_define_convention_that_returns_viewname_in_modulename_folder_in_modulepath_folder_in_views_folder_when_modulepath_contains_leading_slash_with_culture(string culture)
        {
            // Given
            this.viewLocationConventions.Initialise(this.conventions);
            var convention = this.conventions.ViewLocationConventions[0];

            // When
            var result = convention.Invoke(
                "viewname",
                null,
                new ViewLocationContext { ModuleName = "modulename", ModulePath = "/modulepath", Context = new NancyContext() { Culture = new CultureInfo(culture) } });

            // Then
            result.ShouldEqual(string.Concat("views/modulepath/modulename/viewname-", culture));
        }
    }
}