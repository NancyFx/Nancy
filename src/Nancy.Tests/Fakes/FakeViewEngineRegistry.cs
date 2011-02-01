namespace Nancy.Tests.Fakes
{
    using System;
    using System.IO;

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

        public static Func<string, object, Action<Stream>> Executor
        {
            get { return (s, n) => Stream; }
        }

        public static Action<Stream> Stream = (s) => {};
    }
}