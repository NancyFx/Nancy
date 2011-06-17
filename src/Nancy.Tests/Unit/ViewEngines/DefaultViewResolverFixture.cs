namespace Nancy.Tests.Unit.ViewEngines
{
    using System;
    using System.IO;
    using System.Linq;
    using FakeItEasy;
    using Nancy.ViewEngines;
    using Xunit;

    public class DefaultViewResolverFixture
    {
        private readonly IViewLocator viewLocator;
        private readonly DefaultViewResolver viewResolver;

        public DefaultViewResolverFixture()
        {
            this.viewLocator = A.Fake<IViewLocator>();
            this.viewResolver = new DefaultViewResolver(this.viewLocator, Enumerable.Empty<Func<string, object, ViewLocationContext, string>>());
        }

        [Fact]
        public void Should_throw_invalidoperationexception_when_created_with_null_view_locator()
        {
            // Given, When
            var exception = Record.Exception(() => new DefaultViewResolver(null, Enumerable.Empty<Func<string, object, ViewLocationContext, string>>()));

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Should_throw_invalidoperationexception_when_created_with_null_conventions()
        {
            // Given, When
            var exception = Record.Exception(() => new DefaultViewResolver(A.Fake<IViewLocator>(), null));

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [Fact]
        public void Should_return_null_when_the_viewlocationcontext_is_null()
        {
            // Given
            ViewLocationContext context = null;

            // When
            var result = this.viewResolver.GetViewLocation("viewName", new object(), context);

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_when_viewname_is_null()
        {
            // Given
            string viewName = null;

            // When
            var result = this.viewResolver.GetViewLocation(viewName, new object(), null);

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_when_viewname_is_empty()
        {
            // Given
            var viewName = string.Empty;

            // When
            var result = this.viewResolver.GetViewLocation(viewName, new object(), null);

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_call_conventions_with_viewname()
        {
            // Given
            const string viewName = "foo.html";
            string viewNamePassedToFirstConvention = null;
            string viewNamePassedToSecondConvention = null;

            var resolver = new DefaultViewResolver(
                this.viewLocator,
                new Func<string, dynamic, ViewLocationContext, string>[] {
                    (name, model, path) => {
                        viewNamePassedToFirstConvention = viewName;
                        return string.Empty;
                    },
                    (name, model, path) => {
                        viewNamePassedToSecondConvention = viewName;
                        return string.Empty;
                    }
                });

            // When
            resolver.GetViewLocation(viewName, null, new ViewLocationContext());

            // Then
            viewNamePassedToFirstConvention.ShouldEqual(viewName);
            viewNamePassedToSecondConvention.ShouldEqual(viewName);
        }

        [Fact]
        public void Should_call_conventions_with_model()
        {
            // Given
            const string viewName = "foo.html";
            var viewModel = new object();

            object modelPassedToFirstConvention = null;
            object modelPassedToSecondConvention = null;

            var resolver = new DefaultViewResolver(
                this.viewLocator,
                new Func<string, dynamic, ViewLocationContext, string>[] {
                    (name, model, path) => {
                        modelPassedToFirstConvention = model;
                        return string.Empty;
                    },
                    (name, model, path) => {
                        modelPassedToSecondConvention = model;
                        return string.Empty;
                    }
                });

            // When
            resolver.GetViewLocation(viewName, viewModel, new ViewLocationContext());

            // Then
            modelPassedToFirstConvention.ShouldBeSameAs(viewModel);
            modelPassedToSecondConvention.ShouldBeSameAs(viewModel);
        }

        [Fact]
        public void Should_call_conventions_with_viewlocationcontext()
        {
            // Given
            const string viewName = "foo.html";
            var context = new ViewLocationContext();

            ViewLocationContext modulePathPassedToFirstConvention = null;
            ViewLocationContext modulePathPassedToSecondConvention = null;

            var resolver = new DefaultViewResolver(
                this.viewLocator,
                new Func<string, dynamic, ViewLocationContext, string>[] {
                    (name, model, viewLocationContext) => {
                        modulePathPassedToFirstConvention = viewLocationContext;
                        return string.Empty;
                    },
                    (name, model, viewLocationContext) => {
                        modulePathPassedToSecondConvention = viewLocationContext;
                        return string.Empty;
                    }
                });

            // When
            resolver.GetViewLocation(viewName, null, context);

            // Then
            modulePathPassedToFirstConvention.ShouldBeSameAs(context);
            modulePathPassedToSecondConvention.ShouldBeSameAs(context);
        }

        [Fact]
        public void Should_invoke_viewlocator_with_viewname_from_conventions()
        {
            // Given
            const string viewName = "foo.html";

            var resolver = new DefaultViewResolver(
                this.viewLocator,
                new Func<string, dynamic, ViewLocationContext, string>[] {
                    (name, model, path) =>  "bar.html" 
                });

            // When
            resolver.GetViewLocation(viewName, null, new ViewLocationContext());

            // Then
            A.CallTo(() => this.viewLocator.LocateView("bar.html")).MustHaveHappened();
        }

        [Fact]
        public void Should_not_invoke_viewlocator_when_conventions_returns_null_view_name()
        {
            // Given
            const string viewName = "foo.html";

            var resolver = new DefaultViewResolver(
                this.viewLocator,
                new Func<string, dynamic, ViewLocationContext, string>[] {
                    (name, model, path) =>  null 
                });

            // When
            resolver.GetViewLocation(viewName, null, new ViewLocationContext());

            // Then
            A.CallTo(() => this.viewLocator.LocateView(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void Should_not_invoke_viewlocator_when_conventions_returns_empty_view_name()
        {
            // Given
            const string viewName = "foo.html";

            var resolver = new DefaultViewResolver(
                this.viewLocator,
                new Func<string, dynamic, ViewLocationContext, string>[] {
                    (name, model, path) => string.Empty 
                });

            // When
            resolver.GetViewLocation(viewName, null, new ViewLocationContext());

            // Then
            A.CallTo(() => this.viewLocator.LocateView(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void Should_catch_exceptions_that_are_thrown_by_conventions()
        {
            // Given
            const string viewName = "foo.html";

            var resolver = new DefaultViewResolver(
                this.viewLocator,
                new Func<string, dynamic, ViewLocationContext, string>[] {
                    (name, model, path) => { throw new Exception(); }
                });

            // When
            var exception = Record.Exception(() => resolver.GetViewLocation(viewName, null, new ViewLocationContext()));

            // Then
            exception.ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_when_no_view_could_be_located()
        {
            // Given
            const string viewName = "foo.html";

            var resolver = new DefaultViewResolver(
                this.viewLocator,
                new Func<string, dynamic, ViewLocationContext, string>[] {
                    (name, model, path) => "bar.html"
                });

            A.CallTo(() => this.viewLocator.LocateView(A<string>.Ignored)).Returns(null);

            // When
            var result = resolver.GetViewLocation(viewName, null, new ViewLocationContext());

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void return_viewlocationresult_when_view_could_be_located()
        {
            // Given
            const string viewName = "foo.html";

            var resolver = new DefaultViewResolver(
                this.viewLocator,
                new Func<string, dynamic, ViewLocationContext, string>[] {
                    (name, model, path) => "bar.html"
                });

            var locatedView =
                new ViewLocationResult("name", "location", "extension", GetEmptyContentReader());

            A.CallTo(() => this.viewLocator.LocateView(A<string>.Ignored)).Returns(locatedView);

            // When
            var result = resolver.GetViewLocation(viewName, null, new ViewLocationContext());

            // Then
            result.ShouldBeSameAs(locatedView);
        }

        private static Func<TextReader> GetEmptyContentReader()
        {
            return () => new StreamReader(new MemoryStream());
        }
    }
}