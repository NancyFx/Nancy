namespace Nancy.ViewEngines.DotLiquid
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using liquid = global::DotLiquid;
 
    public class DotLiquidViewEngine : IViewEngine
    {
        public DotLiquidViewEngine(liquid.FileSystems.IFileSystem fileSystem)
        {
            if (fileSystem != null)
            {
                liquid.Template.FileSystem = fileSystem;
            }
        }

        public IEnumerable<string> Extensions
        {
            get { return new[] { "liquid" }; }
        }

        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model)
        {
            return stream =>
                       {
                           var hashedModel = liquid.Hash.FromAnonymousObject(new {model = new DynamicDrop(model)});
                           
                           var parsed = liquid.Template.Parse(viewLocationResult.Contents.ReadToEnd());
                           var rendered = parsed.Render(hashedModel);

                           var writer = new StreamWriter(stream);

                           writer.Write(rendered);
                           writer.Flush();
                        };
        }
    }
}
