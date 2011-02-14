namespace Nancy.Tests.Unit.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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

        private DefaultViewFactory CreateFactory(params IViewEngineEx[] viewEngines)
        {
            if (viewEngines == null)
            {
                viewEngines = new IViewEngineEx[] { };
            }

            return new DefaultViewFactory(this.locator, viewEngines);
        }

        [Fact]
        public void Should_retrieve_view_from_view_locator()
        {
            // Given
            var factory = this.CreateFactory();

            // When
            factory.GetRenderedView<object>("viewname.html", null);

            // Then)
            A.CallTo(() => this.locator.GetViewLocation("viewname.html")).MustHaveHappened();
        }

        [Fact]
        public void Should_return_empty_action_when_view_name_is_null()
        {
            // Given
            var factory = this.CreateFactory();
            var action = factory.GetRenderedView<object>(null, null);
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
            var action = factory.GetRenderedView<object>(string.Empty, null);
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
            var action = factory.GetRenderedView<object>("foo", null);
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
              A.Fake<IViewEngineEx>(),
              A.Fake<IViewEngineEx>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => viewEngines[1].Extensions).Returns(new[] { "html" });

            var location = new ViewLocationResult(string.Empty, null);
            A.CallTo(() => this.locator.GetViewLocation("foo.html")).Returns(location);

            var factory = this.CreateFactory(viewEngines);

            // When
            factory.GetRenderedView<object>("foo.html", null);

            // Then
            A.CallTo(() => viewEngines[0].RenderView<object>(location, null)).MustHaveHappened();
        }

        [Fact]
        public void Should_ignore_case_when_locating_view_engine_for_view_name_extension()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngineEx>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "HTML" });

            var location = new ViewLocationResult(string.Empty, null);
            A.CallTo(() => this.locator.GetViewLocation("foo.html")).Returns(location);

            var factory = this.CreateFactory(viewEngines);

            // When
            factory.GetRenderedView<object>("foo.html", null);

            // Then
            A.CallTo(() => viewEngines[0].RenderView<object>(location, null)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_empty_action_when_no_view_engine_could_be_resolved()
        {
            // Given
            var location = new ViewLocationResult(string.Empty, null);
            A.CallTo(() => this.locator.GetViewLocation("foo.html")).Returns(location);

            var stream = new MemoryStream();
            var factory = this.CreateFactory();

            // When
            var action = factory.GetRenderedView<object>("foo.html", null);
            action.Invoke(stream);

            // Then
            stream.Length.ShouldEqual(0L);
        }

        [Fact]
        public void Should_return_action_from_invoked_engine()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngineEx>(),
            };

            Action<Stream> actionReturnedFromEngine = x => { };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => viewEngines[0].RenderView<object>(A<ViewLocationResult>.Ignored, null)).Returns(actionReturnedFromEngine);

            var location = new ViewLocationResult(string.Empty, null);
            A.CallTo(() => this.locator.GetViewLocation("foo.html")).Returns(location);

            var factory = this.CreateFactory(viewEngines);

            // When
            var action = factory.GetRenderedView<object>("foo.html", null);

            // Then
            action.ShouldEqual(actionReturnedFromEngine);
        }

        [Fact]
        public void Should_return_empty_action_when_view_engine_throws_exception()
        {
            var viewEngines = new[] {
              A.Fake<IViewEngineEx>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => viewEngines[0].RenderView<object>(A<ViewLocationResult>.Ignored, null)).Throws(new Exception());

            var location = new ViewLocationResult(string.Empty, null);
            A.CallTo(() => this.locator.GetViewLocation("foo.html")).Returns(location);

            var stream = new MemoryStream();
            var factory = this.CreateFactory(viewEngines);

            // When
            var action = factory.GetRenderedView<object>("foo.html", null);
            action.Invoke(stream);

            // Then
            stream.Length.ShouldEqual(0L);
        }

        [Fact]
        public void Should_invoke_view_engine_with_model()
        {
            // Given
            var viewEngines = new[] {
              A.Fake<IViewEngineEx>(),
            };

            A.CallTo(() => viewEngines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => viewEngines[0].RenderView<object>(A<ViewLocationResult>.Ignored, null)).Throws(new Exception());

            var location = new ViewLocationResult(string.Empty, null);
            A.CallTo(() => this.locator.GetViewLocation("foo.html")).Returns(location);

            var model = new object();
            var factory = this.CreateFactory(viewEngines);

            // When
            factory.GetRenderedView("foo.html", model);

            // Then
            A.CallTo(() => viewEngines[0].RenderView(A<ViewLocationResult>.Ignored, model)).MustHaveHappened();
        }
    }
}