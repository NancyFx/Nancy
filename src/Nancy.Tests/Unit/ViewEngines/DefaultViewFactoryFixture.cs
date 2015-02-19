namespace Nancy.Tests.Unit.ViewEngines
{
    using System;
    using System.Dynamic;
    using System.IO;
    using System.Linq;

    using FakeItEasy;

    using Nancy.Conventions;
    using Nancy.Diagnostics;
    using Nancy.Tests.Fakes;
    using Nancy.ViewEngines;

    using Xunit;

    public class DefaultViewFactoryFixture
    {
        private readonly IViewResolver resolver;
        private readonly IRenderContextFactory renderContextFactory;
        private readonly ViewLocationContext viewLocationContext;
        private readonly ViewLocationConventions conventions;
        private readonly IRootPathProvider rootPathProvider;

        public DefaultViewFactoryFixture()
        {
            this.rootPathProvider = A.Fake<IRootPathProvider>();
            A.CallTo(() => this.rootPathProvider.GetRootPath()).Returns("The root path");

            this.resolver = A.Fake<IViewResolver>();
            this.renderContextFactory = A.Fake<IRenderContextFactory>();
            this.conventions = new ViewLocationConventions(Enumerable.Empty<Func<string, object, ViewLocationContext, string>>());

            this.viewLocationContext =
                new ViewLocationContext
                {
                    Context = new NancyContext
                    {
                        Trace = new DefaultRequestTrace
                        {
                            TraceLog = new DefaultTraceLog()
                        }
                    }
                };
        }

        private DefaultViewFactory CreateFactory(params IViewEngine[] viewEngines)
        {
            if (viewEngines == null)
            {
                viewEngines = new IViewEngine[] { };
            }

            return new DefaultViewFactory(this.resolver, viewEngines, this.renderContextFactory, this.conventions, this.rootPathProvider);
        }

        [Fact]
        public void Should_get_render_context_from_factory_when_rendering_view()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });

            var location = new ViewLocationResult("location", "name", "html", GetEmptyContentReader());
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(location);

            var factory = this.CreateFactory(viewEngines);

            // When
            factory.RenderView("view.html", new object(), this.viewLocationContext);

            // Then
            A.CallTo(() => this.renderContextFactory.GetRenderContext(A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_render_view_with_context_created_by_factory()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });

            var location = new ViewLocationResult("location", "name", "html", GetEmptyContentReader());
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(location);

            var context = A.Fake<IRenderContext>();
            A.CallTo(() => this.renderContextFactory.GetRenderContext(A<ViewLocationContext>.Ignored)).Returns(context);

            var factory = this.CreateFactory(viewEngines);

            // When
            factory.RenderView("view.html", new object(), this.viewLocationContext);

            // Then
            A.CallTo(() => viewEngines[0].RenderView(A<ViewLocationResult>.Ignored, A<object>.Ignored, context)).MustHaveHappened();
        }

        [Fact]
        public void Should_not_build_render_context_more_than_once()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });

            var location = new ViewLocationResult("location", "name", "html", GetEmptyContentReader());
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(location);

            var factory = this.CreateFactory(viewEngines);

            // When
            factory.RenderView("view.html", new object(), this.viewLocationContext);

            // Then
            A.CallTo(() => this.renderContextFactory.GetRenderContext(A<ViewLocationContext>.Ignored)).MustHaveHappened(Repeated.NoMoreThan.Once);
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_rendering_view_and_viewlocationcontext_is_null()
        {
            // Given
            var factory = this.CreateFactory(null);

            // When
            var exception = Record.Exception(() => factory.RenderView("viewName", new object(), null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_rendering_view_and_view_name_is_empty_and_model_is_null()
        {
            // Given
            var factory = this.CreateFactory(null);

            // When
            var exception = Record.Exception(() => factory.RenderView(string.Empty, null, this.viewLocationContext));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_rendering_view_and_both_viewname_and_model_is_null()
        {
            // Given
            var factory = this.CreateFactory(null);

            // When
            var exception = Record.Exception(() => factory.RenderView(null, null, this.viewLocationContext));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_retrieve_view_from_view_locator_using_provided_view_name()
        {
            // Given
            var factory = this.CreateFactory();

            // When
            Record.Exception(() => factory.RenderView("viewname.html", null, this.viewLocationContext));

            // Then
            A.CallTo(() => this.resolver.GetViewLocation("viewname.html", A<object>.Ignored, A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_retrieve_view_from_view_locator_using_provided_model()
        {
            // Given
            var factory = this.CreateFactory();
            var model = new object();

            // When
            Record.Exception(() => factory.RenderView(null, model, this.viewLocationContext));

            // Then
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, model, A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_retrieve_view_from_view_locator_using_provided_module_path()
        {
            // Given
            var factory = this.CreateFactory();
            var model = new object();

            var viewContext = 
                new ViewLocationContext
                {
                    Context = new NancyContext
                    {
                        Trace = new DefaultRequestTrace
                        {
                            TraceLog = new DefaultTraceLog()
                        }
                    },
                    ModulePath = "/bar"
                };

            // When
            Record.Exception(() => factory.RenderView(null, model, viewContext));

            // Then
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.That.Matches(x => x.ModulePath.Equals("/bar")))).MustHaveHappened();
        }

        [Fact]
        public void Should_call_first_view_engine_that_supports_extension_with_view_location_results()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
              A.Fake<IViewEngine>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => viewEngines[1].Extensions).Returns(new[] { "html" });

            var location = new ViewLocationResult("location", "name", "html", GetEmptyContentReader());
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(location);

            var factory = this.CreateFactory(viewEngines);

            // When
            factory.RenderView("foo", null, this.viewLocationContext);

            // Then
            A.CallTo(() => viewEngines[0].RenderView(location, null, A<IRenderContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_ignore_case_when_locating_view_engine_for_view_name_extension()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "HTML" });

            var location = new ViewLocationResult("location", "name", "html", GetEmptyContentReader());
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(location);

            var factory = this.CreateFactory(viewEngines);

            // When
            factory.RenderView("foo", null, this.viewLocationContext);

            // Then
            A.CallTo(() => viewEngines[0].RenderView(location, null, A<IRenderContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_response_from_invoked_engine()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
            };

            Action<Stream> actionReturnedFromEngine = x => { };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => viewEngines[0].RenderView(A<ViewLocationResult>.Ignored, null, A<IRenderContext>.Ignored)).Returns(actionReturnedFromEngine);

            var location = new ViewLocationResult("location", "name", "html", GetEmptyContentReader());
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(location);

            var factory = this.CreateFactory(viewEngines);

            // When
            var response = factory.RenderView("foo", null, this.viewLocationContext);

            // Then
            response.Contents.ShouldBeSameAs(actionReturnedFromEngine);
        }

        [Fact]
        public void Should_return_empty_action_when_view_engine_throws_exception()
        {
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => viewEngines[0].RenderView(A<ViewLocationResult>.Ignored, null, A<IRenderContext>.Ignored)).Throws(new Exception());

            var location = new ViewLocationResult("location", "name", "html", GetEmptyContentReader());
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(location);

            var stream = new MemoryStream();
            var factory = this.CreateFactory(viewEngines);

            // When
            var response = factory.RenderView("foo", null, this.viewLocationContext);
            response.Contents.Invoke(stream);

            // Then
            stream.Length.ShouldEqual(0L);
        }

        [Fact]
        public void Should_invoke_view_engine_with_model()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => viewEngines[0].RenderView(A<ViewLocationResult>.Ignored, null, null)).Throws(new Exception());

            var location = new ViewLocationResult("location", "name", "html", GetEmptyContentReader());
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(location);

            var model = new object();
            var factory = this.CreateFactory(viewEngines);

            // When
            factory.RenderView("foo", model, this.viewLocationContext);

            // Then
            A.CallTo(() => viewEngines[0].RenderView(A<ViewLocationResult>.Ignored, model, A<IRenderContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_covert_anonymoustype_model_to_expandoobject_before_invoking_view_engine()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });

            var location = new ViewLocationResult("location", "name", "html", GetEmptyContentReader());
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(location);

            var model = new { Name = "" };
            var factory = this.CreateFactory(viewEngines);

            // When
            factory.RenderView("foo", model, this.viewLocationContext);

            // Then
            A.CallTo(() => viewEngines[0].RenderView(A<ViewLocationResult>.Ignored, A<object>.That.Matches(x => x.GetType().Equals(typeof(ExpandoObject))), A<IRenderContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_transfer_anonymoustype_model_members_to_expandoobject_members_before_invoking_view_engines()
        {
            // Given
            var viewEngines = new[] {
              new FakeViewEngine { Extensions = new[] { "html"}}
            };

            var location = new ViewLocationResult("location", "name", "html", GetEmptyContentReader());
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(location);

            var model = new { Name = "Nancy" };
            var factory = this.CreateFactory(viewEngines);

            // When
            factory.RenderView("foo", model, this.viewLocationContext);

            // Then
            ((string)viewEngines[0].Model.Name).ShouldEqual("Nancy");
        }

        [Fact]
        public void Should_use_the_name_of_the_model_type_as_view_name_when_only_model_is_specified()
        {
            // Given
            var factory = this.CreateFactory();

            // When
            Record.Exception(() => factory.RenderView(null, new object(), this.viewLocationContext));

            // Then
            A.CallTo(() => this.resolver.GetViewLocation("Object", A<object>.Ignored, A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_use_the_name_of_the_model_type_without_model_suffix_as_view_name_when_only_model_is_specified()
        {
            // Given
            var factory = this.CreateFactory();

            // When
            Record.Exception(() => factory.RenderView(null, new ViewModel(), this.viewLocationContext));

            // Then
            A.CallTo(() => this.resolver.GetViewLocation("View", A<object>.Ignored, A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_throw_when_view_could_not_be_located()
        {
            var factory = this.CreateFactory();

            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(null);

            var result = Record.Exception(() => factory.RenderView("foo", null, this.viewLocationContext));

            result.ShouldBeOfType<ViewNotFoundException>();
        }

        [Fact]
        public void Should_provide_view_name_and_available_extensions_in_not_found_exception()
        {
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
              A.Fake<IViewEngine>(),
            };
            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => viewEngines[1].Extensions).Returns(new[] { "sshtml" });
            var factory = this.CreateFactory(viewEngines);
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(null);

            var result = Record.Exception(() => factory.RenderView("foo", null, this.viewLocationContext)) as ViewNotFoundException;

            result.AvailableViewEngineExtensions.ShouldEqualSequence(new[] { "html", "sshtml" });
            result.ViewName.ShouldEqual("foo");
        }

        [Fact]
        public void Should_provide_list_of_inspected_view_locations_in_not_found_exception()
        {
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
              A.Fake<IViewEngine>(),
            };
            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => viewEngines[1].Extensions).Returns(new[] { "sshtml" });            
             
            var conventions = new Func<string, dynamic, ViewLocationContext, string>[] {(a,b,c) => "baz"};
            var factory = new DefaultViewFactory(this.resolver, viewEngines, this.renderContextFactory, new ViewLocationConventions(conventions), this.rootPathProvider);
            
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(null);

            var result = (Record.Exception(() => factory.RenderView("foo", null, this.viewLocationContext))) as ViewNotFoundException;

            result.AvailableViewEngineExtensions.ShouldEqualSequence(new[] { "html", "sshtml" });
            result.ViewName.ShouldEqual("foo");
            result.InspectedLocations.ShouldEqualSequence(new [] {"baz"});
        }


        private static Func<TextReader> GetEmptyContentReader()
        {
            return () => new StreamReader(new MemoryStream());
        }
    }
}