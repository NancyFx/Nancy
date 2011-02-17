namespace Nancy.Tests.Unit.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Nancy;
    using FakeItEasy;
    using Nancy.ViewEngines;
    using Xunit;

    public class DefaultViewFactoryFixture
    {
        private readonly IViewLocator locator;

        public DefaultViewFactoryFixture()
        {
            this.locator = A.Fake<IViewLocator>();
        }

        private DefaultViewFactory CreateFactory(params IViewEngine[] viewEngines)
        {
            if (viewEngines == null)
            {
                viewEngines = new IViewEngine[] { };
            }

            return new DefaultViewFactory(this.locator, viewEngines);
        }

        [Fact]
        public void Should_ignore_case_when_getting_distinct_list_of_supported_view_engine_extensions()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
              A.Fake<IViewEngine>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => viewEngines[1].Extensions).Returns(new[] { "HTML" });

            var expectedViewEngineExtensions = new[] { "html" };
            var factory = this.CreateFactory(viewEngines);

            // When
            var action = factory["foo"];

            // Then
            A.CallTo(() => this.locator.GetViewLocation("foo",
                A<IEnumerable<string>>.That.IsSameSequenceAs(expectedViewEngineExtensions).Argument)).MustHaveHappened();
        }

        [Fact]
        public void Should_retrieve_view_from_locator_using_distinct_list_of_supported_view_engine_extensions_when_view_name_has_no_extension()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
              A.Fake<IViewEngine>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => viewEngines[1].Extensions).Returns(new[] { "spark" });

            var expectedViewEngineExtensions = new[] {"html", "spark"};
            var factory = this.CreateFactory(viewEngines);

            // When
            var action = factory["foo"];

            // Then
            A.CallTo(() => this.locator.GetViewLocation("foo", 
                A<IEnumerable<string>>.That.Matches(x => expectedViewEngineExtensions.All(y => x.Contains(y))).Argument)).MustHaveHappened();
        }

        [Fact]
        public void Should_retrieve_view_from_view_locator_using_provided_view_name_without_extension()
        {
            // Given
            var factory = this.CreateFactory();

            // When
            var action = factory["viewname.html"];

            // Then)
            A.CallTo(() => this.locator.GetViewLocation("viewname", A<IEnumerable<string>>.Ignored.Argument)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_empty_action_when_view_name_is_null()
        {
            // Given
            var factory = this.CreateFactory();
            var action = factory[null];
            var stream = new MemoryStream();

            // When
            action.Invoke(stream);

            // Then
            stream.Length.ShouldEqual(0L);
        }

        [Fact]
        public void Should_return_empty_action_when_view_name_is_empty()
        {
            // Given
            var factory = this.CreateFactory();
            var action = factory[string.Empty];
            var stream = new MemoryStream();

            // When
            action.Invoke(stream);

            // Then
            stream.Length.ShouldEqual(0L);
        }

        [Fact]
        public void Should_return_empty_action_when_view_name_does_not_contain_extension()
        {
            // Given
            var factory = this.CreateFactory();
            var action = factory["foo"];
            var stream = new MemoryStream();

            // When
            action.Invoke(stream);

            // Then
            stream.Length.ShouldEqual(0L);
        }

        [Fact]
        public void Should_use_view_name_extension_when_available()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
              A.Fake<IViewEngine>(),
            };

            var expectedViewEngineExtensions = new[] { "bar" };
            var factory = this.CreateFactory(viewEngines);

            // When
            var action = factory["foo.bar"];

            // Then
            A.CallTo(() => this.locator.GetViewLocation(A<string>.Ignored,
                A<IEnumerable<string>>.That.IsSameSequenceAs(expectedViewEngineExtensions).Argument)).MustHaveHappened();
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

            var location = new ViewLocationResult(string.Empty, "html", null);
            A.CallTo(() => this.locator.GetViewLocation("foo", A<IEnumerable<string>>.Ignored.Argument)).Returns(location);

            var factory = this.CreateFactory(viewEngines);

            // When
            var action = factory["foo"];

            // Then
            A.CallTo(() => viewEngines[0].RenderView(location, null)).MustHaveHappened();
        }

        [Fact]
        public void Should_ignore_case_when_locating_view_engine_for_view_name_extension()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngine>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "HTML" });

            var location = new ViewLocationResult(string.Empty, "html", null);
            A.CallTo(() => this.locator.GetViewLocation("foo", A<IEnumerable<string>>.Ignored.Argument)).Returns(location);

            var factory = this.CreateFactory(viewEngines);

            // When
            var action = factory["foo"];

            // Then
            A.CallTo(() => viewEngines[0].RenderView(location, null)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_empty_action_when_no_view_engine_could_be_resolved()
        {
            // Given
            var location = new ViewLocationResult(string.Empty, "html", null);
            A.CallTo(() => this.locator.GetViewLocation("foo", A<IEnumerable<string>>.Ignored.Argument)).Returns(location);

            var stream = new MemoryStream();
            var factory = this.CreateFactory();

            // When
            var action = factory["foo"];
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
            A.CallTo(() => viewEngines[0].RenderView(A<ViewLocationResult>.Ignored, null)).Returns(actionReturnedFromEngine);

            var location = new ViewLocationResult(string.Empty, "html", null);
            A.CallTo(() => this.locator.GetViewLocation("foo", A<IEnumerable<string>>.Ignored.Argument)).Returns(location);

            var factory = this.CreateFactory(viewEngines);

            // When
            var action = factory["foo"];

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
            A.CallTo(() => viewEngines[0].RenderView(A<ViewLocationResult>.Ignored, null)).Throws(new Exception());

            var location = new ViewLocationResult(string.Empty, "html", null);
            A.CallTo(() => this.locator.GetViewLocation("foo", A<IEnumerable<string>>.Ignored.Argument)).Returns(location);

            var stream = new MemoryStream();
            var factory = this.CreateFactory(viewEngines);

            // When
            var action = factory["foo"];
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
            A.CallTo(() => viewEngines[0].RenderView(A<ViewLocationResult>.Ignored, null)).Throws(new Exception());

            var location = new ViewLocationResult(string.Empty, "html", null);
            A.CallTo(() => this.locator.GetViewLocation("foo", A<IEnumerable<string>>.Ignored.Argument)).Returns(location);

            var model = new object();
            var factory = this.CreateFactory(viewEngines);

            // When
            var action = factory["foo", model];

            // Then
            A.CallTo(() => viewEngines[0].RenderView(A<ViewLocationResult>.Ignored, model)).MustHaveHappened();
        }
    }
}