namespace Nancy.ViewEngines.Spark.Tests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using FakeItEasy;
    using global::Spark;
    using Nancy.Tests;
    using Nancy.ViewEngines.Spark.Tests.ViewModels;
    using Xunit;
    using SparkViewEngine = Spark.SparkViewEngine;

    public class SparkViewEngineFixture
    {
        private readonly IRenderContext renderContext;
        private string output;
        private readonly FileSystemViewLocationProvider fileSystemViewLocationProvider;
        private readonly IRootPathProvider rootPathProvider;

        public SparkViewEngineFixture()
        {
            this.rootPathProvider = A.Fake<IRootPathProvider>();
            A.CallTo(() => this.rootPathProvider.GetRootPath()).Returns(Path.Combine(Environment.CurrentDirectory, "TestViews"));
						
            this.fileSystemViewLocationProvider = new FileSystemViewLocationProvider(this.rootPathProvider, new DefaultFileSystemReader());
            
            this.renderContext = A.Fake<IRenderContext>();

            var cache = A.Fake<IViewCache>();
            A.CallTo(() => cache.GetOrAdd(A<ViewLocationResult>.Ignored, A<Func<ViewLocationResult, ISparkViewEntry>>.Ignored))
                .ReturnsLazily(x => {
                    var result = x.GetArgument<ViewLocationResult>(0);
                    return x.GetArgument<Func<ViewLocationResult, ISparkViewEntry>>(1).Invoke(result);
                });

            A.CallTo(() => this.renderContext.ViewCache).Returns(cache);
        }

        [Fact]
        public void Application_dot_spark_should_be_used_as_the_master_layout_if_present()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesApplicationLayout");

            //Then
            this.output.ShouldContainInOrder(
                "<title>Child View That Expects Application Layout by default</title>",
                "<div>main application header by default</div>",
                "<h1>Child View That Expects Application Layout by default</h1>",
                "<div>main application footer by default</div>");
        }

        [Fact]
        public void Should_be_able_to_html_encode_using_H_function_from_views()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesHtmlEncoding");

            //Then
            this.output.Replace(" ", "").Replace("\r", "").Replace("\n", "")
                .ShouldEqual("<div>&lt;div&gt;&amp;lt;&amp;gt;&lt;/div&gt;</div>");
        }

        [Fact]
        public void Should_be_able_to_html_encode_null_using_H_function_from_views()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesNullHtmlEncoding");

            //Then
            this.output.ShouldEqual("<div></div>");
        }

        [Fact] 
        public void Should_be_able_to_provide_global_setting_for_views()
        {
            //Given, When
            this.FindViewAndRender("ViewThatChangesGlobalSettings");

            //Then
            this.output.ShouldContainInOrder(
                "<div>default: Global set test</div>",
                "<div>7==7</div>");
        }

        [Fact] 
        public void Should_be_able_to_render_a_child_view_with_a_master_layout()
        {
            //Given, When
            this.FindViewAndRender("ViewThatExpectsALayout");

            //Then
            this.output.ShouldContainInOrder(
                "<title>Child View That Expects A Layout</title>",
                "<div>no header by default</div>",
                "<h1>Child View That Expects A Layout</h1>",
                "<div>no footer by default</div>");
        }

        [Fact]
        public void Should_be_able_to_render_a_plain_view()
        {
            //Given, When
            this.FindViewAndRender("Index");

            //Then
            this.output.ShouldEqual("<div>index</div>");
        }

        [Fact]
        public void Should_be_able_to_render_a_subfolder_view()
        {
            var viewLocationResult = new ViewLocationResult("Stub\\Subfolder", "Subfolderview", "spark", GetEmptyContentReader());
            this.FindViewAndRender<dynamic>("subfolder\\Subfolderview", null, viewLocationResult);

            //Then
            this.output.ShouldEqual("<div>Subfolder</div>");
        }

        [Fact]
        public void Should_be_able_to_render_a_view_even_with_null_view_model()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesANullViewModel");

            //Then
            this.output.ShouldContain("<div>nothing</div>");
        }

        [Fact]
        public void Should_be_able_to_render_a_view_with_a_strongly_typed_model()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesViewModel", new FakeViewModel {Text = "Spark"});

            //Then
            this.output.ShouldContain("<div>Spark</div>");
        }

        [Fact(Skip = "Only add this if we think we'll need to render anonymous types")]
        public void Should_be_able_to_render_a_view_with_an_anonymous_view_model()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesAnonymousViewModel", new {Foo = 42, Bar = new FakeViewModel {Text = "is the answer"}});

            //Then
            this.output.ShouldContain("<div>42 is the answer</div>");
        }

        [Fact(Skip = "Only add this if we think we'll need to render anonymous types")]
        public void Should_be_able_to_render_a_view_with_culture_aware_formatting()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("en-us")))
            {
                //Given, When
                this.FindViewAndRender("ViewThatUsesFormatting", new {Number = 9876543.21, Date = new DateTime(2010, 12, 11)});

                //Then
                this.output.ShouldContainInOrder(
                    "<div>9,876,543.21</div>",
                    "<div>2010/12/11</div>");
            }
        }

        [Fact]
        public void Should_be_able_to_render_partials_that_share_state()
        {
            //Given

            // When
            this.FindViewAndRender("ViewThatRendersPartialsThatShareState");

            //Then
            this.output.ShouldContainInOrder(
                "<div>start</div>",
                "<div>lion</div>",
                "<div>elephant</div>",
                "<div>The Target</div>",
                "<div>Willow</div>",
                "<div>middle</div>",
                "<ul>",
                "<li>one</li>",
                "<li>three</li>",
                "<li>two</li>",
                "</ul>",
                "alphabetagammadelta",
                "<div>end</div>");
            this.output.ShouldNotContain("foo2");
            this.output.ShouldNotContain("bar4");
            this.output.ShouldNotContain("quux7");
        }

        [Fact]
        public void Should_be_able_to_use_a_partial_file_explicitly()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesPartial");

            //Then
            this.output.ShouldContainInOrder(
                "<ul>",
                "<li>Partial where x=\"lion\"</li>",
                "<li>Partial where x=\"hippo\"</li>",
                "<li>Partial where x=\"elephant\"</li>",
                "<li>Partial where x=\"giraffe\"</li>",
                "<li>Partial where x=\"whale\"</li>",
                "</ul>");
        }

        [Fact]
        public void Should_be_able_to_use_a_partial_file_implicitly()
        {
            //Given, Then
            this.FindViewAndRender("ViewThatUsesPartialImplicitly");

            //Then
            this.output.ShouldContainInOrder(
                "<li class=\"odd\">lion</li>",
                "<li class=\"even\">hippo</li>");
        }

        [Fact]
        public void Should_be_able_to_use_foreach_construct_in_the_view()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesForeach");

            //Then
            this.output.ShouldContainInOrder(
                "<li class=\"odd\">1: foo</li>",
                "<li class=\"even\">2: bar</li>",
                "<li class=\"odd\">3: baz</li>");
        }

        [Fact]
        public void Should_be_able_to_use_namespaces_directly()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesNamespaces");

            //Then
            this.output.ShouldContainInOrder(
                "<div>Foo</div>",
                "<div>Bar</div>",
                "<div>Hello</div>");
        }

        [Fact]
        public void Should_capture_named_content_areas_and_render_in_the_correct_order()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesAllNamedContentAreas");

            //Then
            this.output.ShouldContainInOrder(
                "<div>Funny, we can put the header anywhere we like with a name</div>",
                "<div>OK - this is the main content by default because it is not contained</div>",
                "<div>Here is some footer stuff defined at the top</div>",
                "<div>Much better place for footer stuff - or is it?</div>");
        }

        [Fact]
        public void Should_substitute_tilde_in_resource_url_with_parse_result_from_Render_Context()
        {
            //Given
            A.CallTo(() => this.renderContext.ParsePath(A<string>.Ignored))
                .Returns("/mysensationalrootfolder/scripts/test.js");
            
            //When
            this.FindViewAndRender("ViewThatUsesTildeSubstitution");

            //Then
            this.output.ShouldContain(@"<script type=""text/javascript"" src=""/mysensationalrootfolder/scripts/test.js""/>");
        }

        private void FindViewAndRender<T>(string viewName, T viewModel, ViewLocationResult viewLocationResult = null) where T : class
        {
            if (viewLocationResult == null)
            {
                viewLocationResult = new ViewLocationResult("Stub", viewName, "spark", GetEmptyContentReader());
            }

            var stream = new MemoryStream();
            var engine = new SparkViewEngine();

            var context = new ViewEngineStartupContext(
                A.Fake<IViewCache>(),
                this.fileSystemViewLocationProvider.GetLocatedViews(new[] {"spark"}),
                new[] {"spark"});

            engine.Initialize(context);

            //When
            var response = engine.RenderView(viewLocationResult, viewModel, this.renderContext);
            response.Contents.Invoke(stream);
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                this.output = reader.ReadToEnd();
            }
        }

        private void FindViewAndRender(string viewName)
        {
            this.FindViewAndRender<dynamic>(viewName, null);
        }

        private static Func<TextReader> GetEmptyContentReader()
        {
            return () => new StreamReader(new MemoryStream());
        }

        public class ScopedCulture : IDisposable
        {
            private readonly CultureInfo savedCulture;

            public ScopedCulture(CultureInfo culture)
            {
                this.savedCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = culture;
            }

            public void Dispose()
            {
                Thread.CurrentThread.CurrentCulture = this.savedCulture;
            }
        }
    }
}