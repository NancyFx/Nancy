namespace Nancy.ViewEngines.DotLiquid.Tests
{
    using System.Collections.Generic;
    using System.IO;

    using FakeItEasy;

    using global::DotLiquid;
    using global::DotLiquid.Exceptions;

    using Nancy.Tests;

    using Xunit;
    using Xunit.Extensions;

    public class LiquidNancyFileSystemFixture
    {
        [Fact]
        public void Should_locate_template_with_single_quoted_name()
        {
            // Given
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(context, "'views/partial'");

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_locate_template_with_double_quoted_name()
        {
            // Given
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(context, @"""views/partial""");

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_locate_template_with_unquoted_name()
        {
            // Given
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(context, "views/partial");

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_not_locate_templates_that_does_not_have_liquid_extension()
        {
            // Given
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult("views", "partial", "cshtml", () => null));

            // When
            var exception = Record.Exception(() => fileSystem.ReadTemplateFile(context, "views/partial"));

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
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(context, string.Concat("views/", templateName));

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_support_locating_template_when_template_name_contains_liquid_extension()
        {
            // Given
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(context, "views/partial.liquid");

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_support_locating_template_when_template_name_does_not_contains_liquid_extension()
        {
            // Given
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(context, "views/partial");

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
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult("views", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(context, string.Concat("views/partial.", extension));

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_not_locate_view_when_template_location_not_specified()
        {
            // Given
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult("views", "partial", "liquid", () => null));

            // When
            var exception = Record.Exception(() => fileSystem.ReadTemplateFile(context, "partial"));

            // Then
            exception.ShouldBeOfType<FileSystemException>();
        }

        [Theory]
        [InlineData("views", "Located in views/")]
        [InlineData("views/shared", "Located in views/shared")]
        public void Should_locate_templates_with_correct_location_specified(string location, string expectedResult)
        {
            // Given
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult(location, "partial", "liquid", () => new StringReader(expectedResult)));

            // When
            var result = fileSystem.ReadTemplateFile(context, string.Concat(location, "/partial"));

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
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult(location, "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(context, string.Concat(location, "/partial"));

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_support_backslashes_as_location_seperator()
        {
            // Given
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult(@"views/shared", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(context, @"views\shared\partial");

            // Then
            result.ShouldEqual("The correct view");
        }

        [Fact]
        public void Should_support_forward_slashes_as_location_seperator()
        {
            // Given
            Context context;
            var fileSystem = CreateFileSystem(out context,
                new ViewLocationResult(@"views/shared", "partial", "liquid", () => new StringReader("The correct view")));

            // When
            var result = fileSystem.ReadTemplateFile(context, @"views/shared/partial");

            // Then
            result.ShouldEqual("The correct view");
        }

        private LiquidNancyFileSystem CreateFileSystem(out Context context, params ViewLocationResult[] viewLocationResults)
        {
            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>._))
                                               .Returns(viewLocationResults);

            var viewEngine = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine.Extensions).Returns(new[] { "liquid" });

            var viewLocator = new DefaultViewLocator(viewLocationProvider, new[] { viewEngine });

            var startupContext = new ViewEngineStartupContext(
                null,
                viewLocator);
            
            var renderContext = A.Fake<IRenderContext>();
            A.CallTo(() => renderContext.LocateView(A<string>.Ignored, A<object>.Ignored))
                .ReturnsLazily(x => viewLocator.LocateView(x.Arguments.Get<string>(0), null));

            context = new Context(new List<Hash>(), new Hash(),
                Hash.FromAnonymousObject(new { nancy = renderContext }), false);

            return new LiquidNancyFileSystem(startupContext, new[] { "liquid" });
        }
    }
}