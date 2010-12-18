namespace Nancy.ViewEngines.NHaml
{
    using System;
    using System.IO;

    public static class NHamlViewEngineExtensions
    {
        public static Action<Stream> Haml(this IViewEngine source, string name)
        {
            return Haml(source, name, (object) null);
        }

        public static Action<Stream> Haml<TModel>(this IViewEngine source, string name, TModel model)
        {
            var viewEngine = new NHamlViewEngine();

            return stream =>
            {
                var result = viewEngine.RenderView(name, model);
                result.Execute(stream);
            };
        }
    }
}