namespace Nancy.ViewEngines.Markdown.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using FakeItEasy;
    using Nancy.Tests;
    using SuperSimpleViewEngine;
    using Xunit;

    public class MarkdownViewEngineFixture
    {
        private readonly MarkDownViewEngine viewEngine;
        private readonly MarkdownViewEngineHost viewEngineHostHost;
        private readonly NancyViewEngineHost nancyViewEngineHost;
        private readonly IRenderContext renderContext;
        private IRootPathProvider rootPathProvider;
        private FileSystemViewLocationProvider fileSystemViewLocationProvider;


        public MarkdownViewEngineFixture()
        {
            this.renderContext = A.Fake<IRenderContext>();
            this.viewEngineHostHost = new MarkdownViewEngineHost(new NancyViewEngineHost(this.renderContext), this.renderContext);
            this.viewEngine = new MarkDownViewEngine(new SuperSimpleViewEngine());

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
                                > This is a blockquote.
                                ";

            const string htmlResult = @"<h1>Header1</h1>
                              <h2>Header2</h2>
                              <h3>Header3</h3>
                              <p>Hi there!</p>
                              <blockquote>This is a blockquote.</blockquote>";

            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "md",
                () => new StringReader(markdown)
            );

            var stream = new MemoryStream();

            A.CallTo(() => this.renderContext.ViewCache.GetOrAdd(location, A<Func<ViewLocationResult, string>>.Ignored))
             .Returns(htmlResult);

            // When
            var response = this.viewEngine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual(htmlResult);
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
            var result = SetupCallAndReadViewWithMasterPage(useModel:true);

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

        private string SetupCallAndReadViewWithMasterPage(bool useModel = false)
        {
            var location = FindView("home");

            var masterLocation = FindView("master");

            var partialLocation = FindView("partial");

            A.CallTo(() => this.renderContext.ViewCache.GetOrAdd(location, A<Func<ViewLocationResult, string>>.Ignored))
             .Returns(location.Contents().ReadToEnd());

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
            var location = this.fileSystemViewLocationProvider.GetLocatedViews(new[] { "md", "markdown" }).First(r => r.Name == viewName);
            return location;
        }
    }
}
