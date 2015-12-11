namespace Nancy.Demo.Authentication.Forms.TestingDemo
{
    using System;
    using System.IO;

    using Nancy.Testing;
    using Nancy.Testing.Fakes;

    public class TestBootstrapper : FormsAuthBootstrapper
    {
        protected override IRootPathProvider RootPathProvider
        {
            get
            {
                var assemblyFilePath =
                    new Uri(typeof(FormsAuthBootstrapper).Assembly.CodeBase).LocalPath;

                var assemblyPath =
                    Path.GetDirectoryName(assemblyFilePath);

                var rootPath =
                    PathHelper.GetParent(assemblyPath, 3);

                rootPath =
                    Path.Combine(rootPath, @"Nancy.Demo.Authentication.Forms");

                FakeRootPathProvider.RootPath = rootPath;

                return new FakeRootPathProvider();
            }
        }
    }
}