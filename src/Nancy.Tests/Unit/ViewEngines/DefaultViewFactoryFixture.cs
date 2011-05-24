namespace Nancy.Tests.Unit.ViewEngines
{
    using System;
    using System.IO;
    using FakeItEasy;
    using Nancy.ViewEngines;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class DefaultViewFactoryFixture
    {
        private readonly IViewResolver resolver;
        private readonly IRenderContextFactory renderContextFactory;

        public DefaultViewFactoryFixture()
        {
            this.resolver = A.Fake<IViewResolver>();
            this.renderContextFactory = A.Fake<IRenderContextFactory>();
        }

        private DefaultViewFactory CreateFactory(params IViewEngine[] viewEngines)
        {
            if (viewEngines == null)
            {
                viewEngines = new IViewEngine[] { };
            }

            return new DefaultViewFactory(this.resolver, viewEngines, this.renderContextFactory);
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
            var exception = Record.Exception(() => factory.RenderView(string.Empty, null, new ViewLocationContext()));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_rendering_view_and_both_viewname_and_model_is_null()
        {
            // Given
            var factory = this.CreateFactory(null);

            // When
            var exception = Record.Exception(() => factory.RenderView(null, null, new ViewLocationContext()));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_retrieve_view_from_view_locator_using_provided_view_name()
        {
            // Given
            var factory = this.CreateFactory();

            // When
            factory.RenderView("viewname.html", null, new ViewLocationContext());

            // Then)
            A.CallTo(() => this.resolver.GetViewLocation("viewname.html", A<object>.Ignored, A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_retrieve_view_from_view_locator_using_provided_model()
        {
            // Given
            var factory = this.CreateFactory();
            var model = new object();

            // When
            factory.RenderView(null, model, new ViewLocationContext());

            // Then)
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, model, A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_retrieve_view_from_view_locator_using_provided_module_path()
        {
            // Given
            var factory = this.CreateFactory();
            var model = new object();
            var viewLocationContext = new ViewLocationContext { ModulePath = "/bar" };

            // When
            factory.RenderView(null, model, viewLocationContext);

            // Then)
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.That.Matches(x => x.ModulePath.Equals("/bar")))).MustHaveHappened();
        }

        [Fact]
        public void Should_return_empty_action_when_view_could_not_be_located()
        {
            var factory = this.CreateFactory();

            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(null);

            var action = factory.RenderView("foo", null, new ViewLocationContext());
            var stream = new MemoryStream();

            // When
            action.Invoke(stream);

            // Then
            stream.Length.ShouldEqual(0L);
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
            factory.RenderView("foo", null, new ViewLocationContext());

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
            factory.RenderView("foo", null, new ViewLocationContext());

            // Then
            A.CallTo(() => viewEngines[0].RenderView(location, null, A<IRenderContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_empty_action_when_no_view_engine_could_be_resolved()
        {
            // Given
            var location = new ViewLocationResult("location", "name", "html", GetEmptyContentReader());
            A.CallTo(() => this.resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(location);

            var stream = new MemoryStream();
            var factory = this.CreateFactory();

            // When
            var action = factory.RenderView("foo", null, new ViewLocationContext());
            action.Invoke(stream);

            // Then
            stream.Length.ShouldEqual(0L);
        }

        [Fact]
        public void Should_return_action_from_invoked_engine()
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
            var action = factory.RenderView("foo", null, new ViewLocationContext());

            // Then
            action.ShouldEqual(actionReturnedFromEngine);
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
            var action = factory.RenderView("foo", null, new ViewLocationContext());
            action.Invoke(stream);

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
            var action = factory.RenderView("foo", model, new ViewLocationContext());

            // Then
            A.CallTo(() => viewEngines[0].RenderView(A<ViewLocationResult>.Ignored, model, A<IRenderContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_use_the_name_of_the_model_type_as_view_name_when_only_model_is_specified()
        {
            // Given
            var factory = this.CreateFactory();

            // When
            var action = factory.RenderView(null, new object(), new ViewLocationContext());

            // Then
            A.CallTo(() => this.resolver.GetViewLocation("Object", A<object>.Ignored, A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_use_the_name_of_the_model_type_without_model_suffix_as_view_name_when_only_model_is_specified()
        {
            // Given
            var factory = this.CreateFactory();

            // When
            var action = factory.RenderView(null, new ViewModel(), new ViewLocationContext());

            // Then
            A.CallTo(() => this.resolver.GetViewLocation("View", A<object>.Ignored, A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }

        private static Func<TextReader> GetEmptyContentReader()
        {
            return () => new StreamReader(new MemoryStream());
        }
    }
}