namespace Nancy.Demo.Authentication.Token.TestingDemo
{
    using System;
    using System.IO;

    using Nancy.Authentication.Token;
    using Nancy.Authentication.Token.Storage;
    using Nancy.Testing;
    using Nancy.Testing.Fakes;
    using Nancy.TinyIoc;

    public class TestBootstrapper : TokenAuthBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<ITokenizer>(new Tokenizer(cfg => cfg.WithKeyCache(new InMemoryTokenKeyStore())));
        }

        protected override IRootPathProvider RootPathProvider
        {
            get
            {
                var assemblyFilePath =
                    new Uri(typeof(TokenAuthBootstrapper).Assembly.CodeBase).LocalPath;

                var assemblyPath =
                    Path.GetDirectoryName(assemblyFilePath);

                var rootPath =
                    PathHelper.GetParent(assemblyPath, 2);

                rootPath =
                    Path.Combine(rootPath, @"Nancy.Demo.Authentication.Token");

                FakeRootPathProvider.RootPath = rootPath;

                return new FakeRootPathProvider();
            }
        }
    }
}