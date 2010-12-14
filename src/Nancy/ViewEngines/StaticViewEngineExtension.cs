namespace Nancy.ViewEngines
{
    using System;
    using System.IO;
    using System.Web.Hosting;

    public static class StaticViewEngineExtension
    {
        public static Action<Stream> Static(this IViewEngine engine, string virtualPath)
        {
            return stream => {

                var path = HostingEnvironment.MapPath(virtualPath);

                using (var reader = new StreamReader(path))
                {
                    using(var writer = new StreamWriter(stream))
                    {
                        writer.Write(reader.ReadToEnd());
                        writer.Flush();
                    }
                }

            };
        }
    }
}