namespace Nancy.Demo.MarkdownViewEngine.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using MarkdownSharp;
    using Model;
    using ViewEngines;

    public class HomeModule : NancyModule
    {
        private readonly IFileSystemReader fileSystemReader;
        private readonly IRootPathProvider rootPathProvider;
        private readonly string rootPath;

        public HomeModule(IFileSystemReader fileSystemReader, IRootPathProvider rootPathProvider)
        {
            this.fileSystemReader = fileSystemReader;
            this.rootPathProvider = rootPathProvider;
            this.rootPath = rootPathProvider.GetRootPath();

            var path = rootPathProvider.GetRootPath() + Path.DirectorySeparatorChar + "Views" +
                        Path.DirectorySeparatorChar + "Posts";


            Get["/"] = _ =>
                           {
                               var model = GetModel(path, new[] { "md", "markdown" });
                               return View["blogindex", model];
                           };

            Get["/{viewname}"] = parameters =>
                                     {
                                         var model = GetModel(path, new[] { "md", "markdown" });
                                         return View["Posts/" + parameters.viewname, model];
                                     };
        }

        private IEnumerable<BlogModel> GetModel(string path, IEnumerable<string> supportedViewExtensions)
        {
            var views = GetViewsFromPath(path, new[] { "md", "markdown" });

            var model =
                views.Select(
                    x =>
                    {
                        var markdown = x.Contents().ReadToEnd();
                        return new BlogModel(markdown);
                    }).ToList();

            return model;
        }

        private IEnumerable<ViewLocationResult> GetViewsFromPath(string path, IEnumerable<string> supportedViewExtensions)
        {
            var matches = this.fileSystemReader.GetViewsWithSupportedExtensions(path, supportedViewExtensions);

            return from match in matches
                   select
                       new FileSystemViewLocationResult(
                       GetViewLocation(match.Item1, this.rootPath),
                       Path.GetFileNameWithoutExtension(match.Item1),
                       Path.GetExtension(match.Item1).Substring(1),
                       match.Item2,
                       match.Item1,
                       this.fileSystemReader);
        }

        private static string GetViewLocation(string match, string rootPath)
        {
            var location = match
                .Replace(rootPath, string.Empty)
                .TrimStart(new[] { Path.DirectorySeparatorChar })
                .Replace(@"\", "/")
                .Replace(Path.GetFileName(match), string.Empty)
                .TrimEnd(new[] { '/' });

            return location;
        }
    }
}
