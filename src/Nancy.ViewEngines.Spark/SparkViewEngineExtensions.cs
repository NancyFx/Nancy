namespace Nancy.ViewEngines.Spark
{
    using System;
    using System.IO;
    using System.Web.Hosting;

    using global::Spark;
    using global::Spark.FileSystem;

    public static class SparkViewEngineExtensions
    {
        public static Action<Stream> Razor(this IViewEngine source, string name) 
        {
            return source.Razor(name, (object)null);
        }

        public static Action<Stream> Razor<TModel>(this IViewEngine source, string name, TModel model)
        {
            var settings =
                new SparkSettings().SetPageBaseType(typeof(NancySparkView));

            var engine =
                new SparkViewEngine(settings) { ViewFolder = new FileSystemViewFolder(HostingEnvironment.MapPath(@"~/Views")) };

            

            return stream => {
                //var result = engine.RenderView(name, model);
                //result.Execute(stream);
            };
        }
    }

    public abstract class NancySparkView : SparkViewBase
    {
        
    }
}