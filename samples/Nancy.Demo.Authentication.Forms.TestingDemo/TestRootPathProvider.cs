namespace Nancy.Demo.Authentication.Forms.TestingDemo
{
    using System;
    using System.IO;
    using Nancy.Testing;

    public class TestRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            var assemblyFilePath =
                new Uri(typeof(FormsAuthBootstrapper).Assembly.CodeBase).LocalPath;

            var assemblyPath =
                Path.GetDirectoryName(assemblyFilePath);

            var rootPath =
                PathHelper.GetParent(assemblyPath, 3);

            rootPath =
                Path.Combine(rootPath, @"Nancy.Demo.Authentication.Forms");

            return rootPath;
        }
    }
}
