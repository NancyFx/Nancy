namespace Nancy.ViewEngines.NDjango
{
    using System;
    using System.IO;

    public static class NDjangoViewEngineExtensions
    {
        public static Action<Stream> Django(this IViewEngine source, string name)
        {
            return Django(source, name, (object)null);
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
    }
}