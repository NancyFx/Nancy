namespace Nancy.Tests.Unit.ViewEngines
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using FakeItEasy;
    using Nancy.ViewEngines;
    using Xunit;

    public class ViewLocatorFixture
    {
        [Fact]
        public void Should_throw_argumentnullexception_when_initialized_with_null()
        {
            // Given, When
            var exception =
                Record.Exception(() => new ViewLocator(null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_return_null_when_getting_view_location_with_null_view_null()
        {
            // Given
            var locator = new ViewLocator(new[] { A.Fake<IViewSourceProvider>() });

            // When
            var result = locator.GetViewLocation(null);

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_when_getting_view_location_with_empty_view_null()
        {
            // Given
            var locator = new ViewLocator(new[] { A.Fake<IViewSourceProvider>() });

            // When
            var result = locator.GetViewLocation(string.Empty);

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_when_getting_view_location_with_no_view_source_provider()
        {
            // Given
            var locator = new ViewLocator(new IViewSourceProvider[] { });

            // When
            var result = locator.GetViewLocation("viewName");

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_call_locateview_with_view_name_on_all_view_source_providers()
        {
            // Given
            const string viewname = "view name";

            var viewSourceProviders = new[] {
                A.Fake<IViewSourceProvider>(),
                A.Fake<IViewSourceProvider>()
            };

            A.CallTo(() => viewSourceProviders[0].LocateView(viewname)).Returns(null);
            A.CallTo(() => viewSourceProviders[1].LocateView(viewname)).Returns(null);

            var locator = new ViewLocator(viewSourceProviders);

            // When
            locator.GetViewLocation(viewname);

            // Then)
            A.CallTo(() => viewSourceProviders[0].LocateView(viewname)).MustHaveHappened();
            A.CallTo(() => viewSourceProviders[1].LocateView(viewname)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_view_location_result_from_source_provider_when_view_could_be_found()
        {
            // Given
            var viewSourceProvider = A.Fake<IViewSourceProvider>();
            var viewLocationResult = new ViewLocationResult(null, null);
            var locator = new ViewLocator(new[] { viewSourceProvider });

            A.CallTo(() => viewSourceProvider.LocateView(A<string>.Ignored)).Returns(viewLocationResult);

            // When
            var result = locator.GetViewLocation("view name");

            // Then
            result.ShouldBeSameAs(viewLocationResult);
        }

        [Fact]
        public void Should_suppress_exceptions_thrown_by_view_source_provider()
        {
            // Given
            var viewSourceProvider = A.Fake<IViewSourceProvider>();
            A.CallTo(() => viewSourceProvider.LocateView(A<string>.Ignored)).Throws(new Exception());

            var locator = new ViewLocator(new[] { viewSourceProvider });

            // When
            var result = locator.GetViewLocation("view name");

            // Then
            result.ShouldBeNull();
        }
    }

    public class ResourceViewSourceProviderFixture
    {
        
    }

    public class FakeAssembly : Assembly
    {
        private readonly NameValueCollection resources = new NameValueCollection();

        public FakeAssembly()
        {
        }

        public FakeAssembly(Action<FakeAssemblyConfigurator> closure)
        {
            var configurator =
                new FakeAssemblyConfigurator(this);

            closure.Invoke(configurator);
        }

        public override string[] GetManifestResourceNames()
        {
            return this.resources.AllKeys;
        }

        public override Stream GetManifestResourceStream(string name)
        {
            if (!this.resources.AllKeys.Contains(name))
                return null;

            var buffer =
                Encoding.UTF8.GetBytes(this.resources[name]);

            return new MemoryStream(buffer);
        }

        public class FakeAssemblyConfigurator
        {
            private readonly FakeAssembly assembly;

            public FakeAssemblyConfigurator(FakeAssembly assembly)
            {
                this.assembly = assembly;
            }

            public FakeAssemblyConfigurator AddResource(string name, string value)
            {
                this.assembly.resources.Add(name, value);
                return this;
            }
        }
    }
}