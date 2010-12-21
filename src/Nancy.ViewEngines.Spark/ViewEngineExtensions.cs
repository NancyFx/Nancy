namespace Nancy.ViewEngines.Spark
{
    using System;
    using System.IO;

    public static class ViewEngineExtensions 
    {
        public static Action<Stream> Spark(this IViewEngine source, string name) 
        {
            return source.Spark(name, (object)null);
        }

        public static Action<Stream> Spark<TModel>(this IViewEngine source, string name, TModel model)
        {
            var factory = new ViewFactory();

            return stream =>
            {
                var result = factory.RenderView(name, model);
                result.Execute(stream);
            };
        }
    }
}