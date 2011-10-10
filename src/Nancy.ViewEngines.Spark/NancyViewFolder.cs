namespace Nancy.ViewEngines.Spark
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using global::Spark.FileSystem;

    public class NancyViewFolder : IViewFolder
    {
        private readonly ViewEngineStartupContext viewEngineStartupContext;

        public NancyViewFolder(ViewEngineStartupContext viewEngineStartupContext)
        {
            this.viewEngineStartupContext = viewEngineStartupContext;
        }

        public IViewFile GetViewSource(string path)
        {
            var searchPath = ConvertPath(path);

            var viewLocationResult = this.viewEngineStartupContext.ViewLocationResults.FirstOrDefault(v => String.Equals(v.Location + "/" + v.Name + "." + v.Extension, searchPath, StringComparison.OrdinalIgnoreCase));

            if (viewLocationResult == null)
            {
                throw new FileNotFoundException(string.Format("Template {0} not found", path), path);
            }

            return new NancyViewFile(viewLocationResult);
        }

        public IList<string> ListViews(string path)
        {
            return this.viewEngineStartupContext.
                ViewLocationResults.
                Where(v => v.Location.StartsWith(path, StringComparison.OrdinalIgnoreCase)).
                Select(v =>
                    v.Location.Length == path.Length ?
                        v.Name + "." + v.Extension : 
                        v.Location.Substring(path.Length) + "/" + v.Name + "." + v.Extension).
                ToList();
        }

        public bool HasView(string path)
        {
            var searchPath = ConvertPath(path);

            return this.viewEngineStartupContext.ViewLocationResults.Any(v => String.Equals(v.Location + "/" + v.Name + "." + v.Extension, searchPath, StringComparison.OrdinalIgnoreCase));
        }

        private static string ConvertPath(string path)
        {
            return path.Replace(@"\", "/");
        }

        public class NancyViewFile : IViewFile
        {
            private readonly ViewLocationResult viewLocationResult;
            private readonly long created;

            public NancyViewFile(ViewLocationResult viewLocationResult)
            {
                this.viewLocationResult = viewLocationResult;
                this.created = DateTime.Now.Ticks;
            }

            public long LastModified
            {
                get { return StaticConfiguration.DisableCaches ? DateTime.Now.Ticks : this.created; }
            }

            public Stream OpenViewStream()
            {
                string view;
                using (var reader = this.viewLocationResult.Contents.Invoke())
                {
                    view = reader.ReadToEnd();
                }

                return new MemoryStream(Encoding.UTF8.GetBytes(view));
            }
        }
    }
}