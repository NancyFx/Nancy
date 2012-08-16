namespace Nancy.Testing.Experiments.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Bootstrapper;
    using Xunit;

    public class HomeFixture
    {
        [Fact]
        public void Should_be_able_to_test_route_that_renders_view()
        {
            //AppDomainAssemblyTypeScanner.LoadAssemblies("Models.dll");

            //var x =
            //    typeof(HomeFixture).Assembly.GetReferencedAssemblies();

            //AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            //{
            //    Debug.WriteLine(args.Name);
            //    Debug.WriteLine(args.RequestingAssembly.FullName);
            //    return null;
            //};

            //AppDomain.CurrentDomain.CreateInstance("Models", "DefaultModelFactory");

            // Given
            //var rootPathProvider =
            //    new ConfigurableRootPathProvider(typeof(Home));

            //var browser = new Browser(new ConfigurableBootstrapper(with => {
            //    with.RootPathProvider(rootPathProvider);
            //}));

            var browser = new Browser(new ConfigurableBootstrapper(with => {
                with.EnableAutoRegistration();
                //with.Assembly("Models.dll");
            }));

            // When
            var result = browser.Get("/");

            // Then
            result.Body["#container"].ShouldExistOnce();
        }
    }

    //public class ConfigurableRootPathProvider : IRootPathProvider
    //{
    //    private readonly string rootPath;

    //    public ConfigurableRootPathProvider(Type type)
    //        : this(type.Assembly)
    //    {
    //    }

    //    public ConfigurableRootPathProvider(Assembly assembly)
    //    {
    //        var assemblyFilePath =
    //            new Uri(assembly.CodeBase).LocalPath;

    //        var assemblyPath =
    //            Path.GetDirectoryName(assemblyFilePath);

    //        this.rootPath =
    //            PathHelper.GetParent(assemblyPath, 2);
    //    }

    //    public string GetRootPath()
    //    {
    //        return this.rootPath;
    //    }
    //}
}