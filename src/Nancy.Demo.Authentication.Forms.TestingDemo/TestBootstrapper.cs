namespace Nancy.Demo.Authentication.Forms.TestingDemo
{
    using System;
    using System.IO;
    using Nancy.Testing.Fakes;
    using Testing;

    public class TestBootstrapper : FormsAuthBootstrapper
    {
        protected override Type RootPathProvider
        {
            get
            {
                var assemblyPath = PathHelper.GetAssemblyDirectory(typeof (FormsAuthBootstrapper));
                var rootPath = PathHelper.GetParent(assemblyPath, 3);
                
                FakeRootPathProvider.RootPath = Path.Combine(rootPath, @"Nancy.Demo.Authentication.Forms"); ;

                return typeof(FakeRootPathProvider);
            }
        }
    }
}