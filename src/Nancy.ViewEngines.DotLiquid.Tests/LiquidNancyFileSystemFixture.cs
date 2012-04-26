using FakeItEasy;

namespace Nancy.ViewEngines.DotLiquid.Tests
{
    using System.IO;
    using Nancy.Tests;
    using Nancy.ViewEngines.DotLiquid;
    using Xunit;
    using Xunit.Extensions;
    using global::DotLiquid.Exceptions;

    public class LiquidNancyFileSystemFixture
    {
        [Fact]
        public void Should_locate_template_with_single_quoted_name()
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(null, "'views/partial'");

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_locate_template_with_double_quoted_name()
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(null, @"""views/partial""");

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_locate_template_with_unquoted_name()
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(null, "views/partial");

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_not_locate_templates_that_does_not_have_liquid_extension()
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult("views", "partial", "cshtml", () => null));

            // When
            var exception = Record.Exception(() => fileSystem.ReadTemplateFile(null, "views/partial"));

            // Then
            exception.ShouldBeOfType<FileSystemException>();
        }

        [Theory]
        [InlineData("partial")]
        [InlineData("paRTial")]
        [InlineData("PARTIAL")]
        public void Should_ignore_casing_of_template_name(string templateName)
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(null, string.Concat("views/", templateName));

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_support_locating_template_when_template_name_contains_liquid_extension()
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(null, "views/partial.liquid");

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_support_locating_template_when_template_name_does_not_contains_liquid_extension()
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(null, "views/partial");

            // Then
            result.ShouldEqual("The correct view");
        }

        [Theory]
        [InlineData("liquid")]
        [InlineData("liqUID")]
        [InlineData("LIQUID")]
        public void Should_ignore_extension_casing_when_template_name_contains_liquid_extension(string extension)
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(null, string.Concat("views/partial.", extension));

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_not_locate_view_when_template_location_not_specified()
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult("views", "partial", "liquid", () => null));

            // When
            var exception = Record.Exception(() =>fileSystem.ReadTemplateFile(null, "partial"));

            // Then
            exception.ShouldBeOfType<FileSystemException>();
        }

        [Theory]
        [InlineData("views", "Located in views/")]
        [InlineData("views/shared", "Located in views/shared")]
        public void Should_locate_templates_with_correct_location_specified(string location, string expectedResult)
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult(location, "partial", "liquid", () => new StringReader(expectedResult)));

            // When
            var result = fileSystem.ReadTemplateFile(null, string.Concat(location, "/partial"));

            // Then
            result.ShouldEqual(expectedResult);
        }

        [Theory]
        [InlineData("views")]
        [InlineData("viEws")]
        [InlineData("VIEWS")]
        public void Should_ignore_case_of_location_when_locating_template(string location)
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult(location, "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(null, string.Concat(location, "/partial"));

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_support_backslashes_as_location_seperator()
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult(@"views/shared", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(null, @"views\shared\partial");

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_support_forward_slashes_as_location_seperator()
        {
            // Given
            var fileSystem = CreateFileSystem(
                new ViewLocationResult(@"views/shared", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(null, @"views/shared/partial");

            // Then
            result.ShouldEqual("The correct view");
        }

        private static LiquidNancyFileSystem CreateFileSystem(params ViewLocationResult[] viewLocationResults)
        {
            var factory = A.Fake<IFileSystemFactory>();
            var engine = new DotLiquidViewEngine(factory);
            var context = new ViewEngineStartupContext(
                null,
                viewLocationResults,
                new[] { "liquid" });

            return new LiquidNancyFileSystem(context, engine);
        }
    }
}