namespace Nancy.ViewEngines.NHaml
{
    using System;
    using System.IO;
    using System.Web.Hosting;
    using global::NHaml;

    public static class NHamlViewEngineExtensions
    {
        public static Action<Stream> Haml(this IViewEngine source, string name)
        {
            return Haml(source, name, (object) null);
        }

        public static Action<Stream> Haml<TModel>(this IViewEngine source, string name, TModel model)
        {
            var templateEngine = new TemplateEngine();
            var path = HostingEnvironment.MapPath(name);

            return stream =>
            {
                CompiledTemplate compiledTemplate = templateEngine.Compile(path, typeof(Template<TModel>));

                var writer = new StreamWriter(stream);
                var template = (Template<TModel>)compiledTemplate.CreateInstance();
                template.Model = model;
                template.Render(writer);
                writer.Flush();
            };
        }
    }
}