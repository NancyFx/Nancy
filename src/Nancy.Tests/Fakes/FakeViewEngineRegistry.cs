namespace Nancy.Tests.Fakes
{
    using System;
    using System.IO;

    using Nancy.ViewEngines;

    public class FakeViewEngineRegistry : IViewEngineRegistry
    {
        public string Extension
        {
            get { return ".leto2"; }
        }

        Func<string, object, Action<Stream>> IViewEngineRegistry.Executor
        {
            get { return Executor; }
        }

        public static Func<string, object, Action<Stream>> Executor
        {
            get { return (s, n) => Stream; }
        }

        public static Action<Stream> Stream = (s) => {};
    }
}