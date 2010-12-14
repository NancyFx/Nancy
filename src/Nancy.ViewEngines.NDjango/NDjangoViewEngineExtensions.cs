namespace Nancy.ViewEngines.NDjango
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Hosting;
    using global::NDjango;

    public static class NDjangoViewEngineExtensions
    {
        public static Action<Stream> Django(this IViewEngine source, string name)
        {
            return Django(source, name, (object)null);
        }

        public static Action<Stream> Django<TModel>(this IViewEngine source, string name, TModel model)
        {
            var templateManagerProvider = new TemplateManagerProvider();
            var manager = templateManagerProvider.GetNewManager();

            var path = HostingEnvironment.MapPath(name);

            return stream =>
            {
                var context = new Dictionary<string, object> {{"Model", model}};

                var reader = manager.RenderTemplate(path, context);

                var writer = new StreamWriter(stream);
                var buffer = new char[4096];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writer.Write(buffer, 0, count);
                }
                
                writer.Flush();
            };
        }
    }
}