namespace Nancy.Demo.Authentication.Forms.TestingDemo
{
    using System;
    using System.IO;
    using Nancy.Testing.Fakes;
    using Testing;

    public class TestBootstrapper : FormsAuthBootstrapper
    {
        protected override Type GetRootPathProvider()
        {
            // TODO - figure out a nicer way to do this
            var assemblyPath = Path.GetDirectoryName(typeof(FormsAuthBootstrapper).Assembly.CodeBase).Replace(@"file:\", string.Empty);
            var rootPath = PathHelper.GetParent(assemblyPath, 3);
            rootPath = Path.Combine(rootPath, @"Nancy.Demo.Authentication.Forms");

            FakeRootPathProvider.RootPath = rootPath; 

            return typeof(FakeRootPathProvider);
        }
    }
}