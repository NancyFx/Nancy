namespace Nancy.Tests.Unit
{
    using FakeItEasy;
    using Xbehave;

    public class DefaultResponseFormatterFixture
    {
        [Scenario]
        public void GettingTheRootPath(IRootPathProvider rootPathProvider, DefaultResponseFormatter formatter, string rootPath)
        {
            "Given a root path provider which returns a root path of 'foo'"
                .Given(() =>
                {
                    rootPathProvider = A.Fake<IRootPathProvider>();
                    A.CallTo(() => rootPathProvider.GetRootPath()).Returns("foo");
                });

            "And a default response formatter constructed using the root path provider"
                .And(() => formatter = new DefaultResponseFormatter(rootPathProvider, null, new ISerializer[] { }));

            "When getting the root path"
                .When(() => rootPath = formatter.RootPath);

            "Then the root path should equal 'foo'"
                .Then(() => rootPath.ShouldEqual("foo"));
        }

        [Scenario]
        public void GettingTheContext(NancyContext context, DefaultResponseFormatter formatter)
        {
            "Given a context"
                .Given(() => context = new NancyContext());

            "When constructing a default response formatter using the context"
                .When(() => formatter = new DefaultResponseFormatter(null, context, new ISerializer[] { }));

            "Then the formatter context should be the same as the given context"
                .Then(() => formatter.Context.ShouldBeSameAs(context));
        }
    }
}