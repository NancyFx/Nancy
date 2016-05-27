namespace Nancy.ViewEngines.Razor.Tests
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FakeItEasy;
    using Microsoft.CSharp;
    using Nancy.Configuration;
    using Nancy.Tests;
    using Nancy.ViewEngines.Razor.Tests.Models;
    using Xunit;

    public class RazorViewEngineFixture
    {
        private readonly RazorViewEngine engine;
        private readonly IRenderContext renderContext;
        private readonly IRazorConfiguration configuration;
        private readonly FileSystemViewLocationProvider fileSystemViewLocationProvider;
        private readonly IRootPathProvider rootPathProvider;

        public RazorViewEngineFixture()
        {
            var assemblyCatalog = new AppDomainAssemblyCatalog();

            var environment = new DefaultNancyEnvironment();
            environment.Tracing(
                enabled: true,
                displayErrorTraces: true);

            this.configuration = A.Fake<IRazorConfiguration>();
            this.engine = new RazorViewEngine(this.configuration, environment, assemblyCatalog);
            A.CallTo(() => this.configuration.GetAssemblyNames()).Returns(new[] { "Nancy.ViewEngines.Razor.Tests.Models" });

            var cache = A.Fake<IViewCache>();
            A.CallTo(() => cache.GetOrAdd(A<ViewLocationResult>.Ignored, A<Func<ViewLocationResult, Func<INancyRazorView>>>.Ignored))
                .ReturnsLazily(x =>
                {
                    var result = x.GetArgument<ViewLocationResult>(0);
                    return x.GetArgument<Func<ViewLocationResult, Func<INancyRazorView>>>(1).Invoke(result);
                });

            this.renderContext = A.Fake<IRenderContext>();
            A.CallTo(() => this.renderContext.ViewCache).Returns(cache);
            A.CallTo(() => this.renderContext.LocateView(A<string>.Ignored, A<object>.Ignored))
                .ReturnsLazily(x =>
                {
                    var viewName = x.GetArgument<string>(0);
                    return FindView(viewName);
                });

            this.rootPathProvider = A.Fake<IRootPathProvider>();
            A.CallTo(() => this.rootPathProvider.GetRootPath()).Returns(Path.Combine(Environment.CurrentDirectory, "TestViews"));

            this.fileSystemViewLocationProvider = new FileSystemViewLocationProvider(this.rootPathProvider, new DefaultFileSystemReader());
        }

        [Fact]
        public void GetCompiledView_should_render_to_stream()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "cshtml",
                () => new StringReader(@"@{var x = ""test"";}<h1>Hello Mr. @x</h1>")
            );

            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. test</h1>");
        }

        [Fact]
        public void Should_be_able_to_render_view_with_partial_to_stream()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "cshtml",
                () => new StringReader(@"@{var x = ""test"";}<h1>Hello Mr. @x</h1> @Html.Partial(""partial.cshtml"")")
            );

            var partialLocation = new ViewLocationResult(
                string.Empty,
                "partial.cshtml",
                "cshtml",
                () => new StringReader(@"this is partial")
            );

            A.CallTo(() => this.renderContext.LocateView("partial.cshtml", null)).Returns(partialLocation);

            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. test</h1> this is partial");
        }

        [Fact]
        public void Should_support_files_with_the_razor_extensions()
        {
            // Given, When
            var extensions = this.engine.Extensions;

            // Then
            extensions.ShouldHaveCount(1);
            extensions.ShouldEqualSequence(new[] { "cshtml" });
        }

        [Fact]
        public void RenderView_should_accept_a_model_and_read_from_it_into_the_stream()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "cshtml",
                () => new StringReader(@"<h1>Hello Mr. @Model.Name</h1>")
            );

            var stream = new MemoryStream();

            dynamic model = new ExpandoObject();
            model.Name = "test";

            // When
            var response = this.engine.RenderView(location, model, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. test</h1>");
        }

        [Fact]
        public void RenderView_csharp_should_use_model_directive_for_strongly_typed_view()
        {
            // Given
            var location = FindView("ViewThatUsesModelCSharp");

            var stream = new MemoryStream();

            var model = new DateTime(2000, 1, 1);

            // When
            var response = this.engine.RenderView(location, model, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello at " + model.ToString("MM/dd/yyyy") + "</h1>", true);
        }

        [Fact]
        public void RenderView_csharp_should_be_able_to_use_a_model_from_another_assembly()
        {
            // Given
            var view = new StringBuilder()
                .AppendLine("@model Nancy.ViewEngines.Razor.Tests.Models.Person")
                .Append("<h1>Hello Mr. @Model.Name</h1>");

            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "cshtml",
                () => new StringReader(view.ToString())
            );

            var stream = new MemoryStream();

            var model = new Person { Name = "Jeff" };

            // When
            var response = this.engine.RenderView(location, model, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. Jeff</h1>");
        }

        [Fact]
        public void RenderView_csharp_should_be_able_to_use_a_using_statement()
        {
            // Given
            var view = new StringBuilder()
                .AppendLine("@model Nancy.ViewEngines.Razor.Tests.Models.Person")
                .AppendLine("@using Nancy.ViewEngines.Razor.Tests.Models")
                .AppendLine(@"@{ var hobby = new Hobby { Name = ""Music"" }; }")
                .Append("<h1>Mr. @Model.Name likes @hobby.Name!</h1>");

            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "cshtml",
                () => new StringReader(view.ToString())
            );

            var stream = new MemoryStream();

            var model = new Person { Name = "Jeff" };

            // When
            var response = this.engine.RenderView(location, model, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Mr. Jeff likes Music!</h1>", true);
        }

        [Fact]
        public void RenderView_csharp_should_be_able_to_find_the_model_when_a_null_model_is_passed()
        {
            var view = new StringBuilder()
                .AppendLine("@model Nancy.ViewEngines.Razor.Tests.Models.Person")
                .AppendLine(@"@{ var hobby = new Hobby { Name = ""Music"" }; }")
                .Append("<h1>Mr. Somebody likes @hobby.Name!</h1>");

            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "cshtml",
                () => new StringReader(view.ToString())
            );

            var stream = new MemoryStream();

            A.CallTo(() => this.configuration.AutoIncludeModelNamespace).Returns(true);

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Mr. Somebody likes Music!</h1>", true);
        }

        [Fact]
        public void RenderView_csharp_should_include_namespace_of_model_if_specified_in_the_configuration()
        {
            // Given
            var view = new StringBuilder()
                .AppendLine("@model Nancy.ViewEngines.Razor.Tests.Models.Person")
                .AppendLine(@"@{ var hobby = new Hobby { Name = ""Music"" }; }")
                .Append("<h1>Mr. @Model.Name likes @hobby.Name!</h1>");

            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "cshtml",
                () => new StringReader(view.ToString())
            );

            var stream = new MemoryStream();

            var model = new Person { Name = "Jeff" };

            A.CallTo(() => this.configuration.AutoIncludeModelNamespace).Returns(true);

            // When
            var response = this.engine.RenderView(location, model, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Mr. Jeff likes Music!</h1>", true);
        }

        [Fact]
        public void Should_be_able_to_render_view_with_layout_to_stream()
        {
            // Given
            var location = FindView("ViewThatUsesLayout");

            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContainInOrder("<h1>SimplyLayout</h1>", "<div>ViewThatUsesLayout</div>");
        }

        [Fact]
        public void Should_be_able_to_render_view_with_model_and_layout_to_stream()
        {
            // Given
            var location = FindView("ViewThatUsesLayoutAndModel");

            var stream = new MemoryStream();

            dynamic model = new ExpandoObject();
            model.Name = "my test";

            // When
            var response = this.engine.RenderView(location, model, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContainInOrder("<h1>SimplyLayout</h1>", "<div>ViewThatUsesLayoutAndModel: my test</div>");
        }

        [Fact]
        public void Should_be_able_to_render_view_with_layout_and_section_to_stream()
        {
            // Given
            var location = FindView("ViewThatUsesLayoutAndSection");

            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContainInOrder("<h1>SimplyLayout</h1>",
                                        "<div>First section in ViewThatUsesLayoutAndSection</div>",
                                        "<div>ViewThatUsesLayoutAndSection</div>");
        }


        [Fact]
        public void Should_be_able_to_render_view_with_layout_and_many_sections_to_stream()
        {
            // Given
            var location = FindView("ViewThatUsesLayoutAndManySection");

            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContainInOrder("<h1>SimplyLayout</h1>",
                                        "<div>First section in ViewThatUsesLayoutAndManySection</div>",
                                        "<div>Second section in ViewThatUsesLayoutAndManySection</div>",
                                        "<div>ViewThatUsesLayoutAndManySection</div>",
                                        "<div>Third section in ViewThatUsesLayoutAndManySection</div>");
        }

        [Fact]
        public void Should_be_able_to_render_view_with_layout_and_optional_section_to_stream()
        {
            // Given
            var location = FindView("ViewThatUsesLayoutAndOptionalSection");

            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContainInOrder("<h1>SimplyLayout</h1>",
                                        "<div>ViewThatUsesLayoutAndOptionalSection</div>");
        }

        [Fact]
        public void Should_be_able_to_render_view_with_layout_and_optional_section_with_default_to_stream()
        {
            // Given
            var location = FindView("ViewThatUsesLayoutAndOptionalSectionWithDefaults");
            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContainInOrder("<h1>SectionWithDefaultsLayout</h1>",
                                        "<div>OptionalSectionDefault</div>",
                                        "<div>ViewThatUsesLayoutAndOptionalSectionWithDefaults</div>");
        }

        [Fact]
        public void Should_be_able_to_render_view_with_layout_and_optional_section_overriding_the_default_to_stream()
        {
            // Given
            var location = FindView("ViewThatUsesLayoutAndOptionalSectionOverridingDefaults");
            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContainInOrder("<h1>SectionWithDefaultsLayout</h1>",
                                        "<div>OptionalSectionOverride</div>",
                                        "<div>ViewThatUsesLayoutAndOptionalSectionOverridingDefaults</div>");
        }

        [Fact]
        public void Should_be_able_to_render_view_with_helper_to_steam()
        {
            // Given
            var location = FindView("ViewThatUsesHelper");

            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContainInOrder("<h1>SimplyLayout</h1>", "<div>ViewThatUsesHelper</div>", "<p class=\"className\"></p>");
        }

        [Fact]
        public void Should_use_custom_view_base_with_csharp_views()
        {
            // Given
            var view = new StringBuilder()
                .AppendLine("@inherits Nancy.ViewEngines.Razor.Tests.GreetingViewBase")
                .Append("<h1>@Greet()</h1>");

            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "cshtml",
                () => new StringReader(view.ToString())
            );

            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream).Trim();
            output.ShouldEqual("<h1>Hi, Nancy!</h1>");
        }

        [Fact]
        public void Should_render_attributes_with_code_inside()
        {
            var location = FindView("ViewThatUsesAttributeWithCodeInside");
            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, new TestModel { Name = "Bob", Slug = "BobSlug" }, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContain("<a href=\"BobSlug\">Bob</a>");
        }

        [Fact]
        public void Should_render_compilation_source_on_compilation_error()
        {
            // Given
            var view = new StringBuilder()
                .AppendLine("@using Nancy.ViewEngines.Razor.Tests.GreetingViewBase")
                .Append("<h1>@Greet(</h1>");

            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "cshtml",
                () => new StringReader(view.ToString())
            );

            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContain("namespace RazorOutput {");
        }

        [Fact]
        public void Should_render_attributes_with_dynamic_null_inside()
        {
            // Given
            var location = FindView("ViewThatUsesAttributeWithDynamicNullInside");
            var stream = new MemoryStream();

            // When
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContain("<input value=\"\" />");
        }

        [Fact]
        public void Should_render_attributes_with_HtmlString_inside()
        {
            // Given
            var location = FindView("ViewThatUsesAttributeWithHtmlStringInside");
            var stream = new MemoryStream();
            const string PHRASE = "Slugs & bugs are secret spies on gardeners, but <strong>no one</strong> knows who they spy for";
            const string PHRASE_RESULT = "Slugs &amp; bugs are secret spies on gardeners, but &lt;strong&gt;no one&lt;/strong&gt; knows who they spy for";

            // When
            var response = this.engine.RenderView(location, PHRASE, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContain(string.Format("<input value=\"{0}\" />", PHRASE_RESULT));
        }

        [Fact]
        public void Should_render_attributes_with_RawHtmlString_inside()
        {
            // Given
            var location = FindView("ViewThatUsesAttributeWithRawHtmlStringInside");
            var stream = new MemoryStream();
            const string PHRASE = "Slugs & bugs are secret spies on gardeners, but <strong>no one</strong> knows who they spy for";

            // When
            var response = this.engine.RenderView(location, PHRASE, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContain(string.Format("<input value=\"{0}\" />", PHRASE));
        }

        [Fact]
        public void Should_render_attributes_with_NonEncodedHtmlString_inside()
        {
            // Given
            var location = FindView("ViewThatUsesAttributeWithNonEncodedHtmlStringInside");
            var stream = new MemoryStream();
            const string PHRASE = "Slugs are secret spies on gardeners, but no one knows who they spy for";

            // When
            var response = this.engine.RenderView(location, new NonEncodedHtmlString(PHRASE), this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var output = ReadAll(stream);
            output.ShouldContain(string.Format("<input value=\"{0}\" />", PHRASE));
        }

        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(sbyte))]
        [InlineData(typeof(short))]
        [InlineData(typeof(ushort))]
        [InlineData(typeof(int))]
        [InlineData(typeof(uint))]
        [InlineData(typeof(long))]
        [InlineData(typeof(ulong))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(char))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(object))]
        [InlineData(typeof(Tuple<string>))]
        [InlineData(typeof(Tuple<Tuple<string>>))]
        [InlineData(typeof(Tuple<string, int>))]
        [InlineData(typeof(Tuple<string, Tuple<int>>))]
        [InlineData(typeof(Tuple<string, Tuple<int>, int>))]
        [InlineData(typeof(Tuple<string, Tuple<int, string>, Tuple<int>, int>))]
        [InlineData(typeof(Tuple<string, Tuple<int, string>, Tuple<int>, int, Tuple<string, Tuple<int, int>>>))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(int[,]))]
        [InlineData(typeof(int[, , , ,]))]
        [InlineData(typeof(int[][]))]
        [InlineData(typeof(int[][, ,][]))]
        [InlineData(typeof(Tuple<int, string>[][, ,][]))]
        [InlineData(typeof(Tuple<int, int[][, ,][], string>))]
        public void Should_be_able_to_render_csharp_view_with_generic_model(Type expectedType)
        {
            // Given
            var location = CreateViewLocationWithModel(expectedType, new CSharpCodeProvider(), "model");

            // When
            var stream = new MemoryStream();
            var response = this.engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual(expectedType.FullName, true);
        }

        private static ViewLocationResult CreateViewLocationWithModel(Type expectedType, CodeDomProvider provider, string modelDirective)
        {
            var codeTypeRef = new CodeTypeReference(expectedType);
            var modelType = provider.GetTypeOutput(codeTypeRef);

            var modelTypeCode = BuildCodeExtractingModelType(provider);

            var view = string.Format("@{0} {1}\n\n@{2}", modelDirective, modelType, modelTypeCode);

            return new ViewLocationResult(
                string.Empty,
                string.Empty,
                string.Concat(provider.FileExtension, "html"),
                () => new StringReader(view)
                );
        }

        private static string BuildCodeExtractingModelType(CodeDomProvider provider)
        {
            var getType = new CodeMethodInvokeExpression
                (
                    new CodeThisReferenceExpression(),
                    "GetType"
                );

            var baseType = new CodeFieldReferenceExpression
                (
                    getType,
                    "BaseType"
                );

            var getFirstGenericArgument = new CodeArrayIndexerExpression
                (
                    new CodeMethodInvokeExpression
                        (
                        baseType,
                        "GetGenericArguments"
                        ),
                    new CodePrimitiveExpression(0)
                );

            var fullName = new CodeFieldReferenceExpression
                (
                    getFirstGenericArgument,
                    "FullName"
                );

            string modelTypeCode;
            using (var writer = new StringWriter())
            {
                provider.GenerateCodeFromExpression(fullName, writer, new CodeGeneratorOptions());
                modelTypeCode = writer.ToString();
            }
            return modelTypeCode;
        }

        private static string ReadAll(Stream stream)
        {
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private ViewLocationResult FindView(string viewName)
        {
            var location = this.fileSystemViewLocationProvider.GetLocatedViews(new[] { "cshtml", "vbhtml" }).FirstOrDefault(r => r.Name == viewName);
            return location;
        }
    }
}
