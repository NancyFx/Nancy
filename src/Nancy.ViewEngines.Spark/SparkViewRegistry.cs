namespace Nancy.ViewEngines.Spark
{
    using System;
    using System.IO;

    public class SparkViewRegistry : IViewEngineRegistry
    {
        public Action<Stream> Execute<TModel>(string viewTemplate, TModel model)
        {
            var factory = new ViewFactory();
            return (Action<Stream>)(stream =>
            {
                var result = factory.RenderView(viewTemplate, model);
                result.Execute(stream);
            });
        }

        public string Extension
        {
            get { return ".spark"; }
        }
    }
}