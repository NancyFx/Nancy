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

    public class SparkViewEngineFixture
    {
        private readonly IRenderContext renderContext;
        private string output;
        private FileSystemViewLocationProvider fileSystemViewLocationProvider;
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
            A.CallTo(() => this.renderContext.ParsePath("~/"))
                .Returns("/mysensationalrootfolder/");

            //When
            this.FindViewAndRender("ViewThatUsesTildeSubstitution");

            //Then
            this.output.ShouldContain(@"<script type=""text/javascript"" src=""/mysensationalrootfolder/scripts/test.js""/>");
        }

#if !__MonoCS__
        [Fact]
        public void Should_allow_overriding_of_tilde_substitiution_with_resource_paths_from_config()
        {
            //Given
            A.CallTo(() => this.renderContext.ParsePath("~/"))
                .Returns("/mysensationalrootfolder/");

            //When
            this.FindViewAndRender("ViewThatUsesTildeSubstitutionWithSparkReplace");

            //Then
            this.output.ShouldContain(@"<script type=""text/javascript"" src=""http://cdn.example.com/mysite/scripts/test.js""/>");
        }
#endif

        [Fact]
        public void Should_support_files_with_the_spark_extensions()
        {
            // Given
            var engine = new global::Nancy.ViewEngines.Spark.SparkViewEngine();

            //When
            var extensions = engine.Extensions;

            // Then
            extensions.ShouldHaveCount(2);
            extensions.ShouldEqualSequence(new[] { "spark", "shade" });
        }

        [Fact]
        public void Should_use_application_dot_shade_as_the_master_layout_if_present()
        {
            //Given,
            const string viewName = "ShadeThatUsesApplicationLayout";
            var viewLocationResult = GetShadeViewLocation(viewName);

            //When
            this.FindViewAndRender(viewName, viewLocationResult);

            //Then
            this.output.ShouldContainInOrder(
                "<title>Child View That Expects Application Layout by default</title>",
                "<div>main application header by default</div>",
                "<h1>Child View That Expects Application Layout by default</h1>",
                "<div>Hello Spark</div>",
                "<h3>15</h3>",
                "<div>main application footer by default</div>");
        }

        [Fact]
        public void Should_render_a_shade_file()
        {
            //Given
            const string viewName = "ShadeFileRenders";
            var viewLocationResult = GetShadeViewLocation(viewName);

            //When
            this.FindViewAndRender(viewName, viewLocationResult);

            //Then
            this.output.ShouldContainInOrder(
                "<html>",
                "<head>",
                "<title>",
                "offset test",
                "</title>",
                "<body>",
                "<div class=\"container\">",
                "<h1 id=\"top\">",
                "offset test",
                "</h1>",
                "</div>",
                "</body>",
                "</html>");
        }

        [Fact]
        public void Should_evaluate_expressions_in_shade()
        {
            //Given
            const string viewName = "ShadeEvaluatesExpressions";
            var viewLocationResult = GetShadeViewLocation(viewName);

            //When
            this.FindViewAndRender(viewName, viewLocationResult);

            //Then
            this.output.ShouldContainInOrder(
                "<p>",
                "<span>",
                "8",
                "</span>",
                "<span>",
                "2", " and ", "7",
                "</span>",
                "</p>");
        }

        [Fact]
        public void Should_allow_dash_or_braced_code_recognition_in_shade()
        {
            //Given
            const string viewName = "ShadeCodeMayBeDashOrAtBraced";
            var viewLocationResult = GetShadeViewLocation(viewName);

            //When
            this.FindViewAndRender(viewName, viewLocationResult);

            //Then
            this.output.ShouldContainInOrder(
                "<ul>",
                "<li>emocleW</li>",
                "<li>ot</li>",
                "<li>eht</li>",
                "<li>enihcaM</li>",
                "</ul>");
        }

        [Fact]
        public void Should_allow_text_to_contain_expressions_in_shade()
        {
            //Given
            const string viewName = "ShadeTextMayContainExpressions";
            var viewLocationResult = GetShadeViewLocation(viewName);

            //When
            this.FindViewAndRender(viewName, viewLocationResult);

            //Then
            this.output.ShouldContainInOrder(
                "<p>",
                "<span>8</span>",
                "<span>2 and 7</span>",
                "</p>");
        }

        [Fact]
        public void Should_support_attributes_and_treat_some_as_special_nodes_like_partials_in_shade()
        {
            //Given
            const string viewName = "ShadeSupportsAttributesAndMayTreatSomeElementsAsSpecialNodes";
            var viewLocationResult = GetShadeViewLocation(viewName);

            //When
            this.FindViewAndRender(viewName, viewLocationResult);

            //Then
            this.output.ShouldContainInOrder(
                "<ul class=\"nav\">",
                "<li>Welcome</li>",
                "<li>to</li>",
                "<li>the</li>",
                "<li>Machine</li>",
                "</ul>",
                "<p>",
                "<span>4</span>",
                "</p>");
        }

        [Fact]
        public void Should_allow_elements_to_stack_on_one_line_in_shade()
        {
            //Given
            const string viewName = "ShadeElementsMayStackOnOneLine";
            var viewLocationResult = GetShadeViewLocation(viewName);

            //When
            this.FindViewAndRender(viewName, viewLocationResult);

            //Then
            this.output.ShouldContainInOrder(
                "<html>",
                "<head>",
                "<title>",
                "offset test",
                "</title>",
                "<body>",
                "<div class=\"container\">",
                "<h1 id=\"top\">",
                "offset test",
                "</h1>",
                "</div>",
                "</body>",
                "</html>");
        }

        [Fact]
        public void Should_allow_using_viewdata_tag_for_retrieving_values_from_ViewBag()
        {
            // Given
            var nancyContext = new NancyContext();
            nancyContext.ViewBag["foo"] = "bar";
            A.CallTo(() => renderContext.Context).Returns(nancyContext);

            // When
            FindViewAndRender("ViewThatUsesViewDataForViewBag");

            // Then
            this.output.ShouldContain("<div>bar</div>");
        }

        private ViewLocationResult GetShadeViewLocation(string viewName)
        {
            A.CallTo(() => this.rootPathProvider.GetRootPath()).Returns(Path.Combine(Environment.CurrentDirectory, "ShadeViews"));
            this.fileSystemViewLocationProvider = new FileSystemViewLocationProvider(this.rootPathProvider, new DefaultFileSystemReader());
            var viewLocationResult = new ViewLocationResult("Features", viewName, "shade", GetEmptyContentReader());
            return viewLocationResult;
        }

        private void FindViewAndRender<T>(string viewName, T viewModel, ViewLocationResult viewLocationResult = null) where T : class
        {
            if (viewLocationResult == null)
            {
                viewLocationResult = new ViewLocationResult("Stub", viewName, "spark", GetEmptyContentReader());
            }

            var stream = new MemoryStream();
            var engine = new global::Nancy.ViewEngines.Spark.SparkViewEngine();

            var locator = new DefaultViewLocator(this.fileSystemViewLocationProvider, new[] { engine });
            
            var context = new ViewEngineStartupContext(
                                    A.Fake<IViewCache>(),
                                    locator);

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

        private void FindViewAndRender(string viewName, ViewLocationResult viewLocationResult)
        {
            this.FindViewAndRender<dynamic>(viewName, null, viewLocationResult);
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