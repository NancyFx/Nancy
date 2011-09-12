namespace Nancy.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Bootstrapper;
    using Responses;

    public interface IStaticContentsConventionsProvider
    {
        IDictionary<string, Tuple<string, Func<string, bool>>> Conventions { get; }

        IStaticContentsConventionsProvider Add(string nancyPath, string actualPath, Func<string, bool> extensions);
    }

    public class DefaultStaticContentsConventionsProvider : IStaticContentsConventionsProvider
    {
        public IDictionary<string, Tuple<string, Func<string, bool>>> Conventions { get; private set; }
        
        public DefaultStaticContentsConventionsProvider()
        {
            this.Conventions = new Dictionary<string, Tuple<string, Func<string, bool>>>();
        }

        public IStaticContentsConventionsProvider Add(string nancyPath, string actualPath, Func<string, bool> extensions)
        {
            this.Conventions.Add(nancyPath, Tuple.Create(actualPath, extensions));
            return this;
        }
    }

    public class DefaultStaticContentsConventions : IConvention
    {
        public void Initialise(NancyConventions conventions)
        {
            var x =
                new DefaultStaticContentsConventionsProvider();

            x.Add("foo", "content", ext => string.Equals(ext, "png", StringComparison.OrdinalIgnoreCase));

            conventions.StaticContentsConventions = x;
        }

        public Tuple<bool, string> Validate(NancyConventions conventions)
        {
            if (conventions.StaticContentsConventions == null)
            {
                return Tuple.Create(false, "The static contents conventions cannot be null.");
            }

            return (conventions.StaticContentsConventions.Conventions.Count > 0) ?
                Tuple.Create(true, string.Empty) :
                Tuple.Create(false, "The static contents conventions cannot be empty.");
        }
    }

    public class StaticContentStartup : IStartup
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly IStaticContentsConventionsProvider conventions;

        public StaticContentStartup(IRootPathProvider rootPathProvider, IStaticContentsConventionsProvider conventions)
        {
            this.rootPathProvider = rootPathProvider;
            this.conventions = conventions;
        }

        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get { return Enumerable.Empty<TypeRegistration>(); }
        }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get { return Enumerable.Empty<CollectionTypeRegistration>(); }
        }

        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get { return Enumerable.Empty<InstanceRegistration>(); }
        }

        public void Initialize(IApplicationPipelines pipelines)
        {
            StaticContentsHook.Enable(pipelines, this.rootPathProvider, this.conventions);
        }
    }

    public static class StaticContentsHook
    {
        public static void Enable(IApplicationPipelines pipelines, IRootPathProvider rootPathProvider, IStaticContentsConventionsProvider conventions)
        {
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx =>{

                var extension =
                    Path.GetExtension(ctx.Request.Path);

                if (string.IsNullOrEmpty(extension))
                {
                    return null;
                }

                var fileName =
                    Path.GetFileName(ctx.Request.Path);

                var path = ctx.Request.Path
                    .Substring(1)
                    .Replace(fileName, string.Empty)
                    .TrimEnd(new [] { '/' });

                var actualPath = conventions.Conventions
                    .Where(x => x.Key.Equals(path, StringComparison.OrdinalIgnoreCase))
                    .Where(x => x.Value.Item2.Invoke(extension.Substring(1)))
                    .Select(x => x.Value.Item1)
                    .SingleOrDefault();

                if (actualPath == null)
                {
                    return null;
                }
                
                var filePath =
                    Path.Combine(rootPathProvider.GetRootPath(), actualPath, fileName);

                if (File.Exists(filePath))
                {
                    var mimeType =
                        MimeTypes.GetMimeType(ctx.Request.Path);

                    return new GenericFileResponse(filePath, mimeType);
                }

                return null;
            });
        }
    }
}