namespace Nancy.ViewEngines.NDjango
{
    using System;
    using System.IO;

    public static class NDjangoViewEngineExtensions
    {
        public static Action<Stream> Django(this IViewEngine source, string name)
        {
            return source.Django<object>(name, null);
        }
       
        public static Action<Stream> Django<TModel>(this IViewEngine source, string name, TModel model)
        {
            var viewEngine = new NDjangoViewEngine();

            return stream =>
            {
                var result = viewEngine.RenderView(name, model);
                result.Execute(stream);
            };
        }

        public static Action<Stream> DjangoEmbedded(this IViewEngine source, string name)
        {
            return source.DjangoEmbedded<object>(name, null);
        }

        public static Action<Stream> DjangoEmbedded<TModel>(this IViewEngine source, string name, TModel model)
        {
            var viewEngine = new NDjangoViewEngine(new ResourceTemplateLocator());

            return stream =>
            {
                var result = viewEngine.RenderView(name, model);
                result.Execute(stream);
            };
        }
    }
}