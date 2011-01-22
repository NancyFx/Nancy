namespace Nancy.ViewEngines.Spark
{
    using System;
    using System.IO;

    public class SparkViewRegistry : IViewEngineRegistry
    {
        public string Extension
        {
            get { return ".spark"; }
        }

        public Func<string, object, Action<Stream>> Executor
        {
            get { return (name, model) => ViewEngineExtensions.Spark(null, name, model); }
        }
    }
}