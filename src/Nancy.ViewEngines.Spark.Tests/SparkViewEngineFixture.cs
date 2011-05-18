namespace Nancy.ViewEngines.Spark.Tests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Web;
    using FakeItEasy;
    using Nancy.Tests;
    using Nancy.ViewEngines.Spark.Tests.ViewModels;
    using Spark;
    using Xunit;
    using global::Spark;
    using global::Spark.FileSystem;
    using SparkViewEngine = Spark.SparkViewEngine;

    public class SparkViewEngineFixture
    {
        private readonly ActionContext actionContext;
        private readonly SparkViewEngine engine;
        private readonly HttpContextBase httpContext;
        private readonly TextWriter output;
        private readonly HttpResponseBase response;
        private readonly IRootPathProvider rootPathProvider;

        public SparkViewEngineFixture()
        {
            var settings = new SparkSettings();
            this.rootPathProvider = A.Fake<IRootPathProvider>();
            this.actionContext = new ActionContext(httpContext, "Stub");
            this.engine = new SparkViewEngine(this.rootPathProvider) {ViewFolder = new FileSystemViewFolder("TestViews")};
            this.httpContext = MockHttpContextBase.Generate("/", new StringWriter());
            this.response = httpContext.Response;
            this.output = response.Output;
        }

        [Fact]
        public void Application_dot_spark_should_be_used_as_the_master_layout_if_present()
        {
            //Given
            engine.ViewFolder = new InMemoryViewFolder { {"Stub\\baz.spark", ""}, {"Shared\\Application.spark", ""} };

            //When
            var descriptor = engine.CreateDescriptor(actionContext, "baz", null, true, null);

            //Then
            descriptor.Templates.ShouldHaveCount(2);
            descriptor.Templates[0].ShouldEqual("Stub\\baz.spark");
            descriptor.Templates[1].ShouldEqual("Shared\\Application.spark");
        }

        [Fact]
        public void Should_be_able_to_change_view_source_folder_on_the_fly()
        {
            //Given
            var replacement = A.Fake<IViewFolder>();

            //When
            var existing = engine.ViewFolder;

            //Then
            existing.ShouldNotBeSameAs(replacement);
            existing.ShouldBeSameAs(engine.ViewFolder);

            //When
            engine.ViewFolder = replacement;

            //Then
            replacement.ShouldBeSameAs(engine.ViewFolder);
            existing.ShouldNotBeSameAs(engine.ViewFolder);
        }

        [Fact]
        public void Should_be_able_to_get_the_target_namespace_from_the_action_context()
        {
            //Given
            engine.ViewFolder = new InMemoryViewFolder { {"Stub\\Foo.spark", ""}, {"Layouts\\Home.spark", ""} };

            //When
            var descriptor = engine.CreateDescriptor(actionContext, "Foo", null, true, null);

            //Then
            Assert.Equal("Stub", descriptor.TargetNamespace);
        }

        [Fact]
        public void Should_be_able_to_html_encode_using_H_function_from_views()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesHtmlEncoding");

            //Then
            output.ToString().Replace(" ", "").Replace("\r", "").Replace("\n", "")
                .ShouldEqual("<div>&lt;div&gt;&amp;lt;&amp;gt;&lt;/div&gt;</div>");
        }

        [Fact]
        public void Should_be_able_to_provide_global_setting_for_views()
        {
            //Given, When
            this.FindViewAndRender("ViewThatChangesGlobalSettings");

            //Then
            output.ToString().ShouldContainInOrder(
                "<div>default: Global set test</div>",
                "<div>7==7</div>");
        }

        [Fact]
        public void Should_be_able_to_render_a_child_view_with_a_master_layout()
        {
            //Given, When
            this.FindViewAndRender("ChildViewThatExpectsALayout", "Layout");

            //Then
            output.ToString().ShouldContainInOrder(
                "<title>Child View That Expects A Layout</title>",
                "<div>no header by default</div>",
                "<h1>Child View That Expects A Layout</h1>",
                "<div>no footer by default</div>");
        }

        [Fact]
        public void Should_be_able_to_render_a_plain_view()
        {
            //Given
            var viewEngineResult = engine.FindView(actionContext, "index", null);

            //When
            viewEngineResult.View.RenderView(output);

            //Then
            output.ToString().ShouldEqual("<div>index</div>");
        }

        [Fact]
        public void Should_be_able_to_render_a_view_even_with_null_view_model()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesANullViewModel");

            //Then
            output.ToString().ShouldContain("<div>nothing</div>");
        }

        [Fact]
        public void Should_be_able_to_render_a_view_with_a_strongly_typed_model()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesViewModel", new FakeViewModel { Text = "Spark" });

            //Then
            output.ToString().ShouldContain("<div>Spark</div>");
        }

        [Fact(Skip = "Only add this if we think we'll need to render anonymous types")]
        public void Should_be_able_to_render_a_view_with_an_anonymous_view_model()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesAnonymousViewModel", new {Foo = 42, Bar = new FakeViewModel {Text = "is the answer"}});

            //Then
            output.ToString().ShouldContain("<div>42 is the answer</div>");
        }

        [Fact(Skip = "Only add this if we think we'll need to render anonymous types")]
        public void Should_be_able_to_render_a_view_with_culture_aware_formatting()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("en-us")))
            {
                //Given, When
                this.FindViewAndRender("ViewThatUsesFormatting", new { Number = 9876543.21, Date = new DateTime(2010, 12, 11) });

                //Then
                output.ToString().ShouldContainInOrder(
                    "<div>9,876,543.21</div>",
                    "<div>2010/12/11</div>");
            }
        }

        [Fact]
        public void Should_be_able_to_render_partials_that_share_state()
        {
            //Given, When
            this.FindViewAndRender("ViewThatRendersPartialsThatShareState");

            //Then
            output.ToString().ShouldContainInOrder(
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
            output.ToString().ShouldNotContain("foo2");
            output.ToString().ShouldNotContain("bar4");
            output.ToString().ShouldNotContain("quux7");
        }

        [Fact]
        public void Should_be_able_to_use_a_partial_file_explicitly()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesPartial");

            //Then
            output.ToString().ShouldContainInOrder(
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
            output.ToString().ShouldContainInOrder(
                "<li class=\"odd\">lion</li>",
                "<li class=\"even\">hippo</li>");
        }

        [Fact]
        public void Should_be_able_to_use_foreach_construct_in_the_view()
        {
            //Given, When
            this.FindViewAndRender("ViewThatUsesForeach");

            //Then
            output.ToString().ShouldContainInOrder(
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
            output.ToString().ShouldContainInOrder(
                "<div>Foo</div>",
                "<div>Bar</div>",
                "<div>Hello</div>");
        }

        [Fact]
        public void Should_capture_named_content_areas_and_render_in_the_correct_order()
        {
            //Given, When
            this.FindViewAndRender("ChildViewThatUsesAllNamedContentAreas", "Layout");

            //Then
            output.ToString().ShouldContainInOrder(
                "<div>Funny, we can put the header anywhere we like with a name</div>",
                "<div>OK - this is the main content by default because it is not contained</div>",
                "<div>Here is some footer stuff defined at the top</div>",
                "<div>Much better place for footer stuff - or is it?</div>");
        }

        [Fact]
        public void The_master_layout_should_be_empty_by_default()
        {
            //Given
            engine.ViewFolder = new InMemoryViewFolder { {"Stub\\baz.spark", ""} };

            //When
            var descriptor = engine.CreateDescriptor(actionContext, "baz", null, true, null);

            //Then
            descriptor.Templates.ShouldHaveCount(1);
            descriptor.Templates[0].ShouldEqual("Stub\\baz.spark");
        }

        private void FindViewAndRender<T>(string viewName, T viewModel) where T : class
        {
            var result = engine.FindView(actionContext, viewName, null);
            var viewWithModel = result.View as NancySparkView<T>;

            if (viewWithModel != null)
            {
                viewWithModel.SetModel(viewModel);
            }

            result.View.RenderView(output);
        }

        private void FindViewAndRender(string viewName, string masterName = null)
        {
            var result = engine.FindView(actionContext, viewName, masterName);
            result.View.RenderView(output);
        }

        #region Nested type: MockHttpContextBase

        public class MockHttpContextBase
        {
            public static HttpContextBase Generate(string path)
            {
                return Generate(path, new StringWriter(), new MemoryStream());
            }

            public static HttpContextBase Generate(string path, TextWriter output)
            {
                return Generate(path, output, new MemoryStream());
            }

            public static HttpContextBase Generate(string path, Stream outputStream)
            {
                return Generate(path, new StringWriter(), outputStream);
            }

            public static HttpContextBase Generate(string path, TextWriter output, Stream outputStream)
            {
                var contextBase = A.Fake<HttpContextBase>();
                var requestBase = A.Fake<HttpRequestBase>();
                var responseBase = A.Fake<HttpResponseBase>();
                var sessionStateBase = A.Fake<HttpSessionStateBase>();
                var serverUtilityBase = A.Fake<HttpServerUtilityBase>();

                A.CallTo(() => contextBase.Request).Returns(requestBase);
                A.CallTo(() => contextBase.Response).Returns(responseBase);
                A.CallTo(() => contextBase.Session).Returns(sessionStateBase);
                A.CallTo(() => contextBase.Server).Returns(serverUtilityBase);

                responseBase.Output = output;
                A.CallTo(() => responseBase.OutputStream).Returns(outputStream);


                A.CallTo(() => requestBase.ApplicationPath).Returns("/");
                A.CallTo(() => requestBase.Path).Returns(path);

                return contextBase;
            }
        }

        #endregion

        #region Nested type: ScopedCulture

        public class ScopedCulture : IDisposable
        {
            private readonly CultureInfo savedCulture;

            public ScopedCulture(CultureInfo culture)
            {
                savedCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = culture;
            }

            #region IDisposable Members

            public void Dispose()
            {
                Thread.CurrentThread.CurrentCulture = savedCulture;
            }

            #endregion
        }

        #endregion
    }
}