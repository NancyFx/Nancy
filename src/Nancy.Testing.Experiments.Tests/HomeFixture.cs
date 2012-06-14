namespace Nancy.Testing.Experiments.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Xunit;

    public class HomeFixture
    {
        [Fact]
        public void Should_be_able_to_test_route_that_renders_view()
        {
            // Given
            var rootPathProvider =
                new ConfigurableRootPathProvider(typeof(Home));

            var browser = new Browser(new ConfigurableBootstrapper(with => {
                with.RootPathProvider(rootPathProvider);
            }));

            // When
            var result = browser.Get("/");

            // Then
            result.Body["#container"].ShouldExistOnce();
        }
    }

    public class ConfigurableRootPathProvider : IRootPathProvider
    {
        private readonly string rootPath;

        public ConfigurableRootPathProvider(Type type)
            : this(type.Assembly)
        {
        }

        public ConfigurableRootPathProvider(Assembly assembly)
        {
            var assemblyFilePath =
                new Uri(assembly.CodeBase).LocalPath;

            var assemblyPath =
                Path.GetDirectoryName(assemblyFilePath);

            this.rootPath =
                PathHelper.GetParent(assemblyPath, 2);
        }

        public string GetRootPath()
        {
            return this.rootPath;
        }
    }
}