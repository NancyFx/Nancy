namespace Nancy.Conventions
{
    using System;
    using System.Collections.Concurrent;
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
                StaticContentConventionBuilder.AddDirectory("content")
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
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx =>
            {
                return conventions
                    .Select(convention => convention.Invoke(ctx, rootPathProvider.GetRootPath()))
                    .FirstOrDefault(response => response != null);
            });
        }
    }

    public class StaticContentConventionBuilder
    {
        private static readonly ConcurrentDictionary<string, Func<Response>> ResponseFactoryCache;

        static StaticContentConventionBuilder()
        {
            ResponseFactoryCache = new ConcurrentDictionary<string, Func<Response>>();
        }

        public static Func<NancyContext, string, Response> AddDirectory(string contentPath, string virtualPath = null, params string[] allowedExtensions)
        {
            return (ctx, root) =>
            {
                var path =
                    ctx.Request.Path.TrimStart(new[] { '/' });

                virtualPath = virtualPath ?? contentPath;

                if (!path.StartsWith(virtualPath, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                var responseFactory =
                    ResponseFactoryCache.GetOrAdd(path, BuildContentDelegate(root, contentPath, virtualPath, allowedExtensions));

                return responseFactory.Invoke();
            };
        }

        private static Func<string, Func<Response>> BuildContentDelegate(string applicationRootPath, string contentPath, string virtualPath, string[] allowedExtensions)
        {
            return requestPath =>
            {
                var extension = Path.GetExtension(requestPath);

                if (string.IsNullOrEmpty(extension))
                {
                    return () => null;
                }

                if (allowedExtensions.Length != 0 && !allowedExtensions.Any(e => string.Equals(e, extension, StringComparison.OrdinalIgnoreCase)))
                {
                    return () => null;
                }

                if(!contentPath.Equals(virtualPath, StringComparison.OrdinalIgnoreCase))
                {
                    requestPath = String.Concat(contentPath, requestPath.Substring(virtualPath.Length));
                }

                if (!IsSafeToCombinePaths(applicationRootPath, requestPath))
                {
                    return () => null;
                }

                var fileName = Path.Combine(applicationRootPath, requestPath);

                if (!File.Exists(fileName))
                {
                    return () => null;
                }

                return () => new GenericFileResponse(fileName);
            };
        }

        private static bool IsSafeToCombinePaths(string contentsFolderPath, string requestedPath)
        {
            var combinedPath =
                Path.GetFullPath(Path.Combine(contentsFolderPath, requestedPath));

            return combinedPath.StartsWith(contentsFolderPath);
        }
    }
}