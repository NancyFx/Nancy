namespace Nancy.Demo.Authentication.Forms.TestingDemo
{
    public class TestBootstrapper : FormsAuthBootstrapper
    {
        protected override IRootPathProvider RootPathProvider
        {
            get { return new TestRootPathProvider(); }
        }
    }
}
