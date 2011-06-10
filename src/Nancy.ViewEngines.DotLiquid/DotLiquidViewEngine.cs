namespace Nancy.ViewEngines.DotLiquid
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using global::DotLiquid;
    using global::DotLiquid.FileSystems;
 
    public class DotLiquidViewEngine : IViewEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidViewEngine"/> class.
        /// </summary>
        public DotLiquidViewEngine()
            : this(new LiquidNancyFileSystem(string.Empty))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidViewEngine"/> class.
        /// </summary>
        /// <param name="fileSystem"></param>
        public DotLiquidViewEngine(IFileSystem fileSystem)
        {
            if (fileSystem != null)
            {
                Template.FileSystem = fileSystem;
            }
        }

        public IEnumerable<string> Extensions
        {
            get { yield return "liquid"; }
        }

        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            return stream =>
            {
                var hashedModel = 
                    Hash.FromAnonymousObject(new { model = new DynamicDrop(model) });

                var parsed = renderContext.ViewCache.GetOrAdd(
                    viewLocationResult,
                    x => Template.Parse(viewLocationResult.Contents.Invoke().ReadToEnd()));

                var rendered = parsed.Render(hashedModel);

                var writer = new StreamWriter(stream);

                writer.Write(rendered);
                writer.Flush();
            };
        }
    }
}
