namespace Nancy.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Bootstrapper;
    using Responses;

    public class DefaultStaticContentsConventions : IConvention
    {
        public void Initialise(NancyConventions conventions)
        {
            conventions.StaticContentsConventions = new List<Func<NancyContext, string, Response>>
            {
                (ctx, rootPath) => {

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

                    if (!path.Equals(StaticConfiguration.StaticContentsDirectory, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                    var filePath =
                        Path.Combine(rootPath, StaticConfiguration.StaticContentsDirectory, fileName);

                    return File.Exists(filePath) ? 
                        new GenericFileResponse(filePath) : 
                        null;
                }
            };
        }

        public Tuple<bool, string> Validate(NancyConventions conventions)
        {
            if (conventions.StaticContentsConventions == null)
            {
                return Tuple.Create(false, "The static contents conventions cannot be null.");
            }

            return (conventions.StaticContentsConventions.Count > 0) ?
                Tuple.Create(true, string.Empty) :
                Tuple.Create(false, "The static contents conventions cannot be empty.");
        }
    }

    public class StaticContentStartup : IStartup
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly StaticContentsConventions conventions;

        public StaticContentStartup(IRootPathProvider rootPathProvider, StaticContentsConventions conventions)
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
        public static void Enable(IApplicationPipelines pipelines, IRootPathProvider rootPathProvider, StaticContentsConventions conventions)
        {
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx =>
            {
                return conventions
                    .Select(convention => convention.Invoke(ctx, rootPathProvider.GetRootPath()))
                    .FirstOrDefault(response => response != null);
            });
        }
    }
}