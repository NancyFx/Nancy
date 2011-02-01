namespace Nancy.Tests.Fakes
{
    using Nancy.ViewEngines;

    public class FakeViewEngineRegistry : IViewEngineRegistry
    {
        public Action<Stream> Execute<TModel>(string viewTemplate, TModel model)
        {
            return Executor(viewTemplate, model);
        }

        public string Extension
        {
            get { return ".leto2"; }
        }

        public static readonly IViewEngine ViewEngine = new FakeViewEngine();
    }
}
