namespace Nancy.Tests.Fakes
{
    using Nancy.ViewEngines;

    public class FakeViewEngineRegistry : IViewEngineRegistry
    {
        IViewEngine IViewEngineRegistry.ViewEngine
        {
            get { return ViewEngine; }
        }

        public string Extension
        {
            get { return ".leto2"; }
        }

        public static readonly IViewEngine ViewEngine = new FakeViewEngine();
    }
}