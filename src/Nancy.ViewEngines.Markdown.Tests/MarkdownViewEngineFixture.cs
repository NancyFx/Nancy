namespace Nancy.ViewEngines.Markdown.Tests
{
    using System;
    using System.IO;
    using System.Linq;

    using FakeItEasy;

    using Nancy.Tests;
    using Nancy.ViewEngines.SuperSimpleViewEngine;

    using Xunit;

    public class MarkdownViewEngineFixture
    {
        private readonly MarkDownViewEngine viewEngine;
        private readonly IRenderContext renderContext;
        private readonly IRootPathProvider rootPathProvider;
        private readonly FileSystemViewLocationProvider fileSystemViewLocationProvider;

        public MarkdownViewEngineFixture()
        {
            this.renderContext = A.Fake<IRenderContext>();
            this.viewEngine = new MarkDownViewEngine(new SuperSimpleViewEngine(Enumerable.Empty<ISuperSimpleViewEngineMatcher>()));

            this.rootPathProvider = A.Fake<IRootPathProvider>();

            A.CallTo(() => this.rootPathProvider.GetRootPath()).Returns(Path.Combine(Environment.CurrentDirectory, "Markdown"));

            this.fileSystemViewLocationProvider = new FileSystemViewLocationProvider(this.rootPathProvider, new DefaultFileSystemReader());
        }

        [Fact]
        public void Should_return_Markdown()
        {
            // Given
            const string markdown = @"#Header1
##Header2
###Header3
Hi there!
> This is a blockquote.";

            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "md",
                () => new StringReader(markdown)
            );

            var html = this.viewEngine.ConvertMarkdown(location);

            var stream = new MemoryStream();

            A.CallTo(() => this.renderContext.ViewCache.GetOrAdd(location, A<Func<ViewLocationResult, string>>.Ignored))
             .Returns(html);

            // When
            var response = this.viewEngine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            var result = ReadAll(stream);
            result.ShouldEqual(html);
        }

        [Fact]
        public void Should_use_masterpage()
        {
            //Given, When
            var result = SetupCallAndReadViewWithMasterPage();

            //Then
            result.ShouldContain("What does Samuel L Jackson think?");
        }

        [Fact]
        public void Should_render_model()
        {
            //Given, When
            var result = SetupCallAndReadViewWithMasterPage(useModel: true);

            //Then
            result.ShouldContain("My name is Vincent Vega and I come from the model");
        }

        [Fact]
        public void Should_handle_script_tags_before_body_tag()
        {
            //Given, When
            const string expected = @"<script type='text/javascript' src='http://code.jquery.com/jquery-latest.min.js'></script>";

            var result = SetupCallAndReadViewWithMasterPage();

            //Then
            result.ShouldContain(expected);
        }

        [Fact]
        public void Should_convert_markdown_in_master()
        {
            //Given, When
            var result = SetupCallAndReadViewWithMasterPage();

            //Then
            result.ShouldContain("<h1>Markdown Engine Demo</h1>");
        }

        [Fact]
        public void Should_convert_partial_views_with_markdown_content()
        {
            //Given, When
            var result = SetupCallAndReadViewWithMasterPage();

            //Then
            result.ShouldContain("<h4>This is from a partial</h4>");
        }

        [Fact]
        public void Should_convert_standalone()
        {
            var location = FindView("standalone");

            var result = this.viewEngine.ConvertMarkdown(location);

            Assert.True(result.StartsWith("<!DOCTYPE html>"));
        }

        [Fact]
        public void Should_convert_view()
        {
            var location = FindView("home");
            var result = this.viewEngine.ConvertMarkdown(location);

            result.ShouldStartWith("@Master['master']");
        }

        [Fact]
        public void Should_convert_standalone_view_with_no_master()
        {
            var location = FindView("standalone");

            var html = this.viewEngine.ConvertMarkdown(location);

            A.CallTo(() => this.renderContext.ViewCache.GetOrAdd(location, A<Func<ViewLocationResult, string>>.Ignored))
             .Returns(html);

            var stream = new MemoryStream();

            var response = this.viewEngine.RenderView(location, null, this.renderContext);

            response.Contents.Invoke(stream);

            var result = ReadAll(stream);

            Assert.True(result.StartsWith("<!DOCTYPE html>"));
        }

        [Fact]
        public void Should_be_able_to_use_HTML_MasterPage()
        {
            var location = FindView("viewwithhtmlmaster");

            var masterLocation = FindView("htmlmaster");

            var html = this.viewEngine.ConvertMarkdown(location);

            A.CallTo(() => this.renderContext.ViewCache.GetOrAdd(location, A<Func<ViewLocationResult, string>>.Ignored))
             .Returns(html);

            A.CallTo(() => this.renderContext.LocateView("htmlmaster", A<object>.Ignored)).Returns(masterLocation);

            var stream = new MemoryStream();


            var response = this.viewEngine.RenderView(location, null, this.renderContext);

            response.Contents.Invoke(stream);

            var result = ReadAll(stream);

            result.ShouldStartWith("<!DOCTYPE html>");
            result.ShouldContain("<p>Bacon ipsum dolor");
        }

        private string SetupCallAndReadViewWithMasterPage(bool useModel = false)
        {
            var location = FindView("home");

            var masterLocation = FindView("master");

            var partialLocation = FindView("partial");

            var html = this.viewEngine.ConvertMarkdown(location);

            A.CallTo(() => this.renderContext.ViewCache.GetOrAdd(location, A<Func<ViewLocationResult, string>>.Ignored))
             .Returns(html);

            A.CallTo(() => this.renderContext.LocateView("master", A<object>.Ignored)).Returns(masterLocation);

            A.CallTo(() => this.renderContext.LocateView("partial", A<object>.Ignored)).Returns(partialLocation);

            var stream = new MemoryStream();

            var model = useModel ? new UserModel("Vincent", "Vega") : null;

            var response = this.viewEngine.RenderView(location, model, this.renderContext);

            response.Contents.Invoke(stream);

            return ReadAll(stream);
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
            var location = this.fileSystemViewLocationProvider.GetLocatedViews(new[] { "md", "markdown", "html" }).First(r => r.Name == viewName);
            return location;
        }
    }
}
